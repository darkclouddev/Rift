using System;

namespace Rift.Events
{
    public class VoiceUptimeEarnedEventArgs : RiftEventArgs
    {
        public TimeSpan Uptime { get; }

        public VoiceUptimeEarnedEventArgs(ulong userId, TimeSpan uptime) : base(userId)
        {
            Uptime = uptime;
        }
    }
}
