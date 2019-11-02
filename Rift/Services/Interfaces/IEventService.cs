using System;
using System.Threading.Tasks;

using Rift.Data.Models;
using Rift.Events;

namespace Rift.Services.Interfaces
{
    public interface IEventService
    {
        event EventHandler<NormalMonstersKilledEventArgs> NormalMonstersKilled;
        event EventHandler<RareMonstersKilledEventArgs> RareMonstersKilled;
        event EventHandler<EpicMonstersKilledEventArgs> EpicMonstersKilled;

        Task StartAsync(string name, ulong startedById);
        Task StartAsync(RiftEvent dbEvent, ulong startedById);
    }
}
