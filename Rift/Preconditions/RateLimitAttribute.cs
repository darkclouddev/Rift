﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using Humanizer;

namespace Rift.Preconditions
{
    /// <summary> Sets how often a user is allowed to use this command
    /// or any command in this module. </summary>
    /// <remarks>This is backed by an in-memory collection
    /// and will not persist with restarts.</remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public sealed class RateLimitAttribute : PreconditionAttribute
    {
        /// <summary>
        /// The _invoke limit.
        /// </summary>
        readonly uint invokeLimit;

        /// <summary>
        /// The _no limit in d ms.
        /// </summary>
        readonly bool noLimitInDMs;

        /// <summary>
        /// The _no limit for admins.
        /// </summary>
        readonly bool noLimitForAdmins;

        /// <summary>
        /// The _invoke limit period.
        /// </summary>
        readonly TimeSpan invokeLimitPeriod;

        /// <summary>
        /// The _invoke tracker.
        /// </summary>
        readonly Dictionary<ulong, CommandTimeout> invokeTracker = new Dictionary<ulong, CommandTimeout>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimitAttribute"/> class.  Sets how often a user is allowed to use this command. 
        /// </summary>
        /// <param name="times">
        /// The number of times a user may use the command within a certain period.
        /// </param>
        /// <param name="period">
        /// The amount of time since first invoke a user has until the limit is lifted.
        /// </param>
        /// <param name="measure">
        /// The scale in which the <paramref name="period"/> parameter should be measured.
        /// </param>
        /// <param name="flags">
        /// Flags to set behavior of the rate limit.
        /// </param>
        public RateLimitAttribute(uint times,
                                  double period,
                                  Measure measure,
                                  RateLimitFlags flags = RateLimitFlags.None)
        {
            invokeLimit = times;
            noLimitInDMs = (flags & RateLimitFlags.NoLimitInDMs) == RateLimitFlags.NoLimitInDMs;
            noLimitForAdmins = (flags & RateLimitFlags.NoLimitForAdmins) == RateLimitFlags.NoLimitForAdmins;
            invokeLimitPeriod = GetPeriod(measure, period);
        }

        static TimeSpan GetPeriod(Measure measure, double period)
        {
            return measure switch
                {
                Measure.Days => TimeSpan.FromDays(period),
                Measure.Hours => TimeSpan.FromHours(period),
                Measure.Minutes => TimeSpan.FromMinutes(period),
                Measure.Seconds => TimeSpan.FromSeconds(period),
                _ => TimeSpan.Zero
                };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimitAttribute"/> class.  Sets how often a user is allowed to use this command. 
        /// </summary>
        /// <param name="times">
        /// The number of times a user may use the command within a certain period.
        /// </param>
        /// <param name="period">
        /// The amount of time since first invoke a user has until the limit is lifted.
        /// </param>
        /// <param name="flags">
        /// Flags to set behavior of the rate limit.
        /// </param>
        public RateLimitAttribute(
            uint times,
            TimeSpan period,
            RateLimitFlags flags = RateLimitFlags.None)
        {
            invokeLimit = times;
            noLimitInDMs = (flags & RateLimitFlags.NoLimitInDMs) == RateLimitFlags.NoLimitInDMs;
            noLimitForAdmins = (flags & RateLimitFlags.NoLimitForAdmins) == RateLimitFlags.NoLimitForAdmins;

            invokeLimitPeriod = period;
        }

        /// <inheritdoc />
        public override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context,
            CommandInfo command,
            IServiceProvider services)
        {
            if (noLimitInDMs && context.Channel is IPrivateChannel)
                return Task.FromResult(PreconditionResult.FromSuccess());

            if (noLimitForAdmins && context.User is IGuildUser gu && gu.GuildPermissions.Administrator)
                return Task.FromResult(PreconditionResult.FromSuccess());

            var now = DateTime.UtcNow;
            var key = context.User.Id;

            var timeout = invokeTracker.TryGetValue(key, out var t)
                          && now - t.FirstInvoke < invokeLimitPeriod
                ? t
                : new CommandTimeout(now);

            timeout.TimesInvoked++;

            if (timeout.TimesInvoked <= invokeLimit)
            {
                invokeTracker[key] = timeout;
                return Task.FromResult(PreconditionResult.FromSuccess());
            }

            return Task.FromResult(PreconditionResult.FromError(ErrorMessage));
        }

        /// <summary>
        /// The command timeout.
        /// </summary>
        sealed class CommandTimeout
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CommandTimeout"/> class.
            /// </summary>
            /// <param name="timeStarted">
            /// The time started.
            /// </param>
            public CommandTimeout(DateTime timeStarted)
            {
                FirstInvoke = timeStarted;
            }

            /// <summary>
            /// Gets or sets the times invoked.
            /// </summary>
            public uint TimesInvoked { get; set; }

            /// <summary>
            /// Gets the first invoke.
            /// </summary>
            public DateTime FirstInvoke { get; }
        }
    }

    /// <summary> Used to set behavior of the rate limit </summary>
    [Flags]
    public enum RateLimitFlags
    {
        /// <summary> Set none of the flags. </summary>
        None = 0,

        /// <summary> Set whether or not there is no limit to the command in DMs. </summary>
        NoLimitInDMs = 1 << 0,

        /// <summary> Set whether or not there is no limit to the command for guild admins. </summary>
        NoLimitForAdmins = 1 << 1,
    }

    /// <summary> Sets the scale of the period parameter. </summary>
    public enum Measure
    {
        /// <summary> Period is measured in days. </summary>
        Days,

        /// <summary> Period is measured in hours. </summary>
        Hours,

        /// <summary> Period is measured in minutes. </summary>
        Minutes,

        /// <summary> Period is measured in seconds. </summary>
        Seconds
    }
}
