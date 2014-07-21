using System;

namespace DataRefresh.TimeLimited
{
    /// <summary>
    /// Defines a date range for items that are only active for certain times.
    /// </summary>
    public interface IDateRange
    {
        /// <summary>
        /// If null, this instance can be considered to take effect from any
        /// given point in time until the value of <seealso cref="EndTime"/>.
        /// Otherwise, this instance takes effect as of this time.
        /// </summary>
        DateTimeOffset? StartTime { get; }
        /// <summary>
        /// If null, this instance can be considered to take effect from the
        /// value of <seealso cref="StartTime"/> effectively until the end of
        /// time. Otherwise, this instances takes effect until this time.
        /// </summary>
        DateTimeOffset? EndTime { get; }
        /// <summary>
        /// Indicates whether the content becomes active or inactive during
        /// this time range, or if this range is a blank entry.
        /// </summary>
        DateRangeActivation ActivateContent { get; }
        /// <summary>
        /// Indicates whether this instance is in effect currently. Check the
        /// value of <seealso cref="ActivateContent"/> as to whether the
        /// content is therefore active or inactive.
        /// </summary>
        /// <param name="now">
        /// The point in time for which the status of this date range is in
        /// question
        /// </param>
        /// <returns>
        /// Returns true if this date range is in effect at the specified time,
        /// or false if not
        /// </returns>
        Boolean IsCurrent(DateTimeOffset now);
        /// <summary>
        /// Indicates whether this instance overlaps with the provided date
        /// range
        /// </summary>
        /// <param name="other">
        /// The other date range to compare against
        /// </param>
        /// <returns>
        /// Returns true if the two ranges overlap, or false if they do not.
        /// </returns>
        Boolean OverlapsWith(IDateRange other);
    }
}
