// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchingObservableCollection.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Threading;

    /// <summary>
    /// This class is an observable collection which invokes automatically.
    /// This means that any change will be done in the right thread.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the elements
    /// </typeparam>
    public class DispatchingObservableCollection<T> : AggregatingObservableCollection<T>
    {
        #region Constants and Fields

        /// <summary>
        /// The current dispatcher reference
        /// </summary>
        private readonly Dispatcher currentDispatcher;

        /// <summary>
        /// Backing store for Suppress On Collection Changed property
        /// </summary>
        private bool suppressOnCollectionChanged;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchingObservableCollection{T}"/> class. 
        /// The dispatching observable collection.
        /// </summary>
        public DispatchingObservableCollection()
        {
            // Assign the current Dispatcher (owner of the collection)
            this.currentDispatcher = Dispatcher.CurrentDispatcher;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchingObservableCollection{T}"/> class.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public DispatchingObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchingObservableCollection{T}"/> class. 
        /// Initializes a new instance of the DispatchingObservableCollection
        /// class that contains elements copied from the specified list.
        /// </summary>
        /// <param name="list">
        /// The list from which the elements are copied.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="collection"/> parameter cannot be null.
        /// </exception>
        public DispatchingObservableCollection(List<T> list)
            : base(list)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether to suppress collection changed notifications.
        /// When set back to false will sent a notification changed action.
        /// </summary>
        /// <value>
        ///     <c>true</c> if [suppress on collection changed]; otherwise, <c>false</c>.
        /// </value>
        public bool SuppressOnCollectionChanged
        {
            get
            {
                return this.suppressOnCollectionChanged;
            }

            set
            {
                if (this.suppressOnCollectionChanged && value == false)
                {
                    this.suppressOnCollectionChanged = false;
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
                else
                {
                    this.suppressOnCollectionChanged = value;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the range while suppressing change notifications
        /// </summary>
        /// <param name="items">
        /// The items to add to the collection.
        /// </param>
        public void AddRange(IEnumerable<T> items)
        {
            if (null == items)
            {
                throw new ArgumentNullException("items");
            }

            if (items.Any())
            {
                try
                {
                    this.SuppressOnCollectionChanged = true;
                    foreach (T item in items)
                    {
                        this.Add(item);
                    }
                }
                finally
                {
                    this.SuppressOnCollectionChanged = false;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clears all items
        /// </summary>
        protected override void ClearItems()
        {
            this.DoDispatchedAction(this.BaseClearItems);
        }

        /// <summary>
        /// Inserts a item at the specified index
        /// </summary>
        /// <param name="index">
        /// The index where the item should be inserted
        /// </param>
        /// <param name="item">
        /// The item which should be inserted
        /// </param>
        protected override void InsertItem(int index, T item)
        {
            this.DoDispatchedAction(() => this.BaseInsertItem(index, item));
        }

        /// <summary>
        /// Moves an item from one index to another
        /// </summary>
        /// <param name="oldIndex">
        /// The index of the item which should be moved
        /// </param>
        /// <param name="newIndex">
        /// The index where the item should be moved
        /// </param>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            this.DoDispatchedAction(() => this.BaseMoveItem(oldIndex, newIndex));
        }

        /// <summary>
        /// Fires the CollectionChanged Event
        /// </summary>
        /// <param name="e">
        /// The additional arguments of the event
        /// </param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.DoDispatchedAction(() => this.BaseOnCollectionChanged(e));
        }

        /// <summary>
        /// Fires the PropertyChanged Event
        /// </summary>
        /// <param name="e">
        /// The additional arguments of the event
        /// </param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.DoDispatchedAction(() => this.BaseOnPropertyChanged(e));
        }

        /// <summary>
        /// Removes the item at the specified index
        /// </summary>
        /// <param name="index">
        /// The index of the item which should be removed
        /// </param>
        protected override void RemoveItem(int index)
        {
            this.DoDispatchedAction(() => this.BaseRemoveItem(index));
        }

        /// <summary>
        /// Sets the item at the specified index
        /// </summary>
        /// <param name="index">
        /// The index which should be set
        /// </param>
        /// <param name="item">
        /// The new item
        /// </param>
        protected override void SetItem(int index, T item)
        {
            this.DoDispatchedAction(() => this.BaseSetItem(index, item));
        }

        /// <summary>
        /// Calls the bases clear items.
        /// </summary>
        private void BaseClearItems()
        {
            base.ClearItems();
        }

        /// <summary>
        /// Calls the base insert item.
        /// </summary>
        /// <param name="index">
        /// The index in the collection.
        /// </param>
        /// <param name="item">
        /// The item to insert.
        /// </param>
        private void BaseInsertItem(int index, T item)
        {
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Calls the base move item.
        /// </summary>
        /// <param name="oldIndex">
        /// The old index.
        /// </param>
        /// <param name="newIndex">
        /// The new index.
        /// </param>
        private void BaseMoveItem(int oldIndex, int newIndex)
        {
            base.MoveItem(oldIndex, newIndex);
        }

        /// <summary>
        /// Calls the base on collection changed.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void BaseOnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!this.SuppressOnCollectionChanged)
            {
                base.OnCollectionChanged(e);
            }
        }

        /// <summary>
        /// Calls the base on property changed.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void BaseOnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        /// <summary>
        /// Calls the base remove item.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        private void BaseRemoveItem(int index)
        {
            base.RemoveItem(index);
        }

        /// <summary>
        /// Calls the base set item.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="item">
        /// The item to set.
        /// </param>
        private void BaseSetItem(int index, T item)
        {
            base.SetItem(index, item);
        }

        /// <summary>
        /// Executes this action in the right thread
        /// </summary>
        /// <param name="action">
        /// The action which should be executed
        /// </param>
        private void DoDispatchedAction(Action action)
        {
            if (this.currentDispatcher.CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                this.currentDispatcher.Invoke(DispatcherPriority.DataBind, action);
            }
        }

        #endregion
    }
}