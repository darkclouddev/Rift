using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Votes
{
    public class VoteName : TemplateBase
    {
        public VoteName() : base(nameof(VoteName))
        {
        }

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            return ReplaceDataAsync(message, data.VoteData.Name);
        }
    }
}
