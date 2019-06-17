using System;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Services.Message;
using Rift.Services.Reward;
using Rift.Util;

using Discord.WebSocket;

namespace Rift.Services
{
    public class GiftService
    {
        static SemaphoreSlim giftMutex = new SemaphoreSlim(1);

        public async Task SendDescriptionAsync()
        {
            await RiftBot.SendChatMessageAsync("gift-description", null);
        }

        public async Task<IonicMessage> SendGiftAsync(SocketGuildUser fromSgUser, SocketGuildUser toSgUser)
        {
            await giftMutex.WaitAsync().ConfigureAwait(false);

            IonicMessage result;

            RiftBot.Log.Info($"Gift from {fromSgUser.ToLogString()} => {toSgUser.ToLogString()}.");

            try
            {
                result = await GiftAsync(fromSgUser, toSgUser).ConfigureAwait(false);
            }
            finally
            {
                giftMutex.Release();
            }

            return result;
        }

        static async Task<IonicMessage> GiftAsync(SocketGuildUser fromSgUser, SocketGuildUser toSgUser)
        {
            if (toSgUser.IsBot)
            {
                RiftBot.Log.Debug("[Gift] Target is bot.");
                return await RiftBot.GetMessageAsync("gift-target-bot", new FormatData(fromSgUser.Id));
            }

            if (fromSgUser.Id == toSgUser.Id)
            {
                RiftBot.Log.Debug("[Gift] Ouch, self-gift.");
                return await RiftBot.GetMessageAsync("gift-target-self", new FormatData(fromSgUser.Id));
            }

            (var canGift, var remainingTime) = await CanGift(fromSgUser.Id);

            if (!canGift)
                return await RiftBot.GetMessageAsync("gift-cooldown", new FormatData(fromSgUser.Id)
                {
                    Gift = new GiftData
                    {
                        Cooldown = remainingTime
                    }
                });

            var dbInventory = await Database.GetUserInventoryAsync(fromSgUser.Id);

            if (dbInventory.Coins < Settings.Economy.GiftPrice)
                return await RiftBot.GetMessageAsync("gift-nocoins", new FormatData(fromSgUser.Id)
                {
                    Gift = new GiftData
                    {
                        NecessaryCoins = Settings.Economy.GiftPrice - dbInventory.Coins
                    }
                });

            await Database.RemoveInventoryAsync(fromSgUser.Id, new InventoryData { Coins = Settings.Economy.GiftPrice });
            var giftItem = new GiftReward();
            await giftItem.DeliverToAsync(toSgUser.Id);
            RiftBot.Log.Debug("[Gift] Success.");

            await Database.SetLastGiftTimeAsync(fromSgUser.Id, DateTime.UtcNow);
            await Database.AddStatisticsAsync(fromSgUser.Id, giftsSent: 1u);
            await Database.AddStatisticsAsync(toSgUser.Id, giftsReceived: 1u);

            return await RiftBot.GetMessageAsync("gift-success", new FormatData(fromSgUser.Id)
            {
                Gift = new GiftData
                {
                    TargetId = toSgUser.Id,
                },
                Reward = new RewardData
                {
                    Reward = giftItem
                }
            });
        }

        static async Task<(bool, TimeSpan)> CanGift(ulong userId)
        {
            var data = await Database.GetUserCooldownsAsync(userId);

            if (data.LastGiftTime == DateTime.MinValue)
                return (true, TimeSpan.Zero);

            var diff = DateTime.UtcNow - data.LastGiftTime;

            var result = diff > Settings.Economy.GiftCooldown;

            return (result, result ? TimeSpan.Zero : Settings.Economy.GiftCooldown - diff);
        }
    }
}
