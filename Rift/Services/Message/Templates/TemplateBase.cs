using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates
{
    public abstract class TemplateBase
    {
        public string Template { get; }
        string TemplateWithoutPrefix { get; }
        const string Prefix = "$";

        protected TemplateBase(string template)
        {
            TemplateWithoutPrefix = template;
            Template = $"{Prefix}{TemplateWithoutPrefix}";
        }

        public abstract Task<RiftMessage> Apply(RiftMessage message, FormatData data);

        protected Task<RiftMessage> ReplaceData(RiftMessage message, string replacement)
        {
            if (message.Text != null)
                message.Text = message.Text.Replace(Template, replacement);
            
            if (message.Embed != null)
                message.Embed = message.Embed.Replace(Template, replacement);

            return Task.FromResult(message);
        }

        protected void TemplateError(string message)
        {
            RiftBot.Log.Error($"Template error (\"{TemplateWithoutPrefix}\"): {message}");
        }
    }
}
