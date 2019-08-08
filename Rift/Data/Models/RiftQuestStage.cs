using System;

namespace Rift.Data.Models
{
    public class RiftQuestStage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? CompletionRewardId { get; set; }

        public bool IsInProgress()
        {
            var dt = DateTime.UtcNow;

            return StartDate < dt && dt < EndDate;
        }
    }
}
