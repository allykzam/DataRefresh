using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DataRefresh.Fetch
{
    /// <summary>
    /// Implements the basic functionality that should be similar between
    /// classes which implement <seealso cref="IFetchList"/>
    /// </summary>
    /// <typeparam name="D">The field type for the underlying data within each
    /// item which can uniquely identify them. Must implement
    /// <seealso cref="IComparable"/></typeparam>
    /// <typeparam name="T">The internal type which is contained within this
    /// collection. Must implement <seealso cref="IFetchable<D>"/></typeparam>
    public abstract class FetchableList<D, T> : IFetchList<D, T>
        where D : IComparable
        where T : class, IFetchable<D>
    {
        private Object _itemsLock = new Object();
        /// <summary>
        /// The internal collection of items
        /// </summary>
        protected List<T> _items = new List<T>();
        /// <summary>
        /// Property that indicates whether or not the internal collection of
        /// items is considered up-to-date
        /// </summary>
        /// <remarks>Must be implemented by the inheriting class</remarks>
        protected abstract Boolean CacheUpToDate_Internal { get; }
        /// <summary>
        /// Method that updates the internal collection of items
        /// </summary>
        /// <remarks>Must be implemented by the inheriting class</remarks>
        protected abstract void UpdateCache_Internal();
        /// <summary>
        /// Method that selects the index in the current list which should be
        /// selected by default when <seealso cref="DisplayList"/> is called
        /// </summary>
        /// <param name="lastSelected">The currently selected item</param>
        /// <returns>
        /// Returns the new index of <seealso cref="lastSelected"/> if it is in
        /// the current list, selects a suitable alternative if it is not, or
        /// selects the first item in the list if no value is given for
        /// <seealso cref="lastSelected"/>
        /// </returns>
        /// <remarks>Must be implemented by the inheriting class</remarks>
        protected abstract Int32 DefaultInList_Internal(T lastSelected);
        /// <summary>
        /// Method that sorts the current list
        /// </summary>
        protected abstract void Sort_Internal();

        /// <summary>
        /// Retrieves the item at the specified index
        /// </summary>
        /// <param name="index">The index of the item to retrieve</param>
        public T this[Int32 index]
        {
            get
            {
                lock (_itemsLock)
                {
                    return _items[index];
                }
            }
        }

        /// <summary>
        /// Returns the number of items currently contained within this
        /// collection
        /// </summary>
        public Int32 Length
        {
            get
            {
                lock (_itemsLock)
                {
                    return _items.Count;
                }
            }
        }

        /// <summary>
        /// Returns this collection of items as a <seealso cref="List<T>"/>
        /// </summary>
        public List<T> AsList
        {
            get
            {
                lock (_itemsLock)
                {
                    return _items.ToList();
                }
            }
        }

        /// <summary>
        /// Checks to determine whether or not the collection of items given is
        /// up-to-date
        /// </summary>
        public Boolean CacheUpToDate
        {
            get
            {
                return CacheUpToDate_Internal;
            }
        }

        /// <summary>
        /// Adds an item to the end of the collection
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(T item)
        {
            lock (_itemsLock)
            {
                _items.Add(item);
            }
        }

        /// <summary>
        /// Inserts an item at the specified position within the collection
        /// </summary>
        /// <param name="index">The index at which to insert the item</param>
        /// <param name="item">The item to insert</param>
        public void Insert(Int32 index, T item)
        {
            lock (_itemsLock)
            {
                _items.Insert(index, item);
            }
        }

        /// <summary>
        /// Determines whether an item with the specified identifier exists
        /// within this collection or not
        /// </summary>
        /// <param name="identifier">The identifier of the target item</param>
        public Boolean Contains(D identifier)
        {
            lock (_itemsLock)
            {
                return _items.Where((x) => areEqual(x.Identifier, identifier)).Any();
            }
        }

        /// <summary>
        /// Finds the position of the item with the specified identifier within
        /// this collection
        /// </summary>
        /// <param name="identifier">The identifier of the target item</param>
        /// <returns>Returns -1 if the item is not found</returns>
        public Int32 IndexOf(D identifier)
        {
            lock (_itemsLock)
            {
                var matches = _items.Where((x) => areEqual(x.Identifier, identifier));
                if (matches.Take(1).Count() == 0)
                    return -1;
                else
                    return _items.IndexOf(matches.First());
            }
        }

        /// <summary>
        /// Checks the given values to determine if they're equal.
        /// If typeof(D).IsValueType is true, the values are compared with
        /// .CompareTo, if false, they're compared with .Equals
        /// </summary>
        /// <param name="left">The first value</param>
        /// <param name="right">The second value</param>
        private Boolean areEqual(D left, D right)
        {
            if (typeof(D).IsValueType)
                return (left.CompareTo(right) == 0);
            else
                return left.Equals(right);
        }

        /// <summary>
        /// Updates the internal cache of items, discarding all current items
        /// which may have already had their main data fetched
        /// </summary>
        public void UpdateCache()
        {
            lock (_itemsLock)
            {
                UpdateCache_Internal();
            }
        }

        /// <summary>
        /// Pushes the current collection of items to a ComboBox
        /// </summary>
        /// <param name="DisplayBoxCombo">
        /// The ComboBox to display the items in
        /// </param>
        public void DisplayListCombo(ComboBox DisplayBoxCombo)
        {
            DisplayListCombo(null, DisplayBoxCombo);
        }

        /// <summary>
        /// Pushes the current collection of items to a ComboBox, attempting to
        /// select the same item
        /// </summary>
        /// <param name="selectedItem">The currently selected item</param>
        /// <param name="DisplayBoxCombo">
        /// The ComboBox to display the items in
        /// </param>
        public void DisplayListCombo(T selectedItem, ComboBox DisplayBoxCombo)
        {
            this.Sort();
            DisplayListCombo_Internal(selectedItem, DisplayBoxCombo);
        }

        private void DisplayListCombo_Internal(T selectedItem, ComboBox DisplayBoxCombo)
        {
            if (DisplayBoxCombo.InvokeRequired)
            {
                DisplayBoxCombo.Invoke(new Action(() => DisplayListCombo_Internal(selectedItem, DisplayBoxCombo)));
            }
            else
            {
                DisplayBoxCombo.Items.Clear();
                DisplayBoxCombo.Items.AddRange(this.Select((x) => x.DisplayIdentifier).ToArray());
                DisplayBoxCombo.SelectedIndex = DefaultInList(selectedItem);
            }
        }

        /// <summary>
        /// Pushes the current collection of items to a ListBox
        /// </summary>
        /// <param name="DisplayBoxList">
        /// The ListBox to display the items in
        /// </param>
        public void DisplayListList(ListBox DisplayBoxList)
        {
            DisplayListList(null, DisplayBoxList);
        }

        /// <summary>
        /// Pushes the current collection of items to a ListBox, attempting to
        /// select the same item
        /// </summary>
        /// <param name="selectedItem">The currently selected item</param>
        /// <param name="DisplayBoxList">
        /// The ListBox to display the items in
        /// </param>
        public void DisplayListList(T selectedItem, ListBox DisplayBoxList)
        {
            this.Sort();
            DisplayListList_Internal(selectedItem, DisplayBoxList);
        }

        private void DisplayListList_Internal(T selectedItem, ListBox DisplayBoxList)
        {
            if (DisplayBoxList.InvokeRequired)
            {
                DisplayBoxList.Invoke(new Action(() => DisplayListList_Internal(selectedItem, DisplayBoxList)));
            }
            else
            {
                DisplayBoxList.Items.Clear();
                DisplayBoxList.Items.AddRange(this.Select((x) => x.DisplayIdentifier).ToArray());
                DisplayBoxList.SelectedIndex = DefaultInList(selectedItem);
            }
        }

        /// <summary>
        /// Gets the index in the current list of the item which should be
        /// selected by default when <seealso cref="DisplayList"/> is called
        /// </summary>
        /// <param name="lastSelected">The currently selected item</param>
        /// <returns>
        /// Returns the new index of <seealso cref="lastSelected"/> if it is in
        /// the current list, selects a suitable alternative if it is not, or
        /// selects the first item in the list if no value is given for
        /// <seealso cref="lastSelected"/>
        /// </returns>
        public Int32 DefaultInList(T lastSelected)
        {
            return DefaultInList_Internal(lastSelected);
        }

        /// <summary>
        /// A function that sorts the current list of items
        /// </summary>
        public void Sort()
        {
            Sort_Internal();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>A <seealso cref="IEnumerator<T>"/> object that can be used
        /// to iterate through the collection</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return AsList.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>A <seealso cref="IEnumerator"/> object that can be used to
        /// iterate through the collection</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)AsList).GetEnumerator();
        }
    }
}
