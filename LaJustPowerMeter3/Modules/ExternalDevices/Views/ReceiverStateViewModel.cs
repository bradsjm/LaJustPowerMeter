// <copyright file="ReceiverStateViewModel.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace ExternalDevices
{
    using System.ComponentModel.Composition;
    using Infrastructure;
    using Microsoft.Practices.Prism.ViewModel;

    /// <summary>
    /// Receiver State Presenter
    /// </summary>
    [Export]
    public class ReceiverStateViewModel : NotificationObject
    {
        /// <summary>
        /// The Receiver Service
        /// </summary>
        private readonly IReceiverEventPublisher ReceiverService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverStateViewModel"/> class.
        /// </summary>
        [ImportingConstructor]
        public ReceiverStateViewModel(IReceiverEventPublisher receiverService)
        {
            this.ReceiverService = receiverService;
            this.ReceiverService.OnChanged(o => o.ReceiverCount).Do(delegate
            {
                this.RaisePropertyChanged(() => this.ReceiverCount);
                this.RaisePropertyChanged(() => this.Connected);
            });
        }

        /// <summary>
        /// Gets a value indicating whether the receiver is connected.
        /// </summary>
        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        public bool Connected
        {
            get { return this.ReceiverService !=null && this.ReceiverService.ReceiverCount > 0; }
        }

        /// <summary>
        /// Gets the receiver count.
        /// </summary>
        /// <value>The receiver count.</value>
        public int ReceiverCount
        {
            get { return this.ReceiverService !=null ? this.ReceiverService.ReceiverCount : 0; }
        }
    }
}
