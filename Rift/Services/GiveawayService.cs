using System.Threading.Tasks;

using Rift.Services.Message;

namespace Rift.Services
{
    public class GiveawayService
    {
        public async Task<IonicMessage> StartGiveawayAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                RiftBot.Log.Warn("Empty giveaway name, skipping execution.");
                return new IonicMessage("Не указано название розыгрыша!");
            }

            var giveaway = await Database.GetGiveawayAsync(name);

            if (string.IsNullOrWhiteSpace(name))
            {
                RiftBot.Log.Warn("Wrong giveaway name, skipping execution.");

                return new IonicMessage($"Розыгрыш с названием \"{name}\" отсутствует в моей базе данных.");
            }

            return new IonicMessage(new RiftEmbed().AddField("Данные розыгрыша", giveaway.ToString()));
        }
    }
}
