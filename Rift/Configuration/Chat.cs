namespace Rift.Configuration
{
    public class Chat
    {
        public bool CapsFilterEnabled { get; set; } = true;
        public float CapsFilterRatio { get; set; } = 0.7f;
        public bool AttachmentFilterEnabled { get; set; } = true;
        public bool UrlFilterEnabled { get; set; } = true;
        public bool AllowYoutubeLinks { get; set; } = true;
    }
}
