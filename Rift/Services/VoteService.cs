using System;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Services.Message;

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
                        await RiftBot.SendMessageAsync("vote-cooldown-community", Settings.ChannelId.Comms, new FormatData(userId));
                        return;
                    }

                    break;

                case VoteType.Team:
                    if (cds.LastTeamVoteTimeSpan != TimeSpan.Zero)
                    {
                        await RiftBot.SendMessageAsync("vote-cooldown-team", Settings.ChannelId.Comms, new FormatData(userId));
                        return;
                    }

                    break;

                case VoteType.Streamer:
                    if (cds.LastStreamerVoteTimeSpan != TimeSpan.Zero)
                    {
                        await RiftBot.SendMessageAsync("vote-cooldown-streamer", Settings.ChannelId.Comms, new FormatData(userId));
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
                            await RiftBot.SendMessageAsync("vote-already-community", Settings.ChannelId.Comms, new FormatData(userId));
                            return;
                        }

                        break;

                    case VoteType.Team:
                        if (votes.TeamId == (int) voteId)
                        {
                            await RiftBot.SendMessageAsync("vote-already-team", Settings.ChannelId.Comms, new FormatData(userId));
                            return;
                        }

                        break;

                    case VoteType.Streamer:
                        if (votes.StreamerId == voteId)
                        {
                            await RiftBot.SendMessageAsync("vote-already-streamer", Settings.ChannelId.Comms, new FormatData(userId));
                            return;
                        }

                        break;
                }
            }

            switch (type)
            {
                case VoteType.Community:
                    await VoteCommunityAsync(userId, (int) voteId);
                    break;

                case VoteType.Team:
                    await VoteTeamAsync(userId, (int) voteId);
                    break;

                case VoteType.Streamer:
                    await VoteStreamerAsync(userId, voteId);
                    break;
            }
        }

        static async Task VoteCommunityAsync(ulong userId, int id)
        {
            var community = await DB.Communities.GetAsync(id);

            if (community is null)
            {
                await RiftBot.SendMessageAsync(MessageService.Error, Settings.ChannelId.Comms);
                return;
            }

            // TODO: give rewards

            await RiftBot.SendMessageAsync("vote-success-community", Settings.ChannelId.Comms, new FormatData(userId)
            {
                VoteData = new VoteData
                {
                    Name = community.Name
                }
            });
        }

        static async Task VoteTeamAsync(ulong userId, int id)
        {
            var team = await DB.Teams.GetAsync(id);

            if (team is null)
            {
                await RiftBot.SendMessageAsync(MessageService.Error, Settings.ChannelId.Comms);
                return;
            }

            // TODO: give rewards

            await RiftBot.SendMessageAsync("vote-success-team", Settings.ChannelId.Comms, new FormatData(userId)
            {
                VoteData = new VoteData
                {
                    Name = team.Name
                }
            });
        }

        static async Task VoteStreamerAsync(ulong userId, ulong id)
        {
            var streamer = await DB.Streamers.GetAsync(id);

            if (streamer is null)
            {
                await RiftBot.SendMessageAsync(MessageService.Error, Settings.ChannelId.Comms);
                return;
            }

            // TODO: give rewards

            await RiftBot.SendMessageAsync("vote-success-streamer", Settings.ChannelId.Comms, new FormatData(userId)
            {
                VoteData = new VoteData
                {
                    Name = streamer.Name
                }
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
