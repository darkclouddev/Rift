using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rift.Data.Models
{
    public class ScheduledEvent
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Required, Column(Order = 0)]
        public uint Id { get; set; }

        /// <summary>
        /// Using int to match System.DayOfWeek enum type.
        /// </summary>
        [Column(Order = 1)]
        public int DayId { get; set; }

        [Column(Order = 2)]
        public int EventId { get; set; }

        [Column(Order = 3)]
        public int Hour { get; set; }

        [Column(Order = 4)]
        public int Minute { get; set; }
    }
}
