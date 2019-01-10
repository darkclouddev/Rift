using Rift.Configuration;

using IonicLib.Extensions;

namespace Rift.Services.Economy
{
    public class Brag
    {
        public readonly uint Coins;

        public Brag(bool win)
        {
            Coins = Helper.NextUInt(win ? Settings.Economy.BragWinCoinsMin : Settings.Economy.BragLossCoinsMin,
                                    win ? Settings.Economy.BragWinCoinsMax : Settings.Economy.BragLossCoinsMax);
        }
    }
}
