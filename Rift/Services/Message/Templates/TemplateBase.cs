using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates
{
    public abstract class TemplateBase : ITemplate
    {
        public string Template { get; }
        string TemplateWithoutPrefix { get; }
        const string Prefix = "$";
        const string ErrorTemplate = "Template error (\"{0}\"): {1}";

        protected TemplateBase(string template)
        {
            TemplateWithoutPrefix = template;
            Template = $"{Prefix}{TemplateWithoutPrefix}";
        }
        
        public abstract Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data);

        protected Task<RiftMessage> ReplaceDataAsync(RiftMessage message, string replacement)
        {
            if (message.Text != null)
                message.Text = message.Text.Replace(Template, replacement);

            if (message.Embed != null)
                message.Embed = message.Embed.Replace(Template, replacement);

            return Task.FromResult(message);
        }

        protected void TemplateError(string message)
        {
            RiftBot.Log.Error(string.Format(ErrorTemplate, TemplateWithoutPrefix, message));
        }
    }
}
