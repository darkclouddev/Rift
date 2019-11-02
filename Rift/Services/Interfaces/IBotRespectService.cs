using System;
using System.Threading.Tasks;

using Rift.Events;

namespace Rift.Services.Interfaces
{
    public interface IBotRespectService
    {
        event EventHandler<ActivatedBotRespectsEventArgs> ActivatedBotRespects;
        Task ActivateAsync(ulong userId);
    }
}
