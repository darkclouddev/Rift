using System;
using System.Threading;
using System.Threading.Tasks;

using Settings = Rift.Configuration.Settings;

using Rift.Database;
using Rift.Events;
using Rift.Services.Message;
using Rift.Services.Reward;
using Rift.Util;

using Discord.WebSocket;

using IonicLib;

namespace Rift.Services
{
    public class GiftService
    {
        public static event EventHandler<GiftSentEventArgs> GiftSent;
        public static event EventHandler<GiftReceivedEventArgs> GiftReceived;
        public static event EventHandler<GiftedFounderEventArgs> GiftedFounder;
        public static event EventHandler<GiftedDeveloperEventArgs> GiftedDeveloper;
        public static event EventHandler<GiftedModeratorEventArgs> GiftedModerator;
        public static event EventHandler<GiftedStreamerEventArgs> GiftedStreamer;

        static SemaphoreSlim giftMutex = new SemaphoreSlim(1);

        public async Task SendDescriptionAsync()
        {
            await RiftBot.SendMessageAsync("gift-description", Settings.ChannelId.Commands, null);
        }

        public async Task<IonicMessage> SendGiftAsync(SocketGuildUser fromSgUser, SocketGuildUser toSgUser)
        {
            await giftMutex.WaitAsync().ConfigureAwait(false);

            IonicMessage result;

            RiftBot.Log.Information($"Gift from {fromSgUser.ToLogString()} => {toSgUser.ToLogString()}.");

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
            var senderId = fromSgUser.Id;
            var receiverId = toSgUser.Id;

            if (toSgUser.IsBot)
            {
                RiftBot.Log.Debug("[Gift] Target is bot.");
                return await RiftBot.GetMessageAsync("gift-target-bot", new FormatData(senderId));
            }

            if (fromSgUser.Id == toSgUser.Id)
            {
                RiftBot.Log.Debug("[Gift] Ouch, self-gift.");
                return await RiftBot.GetMessageAsync("gift-target-self", new FormatData(senderId));
            }

            (var canGift, var remainingTime) = await CanGift(senderId);

            if (!canGift)
                return await RiftBot.GetMessageAsync("gift-cooldown", new FormatData(senderId)
                {
                    Gift = new GiftData
                    {
                        Cooldown = remainingTime
                    }
                });

            var dbInventory = await DB.Inventory.GetAsync(senderId);

            if (dbInventory.Coins < Settings.Economy.GiftPrice)
                return await RiftBot.GetMessageAsync("gift-nocoins", new FormatData(senderId)
                {
                    Gift = new GiftData
                    {
                        NecessaryCoins = Settings.Economy.GiftPrice - dbInventory.Coins
                    }
                });

            await DB.Inventory.RemoveAsync(senderId, new InventoryData {Coins = Settings.Economy.GiftPrice});
            var giftItem = new GiftReward();
            await giftItem.DeliverToAsync(receiverId);

            GiftSent?.Invoke(null, new GiftSentEventArgs(senderId, receiverId));
            GiftReceived?.Invoke(null, new GiftReceivedEventArgs(senderId, fromSgUser.Id));

            if (toSgUser.Id == 178443743026872321ul)
                GiftedFounder?.Invoke(null, new GiftedFounderEventArgs(receiverId, senderId));

            var developers = await DB.Roles.GetAsync(44);
            if (IonicHelper.HasRolesAny(toSgUser, developers.RoleId))
                GiftedDeveloper?.Invoke(null, new GiftedDeveloperEventArgs(senderId, receiverId));

            if (await RiftBot.IsModeratorAsync(toSgUser))
                GiftedModerator?.Invoke(null, new GiftedModeratorEventArgs(senderId, receiverId));

            if (!(await DB.Streamers.GetAsync(receiverId) is null))
                GiftedStreamer?.Invoke(null, new GiftedStreamerEventArgs(senderId, receiverId));

            RiftBot.Log.Debug("[Gift] Success.");

            await DB.Cooldowns.SetLastGiftTimeAsync(senderId, DateTime.UtcNow);
            await DB.Statistics.AddAsync(senderId, new StatisticData {GiftsSent = 1u});
            await DB.Statistics.AddAsync(receiverId, new StatisticData {GiftsReceived = 1u});

            return await RiftBot.GetMessageAsync("gift-success", new FormatData(senderId)
            {
                Gift = new GiftData
                {
                    TargetId = receiverId
                },
                Reward = giftItem
            });
        }

        static async Task<(bool, TimeSpan)> CanGift(ulong userId)
        {
            var data = await DB.Cooldowns.GetAsync(userId);

            if (data.LastGiftTime == DateTime.MinValue)
                return (true, TimeSpan.Zero);

            var diff = DateTime.UtcNow - data.LastGiftTime;

            var result = diff > Settings.Economy.GiftCooldown;

            return (result, result ? TimeSpan.Zero : Settings.Economy.GiftCooldown - diff);
        }
    }
}
