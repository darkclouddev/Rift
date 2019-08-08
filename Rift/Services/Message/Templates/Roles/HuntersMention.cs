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

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var role = await DB.Roles.GetAsync(39);
            
            return await ReplaceDataAsync(message, $"<@&{role.RoleId.ToString()}>");
        }
    }
}
