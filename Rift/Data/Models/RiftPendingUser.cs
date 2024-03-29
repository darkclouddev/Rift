﻿using System;

namespace Rift.Data.Models
{
    public class RiftPendingUser
    {
        public ulong UserId { get; set; }
        public string Region { get; set; }
        public string PlayerUUID { get; set; }
        public string AccountId { get; set; }
        public string SummonedId { get; set; }
        public string ConfirmationCode { get; set; }
        public DateTime ExpirationTime { get; set; }

        public RiftUser User { get; set; }
    }
}
