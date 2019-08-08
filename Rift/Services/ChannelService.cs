using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Discord;
using Discord.Rest;
using Discord.WebSocket;

using IonicLib;

using Rift.Database;
using Rift.Util;

using Settings = Rift.Configuration.Settings;

namespace Rift.Services
{
    public class ChannelService
    {
        const ulong VoiceCategoryId = 360570328197496833ul;
        static Timer voiceUptimeTimer;
        static readonly TimeSpan VoiceRewardsInterval = TimeSpan.FromMinutes(5);

        public ChannelService(DiscordSocketClient client)
        {
            voiceUptimeTimer = new Timer(
                async delegate { await UpdateUsersVoiceUptimeAsync(); },
                null,
                TimeSpan.FromSeconds(15),
                VoiceRewardsInterval);
        }

        static async Task ManageChannelsAsync(SocketUser user, SocketVoiceState fromState, SocketVoiceState toState)
        {
            var prevChannelNull = fromState.VoiceChannel is null;
            var nextChannelNull = toState.VoiceChannel is null;
            var joinedSetupChannel = !nextChannelNull && toState.VoiceChannel.Id == Settings.ChannelId.VoiceSetup;

            if (joinedSetupChannel)
            {
                (var isSuccess, var channel) = await CreateRoomForUser(user);

                if (isSuccess && user is SocketGuildUser sgUser)
                {
                    await sgUser.ModifyAsync(x => { x.Channel = channel; });

                    if (sgUser.VoiceChannel is null) await channel.DeleteAsync();
                }
            }

            var leftChannel = !prevChannelNull && (nextChannelNull || toState.VoiceChannel.Id != fromState.VoiceChannel.Id);
            if (leftChannel)
            {
                if (fromState.VoiceChannel.CategoryId != VoiceCategoryId)
                    return;

                if (fromState.VoiceChannel.Id == Settings.ChannelId.VoiceSetup)
                    return;

                if (fromState.VoiceChannel.Users.Count > 0)
                    return;

                await fromState.VoiceChannel.DeleteAsync();
            }
        }

        static async Task<(bool, RestVoiceChannel)> CreateRoomForUser(IUser user)
        {
            if (!IonicClient.GetCategory(Settings.App.MainGuildId, VoiceCategoryId, out var category))
                return (false, null);

            var channel = await category.Guild.CreateVoiceChannelAsync(user.Username, x =>
            {
                x.UserLimit = 5;
                x.CategoryId = VoiceCategoryId;
            });

            await channel.AddPermissionOverwriteAsync(user, new OverwritePermissions(manageChannel: PermValue.Allow));

            return (true, channel);
        }

        static async Task UpdateUsersVoiceUptimeAsync()
        {
            if (!IonicClient.GetGuild(Settings.App.MainGuildId, out var guild))
                return;

            var channels = guild.VoiceChannels
                .Where(x => x.CategoryId == VoiceCategoryId && x.Id != Settings.ChannelId.VoiceSetup)
                .ToList();

            if (!channels.Any())
                return;

            var users = new List<ulong>();

            foreach (var channel in channels)
                users.AddRange(channel.Users.Select(x => x.Id));

            if (!users.Any())
                return;

            var reward = await DB.Rewards.GetAsync(24);

            foreach (var userId in users)
            {
                await reward.DeliverToAsync(userId);
                await DB.Statistics.AddAsync(userId, new StatisticData {VoiceUptime = VoiceRewardsInterval});
            }

            RiftBot.Log.Info($"Gived out voice online rewards for {users.Count.ToString()} user(s).");
        }

        public async Task DenyAccessToUserAsync(IUser roomOwnerUser, IUser targetUser)
        {
            if (!(roomOwnerUser is SocketGuildUser roomOwnerSgUser)
                || !(targetUser is SocketGuildUser targetSgUser))
                return;
            
            if (roomOwnerSgUser.VoiceChannel is null)
                return; // TODO: issuer not in any voice channel
            
            if (targetSgUser.VoiceChannel is null)
                return; // TODO: target is not in any voice channel

            if (roomOwnerSgUser.VoiceChannel.Id != targetSgUser.VoiceChannel.Id)
                return; // TODO: target is not in issuer's channel
            
            var channel = roomOwnerSgUser.VoiceChannel;

            var channelPerms = channel.GetPermissionOverwrite(roomOwnerSgUser);

            if (!channelPerms.HasValue || channelPerms.Value.ManageChannel != PermValue.Allow)
                return; // TODO: no rights to kick
            
            var targetPerms = channel.GetPermissionOverwrite(targetSgUser);

            if (targetPerms.HasValue && targetPerms.Value.Connect == PermValue.Deny)
                return; // TODO: already banned from this channel

            try
            {
                var permissions = new OverwritePermissions(connect: PermValue.Deny);
                await channel.AddPermissionOverwriteAsync(targetSgUser, permissions);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Warn("Channel was disposed while executing command, skipping");
                RiftBot.Log.Warn(ex);
                return;
            }
            
            try
            {
                if (!IonicClient.GetVoiceChannel(Settings.App.MainGuildId, Settings.ChannelId.Afk, out var afkChannel))
                    return;
                    
                await targetSgUser.ModifyAsync(x => { x.Channel = afkChannel; });
            }
            catch (Exception ex)
            {
                RiftBot.Log.Warn($"User {targetSgUser.ToLogString()} wasn't in any channel when kick issued, skipping");
                RiftBot.Log.Warn(ex);
                return;
            }
            
            // TODO: done
        }
    }
}
