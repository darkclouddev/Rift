using System;
using System.Threading.Tasks;

using Rift.Events;

namespace Rift.Services.Interfaces
{
    public interface IBragService
    {
        event EventHandler<BragEventArgs> OnUserBrag;
        Task GetUserBragAsync(ulong userId);
    }
}
