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
using Rift.Services.Interfaces;
using Rift.Util;

using Settings = Rift.Configuration.Settings;

namespace Rift.Services
{
    public class ChannelService : IChannelService
    {
        const ulong VoiceCategoryId = 360570328197496833ul;
        static Timer voiceUptimeTimer;
        static readonly TimeSpan VoiceRewardsInterval = TimeSpan.FromMinutes(5);
        const string TimerName = "voice-uptime";

        readonly IRewardService rewardService;
        
        public ChannelService(IRewardService rewardService)
        {
            this.rewardService = rewardService;
            
            Task.Run(async () =>
            {
                var timer = await DB.SystemTimers.GetAsync(TimerName);

                if (timer is null)
                {
                    RiftBot.Log.Error($"Failed to get system timer \"{TimerName}\"");
                    return;
                }

                var delay = DateTime.UtcNow - timer.LastInvoked > timer.Interval
                    ? TimeSpan.Zero
                    : timer.Interval - (DateTime.UtcNow - timer.LastInvoked);

                voiceUptimeTimer = new Timer(async delegate { await StartAsync(); },
                    null,
                    delay,
                    timer.Interval);
            });
        }
        
        async Task StartAsync()
        {
            await DB.SystemTimers.UpdateAsync(TimerName, DateTime.UtcNow);
            await UpdateUsersVoiceUptimeAsync();
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

                    if (sgUser.VoiceChannel is null)
                        await channel.DeleteAsync();
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
            if (!IonicHelper.GetCategory(Settings.App.MainGuildId, VoiceCategoryId, out var category))
                return (false, null);

            var channel = await category.Guild.CreateVoiceChannelAsync(user.Username, x =>
            {
                x.UserLimit = 5;
                x.CategoryId = VoiceCategoryId;
            });

            await channel.AddPermissionOverwriteAsync(user, new OverwritePermissions(manageChannel: PermValue.Allow));

            return (true, channel);
        }

        async Task UpdateUsersVoiceUptimeAsync()
        {
            if (!IonicHelper.GetGuild(Settings.App.MainGuildId, out var guild))
                return;

            var channels = guild.VoiceChannels
                .Where(x => x.CategoryId == VoiceCategoryId && x.Id != Settings.ChannelId.VoiceSetup)
                .ToList();

            if (!channels.Any())
                return;

            var users = new List<ulong>();

            foreach (var channel in channels)
            {
                if (channel.Users.Count <= 1)
                    continue;
                
                if (channel.UserLimit == 1)
                    continue;

                foreach (var sgUser in channel.Users)
                {
                    if (!sgUser.VoiceState.HasValue)
                        continue;
                    
                    var voiceState = sgUser.VoiceState.Value;
                    
                    if (voiceState.IsMuted || voiceState.IsDeafened
                        || voiceState.IsSelfMuted || voiceState.IsSelfDeafened)
                        continue;
                    
                    users.Add(sgUser.Id);
                }
            }

            if (!users.Any())
                return;

            var dbReward = await DB.Rewards.GetAsync(24);
            var reward = dbReward.ItemReward;
            
            foreach (var userId in users)
            {
                await rewardService.DeliverToAsync(userId, reward);
                await DB.Statistics.AddAsync(userId, new StatisticData {VoiceUptime = VoiceRewardsInterval});
            }

            RiftBot.Log.Information($"Gived out voice online rewards for {users.Count.ToString()} user(s).");
        }

        public async Task DenyAccessToUserAsync(IUser roomOwnerUser, IUser targetUser)
        {
            if (!(roomOwnerUser is SocketGuildUser roomOwnerSgUser)
                || !(targetUser is SocketGuildUser targetSgUser))
                return;
            
            if (roomOwnerSgUser.VoiceChannel is null)
                return;
            
            if (targetSgUser.VoiceChannel is null)
                return;

            if (roomOwnerSgUser.VoiceChannel.Id != targetSgUser.VoiceChannel.Id)
                return;
            
            var channel = roomOwnerSgUser.VoiceChannel;

            var channelPerms = channel.GetPermissionOverwrite(roomOwnerSgUser);

            if (!channelPerms.HasValue || channelPerms.Value.ManageChannel != PermValue.Allow)
                return;
            
            var targetPerms = channel.GetPermissionOverwrite(targetSgUser);

            if (targetPerms.HasValue && targetPerms.Value.Connect == PermValue.Deny)
                return;

            try
            {
                var permissions = new OverwritePermissions(connect: PermValue.Deny);
                await channel.AddPermissionOverwriteAsync(targetSgUser, permissions);
            }
            catch (Exception ex)
            {
                RiftBot.Log.Warning(ex, "Channel was disposed while executing command, skipping");
                return;
            }
            
            try
            {
                if (!IonicHelper.GetVoiceChannel(Settings.App.MainGuildId, Settings.ChannelId.Afk, out var afkChannel))
                    return;
                    
                await targetSgUser.ModifyAsync(x => { x.Channel = afkChannel; });
            }
            catch (Exception ex)
            {
                RiftBot.Log.Warning(ex, $"User {targetSgUser.ToLogString()} wasn't in any channel when kick issued, skipping");
                return;
            }
            
            // TODO: done
        }
    }
}
