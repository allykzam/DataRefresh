using System;
using System.Collections.ObjectModel;

namespace DataRefresh.TimeLimited
{
    /// <summary>
    /// Defines a type of content which can be activated or inactivated by
    /// associated date ranges
    /// </summary>
    /// <typeparam name="D">
    /// The type of date ranges this content type will utilize -- must
    /// implement the <seealso cref="IDateRange"/> interface and be a reference
    /// type
    /// </typeparam>
    /// <typeparam name="I">
    /// The type of data the implementing type uses to uniquely identify
    /// instances of itself
    /// </typeparam>
    public interface ITimeLimited<D, I>
        where D : class, IDateRange
        where I : IComparable
    {
        /// <summary>
        /// A collection of the time ranges for this content which contain a
        /// start or end time
        /// </summary>
        ReadOnlyCollection<D> ValidTimeRanges { get; }
        /// <summary>
        /// Returns a boolean indicating whether or not the attached content is
        /// considered available at the specified point in time
        /// </summary>
        /// <param name="now">
        /// The point in time at which the content's availability is in
        /// question
        /// </param>
        /// <returns>
        /// Returns true if the content is available at the specified time, or
        /// false if it is not.
        /// </returns>
        Boolean IsAvailable(DateTimeOffset now);
        /// <summary>
        /// A flag indicating whether or not this content should be considered
        /// available when no date ranges are available
        /// </summary>
        Boolean AvailableWithoutRanges { get; }
        /// <summary>
        /// A flag indicating whether or not this content should be considered
        /// available when date ranges are available, but none are current
        /// </summary>
        Boolean AvailableOutsideRanges { get; }
        /// <summary>
        /// An identifier that is unique to this instance of the implementing
        /// type
        /// </summary>
        I UniqueIdentifier { get; }
    }
}
