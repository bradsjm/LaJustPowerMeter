/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */

namespace LaJust.PowerMeter.Modules.Receiver.Presenters
{
    using System.ComponentModel;
    using LaJust.PowerMeter.Common.BaseClasses;
    using LaJust.PowerMeter.Modules.Receiver.Services;
    using LaJust.PowerMeter.Common.Helpers;

    /// <summary>
    /// Receiver State Presenter
    /// </summary>
    public class ReceiverStatePresenter : Presenter
    {
        private readonly ReceiverService _service;
        private readonly PropertyObserver<ReceiverService> _observer;

        #region Public Fields

        /// <summary>
        /// Gets or sets a value indicating whether the receiver is connected.
        /// </summary>
        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        public bool Connected
        {
            get { return _service.ReceiverCount > 0; }
        }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverStatePresenter"/> class.
        /// </summary>
        public ReceiverStatePresenter(ReceiverService service)
        {
            _service = service;
            _observer = new PropertyObserver<ReceiverService>(_service)
                .RegisterHandler(o => o.ReceiverCount, o => OnPropertyChanged("Connected"));
        }

        #endregion
    }
}
