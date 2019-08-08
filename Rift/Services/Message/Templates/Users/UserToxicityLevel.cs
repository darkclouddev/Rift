using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Users
{
    public class UserToxicityLevel : TemplateBase
    {
        public UserToxicityLevel() : base(nameof(UserToxicityLevel))
        {
        }

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var toxicity = await DB.Toxicity.GetAsync(data.UserId);
            return await ReplaceDataAsync(message, toxicity.Level.ToString());
        }
    }
}
