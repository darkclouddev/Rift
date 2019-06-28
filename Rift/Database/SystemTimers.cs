using System;
using System.Threading.Tasks;

using Rift.Data;
using Rift.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Rift.Database
{
    public class SystemTimers
    {
        public async Task<RiftSystemTimer> GetAsync(string name)
        {
            using (var context = new RiftContext())
            {
                return await context.SystemCooldowns.FirstOrDefaultAsync(x =>
                    x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public async Task UpdateAsync(string name, DateTime lastUpdated)
        {
            var timer = await GetAsync(name);

            if (timer is null)
            {
                RiftBot.Log.Error($"Timer \"{name}\" does not exist!");
                return;
            }

            using (var context = new RiftContext())
            {
                timer.LastInvoked = lastUpdated;
                context.Entry(timer).Property(x => x.LastInvoked).IsModified = true;
                await context.SaveChangesAsync();
            }
        }
    }
}
