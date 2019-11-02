using System;
using System.Threading.Tasks;

using Rift.Events;

namespace Rift.Services.Interfaces
{
    public interface ISphereService
    {
        event EventHandler<OpenedSphereEventArgs> OpenedSphere;
        Task OpenAsync(ulong userId);
    }
}
