using System;
using System.Threading;
using System.Threading.Tasks;

using Rift.Configuration;
using Rift.Data.Models;

namespace Rift.Services
{
    public class ToxicityService
    {
        const string timerName = "toxicity-reducer";
        Timer checkTimer;

        public ToxicityService()
        {
            Task.Run(async () =>
            {
                var toxicityTimer = await DB.SystemTimers.GetAsync(timerName);

                if (toxicityTimer is null)
                {
                    RiftBot.Log.Error($"Failed to get system timer \"{timerName}\"");
                    return;
                }

                var delay = DateTime.UtcNow - toxicityTimer.LastInvoked > toxicityTimer.Interval
                    ? TimeSpan.Zero
                    : toxicityTimer.Interval - (DateTime.UtcNow - toxicityTimer.LastInvoked);

                checkTimer = new Timer(async delegate { await StartAsync(); },
                                       null,
                                       delay,
                                       toxicityTimer.Interval);
            });
        }

        async Task StartAsync()
        {
            await DB.SystemTimers.UpdateAsync(timerName, DateTime.UtcNow).ConfigureAwait(false);
            await CheckToxicityAsync();
        }

        public async Task CheckToxicityAsync()
        {
            var toxicity = await DB.Toxicity.GetNonZeroAsync();

            if (toxicity is null || toxicity.Length == 0)
            {
                RiftBot.Log.Info("No toxicity data was found, skipping checks.");
                return;
            }

            RiftBot.Log.Info("Starting up toxicity reducer.");

            var count = 0;

            foreach (var toxic in toxicity)
            {
                if (DateTime.UtcNow - toxic.LastIncreased < Settings.Economy.ToxicityWaitTime)
                    continue; // too soon to reduce

                if (DateTime.UtcNow - toxic.LastDecreased < Settings.Economy.ToxicityWaitTime)
                    continue; // already reduced

                count++;

                await ReduceToxicityAsync(toxic).ConfigureAwait(false);
            }

            RiftBot.Log.Info($"Successfully reduced toxicity level of {count} users.");
        }

        public async Task ReduceToxicityAsync(RiftToxicity toxicity)
        {
            var perc = toxicity.Percent > Settings.Economy.ToxicityWeeklyDropRate
                ? toxicity.Percent - Settings.Economy.ToxicityWeeklyDropRate
                : 0u;

            RiftBot.Log.Warn($"Reduced {toxicity.UserId}'s toxicity to {perc}% (level {toxicity.Level}).");

            await DB.Toxicity.UpdatePercentAsync(toxicity.UserId, perc);
        }
    }
}
