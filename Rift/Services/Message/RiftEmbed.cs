using System;

using Discord;

namespace Rift.Services.Message
{
    public class RiftEmbed
    {
        public string Title { get; set; }
        public RiftAuthor Author { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public string ImageUrl { get; set; }
        public bool HasTimestamp { get; set; } = false;
        public int ColorRed { get; set; } = -1;
        public int ColorGreen { get; set; } = -1;
        public int ColorBlue { get; set; } = -1;

        public string Footer { get; set; }
        public RiftField[] Fields { get; set; }

        public RiftEmbed WithTitle(string title)
        {
            Title = title;
            return this;
        }

        public RiftEmbed WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public RiftEmbed WithThumbnailUrl(string url)
        {
            ThumbnailUrl = url;
            return this;
        }

        public Embed ToEmbed()
        {
            var eb = new EmbedBuilder();

            if (!string.IsNullOrWhiteSpace(Title))
                eb.WithTitle(Title);

            if (Author != null)
            {
                var hasName = !string.IsNullOrWhiteSpace(Author.Name);
                var hasUrl = !string.IsNullOrWhiteSpace(Author.IconUrl);

                if (hasUrl && !hasName)
                    throw new InvalidOperationException(
                        $"Cannot set field \"{nameof(Author.IconUrl)}\" when \"{nameof(Author.Name)}\" is null!");

                if (hasName && hasUrl)
                    eb.WithAuthor(Author.Name, Author.IconUrl);
                else
                {
                    if (hasName)
                        eb.WithAuthor(Author.Name);
                }
            }

            if (!string.IsNullOrWhiteSpace(Description))
                eb.WithDescription(Description);

            if (!string.IsNullOrWhiteSpace(Url))
                eb.WithUrl(Url);

            if (!string.IsNullOrWhiteSpace(ThumbnailUrl))
                eb.WithThumbnailUrl(ThumbnailUrl);

            if (!string.IsNullOrWhiteSpace(ImageUrl))
                eb.WithImageUrl(ImageUrl);

            if (HasTimestamp)
                eb.WithCurrentTimestamp();

            if (ColorRed >= 0 && ColorGreen >= 0 && ColorBlue >= 0)
                eb.WithColor(ColorRed, ColorGreen, ColorBlue);

            if (!string.IsNullOrWhiteSpace(Footer))
                eb.WithFooter(Footer);

            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    if (!string.IsNullOrWhiteSpace(field.Header) && !string.IsNullOrEmpty(field.Content))
                        eb.AddField(field.Header, field.Content, field.IsInline);
                }
            }

            return eb.Build();
        }
    }
    
    public class RiftAuthor
    {
        public string IconUrl { get; set; }
        public string Name { get; set; }
    }

    public class RiftField
    {
        public string Header { get; set; }
        public string Content { get; set; }
        public bool IsInline { get; set; } = false;
    }
}
