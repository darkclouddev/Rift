using System.Threading.Tasks;

using Rift.Configuration;

using IonicLib;

using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace Rift.Services
{
    public class ChannelService
    {
        const ulong VoiceCategoryId = 360570328197496833ul;

        public ChannelService(DiscordSocketClient client)
        {
            client.UserVoiceStateUpdated += Client_UserVoiceStateUpdated;
        }

        static async Task Client_UserVoiceStateUpdated(SocketUser user, SocketVoiceState fromState, SocketVoiceState toState)
        {
            var prevChannelNull = fromState.VoiceChannel is null;
            var nextChannelNull = toState.VoiceChannel is null;
            var joinedSetupChannel = !nextChannelNull && toState.VoiceChannel.Id == Settings.ChannelId.VoiceSetup;

            if (joinedSetupChannel)
            {
                (var isSuccess, var channel) = await CreateRoomForUser(user);

                if (isSuccess && user is SocketGuildUser sgUser)
                {
                    await sgUser.ModifyAsync(x => { x.Channel = channel; });

                    if (sgUser.VoiceChannel is null)
                    {
                        await channel.DeleteAsync();
                    }
                }
            }

            bool leftChannel = !prevChannelNull && (nextChannelNull || toState.VoiceChannel.Id != fromState.VoiceChannel.Id);

            if (leftChannel)
            {
                if (fromState.VoiceChannel.CategoryId != VoiceCategoryId)
                    return;

                if (fromState.VoiceChannel.Id == Settings.ChannelId.VoiceSetup)
                    return;

                if (fromState.VoiceChannel.Users.Count > 0)
                    return;

                await fromState.VoiceChannel.DeleteAsync();
            }
        }

        static async Task<(bool, RestVoiceChannel)> CreateRoomForUser(IUser user)
        {
            if (!IonicClient.GetCategory(Settings.App.MainGuildId, VoiceCategoryId, out var category))
                return (false, null);

            var channel = await category.Guild.CreateVoiceChannelAsync(user.Username, x =>
            {
                x.UserLimit = 5;
                x.CategoryId = VoiceCategoryId;
            });

            await channel.AddPermissionOverwriteAsync(user, new OverwritePermissions(manageChannel: PermValue.Allow));

            return (true, channel);
        }
    }
}
