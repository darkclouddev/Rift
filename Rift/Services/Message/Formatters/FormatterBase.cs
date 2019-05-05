using System;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters
{
    public abstract class FormatterBase
    {
        protected string Template { get; }

        public FormatterBase(string template)
        {
            Template = template;
        }

        public abstract RiftMessage Format(RiftMessage message, ulong userId);

        protected RiftMessage ReplaceData(RiftMessage message, string replacement)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));
            
            if (string.IsNullOrWhiteSpace(replacement))
                throw new ArgumentNullException(nameof(replacement));
            
            if (message.Text != null)
                message.Text = message.Text.Replace(Template, replacement);
            
            if (message.Embed != null)
                message.Embed = message.Embed.Replace(Template, replacement);

            return message;
        }
    }
}
