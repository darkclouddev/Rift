using System;

namespace Rift.Data.Models
{
    public class RiftMessage
    {
        public Guid Id { get; set; }
        public ulong GuildId { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string EmbedJson { get; set; }
        public string ImageUrl { get; set; }

        public override string ToString()
        {
            return $"Guild: {GuildId.ToString()}, id: {Id.ToString()}, name: \"{Name}\"";
        }
    }
}
