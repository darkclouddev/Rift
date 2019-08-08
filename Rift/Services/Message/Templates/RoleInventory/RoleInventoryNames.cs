using System.Linq;
using System.Threading.Tasks;

using Rift.Data.Models;

namespace Rift.Services.Message.Templates.RoleInventory
{
    public class RoleInventoryNames : TemplateBase
    {
        public RoleInventoryNames() : base(nameof(RoleInventoryNames))
        {
        }
        
        const string NoItems = "-";

        public override async Task<RiftMessage> ApplyAsync(RiftMessage message, FormatData data)
        {
            var userRoles = await DB.RoleInventory.GetAsync(data.UserId);

            if (userRoles is null || userRoles.Count == 0)
                return await ReplaceDataAsync(message, NoItems);

            var roles = await DB.Roles.GetAllAsync();

            var replacement = string.Join('\n', userRoles.Select(x =>
            {
                var role = roles.FirstOrDefault(y => y.Id == x.RoleId);

                if (role is null)
                {
                    TemplateError($"Role ID {x.RoleId.ToString()} does not exist in DB!");
                    return "N/A";
                }

                return role.Name;
            }));

            return await ReplaceDataAsync(message, replacement);
        }
    }
}
