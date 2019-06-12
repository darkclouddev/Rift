using System;

namespace Rift.Middleware.Discord
{
    public static class DiscordSettings
    {
        public static readonly string AuthenticationScheme = "Discord";
        public static readonly string DisplayName = "Discord";

        public static readonly string AuthorizationEndpoint = "https://discordapp.com/api/oauth2/authorize";
        public static readonly string TokenEndpoint = "https://discordapp.com/api/oauth2/token";
        public static readonly string UserInformationEndpoint = "https://discordapp.com/api/users/@me";
    }
}
