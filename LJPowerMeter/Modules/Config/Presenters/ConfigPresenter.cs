/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.Config.Presenters
{
    using System;
    using LaJust.PowerMeter.Common.BaseClasses;
    using LaJust.PowerMeter.Common;
using LaJust.PowerMeter.Common.Helpers;
using System.Windows.Input;
    using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Events;
    using LaJust.PowerMeter.Common.Events;

    public class ConfigPresenter : Presenter
    {
        private readonly ConfigProperties _config;
        private readonly PropertyObserver<ConfigProperties> _configObserver;

        #region Public Binding Properties

        /// <summary>
        /// Gets or sets the required impact level.
        /// </summary>
        /// <value>The required impact level.</value>
        public byte RequiredImpactLevel
        {
            get { return _config.RequiredImpactLevel; }
            set { _config.RequiredImpactLevel = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [contact sensor required].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [contact sensor required]; otherwise, <c>false</c>.
        /// </value>
        public bool ContactSensorRequired
        {
            get { return _config.ContactSensorRequired; }
            set { _config.ContactSensorRequired = value; }
        }

        public ICommand RegisterChungCommand { get; private set; }
        public ICommand RegisterHongCommand { get; private set; }
        public ICommand RegisterTargetCommand { get; private set; }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigPresenter"/> class.
        /// </summary>
        /// <param name="config">The config.</param>
        public ConfigPresenter(ConfigProperties config, IEventAggregator eventAggregator)
        {
            _config = config;
            _configObserver = new PropertyObserver<ConfigProperties>(_config);
            _configObserver.RegisterHandler(o => o.RequiredImpactLevel, o => OnPropertyChanged("RequiredImpactLevel"));
            _configObserver.RegisterHandler(o => o.ContactSensorRequired, o => OnPropertyChanged("ContactSensorRequired"));

            RegisterChungCommand = new DelegateCommand<object>(obj =>
            {
                eventAggregator.GetEvent<RemoteEvents.ButtonPressed>().Publish(RemoteEvents.Buttons.RegisterChung);
            });

            RegisterHongCommand = new DelegateCommand<object>(obj =>
            {
                eventAggregator.GetEvent<RemoteEvents.ButtonPressed>().Publish(RemoteEvents.Buttons.RegisterHong);
            });

            RegisterTargetCommand = new DelegateCommand<object>(obj =>
            {
                eventAggregator.GetEvent<RemoteEvents.ButtonPressed>().Publish(RemoteEvents.Buttons.RegisterTarget);
            });
        }

        #endregion
    }
}
