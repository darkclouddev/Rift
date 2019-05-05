using Rift.Data.Models;

namespace Rift.Services.Message.Formatters
{
    public class UserId : FormatterBase
    {
        public UserId() : base("$userId") {}

        public override RiftMessage Format(RiftMessage message, ulong userId)
        {
            return ReplaceData(message, userId.ToString());
        }
    }
}
