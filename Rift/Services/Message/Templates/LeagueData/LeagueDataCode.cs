using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.LeagueData
{
    public class LeagueDataCode : TemplateBase
    {
        public LeagueDataCode() : base(nameof(LeagueDataCode))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, data.LeagueRegistration.ConfirmationCode);
        }
    }
}
