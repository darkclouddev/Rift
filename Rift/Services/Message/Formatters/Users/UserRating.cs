using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Formatters.Users
{
    public class UserRating : FormatterBase
    {
        public UserRating() : base("$userRating") {}

        const string NoRating = "-";

        public override Task<RiftMessage> Format(RiftMessage message, FormatData data)
        {
            var rating = EconomyService.SortedRating;

            var position = rating is null
                ? NoRating
                : $"{(rating.IndexOf(data.UserId) + 1).ToString()} / {rating.Count.ToString()}";

            return ReplaceData(message, position);
        }
    }
}
