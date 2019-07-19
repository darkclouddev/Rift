using Rift.Services;

namespace Rift.Modules
{
    public class VoteModule : RiftModuleBase
    {
        readonly VoteService voteService;

        public VoteModule(VoteService voteService)
        {
            this.voteService = voteService;
        }
    }
}
