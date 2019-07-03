using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Users
{
    public class Mention : TemplateBase
    {
        public Mention() : base(nameof(Mention)) {}

        public override Task<RiftMessage> Apply(RiftMessage message, FormatData data)
        {
            return ReplaceData(message, $"<@{data.UserId.ToString()}>");
        }
    }
}
