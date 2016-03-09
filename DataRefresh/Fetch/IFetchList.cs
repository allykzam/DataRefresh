using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DataRefresh.Fetch
{
    /// <summary>
    /// Defines a collection of items whose meta-data can be kept up-to-date
    /// separately from their main data, which can be fetched separately on an
    /// item-by-item basis.
    /// </summary>
    /// <typeparam name="D">
    /// The field type for the underlying data within each item which can
    /// uniquely identify them. Must implement <seealso cref="IComparable"/>
    /// </typeparam>
    /// <typeparam name="T">
    /// The internal type which is contained within this collection. Must be a
    /// reference type and implement <seealso cref="IFetchable<D>"/>
    /// </typeparam>
    public interface IFetchList<D, T> : IEnumerable<T>
        where D : IComparable
        where T : class, IFetchable<D>
    {
        /// <summary>
        /// Retrieves the item at the specified index
        /// </summary>
        /// <param name="index">The index of the item to retrieve</param>
        T this[Int32 index] { get; }
        /// <summary>
        /// Returns the number of items currently contained within this
        /// collection
        /// </summary>
        Int32 Length { get; }
        /// <summary>
        /// Returns this collection of items as a <seealso cref="List<T>"/>
        /// </summary>
        List<T> AsList { get; }
        /// <summary>
        /// Checks to determine whether or not the collection of items given is
        /// up-to-date
        /// </summary>
        Boolean CacheUpToDate { get; }
        /// <summary>
        /// Adds an item to the end of the collection
        /// </summary>
        /// <param name="item">The item to add</param>
        void Add(T item);
        /// <summary>
        /// Inserts an item at the specified position within the collection
        /// </summary>
        /// <param name="index">The index at which to insert the item</param>
        /// <param name="item">The item to insert</param>
        void Insert(Int32 index, T item);
        /// <summary>
        /// Determines whether an item with the specified identifier exists
        /// within this collection or not
        /// </summary>
        /// <param name="identifier">The identifier of the target item</param>
        Boolean Contains(D identifier);
        /// <summary>
        /// Finds the position of the item with the specified identifier within
        /// this collection
        /// </summary>
        /// <param name="identifier">The identifier of the target item</param>
        /// <returns>Returns -1 if the item is not found</returns>
        Int32 IndexOf(D identifier);
        /// <summary>
        /// Updates the internal cache of items, discarding all current items
        /// which may have already had their main data fetched
        /// </summary>
        void UpdateCache();
        /// <summary>
        /// Pushes the current collection of items to a ComboBox
        /// </summary>
        /// <param name="DisplayBoxCombo">
        /// The ComboBox to display the items in
        /// </param>
        void DisplayListCombo(ComboBox DisplayBoxCombo);
        /// <summary>
        /// Pushes the current collection of items to a ComboBox, attempting to
        /// select the same item
        /// </summary>
        /// <param name="selectedItem">The currently selected item</param>
        /// <param name="DisplayBoxCombo">
        /// The ComboBox to display the items in
        /// </param>
        void DisplayListCombo(T selectedItem, ComboBox DisplayBoxCombo);
        /// <summary>
        /// Pushes the current collection of items to a ListBox
        /// </summary>
        /// <param name="DisplayBoxList">
        /// The ListBox to display the items in
        /// </param>
        void DisplayListList(ListBox DisplayBoxList);
        /// <summary>
        /// Pushes the current collection of items to a ListBox, attempting to
        /// select the same item
        /// </summary>
        /// <param name="selectedItem">The currently selected item</param>
        /// <param name="DisplayBoxList">
        /// The ListBox to display the items in
        /// </param>
        void DisplayListList(T selectedItem, ListBox DisplayBoxList);
        /// <summary>
        /// Gets the index in the current list of the item which should be
        /// selected by default when <seealso cref="DisplayList"/> is called
        /// </summary>
        /// <param name="lastSelected">The currently selected item</param>
        /// <remarks>
        /// Returns the new index of <seealso cref="lastSelected"/> if it is in
        /// the current list, selects a suitable alternative if it is not, or
        /// selects the first item in the list if no value is given for
        /// <seealso cref="lastSelected"/>
        /// </remarks>
        Int32 DefaultInList(T lastSelected);
        /// <summary>
        /// A function that sorts the current list of items
        /// </summary>
        void Sort();
    }
}
