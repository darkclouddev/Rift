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

        public RiftGiveaway Giveaway { get; set; }

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
                    RiftBot.Log.Error($"Failed to deserialize {nameof(ItemsData)} to {nameof(ItemReward)}!");
                    RiftBot.Log.Error(ex);
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
                    ItemsData = JsonConvert.SerializeObject(value);
                }
                catch (Exception ex)
                {
                    RiftBot.Log.Error($"Failed to serialize given value to {nameof(ItemReward)}!");
                    RiftBot.Log.Error(ex);
                    return;
                }
            }
        }

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
                    RiftBot.Log.Error($"Failed to deserialize {nameof(RoleData)} to {nameof(RoleReward)}!");
                    RiftBot.Log.Error(ex);
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
                    RoleData = JsonConvert.SerializeObject(value);
                }
                catch (Exception ex)
                {
                    RiftBot.Log.Error($"Failed to serialize given value to {nameof(RoleReward)}!");
                    RiftBot.Log.Error(ex);
                    return;
                }
            }
        }

        public override string ToString()
        {
            var text = "";

            if (ItemsData != null)
                text += ItemReward.ToString();

            if (RoleData != null)
                text += RoleReward.ToString();

            return text;
        }
    }
}
