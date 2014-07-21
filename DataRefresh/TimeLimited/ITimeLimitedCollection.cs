using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DataRefresh.TimeLimited
{
    /// <summary>
    /// Defines a collection of content which all be activated or inactivated
    /// by associated date ranges
    /// </summary>
    /// <typeparam name="D">
    /// The type of date ranges the content type this collection contains will
    /// utilize
    /// </typeparam>
    /// <typeparam name="I">
    /// The type of data the content's implementing type uses to uniquely
    /// identify instances of itself
    /// </typeparam>
    /// <typeparam name="T">
    /// Any content type which implements <seealso cref="ITimeLimited<D, I>"/>
    /// </typeparam>
    public interface ITimeLimitedCollection<D, I, T> : IEnumerable<T>
        where D : class, IDateRange
        where I : IComparable
        where T : ITimeLimited<D, I>
    {
        /// <summary>
        /// All content contained in this collection
        /// </summary>
        ReadOnlyCollection<T> Values { get; }
        /// <summary>
        /// Forces the collection's contents to be updated
        /// </summary>
        void GetLatest();
        /// <summary>
        /// Returns the content contained in this collection which would be
        /// considered active at the specified point in time
        /// </summary>
        /// <param name="now">
        /// The time for which each piece of content is having its availability
        /// questioned
        /// </param>
        ReadOnlyCollection<T> ActiveValues(DateTimeOffset now);
        /// <summary>
        /// Determines whether or not the "active" content contained in this
        /// collection differs between two provided points in time
        /// </summary>
        /// <param name="old">
        /// The first point in time to be used in the comparison
        /// </param>
        /// <param name="now">
        /// The second point in time to be used in the comparison
        /// </param>
        /// <returns>
        /// Returns true if the content which would be considered active at one
        /// specified point in time is in any way different from the same
        /// subset of content at the second specified point in time; returns
        /// false if both subsets of content contain the same values
        /// </returns>
        Boolean ActiveValuesChanged(DateTimeOffset old, DateTimeOffset now);
    }
}
