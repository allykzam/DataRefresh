using System;

namespace DataRefresh.Fetch
{
    /// <summary>
    /// Defines a item type which maintains some level of its meta-data in a
    /// cached state, but must fetch the full data on request
    /// </summary>
    /// <typeparam name="D">The field type for the underlying data which can
    /// uniquely identify each object. Must implement
    /// <seealso cref="IComparable"/></typeparam>
    public interface IFetchable<D> : IComparable<IFetchable<D>>
        where D : IComparable
    {
        /// <summary>
        /// Updates the main underlying data
        /// </summary>
        Boolean FetchData();
        /// <summary>
        /// Indicates whether or not the underlying data has been fetched yet
        /// </summary>
        /// <remarks>Does not indicate whether or not the data is up-to-date,
        /// only whether or not it has been fetched</remarks>
        Boolean Fetched { get; }
        /// <summary>
        /// The unique identifier for each instance of this type
        /// </summary>
        D Identifier { get; }
        /// <summary>
        /// The display value shown to users if this type is displayed on a UI
        /// </summary>
        String DisplayIdentifier { get; }
    }
}
