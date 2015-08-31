// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyObserver.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// The observable property changed.
    /// </summary>
    public static class ObservablePropertyChanged
    {
        #region Public Methods

        /// <summary>
        /// returns a PropertyChangedSubscriber so that you can hook to PropertyChanged
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <returns>
        /// Returns the subscriber
        /// </returns>
        public static PropertyChangedSubscriber<TSource, TProperty> OnChanged<TSource, TProperty>(
            this TSource source, Expression<Func<TSource, TProperty>> property)
            where TSource : class, INotifyPropertyChanged
        {
            return new PropertyChangedSubscriber<TSource, TProperty>(source, property);
        }

        #endregion
    }

    /// <summary>
    /// Shortcut to subscribe to PropertyChanged on an INotfiyPropertyChanged and executes an action when that happens
    /// </summary>
    /// <typeparam name="TSource">
    /// Must implement INotifyPropertyChanged
    /// </typeparam>
    /// <typeparam name="TProperty">
    /// Can be any type
    /// </typeparam>
    public class PropertyChangedSubscriber<TSource, TProperty> : IDisposable
        where TSource : class, INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>
        /// The _property validation.
        /// </summary>
        private readonly Expression<Func<TSource, TProperty>> propertyValidation;

        /// <summary>
        /// The _source.
        /// </summary>
        private readonly TSource source;

        /// <summary>
        /// The _on change.
        /// </summary>
        private Action<TSource> onChange;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedSubscriber{TSource,TProperty}"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="property">
        /// The property.
        /// </param>
        public PropertyChangedSubscriber(TSource source, Expression<Func<TSource, TProperty>> property)
        {
            this.propertyValidation = property;
            this.source = source;
            source.PropertyChanged += this.SourcePropertyChanged;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Executes the action and returns an IDisposable so that you can unregister 
        /// </summary>
        /// <param name="onChanged">
        /// The action to execute
        /// </param>
        /// <returns>
        /// The IDisposable so that you can unregister
        /// </returns>
        public IDisposable Do(Action<TSource> onChanged)
        {
            this.onChange = onChanged;
            return this;
        }

        /// <summary>
        /// Executes the action only once and automatically unregisters
        /// </summary>
        /// <param name="onChanged">
        /// The action to be executed
        /// </param>
        public void DoOnce(Action<TSource> onChanged)
        {
            Action<TSource> dispose = x => this.Dispose();
            this.onChange = (Action<TSource>)Delegate.Combine(onChanged, dispose);
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        /// Unregisters the property
        /// </summary>
        public void Dispose()
        {
            this.source.PropertyChanged -= this.SourcePropertyChanged;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether [is property valid] [the specified property name].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        ///     <c>true</c> if [is property valid] [the specified property name]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsPropertyValid(string propertyName)
        {
            var propertyInfo = ((MemberExpression)this.propertyValidation.Body).Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }

            return propertyInfo.Name == propertyName;
        }

        /// <summary>
        /// Sources the property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void SourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsPropertyValid(e.PropertyName))
            {
                this.onChange(sender as TSource);
            }
        }

        #endregion
    }
}