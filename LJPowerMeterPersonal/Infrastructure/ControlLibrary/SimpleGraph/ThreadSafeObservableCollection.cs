namespace LaJust.PowerMeter.ControlLibrary.SimpleGraph
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Windows.Threading;

    /// <summary>
    /// Provides a threadsafe ObservableCollection of T
    /// </summary>
    public class ThreadSafeObservableCollection<T> : ObservableCollection<T>
    {        
        #region Private Fields

        private Dispatcher _dispatcher;
        private ReaderWriterLockSlim _lock;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeObservableCollection&lt;T&gt;"/> class.
        /// </summary>
        public ThreadSafeObservableCollection()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _lock = new ReaderWriterLockSlim();
        }

        #endregion

        #region ObservableCollection Thread Safe Overrides

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            _dispatcher.InvokeIfRequired(() =>
                {
                    _lock.EnterWriteLock();
                    try
                    {
                        base.ClearItems();
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }, DispatcherPriority.DataBind);
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            _dispatcher.InvokeIfRequired(() =>
            {
                if (index > this.Count)
                    return;

                _lock.EnterWriteLock();
                try
                {
                    base.InsertItem(index, item);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }, DispatcherPriority.DataBind);

        }

        /// <summary>
        /// Moves the item at the specified index to a new location in the collection.
        /// </summary>
        /// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param>
        /// <param name="newIndex">The zero-based index specifying the new location of the item.</param>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            _dispatcher.InvokeIfRequired(() =>
            {
                _lock.EnterReadLock();
                Int32 itemCount = this.Count;
                _lock.ExitReadLock();

                if (oldIndex >= itemCount | 
                    newIndex >= itemCount | 
                    oldIndex == newIndex)
                    return;

                _lock.EnterWriteLock();
                try
                {
                    base.MoveItem(oldIndex, newIndex);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }, DispatcherPriority.DataBind);
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {

            _dispatcher.InvokeIfRequired(() =>
            {
                if (index >= this.Count)
                    return;

                _lock.EnterWriteLock();
                try
                {
                    base.RemoveItem(index);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }, DispatcherPriority.DataBind);
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, T item)
        {
            _dispatcher.InvokeIfRequired(() =>
            {
                _lock.EnterWriteLock();
                try
                {
                    base.SetItem(index, item);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }, DispatcherPriority.DataBind);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Return as a cloned copy of this Collection
        /// </summary>
        public T[] ToSyncArray()
        {
            _lock.EnterReadLock();
            try
            {
                T[] _sync = new T[this.Count];
                this.CopyTo(_sync, 0);
                return _sync;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        #endregion
    }
}
