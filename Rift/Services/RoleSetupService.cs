using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using IonicLib.Extensions;

namespace Rift.Services
{
    public class RoleSetupService
    {
        const string TopEmoteName = "r1";
        const string JungleEmoteName = "r2";
        const string MidEmoteName = "r3";
        const string BotEmoteName = "r4";
        const string SupportEmoteName = "r5";
        const string FillEmoteName = "r6";
        const ulong MessageId = 606229842605899776ul;
        
        static readonly int[] StarterRoles = { 86, 87, 88 };
        
        public RoleSetupService(DiscordSocketClient client)
        {
            client.ReactionAdded += ReactionAdded;
            client.ReactionRemoved += ReactionRemoved;
        }

        static async Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var msg = await message.GetOrDownloadAsync();
            if (msg is null)
                return;

            if (msg.Id != MessageId)
                return;

            if (reaction is null)
                return;

            var userId = reaction.UserId;

            await GiveStarterRoles(userId);

            switch (reaction.Emote.Name)
            {
                case TopEmoteName:
                    await AddTopAsync(userId);
                    break;
                
                case JungleEmoteName:
                    await AddJungleAsync(userId);
                    break;
                
                case MidEmoteName:
                    await AddMidAsync(userId);
                    break;
                
                case BotEmoteName:
                    await AddBotAsync(userId);
                    break;
                
                case SupportEmoteName:
                    await AddSupportAsync(userId);
                    break;
                
                case FillEmoteName:
                    await AddFillAsync(userId);
                    break;
                
                default:
                    return;
            }
        }
        static async Task ReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var msg = await message.GetOrDownloadAsync();
            if (msg is null)
                return;

            if (msg.Id != MessageId)
                return;

            if (reaction is null)
                return;

            var userId = reaction.UserId;
            
            switch (reaction.Emote.Name)
            {
                case TopEmoteName:
                    await RemoveTopAsync(userId);
                    break;
                
                case JungleEmoteName:
                    await RemoveJungleAsync(userId);
                    break;
                
                case MidEmoteName:
                    await RemoveMidAsync(userId);
                    break;
                
                case BotEmoteName:
                    await RemoveBotAsync(userId);
                    break;
                
                case SupportEmoteName:
                    await RemoveSupportAsync(userId);
                    break;
                
                case FillEmoteName:
                    await RemoveFillAsync(userId);
                    break;
                
                default:
                    return;
            }
        }

        static async Task GiveStarterRoles(ulong userId)
        {
            var dbUser = await DB.Users.GetAsync(userId);
            
            if (await DB.RoleInventory.HasAnyAsync(dbUser.UserId, StarterRoles))
                return;
            
            var roleId = StarterRoles.Random();
            
            await DB.RoleInventory.AddAsync(userId, StarterRoles.Random(), "Starter role");
            var dbRole = await DB.Roles.GetAsync(roleId);
            await RiftBot.GetService<RoleService>().AddPermanentRoleAsync(userId, dbRole.RoleId);
        }

        static async Task AddTopAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(16);
            await AddRoleAsync(userId, role.RoleId);
        }
        static async Task AddJungleAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(65);
            await AddRoleAsync(userId, role.RoleId);
        }
        static async Task AddMidAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(4);
            await AddRoleAsync(userId, role.RoleId);
        }
        static async Task AddBotAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(61);
            await AddRoleAsync(userId, role.RoleId);
        }
        static async Task AddSupportAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(77);
            await AddRoleAsync(userId, role.RoleId);
        }
        static async Task AddFillAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(56);
            await AddRoleAsync(userId, role.RoleId);
        }

        static async Task RemoveTopAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(16);
            await RemoveRoleAsync(userId, role.RoleId);
        }
        static async Task RemoveJungleAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(65);
            await RemoveRoleAsync(userId, role.RoleId);
        }
        static async Task RemoveMidAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(4);
            await RemoveRoleAsync(userId, role.RoleId);
        }
        static async Task RemoveBotAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(61);
            await RemoveRoleAsync(userId, role.RoleId);
        }
        static async Task RemoveSupportAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(77);
            await RemoveRoleAsync(userId, role.RoleId);
        }
        static async Task RemoveFillAsync(ulong userId)
        {
            var role = await DB.Roles.GetAsync(56);
            await RemoveRoleAsync(userId, role.RoleId);
        }

        static async Task AddRoleAsync(ulong userId, ulong roleId)
        {
            await RiftBot.GetService<RoleService>().AddPermanentRoleAsync(userId, roleId);
        }
        static async Task RemoveRoleAsync(ulong userId, ulong roleId)
        {
            await RiftBot.GetService<RoleService>().RemovePermanentRoleAsync(userId, roleId);
        }
    }
}
