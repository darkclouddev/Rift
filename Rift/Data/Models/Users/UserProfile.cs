using System;

namespace Rift.Data.Models.Users
{
    public class UserProfile
    {
        public UInt64 UserId;
        public UInt32 Experience;
        public UInt32 Level;
        public Decimal TotalDonate;
        public DateTime DoubleExpTime;
        public DateTime BotRespectTime;
    }
}
