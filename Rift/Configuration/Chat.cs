namespace Rift.Configuration
{
    public class Chat
    {
        public bool CapsFilterEnabled { get; set; } = false;
        public float CapsFilterRatio { get; set; } = 0.7f;
        public bool AttachmentFilterEnabled { get; set; } = false;
        public bool UrlFilterEnabled { get; set; } = false;
        public bool AllowYoutubeLinks { get; set; } = false;
        public bool ProcessUserNames { get; set; } = false;
        public int MessageMinimumLength { get; set; } = 8;
    }
}
