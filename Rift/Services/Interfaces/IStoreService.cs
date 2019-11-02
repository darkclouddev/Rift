using System;
using System.Threading.Tasks;

using Rift.Events;
using Rift.Services.Message;
using Rift.Services.Store;

namespace Rift.Services.Interfaces
{
    public interface IStoreService
    {
        event EventHandler<BoughtChestsEventArgs> BoughtChests;
        event EventHandler<RolesPurchasedEventArgs> RolesPurchased;

        IonicMessage ItemShopMessage { get; }
        IonicMessage RoleShopMessage { get; }
        IonicMessage BackgroundShopMessage { get; }

        Task<IonicMessage> PurchaseItemAsync(ulong userId, uint itemId);
        Task<IonicMessage> PurchaseRoleAsync(ulong userId, uint itemId);
        Task<IonicMessage> PurchaseBackgroundAsync(ulong userId, uint itemId);

        string FormatName(StoreItem item);
        string FormatPrice(StoreItem item);
    }
}
