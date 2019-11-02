using System;
using System.Threading.Tasks;

using Rift.Events;

namespace Rift.Services.Interfaces
{
    public interface IChestService
    {
        event EventHandler<ChestsOpenedEventArgs> ChestsOpened;
        Task OpenAsync(ulong userId, uint amount);
        Task OpenAllAsync(ulong userId);
    }
}
