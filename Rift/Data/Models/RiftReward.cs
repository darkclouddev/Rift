using System;

using Rift.Services.Reward;

using Newtonsoft.Json;

namespace Rift.Data.Models
{
    public class RiftReward
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string ItemsData { get; set; }
        public string RoleData { get; set; }

        /// <summary>
        /// This method does not cache result data
        /// </summary>
        public ItemReward ItemReward
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ItemsData))
                    return null;

                try
                {
                    var kek = JsonConvert.DeserializeObject<ItemReward>(ItemsData);
                    return kek;
                }
                catch (Exception ex)
                {
                    RiftBot.Log.Error(ex, $"Failed to deserialize {nameof(ItemsData)} to {nameof(ItemReward)}!");
                    return null;
                }
            }
            set
            {
                if (value is null)
                {
                    RiftBot.Log.Error($"Failed to serialize {nameof(ItemReward)}: given value is null");
                    return;
                }

                try
                {
                    ItemsData = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }
                catch (Exception ex)
                {
                    RiftBot.Log.Error(ex, $"Failed to serialize given value to {nameof(ItemReward)}!");
                    return;
                }
            }
        }

        /// <summary>
        /// This method does not cache result data
        /// </summary>
        public RoleReward RoleReward
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RoleData))
                    return null;

                try
                {
                    return JsonConvert.DeserializeObject<RoleReward>(RoleData);
                }
                catch (Exception ex)
                {
                    RiftBot.Log.Error(ex, $"Failed to deserialize {nameof(RoleData)} to {nameof(RoleReward)}!");
                    return null;
                }
            }
            set
            {
                if (value is null)
                {
                    RiftBot.Log.Error($"Failed to serialize {nameof(RoleReward)}: given value is null");
                    return;
                }

                try
                {
                    RoleData = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }
                catch (Exception ex)
                {
                    RiftBot.Log.Error(ex, $"Failed to serialize given value to {nameof(RoleReward)}!");
                    return;
                }
            }
        }

        public RewardBase ToRewardBase()
        {
            RewardBase reward = null;

            var itemReward = ItemReward;
            if (itemReward is null)
            {
                var roleReward = RoleReward;

                if (!(roleReward is null))
                    reward = roleReward;
            }
            else
            {
                reward = itemReward;
            }

            return reward;
        }
    }
}
