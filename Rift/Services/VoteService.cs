using System;
using System.Threading;
using System.Threading.Tasks;

using Rift.Services.Message;
using Rift.Services.Reward;

using Settings = Rift.Configuration.Settings;

namespace Rift.Services
{
    public class VoteService
    {
        static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);

        public async Task VoteAsync(ulong userId, VoteType type, ulong voteId)
        {
            await Mutex.WaitAsync();

            try
            {
                await VoteInternalAsync(userId, type, voteId);
            }
            finally
            {
                Mutex.Release();
            }
        }
        
        async Task VoteInternalAsync(ulong userId, VoteType type, ulong voteId)
        {
            var cds = await DB.Cooldowns.GetAsync(userId);

            switch (type)
            {
                case VoteType.Community:
                    if (cds.LastCommunityVoteTimeSpan != TimeSpan.Zero)
                    {
                        await RiftBot.SendMessageAsync("vote-cooldown-community", Settings.ChannelId.Commands, new FormatData(userId));
                        return;
                    }

                    break;

                case VoteType.Team:
                    if (cds.TeamVoteTimeSpan != TimeSpan.Zero)
                    {
                        await RiftBot.SendMessageAsync("vote-cooldown-team", Settings.ChannelId.Commands, new FormatData(userId));
                        return;
                    }

                    break;

                case VoteType.Streamer:
                    if (cds.StreamerVoteTimeSpan != TimeSpan.Zero)
                    {
                        await RiftBot.SendMessageAsync("vote-cooldown-streamer", Settings.ChannelId.Commands, new FormatData(userId));
                        return;
                    }

                    break;
            }

            var votes = await DB.Votes.GetAsync(userId);

            if (!(votes is null))
            {
                switch (type)
                {
                    case VoteType.Community:
                        if (votes.CommunityId == (int) voteId)
                        {
                            await RiftBot.SendMessageAsync("vote-already-community", Settings.ChannelId.Commands, new FormatData(userId));
                            return;
                        }

                        break;

                    case VoteType.Team:
                        if (votes.TeamId == (int) voteId)
                        {
                            await RiftBot.SendMessageAsync("vote-already-team", Settings.ChannelId.Commands, new FormatData(userId));
                            return;
                        }

                        break;

                    case VoteType.Streamer:
                        if (votes.StreamerId == voteId)
                        {
                            await RiftBot.SendMessageAsync("vote-already-streamer", Settings.ChannelId.Commands, new FormatData(userId));
                            return;
                        }

                        break;
                }
            }

            var rewardId = 0;

            switch (type)
            {
                case VoteType.Community:
                    rewardId = 20;
                    break;

                case VoteType.Team:
                    rewardId = 21;
                    break;

                case VoteType.Streamer:
                    rewardId = 22;
                    break;
            }
            
            var dbReward = await DB.Rewards.GetAsync(rewardId);

            if (dbReward is null)
            {
                RiftBot.Log.Error($"Could not get reward ID {rewardId.ToString()}");
                return;
            }

            var reward = dbReward.ToRewardBase();
            
            switch (type)
            {
                case VoteType.Community:
                    await VoteCommunityAsync(userId, (int) voteId, reward);
                    break;

                case VoteType.Team:
                    await VoteTeamAsync(userId, (int) voteId, reward);
                    break;

                case VoteType.Streamer:
                    await VoteStreamerAsync(userId, voteId, reward);
                    break;
            }
        }

        static async Task VoteCommunityAsync(ulong userId, int id, RewardBase reward)
        {
            var community = await DB.Communities.GetAsync(id);

            if (community is null)
            {
                await RiftBot.SendMessageAsync(MessageService.Error, Settings.ChannelId.Commands);
                return;
            }

            await DB.Cooldowns.SetLastCommunityVoteTimeAsync(userId, DateTime.UtcNow);
            
            await DB.Votes.AddOrUpdateAsync(userId, communityId: id);

            await reward.DeliverToAsync(userId);
            
            await DB.BackgroundInventory.RemoveCommunitiesAsync(userId)
                .ContinueWith(async _ =>
                {
                    await DB.BackgroundInventory.UnsetCommunitiesBackgroundsAsync(userId);
                    await DB.BackgroundInventory.AddAsync(userId, community.BackgroundId);
                });

            await RiftBot.SendMessageAsync("vote-success-community", Settings.ChannelId.Commands, new FormatData(userId)
            {
                VoteData = new VoteData
                {
                    Name = community.Name
                },
                Reward = reward
            });
        }

        static async Task VoteTeamAsync(ulong userId, int id, RewardBase reward)
        {
            var team = await DB.Teams.GetAsync(id);

            if (team is null)
            {
                await RiftBot.SendMessageAsync(MessageService.Error, Settings.ChannelId.Commands);
                return;
            }
            
            await DB.Cooldowns.SetLastTeamVoteTimeAsync(userId, DateTime.UtcNow);
            
            await DB.Votes.AddOrUpdateAsync(userId, teamId: id);

            await reward.DeliverToAsync(userId);
            
            await DB.BackgroundInventory.RemoveTeamsAsync(userId)
                .ContinueWith(async _ =>
                {
                    await DB.BackgroundInventory.UnsetTeamsBackgroundsAsync(userId);
                    await DB.BackgroundInventory.AddAsync(userId, team.BackgroundId);
                });

            await RiftBot.SendMessageAsync("vote-success-team", Settings.ChannelId.Commands, new FormatData(userId)
            {
                VoteData = new VoteData
                {
                    Name = team.Name
                },
                Reward = reward
            });
        }

        static async Task VoteStreamerAsync(ulong userId, ulong id, RewardBase reward)
        {
            var streamer = await DB.Streamers.GetAsync(id);

            if (streamer is null)
            {
                await RiftBot.SendMessageAsync(MessageService.Error, Settings.ChannelId.Commands);
                return;
            }
            
            await DB.Cooldowns.SetLastStreamerVoteTimeAsync(userId, DateTime.UtcNow);

            await DB.Votes.AddOrUpdateAsync(userId, streamerId: id);

            await reward.DeliverToAsync(userId);
            
            await DB.BackgroundInventory.RemoveStreamersAsync(userId)
                .ContinueWith(async _ =>
                {
                    await DB.BackgroundInventory.UnsetStreamersBackgroundsAsync(userId);
                    await DB.BackgroundInventory.AddAsync(userId, streamer.BackgroundId);
                });

            await RiftBot.SendMessageAsync("vote-success-streamer", Settings.ChannelId.Commands, new FormatData(userId)
            {
                VoteData = new VoteData
                {
                    Name = streamer.Name
                },
                Reward = reward
            });
        }
    }

    public enum VoteType
    {
        Community,
        Team,
        Streamer,
    }
}
