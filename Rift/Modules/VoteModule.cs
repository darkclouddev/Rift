using System.Threading.Tasks;

using Discord.Commands;

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

        #region Communities
        
        [Command("GGS")]
        [RequireContext(ContextType.Guild)]
        public async Task VoteGGS()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Community, 1);
        }

        [Command("SPL")]
        [RequireContext(ContextType.Guild)]
        public async Task VoteSPL()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Community, 2);
        }

        [Command("runeterra")]
        [RequireContext(ContextType.Guild)]
        public async Task VoteRuneterra()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Community, 3);
        }

        [Command("kensiro")]
        [RequireContext(ContextType.Guild)]
        public async Task VoteKensiro()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Community, 4);
        }

        [Command("playmaker")]
        [RequireContext(ContextType.Guild)]
        public async Task VotePlaymaker()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Community, 5);
        }
        
        #endregion Communities
        
        #region Streamers

        [Command("teynor")]
        public async Task VoteTeynor()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Streamer, 232221573757534208ul);
        }

        [Command("etacarinae")]
        public async Task VoteEtaCarinae()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Streamer, 280628803351478274ul);
        }

        [Command("fantastictouch")]
        public async Task VoteFantasticTouch()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Streamer, 119837878225338370ul);
        }

        [Command("genes1s")]
        public async Task VoteGenes1s()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Streamer, 198486648995053569ul);
        }

        [Command("summonersinc")]
        public async Task VoteSummonersInc()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Streamer, 114315170175516673ul);
        }

        [Command("TeamleSS")]
        public async Task VoteTeamless()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Streamer, 170854011975630849ul);
        }

        [Command("Archie2b")]
        public async Task VoteArchie2b()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Streamer, 116637984454868992ul);
        }

        [Command("VioletFairy")]
        public async Task VoteVioletFairy()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Streamer, 161217284776394753ul);
        }

        [Command("Akeymu")]
        public async Task VoteAkeymu()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Streamer, 436153271900569611ul);
        }
        
        #endregion Streamers
        
        #region Teams
        
        [Command("UOL")]
        public async Task VoteUOL()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Team, 1);
        }
        
        [Command("M19")]
        public async Task VoteM19()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Team, 2);
        }
        
        [Command("Gambit")]
        public async Task VoteGambit()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Team, 3);
        }
        
        [Command("EPG")]
        public async Task VoteEPG()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Team, 4);
        }
        
        [Command("Vega")]
        public async Task VoteVega()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Team, 5);
        }
        
        [Command("DA")]
        public async Task VoteDA()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Team, 6);
        }
        
        [Command("VAE")]
        public async Task VoteVAE()
        {
            await voteService.VoteAsync(Context.User.Id, VoteType.Team, 7);
        }
        
        #endregion Teams
    }
}
