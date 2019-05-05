namespace Rift.Data.Models
{
    public class RiftMessage
    {
        public int MessageId { get; set; }
        public string MessageName { get; set; }
        public string Text { get; set; } = "";
        public string Embed { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public bool ApplyFormat { get; set; } = true;
    }
}
