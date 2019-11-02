using System;
using System.Threading.Tasks;

using Rift.Events;

namespace Rift.Services.Interfaces
{
    public interface IGiveawayService
    {
        event EventHandler<GiveawaysParticipatedEventArgs> GiveawaysParticipated;
        Task StartGiveawayAsync(string name, ulong startedById);
        Task StartTicketGiveawayAsync(int rewardId, ulong startedBy);
        Task GiveTicketsToLowLevelUsersAsync(ulong startedBy);
    }
}
