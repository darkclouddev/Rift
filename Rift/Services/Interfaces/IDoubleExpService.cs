using System.Threading.Tasks;

namespace Rift.Services.Interfaces
{
    public interface IDoubleExpService
    {
        Task ActivateAsync(ulong userId);
    }
}
