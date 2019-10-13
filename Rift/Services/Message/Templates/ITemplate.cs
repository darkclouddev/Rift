using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates
{
    public interface ITemplate
    {
        string Template { get; }
        Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data);
    }
}
