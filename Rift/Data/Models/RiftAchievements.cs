namespace Rift.Data.Models
{
    public class RiftAchievements
    {
        public ulong UserId { get; set; }
        public bool Write100Messages { get; set; } = false;
        public bool Write1000Messages { get; set; } = false;
        public bool Reach10Level { get; set; } = false;
        public bool Reach30Level { get; set; } = false;
        public bool Brag100Times { get; set; } = false;
        public bool Attack200Times { get; set; } = false;
        public bool OpenSphere { get; set; } = false;
        public bool Purchase200Items { get; set; } = false;
        public bool Open100Chests { get; set; } = false;
        public bool Send100Gifts { get; set; } = false;
        public bool IsDonator { get; set; } = false;
        public bool HasDonatedRole { get; set; } = false;
        public bool GiftToBotKeeper { get; set; } = false;
        public bool GiftToModerator { get; set; } = false;
        public bool AttackWise { get; set; } = false;

        public RiftUser User { get; set; }
    }
}
