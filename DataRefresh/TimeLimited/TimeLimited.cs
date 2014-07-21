using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRefresh.TimeLimited
{
    /// <summary>
    /// Implements the basic functionality that should be similar between
    /// classes which implement <seealso cref="ITimeLimited<D>"/>
    /// </summary>
    /// <typeparam name="D">
    /// The type of date ranges this content type will utilize -- must
    /// implement the <seealso cref="IDateRange"/> interface and be a reference
    /// type
    /// </typeparam>
    public class TimeLimited<D, I> : ITimeLimited<D, I>
        where D : class, IDateRange
        where I : IComparable
    {
        protected ReadOnlyCollection<D> _AllRanges;
        /// <summary>
        /// A collection of the time ranges for this content which contain a
        /// start or end time
        /// </summary>
        public ReadOnlyCollection<D> ValidTimeRanges
        {
            get
            {
                return new ReadOnlyCollection<D>(
                    _AllRanges
                    .Where((x) => x.ActivateContent != DateRangeActivation.Empty)
                    .ToList());
            }
        }

        /// <summary>
        /// Given the provided options, returns a boolean indicating whether or
        /// not the attached content is considered available at the specified
        /// point in time
        /// </summary>
        /// <param name="now">
        /// The point in time at which the content's availability is in
        /// question
        /// </param>
        /// <returns>
        /// Returns true if the content is available at the specified time, or
        /// false if it is not.
        /// </returns>
        public bool IsAvailable(DateTimeOffset now)
        {
            // If there are no ranges available, or there is only one available
            // range and it's empty, return the appropriate flag
            if (_AllRanges.Count < 1 ||
                (_AllRanges.Count == 1 &&
                _AllRanges[0].ActivateContent == DateRangeActivation.Empty))
            {
                return AvailableWithoutRanges;
            }
            else
            {
                var currentRanges = ValidTimeRanges.Where((x) => x.IsCurrent(now));
                var activatingRanges = currentRanges.Where((x) => x.ActivateContent == DateRangeActivation.Active);
                // If there are no ranges which are current, return the
                // appropriate flag
                if (!currentRanges.Any())
                {
                    return AvailableOutsideRanges;
                }
                // If any of the current ranges have their flag set to activate
                // this content, return true
                else if (activatingRanges.Any())
                {
                    return true;
                }
                // If none of the current ranges have their flag set to
                // activate this content, return false
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns a flag indicating whether or not this content type is
        /// available when no date ranges are associated with it
        /// </summary>
        /// <remarks>
        /// This default implementation returns true, but the inheriting class
        /// may override this value
        /// </remarks>
        public virtual Boolean AvailableWithoutRanges
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns a flag indicating whether or not this content type is
        /// available when none of the associated date ranges are current
        /// </summary>
        /// <remarks>
        /// This default implementation returns false, but the inheriting class
        /// may override this value
        /// </remarks>
        public virtual Boolean AvailableOutsideRanges
        {
            get
            {
                return false;
            }
        }

        protected I _UniqueIdentifier;
        /// <summary>
        /// An identifier that is unique to this instance of the inheriting
        /// class
        /// </summary>
        public I UniqueIdentifier
        {
            get
            {
                return _UniqueIdentifier;
            }
        }

        /// <summary>
        /// Creates a new instance of this content type
        /// </summary>
        /// <param name="dateRanges">
        /// The applicable date ranges for this piece of content
        /// </param>
        /// <param name="identifier">
        /// </param>
        public TimeLimited(IEnumerable<D> dateRanges, I identifier)
        {
            Boolean overlappingRanges = dateRanges.Where((x, i) =>
                {
                    return dateRanges.Where((y, j) =>
                        {
                            if (i == j)
                            {
                                return false;
                            }
                            else
                            {
                                return x.OverlapsWith(y);
                            }
                        }).Any();
                }).Any();
            Boolean badRanges = dateRanges.Where((x) =>
                {
                    return (!x.EndTime.HasValue && !x.StartTime.HasValue);
                }).Any();

            if (overlappingRanges)
            {
                throw new ArgumentException(
                    "The provided date ranges include ranges that overlap.",
                    "dateRanges");
            }
            // Empty range are only invalid if there's more than one range
            if (badRanges && dateRanges.Take(2).Count() > 1)
            {
                throw new ArgumentException(
                    "The provided date ranges include empty ranges.",
                    "dateRanges");
            }

            _AllRanges = new ReadOnlyCollection<D>(dateRanges.ToList());
            _UniqueIdentifier = identifier;
        }
    }
}
