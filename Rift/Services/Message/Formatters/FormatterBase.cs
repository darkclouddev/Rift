using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters
{
    public abstract class FormatterBase
    {
        public string Template { get; }

        protected FormatterBase(string template)
        {
            Template = template;
        }

        public abstract Task<RiftMessage> Format(RiftMessage message, FormatData data);

        protected Task<RiftMessage> ReplaceData(RiftMessage message, string replacement)
        {
            if (message.Text != null)
                message.Text = message.Text.Replace(Template, replacement);
            
            if (message.Embed != null)
                message.Embed = message.Embed.Replace(Template, replacement);

            return Task.FromResult(message);
        }
    }
}
