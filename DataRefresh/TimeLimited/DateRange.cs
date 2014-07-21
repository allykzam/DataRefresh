using System;

namespace DataRefresh.TimeLimited
{
    /// <summary>
    /// Implements the basic functionality that should be similar between
    /// classes which implement <seealso cref="IDateRange"/>
    /// </summary>
    public class DateRange : IDateRange
    {
        /// <summary>
        /// Internal value used for <seealso cref="StartTime"/>
        /// </summary>
        protected DateTimeOffset? _StartTime;
        /// <summary>
        /// If null, this instance can be considered to take effect from any
        /// given point in time until the value of <seealso cref="EndTime"/>.
        /// Otherwise, this instance takes effect as of this time.
        /// </summary>
        public DateTimeOffset? StartTime
        {
            get
            {
                return _StartTime;
            }
        }

        /// <summary>
        /// Internal value used for <seealso cref="EndTime"/>
        /// </summary>
        protected DateTimeOffset? _EndTime;
        /// <summary>
        /// If null, this instance can be considered to take effect from the
        /// value of <seealso cref="StartTime"/> effectively until the end of
        /// time. Otherwise, this instances takes effect until this time.
        /// </summary>
        public DateTimeOffset? EndTime
        {
            get
            {
                return _EndTime;
            }
        }

        /// <summary>
        /// Internal value used for <seealso cref="ActivateContent"/>
        /// </summary>
        protected DateRangeActivation _ActivateContent;
        /// <summary>
        /// Indicates whether the content becomes active or inactive during
        /// this time range, or if this range is a blank entry.
        /// </summary>
        public DateRangeActivation ActivateContent
        {
            get
            {
                return _ActivateContent;
            }
        }

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
        public bool IsCurrent(DateTimeOffset now)
        {
            // Return false if this is an empty date range
            if (_ActivateContent == DateRangeActivation.Empty)
            {
                return false;
            }
            else
            {
                if ((_StartTime == null || _StartTime < now) &&
                    (_EndTime   == null || _EndTime   > now))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

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
        public bool OverlapsWith(IDateRange other)
        {
            // If both ranges are missing the same value, they overlap
            if ((!this.StartTime.HasValue && !other.StartTime.HasValue) ||
                (!this.EndTime.HasValue && !other.EndTime.HasValue))
            {
                return true;
            }
            // If this range is missing its start and either side of the other
            // range is before this range's end, then the two overlap
            else if (!this.StartTime.HasValue &&
               ((other.StartTime.HasValue && other.StartTime.Value < this.EndTime.Value) ||
                (other.EndTime.HasValue   && other.EndTime.Value   < this.EndTime.Value)))
            {
                return true;
            }
            // If this range is missing its end and either side of the other
            // range is after this range's start, then the two overlap
            else if (!this.EndTime.HasValue &&
               ((other.EndTime.HasValue   && other.EndTime.Value   > this.StartTime.Value) ||
                (other.StartTime.HasValue && other.StartTime.Value > this.StartTime.Value)))
            {
                return true;
            }
            // If the other range is missing its start and either side of this
            // range is before the other range's end, then the two overlap
            else if (!other.StartTime.HasValue &&
               ((this.StartTime.HasValue && this.StartTime.Value < other.EndTime.Value) ||
                (this.EndTime.HasValue   && this.EndTime.Value   < other.EndTime.Value)))
            {
                return true;
            }
            // If the other range is missing its end and either side of this
            // range is after the other range's start, then the two overlap
            else if (!other.EndTime.HasValue &&
               ((this.EndTime.HasValue   && this.EndTime.Value   > other.StartTime.Value) ||
                (this.StartTime.HasValue && this.StartTime.Value > other.StartTime.Value)))
            {
                return true;
            }
            // If both range have both sides
            else
            {
                return this.StartTime.Value < other.EndTime.Value && other.StartTime.Value < this.EndTime.Value;
            }
        }

        /// <summary>
        /// Creates a <seealso cref="DateRange"/> instance
        /// </summary>
        /// <param name="startTime">
        /// The starting time for the new date range
        /// </param>
        /// <param name="endTime">
        /// The end time for the new date range
        /// </param>
        /// <param name="activateContent">
        /// Whether or not the new date range should cause its parent content
        /// to become active
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a start or end time is present but the
        /// <paramref name="activateContent"/> flag is null; or when the
        /// <paramref name="activateContent"/> flag is not null, but both the
        /// start and end times are null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the start and end times are backwards.
        /// </exception>
        public DateRange(DateTimeOffset? startTime, DateTimeOffset? endTime, Boolean? activateContent)
        {
            // No activation flag, but we have a start or end time?
            if (!activateContent.HasValue && (startTime.HasValue || endTime.HasValue))
            {
                throw new ArgumentNullException(
                    "activateContent",
                    "A date range cannot have a start or end time without also having an activation flag");
            }
            // Have the activation flag, but no start or end times?
            if (activateContent.HasValue && !startTime.HasValue && !endTime.HasValue)
            {
                throw new ArgumentNullException(
                    "startTime/endTime",
                    "A date range with an activation flag must have a start and/or end time");
            }
            // Start and end times are backwards
            if (startTime.HasValue && endTime.HasValue && startTime.Value > endTime.Value)
            {
                throw new ArgumentException(
                    "The start date must precede the end date",
                    "startTime/endTime");
            }

            _StartTime = startTime;
            _EndTime = endTime;
            if (!activateContent.HasValue)
            {
                _ActivateContent = DateRangeActivation.Empty;
            }
            else if(activateContent.Value)
            {
                _ActivateContent = DateRangeActivation.Active;
            }
            else
            {
                _ActivateContent = DateRangeActivation.Inactive;
            }
        }
    }
}
