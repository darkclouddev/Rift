using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace Rift.Middleware.Discord
{
    public class DiscordOptions : OAuthOptions
    {
        public string AppId
        {
            get => ClientId;
            set => ClientId = value;
        }

        public string AppSecret
        {
            get => ClientSecret;
            set => ClientSecret = value;
        }
        
        public DiscordOptions()
        {
            CallbackPath = new PathString("/signin-discord");
            AuthorizationEndpoint = DiscordSettings.AuthorizationEndpoint;
            TokenEndpoint = DiscordSettings.TokenEndpoint;
            UserInformationEndpoint = DiscordSettings.UserInformationEndpoint;
            Scope.Add("identify");

            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id", ClaimValueTypes.UInteger64);
            ClaimActions.MapJsonKey(ClaimTypes.Name, "username", ClaimValueTypes.String);
            ClaimActions.MapJsonKey(ClaimTypes.Email, "email", ClaimValueTypes.Email);
            ClaimActions.MapJsonKey("urn:discord:discriminator", "discriminator", ClaimValueTypes.UInteger32);
            ClaimActions.MapJsonKey("urn:discord:avatar", "avatar", ClaimValueTypes.String);
            ClaimActions.MapJsonKey("urn:discord:verified", "verified", ClaimValueTypes.Boolean);
        }
    }
}
