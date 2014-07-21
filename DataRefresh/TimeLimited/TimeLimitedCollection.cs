using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DataRefresh.TimeLimited
{
    /// <summary>
    /// Implements the basic functionality that should be similar between
    /// classes which implement <seealso cref="ITimeLimitedCollection<D, I, T>"/>
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
    public abstract class TimeLimitedCollection<D, I, T> : ITimeLimitedCollection<D, I, T>
        where D : class, IDateRange
        where I : IComparable
        where T : ITimeLimited<D, I>
    {
        protected ReadOnlyCollection<T> _AllValues;
        /// <summary>
        /// All content contained in this collection
        /// </summary>
        public ReadOnlyCollection<T> Values
        {
            get
            {
                return _AllValues;
            }
        }

        /// <summary>
        /// Forces the collection's contents to be updated
        /// </summary>
        public abstract void GetLatest();

        /// <summary>
        /// Returns the content contained in this collection which would be
        /// considered active at the specified point in time
        /// </summary>
        /// <param name="now">
        /// The time for which each piece of content is having its availability
        /// questioned
        /// </param>
        public ReadOnlyCollection<T> ActiveValues(DateTimeOffset now)
        {
            return new ReadOnlyCollection<T>(
                Values.Where((x) => x.IsAvailable(now)).ToList()
                );
        }

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
        public bool ActiveValuesChanged(DateTimeOffset old, DateTimeOffset now)
        {
            IEnumerable<T> thenValues = Values.Where((x) => x.IsAvailable(old));
            IEnumerable<T> nowValues = Values.Where((x) => x.IsAvailable(now));
            IEnumerable<I> thenIdentifiers = thenValues.Select((x) => x.UniqueIdentifier);
            IEnumerable<I> nowIdentifiers = nowValues.Select((x) => x.UniqueIdentifier);
            I[] thenSorted = thenIdentifiers.ToArray();
            Array.Sort(thenSorted);
            I[] nowSorted = nowIdentifiers.ToArray();
            Array.Sort(nowSorted);
            return thenSorted.SequenceEqual(nowSorted);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <seealso cref="IEnumerator<T>"/> that can be used to iterate
        /// through the collection.
        /// </returns>
        /// <remarks>
        /// See <seealso cref="IEnumerable<T>.GetEnumerator"/> for additional
        /// information
        /// </remarks>
        public IEnumerator<T> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <seealso cref="IEnumerator"/> that can be used to iterate through
        /// the collection.
        /// </returns>
        /// <remarks>
        /// See <seealso cref="IEnumerable.GetEnumerator"/> for additional
        /// information
        /// </remarks>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }
    }
}
