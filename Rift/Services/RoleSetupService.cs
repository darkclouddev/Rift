using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using IonicLib.Extensions;

using Rift.Services.Interfaces;

namespace Rift.Services
{
    public class RoleSetupService : IRoleSetupService
    {
        const string TopEmoteName = "r1";
        const string JungleEmoteName = "r2";
        const string MidEmoteName = "r3";
        const string BotEmoteName = "r4";
        const string SupportEmoteName = "r5";
        const string FillEmoteName = "r6";
        const string HuntersEmoteName = "swords";
        const ulong PositionsMessageId = 606528013022003267ul;
        const ulong HuntersMessageId = 608748661773697051ul;
        
        static readonly int[] StarterRoles = { 81, 87, 88 };

        readonly DiscordSocketClient discordClient;
        readonly IRoleService roleService;
        
        public RoleSetupService(DiscordSocketClient discordClient,
                                IRoleService roleService)
        {
            this.discordClient = discordClient;
            this.roleService = roleService;
        }

        void SetupEvents()
        {
            discordClient.ReactionAdded += ReactionAdded;
            discordClient.ReactionRemoved += ReactionRemoved;
        }

        async Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            await OnReactionAsync(true, message, channel, reaction);
        }
        async Task ReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            await OnReactionAsync(false, message, channel, reaction);
        }
        
        async Task OnReactionAsync(bool added, Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var msg = await message.GetOrDownloadAsync();
            if (msg is null)
                return;

            if (reaction is null)
                return;

            var userId = reaction.UserId;
            
            if (msg.Id == PositionsMessageId)
            {
                await LeagueReactionAsync(userId, added, reaction.Emote.Name);
                return;
            }

            if (msg.Id == HuntersMessageId)
            {
                await HuntersReactionAsync(userId, added, reaction.Emote.Name);
                return;
            }
        }
        
        async Task LeagueReactionAsync(ulong userId, bool added, string emoteName)
        {
            if (added) 
                await TryGiveStarterRoles(userId);

            switch (emoteName)
            {
                case TopEmoteName:
                    await (added ? AddTopAsync(userId) : RemoveTopAsync(userId));
                    break;
                
                case JungleEmoteName:
                    await (added ? AddJungleAsync(userId) : RemoveJungleAsync(userId));
                    break;
                
                case MidEmoteName:
                    await (added ? AddMidAsync(userId) : RemoveMidAsync(userId));
                    break;
                
                case BotEmoteName:
                    await (added ? AddBotAsync(userId) : RemoveBotAsync(userId));
                    break;
                
                case SupportEmoteName:
                    await (added ? AddSupportAsync(userId) : RemoveSupportAsync(userId));
                    break;
                
                case FillEmoteName:
                    await (added ? AddFillAsync(userId) : RemoveFillAsync(userId));
                    break;
                
                default:
                    return;
            }
        }
        async Task HuntersReactionAsync(ulong userId, bool added, string emoteName)
        {
            if (!emoteName.Equals(HuntersEmoteName))
                return;

            await (added ? AddHuntersAsync(userId) : RemoveHuntersAsync(userId));
        }
        
        async Task TryGiveStarterRoles(ulong userId)
        {
            var dbUser = await DB.Users.GetAsync(userId);
            
            if (await DB.RoleInventory.HasAnyAsync(dbUser.UserId, StarterRoles))
                return;
            
            var roleId = StarterRoles.Random();
            
            await DB.RoleInventory.AddAsync(userId, roleId, "Starter role");
            var dbRole = await DB.Roles.GetAsync(roleId);
            await roleService.AddPermanentRoleAsync(userId, dbRole.RoleId);
        }

        async Task AddHuntersAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(39);
            await AddRoleAsync(userId, role.RoleId);
        }
        async Task RemoveHuntersAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(39);
            await RemoveRoleAsync(userId, role.RoleId);
        }
        
        async Task AddTopAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(16);
            await AddRoleAsync(userId, role.RoleId);
        }
        async Task AddJungleAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(65);
            await AddRoleAsync(userId, role.RoleId);
        }
        async Task AddMidAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(4);
            await AddRoleAsync(userId, role.RoleId);
        }
        async Task AddBotAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(61);
            await AddRoleAsync(userId, role.RoleId);
        }
        async Task AddSupportAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(77);
            await AddRoleAsync(userId, role.RoleId);
        }
        async Task AddFillAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(56);
            await AddRoleAsync(userId, role.RoleId);
        }

        async Task RemoveTopAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(16);
            await RemoveRoleAsync(userId, role.RoleId);
        }
        async Task RemoveJungleAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(65);
            await RemoveRoleAsync(userId, role.RoleId);
        }
        async Task RemoveMidAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(4);
            await RemoveRoleAsync(userId, role.RoleId);
        }
        async Task RemoveBotAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(61);
            await RemoveRoleAsync(userId, role.RoleId);
        }
        async Task RemoveSupportAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(77);
            await RemoveRoleAsync(userId, role.RoleId);
        }
        async Task RemoveFillAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(56);
            await RemoveRoleAsync(userId, role.RoleId);
        }

        async Task AddRoleAsync(ulong userId, ulong roleId)
        {
            await roleService.AddPermanentRoleAsync(userId, roleId);
        }
        async Task RemoveRoleAsync(ulong userId, ulong roleId)
        {
            await roleService.RemovePermanentRoleAsync(userId, roleId);
        }
    }
}
