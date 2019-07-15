using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Roles
{
    public class HuntersMention : TemplateBase
    {
        public HuntersMention() : base(nameof(HuntersMention))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, $"<@&{Settings.RoleId.Hunters.ToString()}>");
        }
    }
}
