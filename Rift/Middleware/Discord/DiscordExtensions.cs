using System;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Rift.Middleware.Discord
{
    public static class DiscordExtensions
    {
        public static AuthenticationBuilder AddDiscord(this AuthenticationBuilder builder)
            => builder.AddDiscord(DiscordSettings.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddDiscord(this AuthenticationBuilder builder,
            Action<DiscordOptions> configureOptions)
            => builder.AddDiscord(DiscordSettings.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddDiscord(this AuthenticationBuilder builder, string authenticationScheme,
            Action<DiscordOptions> configureOptions)
            => builder.AddDiscord(authenticationScheme, DiscordSettings.DisplayName, configureOptions);

        public static AuthenticationBuilder AddDiscord(this AuthenticationBuilder builder, string authenticationScheme,
            string displayName, Action<DiscordOptions> configureOptions)
            => builder.AddOAuth<DiscordOptions, DiscordHandler>(authenticationScheme, displayName, configureOptions);
    }
}
