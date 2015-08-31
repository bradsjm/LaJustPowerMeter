// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregatingObservableCollection.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Observable Collection that bubbles up property change notifications from child members
    /// </summary>
    /// <typeparam name="T">IEnumerable collection</typeparam>
    public class AggregatingObservableCollection<T> : ObservableCollection<T>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatingObservableCollection{T}"/> class. 
        /// Initializes a new instance of the <see cref="AggregatingObservableCollection&lt;T&gt;"/> class.
        /// </summary>
        protected AggregatingObservableCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatingObservableCollection{T}"/> class. 
        /// Initializes a new instance of the <see cref="AggregatingObservableCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        protected AggregatingObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatingObservableCollection{T}"/> class. 
        /// Initializes a new instance of the <see cref="AggregatingObservableCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="list">
        /// The generic list.
        /// </param>
        protected AggregatingObservableCollection(List<T> list)
            : base(list)
        {
        }

        #endregion

        #region Delegates

        /// <summary>
        /// The child element property changed event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The ChildElementPropertyChangedEventArgs.
        /// </param>
        public delegate void ChildElementPropertyChangedEventHandler(
            object sender, ChildElementPropertyChangedEventArgs e);

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [child element property changed].
        /// </summary>
        public event ChildElementPropertyChangedEventHandler ChildElementPropertyChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Clears the items.
        /// </summary>
        protected override void ClearItems()
        {
            foreach (T item in this.Items)
            {
                var convertedItem = item as INotifyPropertyChanged;
                if (convertedItem != null)
                {
                    convertedItem.PropertyChanged -= this.ConvertedItemPropertyChanged;
                }
            }

            base.ClearItems();
        }

        /// <summary>
        /// Raises the <see cref="OnCollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var convertedItem in e.NewItems.OfType<INotifyPropertyChanged>())
                    {
                        convertedItem.PropertyChanged += this.ConvertedItemPropertyChanged;
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var convertedItem in e.OldItems.OfType<INotifyPropertyChanged>())
                    {
                        convertedItem.PropertyChanged -= this.ConvertedItemPropertyChanged;
                    }

                    break;
            }
        }

        /// <summary>
        /// Called when child element property changes.
        /// </summary>
        /// <param name="childelement">
        /// The child element that changed.
        /// </param>
        /// <param name="propertyName">
        /// The property Name that changed.
        /// </param>
        private void OnChildElementPropertyChanged(object childelement, string propertyName)
        {
            if (this.ChildElementPropertyChanged != null)
            {
                this.ChildElementPropertyChanged(
                    this, new ChildElementPropertyChangedEventArgs(childelement, propertyName));
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the convertedItem control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void ConvertedItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnChildElementPropertyChanged(sender, e.PropertyName);
        }

        #endregion

        /// <summary>
        /// The child element property changed event args.
        /// </summary>
        public class ChildElementPropertyChangedEventArgs : EventArgs
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="AggregatingObservableCollection&lt;T&gt;.ChildElementPropertyChangedEventArgs"/> class.
            /// </summary>
            /// <param name="item">The item that changed.</param>
            /// <param name="propertyName">Name of the property that changed.</param>
            public ChildElementPropertyChangedEventArgs(object item, string propertyName)
            {
                this.ChildElement = item;
                this.PropertyName = propertyName;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets ChildElement.
            /// </summary>
            public object ChildElement { get; set; }

            /// <summary>
            /// Gets or sets PropertyName.
            /// </summary>
            public string PropertyName { get; set; }

            #endregion
        }
    }
}