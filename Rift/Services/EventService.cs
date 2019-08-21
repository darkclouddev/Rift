using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Discord;

using Humanizer;

using Rift.Configuration;
using Rift.Data.Models;
using Rift.Events;
using Rift.Services.Message;

using IonicLib;
using IonicLib.Extensions;

namespace Rift.Services
{
    public class EventService
    {
        public static event EventHandler<NormalMonstersKilledEventArgs> NormalMonstersKilled;
        public static event EventHandler<RareMonstersKilledEventArgs> RareMonstersKilled;
        public static event EventHandler<EpicMonstersKilledEventArgs> EpicMonstersKilled;

        Timer startTimer;
        Timer checkTimer;
        readonly CultureInfo schedulerCulture;

        public EventService()
        {
            RiftBot.Log.Info("Starting EventService..");

            schedulerCulture = new CultureInfo("en-US");
            InitializeStartTimer(TimeSpan.FromSeconds(4));
            InitializeCheckTimer(TimeSpan.FromSeconds(5));

            /*var events = new List<RiftScheduledEvent>();

            var g19 = GenerateEventsForYear(2019);
            var g20 = GenerateEventsForYear(2020);

            events.AddRange(g19);
            events.AddRange(g20);

            Task.Run(async () => await DB.EventSchedule.AddRangeAsync(events));*/

            RiftBot.Log.Info("EventService loaded successfully.");
        }

        void InitializeStartTimer(TimeSpan delay)
        {
            startTimer = new Timer(async delegate { await ScheduleTimerToNextEventAsync(); },
                null, delay, TimeSpan.Zero);
        }

        void InitializeCheckTimer(TimeSpan delay)
        {
            checkTimer = new Timer(async delegate { await CheckExpiredAsync(); },
                null, delay, TimeSpan.Zero);
        }

        async Task CheckExpiredAsync()
        {
            var expiredEvents = await DB.ActiveEvents.GetExpiredAsync();

            if (expiredEvents is null || expiredEvents.Count == 0)
            {
                await ScheduleTimerToClosestActiveAsync().ConfigureAwait(false);
                return;
            }

            foreach (var e in expiredEvents)
                await FinishAsync(e);

            if (!await DB.ActiveEvents.AnyAsync())
                return;

            await ScheduleTimerToClosestActiveAsync().ConfigureAwait(false);
        }

        async Task ScheduleTimerToNextEventAsync()
        {
            var dt = DateTime.UtcNow;

            var closest = await DB.EventSchedule.GetClosestAsync(dt);

            if (closest is null)
            {
                RiftBot.Log.Error("Next schedule event is null!");
                return;
            }

            var ts = closest.StartAt - dt;

            startTimer = new Timer(async delegate
            {
                var nextEvent = await RandomizeEventAsync(closest);
                await StartAsync(nextEvent, IonicClient.Client.CurrentUser.Id);

                await ScheduleTimerToNextEventAsync();
            }, null, ts, TimeSpan.Zero);

            RiftBot.Log.Info(
                $"Event starter scheduled to {ts.Humanize(culture: schedulerCulture)} (type ID: {closest.EventType.ToString()}).");
        }

        async Task ScheduleTimerToClosestActiveAsync()
        {
            var closest = await DB.ActiveEvents.GetClosestAsync();

            if (closest is null)
                return;

            if (DateTime.UtcNow > closest.DueTime)
                return;

            // Sometimes timer invokes milliseconds before event expiration
            // Waiting for one second should fix this behaviour
            var ts = closest.DueTime - DateTime.UtcNow + TimeSpan.FromSeconds(1);
            checkTimer.Change(ts, TimeSpan.Zero);

            RiftBot.Log.Info($"Expired event check scheduled to {ts.Humanize(culture: schedulerCulture)}.");
        }

        async Task<RiftEvent> RandomizeEventAsync(RiftScheduledEvent scheduledEvent)
        {
            var events = await DB.Events.GetAllOfTypeAsync(scheduledEvent.EventType);

            if (events is null || events.Count == 0)
            {
                RiftBot.Log.Error($"No events of type {scheduledEvent.EventType.ToString()}!");
                return null;
            }

            return events.Random();
        }

        public async Task StartAsync(string name, ulong startedById)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                RiftBot.Log.Warn("Empty event name, skipping execution.");
                return;
            }

            var dbEvent = await DB.Events.GetAsync(name);

            await StartAsync(dbEvent, startedById);
        }

        public async Task StartAsync(RiftEvent dbEvent, ulong startedById)
        {
            if (dbEvent is null)
            {
                RiftBot.Log.Warn("Wrong event name, skipping execution.");
                return;
            }

            var msg = await DB.StoredMessages.GetMessageByIdAsync(dbEvent.StoredMessageId);

            if (msg is null)
            {
                RiftBot.Log.Warn("Wrong event message ID, skipping execution.");
                return;
            }

            var dbSharedReward = await DB.Rewards.GetAsync(dbEvent.SharedRewardId);

            if (dbSharedReward is null)
            {
                RiftBot.Log.Warn("Wrong event reward ID, skipping execution.");
                return;
            }

            if (!IonicClient.GetEmote(213672490491314176, "smite", out var smite))
            {
                RiftBot.Log.Warn("No event emote, skipping execution.");
                return;
            }

            var activeGiveaway = new RiftActiveEvent
            {
                EventName = dbEvent.Name,
                StoredMessageId = dbEvent.StoredMessageId,
                StartedBy = startedById == 0u ? IonicClient.Client.CurrentUser.Id : startedById,
                StartedAt = DateTime.UtcNow,
                DueTime = DateTime.UtcNow + dbEvent.Duration,
            };

            var formattedMsg = await RiftBot.GetService<MessageService>().FormatMessageAsync(
                msg, new FormatData(startedById));

            var eventMessage = await RiftBot.SendMessageAsync(formattedMsg, Settings.ChannelId.Monsters).ConfigureAwait(false);

            activeGiveaway.ChannelMessageId = eventMessage.Id;

            await DB.ActiveEvents.AddAsync(activeGiveaway).ConfigureAwait(false);

            await eventMessage.AddReactionAsync(smite);

            await ScheduleTimerToClosestActiveAsync().ConfigureAwait(false);
        }

        async Task FinishAsync(RiftActiveEvent expiredEvent)
        {
            var dbEvent = await DB.Events.GetAsync(expiredEvent.EventName);

            var eventLogString = $"ID {expiredEvent.Id.ToString()} \"{expiredEvent.EventName}\"";

            if (dbEvent is null)
            {
                RiftBot.Log.Error($"Could not finish event {eventLogString}: {nameof(RiftEvent)} is null!");
                return;
            }

            if (!IonicClient.GetTextChannel(Settings.App.MainGuildId, Settings.ChannelId.Monsters, out var channel))
            {
                RiftBot.Log.Error($"Could not finish event {eventLogString}: Event channel is null!");
                return;
            }

            var message = (IUserMessage) await channel.GetMessageAsync(expiredEvent.ChannelMessageId);

            if (message is null)
            {
                RiftBot.Log.Error($"Could not finish event {eventLogString}: Event message is null! Deleted?");
                return;
            }

            if (!IonicClient.GetEmote(213672490491314176ul, "smite", out var emote))
            {
                RiftBot.Log.Error($"Could not finish event {eventLogString}: Emote is null! Deleted?");
                return;
            }

            // Reaction amount is limited by discord itself.
            // See https://discordapp.com/developers/docs/resources/channel#get-reactions
            var reactions = await message.GetReactionUsersAsync(emote, 100).FlattenAsync();

            if (reactions is null)
            {
                RiftBot.Log.Error($"Could not finish event {eventLogString}: Unable to get reactions.");
                return;
            }

            var dbReward = await DB.Rewards.GetAsync(dbEvent.SharedRewardId);

            if (dbReward is null)
            {
                RiftBot.Log.Error($"Could not finish event {eventLogString}: " +
                                  $"Unable to get reward ID {dbEvent.SharedRewardId.ToString()}.");
                return;
            }

            var participants = reactions
                .Where(x => !x.IsBot && x.Id != IonicClient.Client.CurrentUser.Id)
                .Select(x => x.Id)
                .ToArray();

            if (participants.Length == 0)
            {
                await LogEventAsync(dbEvent.Name, null, dbReward.ToPlainString(), expiredEvent.StartedBy,
                    expiredEvent.StartedAt, dbEvent.Duration);
                await DB.ActiveEvents.RemoveAsync(expiredEvent.Id);

                RiftBot.Log.Error($"Could not finish event {eventLogString}: No participants.");
                return;
            }

            RiftReward specialReward = null;
            var specialWinnerId = 0ul;

            if (dbEvent.HasSpecialReward)
            {
                specialReward = await DB.Rewards.GetAsync(dbEvent.SpecialRewardId);

                if (specialReward is null)
                {
                    RiftBot.Log.Error($"Could not finish event {eventLogString}: " +
                                      $"Unable to get special reward ID {dbEvent.SharedRewardId.ToString()}.");
                    return;
                }

                specialWinnerId = participants.Random();
                await specialReward.DeliverToAsync(specialWinnerId);
            }

            var reward = dbReward.ToRewardBase();

            foreach (var userId in participants)
                await reward.DeliverToAsync(userId);

            await DB.ActiveEvents.RemoveAsync(expiredEvent.Id);

            var eventType = (EventType) dbEvent.Type;

            foreach (var participant in participants)
            {
                switch (eventType)
                {
                    case EventType.Normal:
                        NormalMonstersKilled?.Invoke(
                            null, new NormalMonstersKilledEventArgs(participant, 1u));
                        break;

                    case EventType.Rare:
                        RareMonstersKilled?.Invoke(
                            null, new RareMonstersKilledEventArgs(participant, 1u));
                        break;

                    case EventType.Epic:
                        EpicMonstersKilled?.Invoke(
                            null, new EpicMonstersKilledEventArgs(participant, 1u));
                        break;
                }
            }

            var log = new RiftEventLog
            {
                Name = dbEvent.Name,
                ParticipantsAmount = (uint) participants.Length,
                Reward = dbReward.ToPlainString(),
                StartedBy = expiredEvent.StartedBy,
                StartedAt = expiredEvent.StartedAt,
                Duration = dbEvent.Duration,
                FinishedAt = DateTime.UtcNow,
                SpecialWinnerId = specialWinnerId
            };

            await RiftBot.SendMessageAsync("event-finished", Settings.ChannelId.Monsters, new FormatData(expiredEvent.StartedBy)
            {
                EventData = new EventData
                {
                    Log = log,
                    Stored = dbEvent,
                }
            });

            if (dbEvent.HasSpecialReward)
            {
                await RiftBot.SendMessageAsync("event-finished-special", Settings.ChannelId.Monsters, new FormatData(specialWinnerId)
                {
                    Reward = specialReward.ToRewardBase()
                });
            }

            await LogEventAsync(log).ConfigureAwait(false);
        }

        static async Task LogEventAsync(RiftEventLog log)
        {
            await DB.EventLogs.AddAsync(log);
        }

        static async Task LogEventAsync(string name, ulong[] participants, string rewardPlain,
                                        ulong startedBy, DateTime startedAt, TimeSpan duration)
        {
            var log = new RiftEventLog
            {
                Name = name,
                ParticipantsAmount = (uint) participants.Length,
                Reward = rewardPlain,
                StartedBy = startedBy,
                StartedAt = startedAt,
                Duration = duration,
                FinishedAt = DateTime.UtcNow,
            };

            await LogEventAsync(log);
        }

        static List<RiftScheduledEvent> GenerateEventsForYear(int year)
        {
            // settings

            const int rareEventsAmount = 2;
            var rareEventsBaseHour = TimeSpan.FromHours(16);
            var rareEventsOffset = TimeSpan.FromHours(4);

            var epicEventsBaseHour = TimeSpan.FromHours(16);
            var epicEventsOffset = TimeSpan.FromHours(4);

            const int typeNormal = (int) EventType.Normal;
            const int typeRare = (int) EventType.Rare;
            const int typeEpic = (int) EventType.Epic;

            // settings

            var events = new List<RiftScheduledEvent>();

            for (var month = 1; month <= 12; month++)
            {
                var monthStart = new DateTime(year, month, 1);

                var daysInMonth = monthStart.AddMonths(1).AddDays(-1).Day;

                var rareEventDays = new int[rareEventsAmount];

                for (var i = 0; i < rareEventDays.Length; i++)
                {
                    var ratio = (int) Math.Floor((double) daysInMonth / rareEventsAmount);
                    var min = i * ratio;
                    var max = (i + 1) * ratio;

                    rareEventDays[i] = Helper.NextInt(min, max + 1);
                }

                for (var dayNumber = 1; dayNumber <= daysInMonth; dayNumber++)
                {
                    var dt = new DateTime(monthStart.Year, monthStart.Month, dayNumber);

                    if (rareEventDays.Contains(dayNumber))
                    {
                        dt += GetEventTime(rareEventsBaseHour, rareEventsOffset);

                        events.Add(new RiftScheduledEvent
                        {
                            StartAt = dt,
                            EventType = typeRare
                        });

                        continue;
                    }

                    if (dt.DayOfWeek == DayOfWeek.Sunday) // epic day
                    {
                        dt += GetEventTime(epicEventsBaseHour, epicEventsOffset);

                        events.Add(new RiftScheduledEvent
                        {
                            StartAt = dt,
                            EventType = typeEpic
                        });

                        continue;
                    }
                    else // normal day
                    {
                        var deviation = TimeSpan.FromHours(2);

                        var dt10 = dt + GetEventTime(TimeSpan.FromHours(7), deviation);
                        var dt16 = dt + GetEventTime(TimeSpan.FromHours(13), deviation);
                        var dt22 = dt + GetEventTime(TimeSpan.FromHours(19), deviation);

                        events.Add(new RiftScheduledEvent()
                        {
                            StartAt = dt10,
                            EventType = typeNormal
                        });

                        events.Add(new RiftScheduledEvent
                        {
                            StartAt = dt16,
                            EventType = typeNormal
                        });

                        events.Add(new RiftScheduledEvent
                        {
                            StartAt = dt22,
                            EventType = typeNormal
                        });
                    }
                }
            }

            return events;
        }

        static TimeSpan GetEventTime(TimeSpan start, TimeSpan maxDeviation)
        {
            var minimum = start - maxDeviation;
            var diff = (int) (maxDeviation * 2).TotalMinutes;

            var offset = Helper.NextInt(0, diff + 1);
            var result = minimum + TimeSpan.FromMinutes(offset);

            return result;
        }
    }

    public enum EventType
    {
        Normal = 0,
        Epic = 1,
        Rare = 2,
    }
}
