using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rift.Data.Models
{
    public class RiftAchievements
    {
        [ForeignKey(nameof(User)), Required, Column(Order = 0)]
        public ulong UserId { get; set; }
        public RiftUser User { get; set; }

        [Column(Order = 1)]
        public bool Write100Messages { get; set; } = false;

        [Column(Order = 2)]
        public bool Write1000Messages { get; set; } = false;

        [Column(Order = 3)]
        public bool Reach10Level { get; set; } = false;

        [Column(Order = 4)]
        public bool Reach30Level { get; set; } = false;

        [Column(Order = 5)]
        public bool Brag100Times { get; set; } = false;

        [Column(Order = 6)]
        public bool Attack200Times { get; set; } = false;

        [Column(Order = 7)]
        public bool OpenSphere { get; set; } = false;

        [Column(Order = 8)]
        public bool GiftSphere { get; set; } = false;

        [Column(Order = 9)]
        public bool Purchase200Items { get; set; } = false;

        [Column(Order = 10)]
        public bool Open100Chests { get; set; } = false;

        [Column(Order = 11)]
        public bool Send100Gifts { get; set; } = false;

        [Column(Order = 12)]
        public bool IsDonator { get; set; } = false;

        [Column(Order = 13)]
        public bool HasDonatedRole { get; set; } = false;

        [Column(Order = 14)]
        public bool GiftToBotKeeper { get; set; } = false;

        [Column(Order = 15)]
        public bool GiftToModerator { get; set; } = false;

        [Column(Order = 16)]
        public bool AttackWise { get; set; } = false;
    }
}
