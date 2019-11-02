using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Services.Interfaces;
using Rift.Services.Message;

namespace Rift.Services
{
    public class BackgroundService : IBackgroundService
    {
        readonly IMessageService messageService;
        readonly IRoleService roleService;
        
        Timer timer;
        const int NitroBoosterBackgroundId = 13;
        
        static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);

        const string InventoryIdentifier = "background-inventory-list";

        public BackgroundService(IMessageService messageService, IRoleService roleService)
        {
            this.messageService = messageService;
            this.roleService = roleService;
            
            timer = new Timer(
                async delegate { await TimerProcAsync(); },
                null,
                TimeSpan.FromSeconds(10),
                TimeSpan.FromMinutes(5));
        }
        
        async Task TimerProcAsync()
        {
            RiftBot.Log.Information("Checking nitro boosters integrity..");
            
            var nitroUsers = await roleService.GetNitroBoostersAsync();

            if (nitroUsers is null || nitroUsers.Count == 0)
            {
                await RemoveBackgroundsIfNotBoosting(new List<ulong>());
                return;
            }

            await FixMissingBackgroundsAsync(nitroUsers);
            await RemoveBackgroundsIfNotBoosting(nitroUsers);
        }

        static async Task FixMissingBackgroundsAsync(IEnumerable<ulong> users)
        {
            var fixedBackgrounds = 0;
            
            foreach (var userId in users)
            {
                if (!await DB.BackgroundInventory.HasAsync(userId, NitroBoosterBackgroundId))
                {
                    await DB.BackgroundInventory.AddAsync(userId, NitroBoosterBackgroundId);
                    fixedBackgrounds++;
                }
            }
            
            if (fixedBackgrounds > 0)
                RiftBot.Log.Information($"Added {fixedBackgrounds.ToString()} missing nitro backgrounds.");
        }

        static async Task RemoveBackgroundsIfNotBoosting(List<ulong> users)
        {
            var backOwners = await DB.BackgroundInventory.GetNitroBoostUsersAsync();
            
            if (backOwners is null || backOwners.Count == 0)
                return;

            var fixedBackgrounds = 0;
            
            foreach (var userId in backOwners)
            {
                if (!users.Contains(userId))
                {
                    await DB.BackgroundInventory.DeleteAsync(userId, 13);
                    await DB.Users.SetBackgroundAsync(userId, 0);
                    fixedBackgrounds++;
                }
            }
            
            if (fixedBackgrounds > 0)
                RiftBot.Log.Information($"Removed {fixedBackgrounds.ToString()} nitro backgrounds.");
        }

        public async Task GetInventoryAsync(ulong userId)
        {
            await messageService.SendMessageAsync(InventoryIdentifier, Settings.ChannelId.Commands, new FormatData(userId))
                         .ConfigureAwait(false);
        }

        public async Task SetActiveAsync(ulong userId, int backgroundId)
        {
            await Mutex.WaitAsync();
            
            try
            {
                await SetActiveInternalAsync(userId, backgroundId);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Error(ex, $"Failed to set active background {backgroundId.ToString()} for user {userId.ToString()}");
            }
            finally
            {
                Mutex.Release();
            }
        }
        
        async Task SetActiveInternalAsync(ulong userId, int backgroundId)
        {
            var setDefault = backgroundId == 0;
            
            if (!setDefault && !await DB.BackgroundInventory.HasAsync(userId, backgroundId))
            {
                await messageService.SendMessageAsync("backgrounds-wrongnumber", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            var dbUser = await DB.Users.GetAsync(userId);
            if (dbUser is null)
            {
                await messageService.SendMessageAsync(MessageService.UserNotFound, Settings.ChannelId.Commands);
                return;
            }
            
            if (!setDefault && dbUser.ProfileBackground == backgroundId)
            {
                await messageService.SendMessageAsync("backgrounds-alreadyactive", Settings.ChannelId.Commands, new FormatData(userId));
                return;
            }

            await DB.Users.SetBackgroundAsync(userId, backgroundId);
            await messageService.SendMessageAsync("backgrounds-set-success", Settings.ChannelId.Commands, new FormatData(userId));
        }
    }
}
