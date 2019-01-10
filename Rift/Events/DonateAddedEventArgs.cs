using System;

namespace Rift.Events
{
    public class DonateAddedEventArgs : EventArgs
    {
        public readonly ulong UserId;
        public readonly decimal DonateBefore;
        public readonly decimal DonateAmount;
        public readonly decimal DonateTotal;

        public DonateAddedEventArgs(ulong userId, decimal donateBefore, decimal donateAmount, decimal donateTotal)
        {
            UserId = userId;
            DonateAmount = donateAmount;
            DonateBefore = donateBefore;
            DonateTotal = donateTotal;
        }
    }
}
