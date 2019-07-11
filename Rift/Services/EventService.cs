using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Rift.Data.Models;

using IonicLib.Extensions;

namespace Rift.Services
{
    public class EventService
    {
        public EventService()
        {
            RiftBot.Log.Info("Starting EventService..");

            // TODO
            
            RiftBot.Log.Info("EventService loaded successfully.");
        }

        static List<RiftScheduledEvent> ScheduleEventsForMonth(int month)
        {
            // settings

            const int rareEventsAmount = 2;
            var rareEventsBaseHour = TimeSpan.FromHours(16);
            var rareEventsOffset = TimeSpan.FromHours(4);

            var epicEventsBaseHour = TimeSpan.FromHours(16);
            var epicEventsOffset = TimeSpan.FromHours(4);

            // settings

            var events = new List<RiftScheduledEvent>();

            var monthStart = new DateTime(2000, month, 1);

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
                        Month = dt.Month,
                        Day = dt.Day,
                        Hour = dt.Hour,
                        Minute = dt.Minute,
                        EventName = "**Rare shit**"
                    });

                    continue;
                }

                if (dt.DayOfWeek == DayOfWeek.Sunday) // epic day
                {
                    dt += GetEventTime(epicEventsBaseHour, epicEventsOffset);

                    events.Add(new RiftScheduledEvent()
                    {
                        Month = month,
                        Day = dt.Day,
                        Hour = dt.Hour,
                        Minute = dt.Minute,
                        EventName = "**Epic shit**",
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
                        Month = dt10.Month,
                        Day = dt10.Day,
                        Hour = dt10.Hour,
                        Minute = dt10.Minute,
                        EventName = "Normal shit",
                    });

                    events.Add(new RiftScheduledEvent
                    {
                        Month = dt16.Month,
                        Day = dt16.Day,
                        Hour = dt16.Hour,
                        Minute = dt16.Minute,
                        EventName = "Normal shit",
                    });

                    events.Add(new RiftScheduledEvent
                    {
                        Month = dt22.Month,
                        Day = dt22.Day,
                        Hour = dt22.Hour,
                        Minute = dt22.Minute,
                        EventName = "Normal shit",
                    });
                }
            }

            return events;
        }

        static TimeSpan GetEventTime(TimeSpan start, TimeSpan maxDeviation)
        {
            var minimum = start - maxDeviation;
            var diff = (int) (maxDeviation * 2).TotalMinutes;

            var offset = Helper.NextUInt(0, diff + 1);
            var result = minimum + TimeSpan.FromMinutes(offset);

            return result;
        }
    }

    public enum EventType
    {
        Wolves = 0,
        Razorfins = 1,
        Krugs = 2,
        Gromp = 3,
        ScuttleCrab = 4,
        BlueBuff = 5,
        RedBuff = 6,
        Drake = 7,
        Baron = 8,
        Armadillo = 9,
        BigEyes = 10,
        DevastatorCrab = 11,
        CrookedTail = 12,
    }
}
