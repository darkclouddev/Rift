using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Services.Message;
using Rift.Services.Role;

using IonicLib;
using IonicLib.Extensions;
using IonicLib.Util;

using Discord;
using Discord.WebSocket;

using Newtonsoft.Json;

namespace Rift.Services
{
	public class RoleService
	{
		static ConcurrentDictionary<TempRole, byte> tempRoles = new ConcurrentDictionary<TempRole, byte>();
		static IEnumerable<TempRole> rolesToRemove = new List<TempRole>();
		static readonly string TempRoleFilePath = Path.Combine(Directory.GetCurrentDirectory(), "tempRoles.json");

		static readonly TimeSpan tempRoleCheckTimerCooldown = TimeSpan.FromSeconds(15);

		static Timer tempRoleCheckTimer;

		public RoleService()
		{
			tempRoleCheckTimer = new Timer(async delegate { await CheckExpiredRolesAsync(); }, null, tempRoleCheckTimerCooldown, tempRoleCheckTimerCooldown);

			if (File.Exists(TempRoleFilePath))
			{
				string json = File.ReadAllText(TempRoleFilePath);
				var deserialized = JsonConvert.DeserializeObject<List<TempRole>>(json);

				foreach(var t in deserialized)
				{
					tempRoles.TryAdd(t, default);
				}
			}

			if (!(tempRoles is null))
				RiftBot.Log.Info($"Loaded {tempRoles.Count} temp roles.");
		}

		public IEnumerable<TempRole> GetMutes()
			=> tempRoles.Keys.ToList().Where(x => x.RoleId == Settings.RoleId.Muted);

		async Task CheckExpiredRolesAsync()
		{
			if (tempRoles is null && tempRoles.Count == 0)
				return;

			rolesToRemove = tempRoles.Keys.Where(x => x.UntilTimestamp <= IonicLib.Extensions.Helper.CurrentUnixTimestamp);

			if (!rolesToRemove.Any())
				return;

			await RemoveExpiredAsync();
		}

		public async Task RemoveExpiredAsync()
		{
			foreach(var role in rolesToRemove)
			{
				if (!await RemoveRoleAsync(role))
				{
                    RiftBot.Log.Error($"Failed to remove role {role.RoleId} from user {role.UserId}");
					continue;
				}

                RiftBot.Log.Info($"Removed {role.UserId} from list");

				await Task.Run(SaveAsync);

				var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, role.UserId);

				if (sgUser is null)
					return;

				if (!IonicClient.GetRole(Settings.App.MainGuildId, role.RoleId, out var serverRole))
					return;

				await sgUser.RemoveRoleAsync(serverRole);

				if (serverRole.Id == Settings.RoleId.Muted)
				{
					if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var chatChannel))
						return;

					var eb = new EmbedBuilder()
						.WithAuthor("Оповещение")
						.WithDescription($"Призывателю {sgUser.Mention} снято ограничение на чат.");

					await chatChannel.SendEmbedAsync(eb);

					break;
				}
				else if (serverRole.Id == Settings.RoleId.Attacked)
				{
					if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Chat, out var channel))
						return;

					var eb = new EmbedBuilder()
						.WithDescription($"Призыватель {sgUser.Mention} вернулся в чат после атаки.");

					var msg = await channel.SendEmbedAsync(eb);

                    RiftBot.GetService<MessageService>().TryAddDelete(new DeleteMessage(msg, TimeSpan.FromSeconds(30)));
					break;
				}
				else
				{
					var eb = new EmbedBuilder()
						.WithDescription($"Призыватель, у вас истек срок роли **{serverRole.Name}**.");

					await sgUser.SendEmbedAsync(eb);
					break;
				}
			}
		}

		public async Task<bool> RemoveRoleAsync(ulong userId, ulong roleId)
		{
			if (!GetTempRole(userId, roleId, out var role))
				return false;

			return await Task.Run(() => tempRoles.TryRemove(role, out var garbage));
		}

		public async Task<bool> RemoveRoleAsync(TempRole role)
		{
			return await Task.Run(() => tempRoles.TryRemove(role, out var garbage));
		}

		public async Task<bool> SetExpiredAsync(ulong userId, ulong roleId)
		{
			if (!GetTempRole(userId, roleId, out var role))
				return false;

			role.UntilTimestamp = 0ul;

			return await AddTempRoleAsync(role);
		}

		public bool HasTempRoles(ulong userId)
			=> tempRoles.Any(x => x.Key.UserId == userId);

		public List<TempRole> GetTempRoles(ulong userId)
			=> tempRoles.Where(x => x.Key.UserId == userId).Select(x => x.Key).ToList();

		public bool HasTempRole(ulong userId, ulong roleID)
		{
			return tempRoles.Any(x => x.Key.UserId == userId && x.Key.RoleId == roleID);
		}

		public static bool GetTempRole(ulong userId, ulong roleId, out TempRole role)
		{
			var result = tempRoles.Keys.FirstOrDefault(x => x.UserId == userId && x.RoleId == roleId);

			role = result;

			return !(result is null);
		}

		public async Task RestoreTempRoles(SocketGuildUser sgUser)
		{
            RiftBot.Log.Info($"User {sgUser} ({sgUser.Id}) joined, checking temp roles");

			var userTempRoles = GetTempRoles(sgUser.Id);

			if (userTempRoles is null)
			{
                RiftBot.Log.Debug($"No temp roles for user {sgUser}");
				return;
			}

			foreach(var tempRole in userTempRoles)
			{
				if (sgUser.Roles.Any(x => x.Id == tempRole.RoleId))
				{
                    RiftBot.Log.Error($"User {sgUser} already has temp role {tempRole.RoleId}");
					continue;
				}

				if (!IonicClient.GetRole(Settings.App.MainGuildId, tempRole.RoleId, out var role))
				{
                    RiftBot.Log.Error($"Applying role {tempRole.RoleId}: FAILED");
					continue;
				}

				await sgUser.AddRoleAsync(role);
                RiftBot.Log.Debug($"Successfully added temp role \"{role.Name}\" for user {sgUser}");
			}
		}

		public async Task AddEventRoleAsync(ulong userId, ulong roleId)
		{
			var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

			if (sgUser is null)
				return;

			if (!IonicClient.GetRole(Settings.App.MainGuildId, roleId, out var role))
			{
                RiftBot.Log.Error($"Failed to get server role {roleId} for user {sgUser.Id}.");
				return;
			}

			if (!(role is SocketRole socketRole))
			{
                RiftBot.Log.Error($"Failed to cast IRole {roleId} to SocketRole.");
				return;
			}

			try
			{
				foreach (var user in socketRole.Members)
				{
					await user.RemoveRoleAsync(role);
				}

				await sgUser.AddRoleAsync(role);
			}
			catch (Exception ex)
			{
                RiftBot.Log.Error(ex, $"Failed to set role {roleId} for user {sgUser.Id}.");
				return;
			}

            RiftBot.Log.Info($"Granted temp role {role.Id} to user {sgUser.Id}.");
		}

		public async Task<bool> AddTempRoleAsync(TempRole role, bool stack = false)
		{
			var existing = tempRoles.Keys.FirstOrDefault(x => x.UserId == role.UserId && x.RoleId == role.RoleId);

			if (!(existing is null))
			{
				await RemoveRoleAsync(existing);

				if (stack)
					role.UntilTimestamp = (IonicLib.Extensions.Helper.FromTimestamp(role.UntilTimestamp) + role.Duration).ToUnixTimestamp();

				return tempRoles.TryAdd(role, default);
			}

			await AddRoleAsync(role.UserId, role.RoleId);

			bool success = tempRoles.TryAdd(role, default);

			if (success)
				await Task.Run(SaveAsync);

			return success;
		}

		public async Task AddRoleAsync(ulong userId, ulong roleId)
		{
			var sgUser = IonicClient.GetGuildUserById(Settings.App.MainGuildId, userId);

			if (sgUser is null)
				return;

			if (!IonicClient.GetRole(Settings.App.MainGuildId, roleId, out var role))
			{
                RiftBot.Log.Error($"Failed to get server role {roleId} for user {sgUser.Id}.");
				return;
			}

			try
			{
				await sgUser.AddRoleAsync(role);
			}
			catch(Exception ex)
			{
                RiftBot.Log.Error(ex, $"Failed to set role {roleId} for user {sgUser.Id}.");
				return;
			}

            RiftBot.Log.Info($"Granted temp role {role.Id} to user {sgUser.Id}.");
		}

		public async Task SaveAsync()
		{
			if (File.Exists(TempRoleFilePath))
				File.Delete(TempRoleFilePath);

			var list = tempRoles.Select(x => x.Key).ToList();

			string json = JsonConvert.SerializeObject(list, Formatting.Indented);

			await File.WriteAllTextAsync(TempRoleFilePath, json);
		}
	}
}