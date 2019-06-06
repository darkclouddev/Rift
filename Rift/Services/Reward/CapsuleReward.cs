namespace Rift.Services.Reward
{
    public class CapsuleReward : ItemReward
    {
        public CapsuleReward()
        {
            AddRandomTokens(10, 21);
            AddRandomChests(10, 21);
            AddRandomTickets(6, 11);
            AddBotRespects(1u);
            AddDoubleExps(1u);
        }
    }
}
