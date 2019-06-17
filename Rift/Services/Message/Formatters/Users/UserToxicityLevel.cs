using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Users
{
    public class UserToxicityLevel : FormatterBase
    {
        public UserToxicityLevel() : base("$userToxicityLevel") {}
        
        public override async Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var toxicity = await Database.GetToxicityAsync(data.UserId);
            return await ReplaceData(message, toxicity.Level.ToString());
        }
    }
}
