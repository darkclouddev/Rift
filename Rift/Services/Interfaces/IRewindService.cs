using System.Threading.Tasks;

namespace Rift.Services.Interfaces
{
    public interface IRewindService
    {
        Task ActivateAsync(ulong userId);
        
    }
}
