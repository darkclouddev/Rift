using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.Users
{
    public class UserRating : TemplateBase
    {
        public UserRating() : base(nameof(UserRating)) {}

        const string NoRating = "-";

        public override Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var rating = EconomyService.SortedRating;

            var position = rating is null
                ? NoRating
                : $"{(rating.IndexOf(data.UserId) + 1).ToString()} / {rating.Count.ToString()}";

            return ReplaceDataAsync(message, position);
        }
    }
}
