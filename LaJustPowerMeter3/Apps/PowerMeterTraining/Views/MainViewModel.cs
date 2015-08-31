// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// --------------------------------------------------------------------------------------------------------------------

namespace Shell
{
    using System.ComponentModel.Composition;
    using System.Windows.Input;

    using Infrastructure;

    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.Events;
    using Microsoft.Practices.Prism.Regions;
    using Microsoft.Practices.Prism.ViewModel;

    /// <summary>
    /// View Model for Main Window View
    /// </summary>
    [Export]
    public class MainViewModel : NotificationObject
    {
        #region Constants and Fields

        /// <summary>
        /// The popup visible.
        /// </summary>
        private bool popupVisible;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="regionManager">
        /// The region Manager.
        /// </param>
        /// <param name="showOverlayRequest">
        /// The show Overlay Request.
        /// </param>
        [ImportingConstructor]
        public MainViewModel(IRegionManager regionManager, ApplicationEvents.ShowOverlay showOverlayRequest)
        {
            showOverlayRequest.Subscribe(
                viewName =>
                    {
                        object view = regionManager.Regions[RegionNames.OverlayRegion].GetView(viewName);
                        regionManager.Regions[RegionNames.OverlayRegion].Activate(view);
                        this.PopupVisible = true;
                    }, 
                ThreadOption.UIThread, 
                true);

            this.CloseCommand = new DelegateCommand(delegate { this.PopupVisible = false; });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the popup close command.
        /// </summary>
        /// <value>The close command.</value>
        public ICommand CloseCommand { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether popup region is visible.
        /// </summary>
        /// <value><c>true</c> if [popup visible]; otherwise, <c>false</c>.</value>
        public bool PopupVisible
        {
            get
            {
                return this.popupVisible;
            }

            set
            {
                this.popupVisible = value;
                this.RaisePropertyChanged(() => this.PopupVisible);
            }
        }

        #endregion
    }
}