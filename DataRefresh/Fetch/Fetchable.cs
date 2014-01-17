using System;

namespace DataRefresh.Fetch
{
    /// <summary>
    /// Implements the basic functionality that should be similar between
    /// classes which implement <seealso cref="IFetchable"/>
    /// </summary>
    /// <typeparam name="D">The field type for the underlying data which can
    /// uniquely identify each object. Must implement
    /// <seealso cref="IComparable"/></typeparam>
    public abstract class Fetchable<D> : IFetchable<D>
        where D : IComparable
    {
        private D _identifier;
        private Object _updateLock = new Object();
        private Boolean _IsUpdated = false;

        /// <summary>
        /// Initializes a new instance of this class
        /// </summary>
        /// <param name="identifier">The identifier for the data to be stored
        /// within this instance</param>
        public Fetchable(D identifier)
        {
            _identifier = identifier;
        }

        /// <summary>
        /// A method to be implemented by inherited classes, which updates the
        /// underlying data contained within an instance
        /// </summary>
        /// <returns>Must return true on success or false on failure</returns>
        protected abstract Boolean UpdateData_Internal();

        /// <summary>
        /// Returns the identifier for the data contained within this instance
        /// </summary>
        public D Identifier
        {
            get
            {
                return _identifier;
            }
        }

        /// <summary>
        /// A property to be implemented by inherited classes, which returns
        /// the display value shown to users if this type if displayed on a UI
        /// </summary>
        protected abstract String DisplayIdentifier_Internal { get; }

        /// <summary>
        /// Returns the display value shown to users if this type is displayed
        /// on a UI
        /// </summary>
        public String DisplayIdentifier
        {
            get
            {
                return DisplayIdentifier_Internal;
            }
        }

        /// <summary>
        /// Compares the current object with another object that implements
        /// <seealso cref="IFetchable<D>"/>
        /// </summary>
        /// <param name="other">An object to compare with this object</param>
        public Int32 CompareTo(IFetchable<D> other)
        {
            return _identifier.CompareTo(other.Identifier);
        }

        /// <summary>
        /// Updates the underlying data contained within this instance
        /// </summary>
        /// <returns>Returns true on success or false on failure</returns>
        public Boolean FetchData()
        {
            lock (_updateLock)
            {
                if (UpdateData_Internal())
                {
                    _IsUpdated = true;
                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Indicates whether or not the underlying data has been fetched yet
        /// </summary>
        /// <remarks>Does not indicate whether or not the data is up-to-date,
        /// only whether or not it has been fetched</remarks>
        public Boolean Fetched
        {
            get
            {
                lock (_updateLock)
                {
                    return _IsUpdated;
                }
            }
        }
    }
}
