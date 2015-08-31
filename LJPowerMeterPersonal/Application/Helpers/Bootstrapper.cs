/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.UI
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Threading;
    using LaJust.PowerMeter.Common;
    using LaJust.PowerMeter.Common.Events;
    using LaJust.PowerMeter.Common.Extensions;
    using LaJust.PowerMeter.Common.Helpers;
    using LaJust.PowerMeter.Common.Models;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Composite.Presentation.Regions;
    using Microsoft.Practices.Composite.UnityExtensions;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Application Bootstapper
    /// </summary>
    public class Bootstrapper : UnityBootstrapper
    {
        /// <summary>
        /// Configures the <see cref="T:Microsoft.Practices.Unity.IUnityContainer"/>. May be overwritten in a derived class to add specific
        /// type mappings required by the application.
        /// </summary>
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.AddExtension(new UnityExtensions.UnityExtensionWithTypeTracking());

            // Register our common singletons
            Container.RegisterType<ConfigProperties>(new ContainerControlledLifetimeManager());
            Container.RegisterType<GameMetaDataModel>(new ContainerControlledLifetimeManager());
        }

        /// <summary>
        /// Initializes the modules. May be overwritten in a derived class to use a custom Modules Catalog
        /// </summary>
        protected override void InitializeModules()
        {
            PopulateConfiguration();

            base.InitializeModules();

            Dispatcher.CurrentDispatcher.BeginInvoke((Action)delegate() 
            {
                // Publish event to let modules know all modules have completed initializing
                Container.Resolve<IEventAggregator>()
                    .GetEvent<ProcessEvent>().Publish(ProcessEventType.ModulesInitialized);
            }, System.Windows.Threading.DispatcherPriority.SystemIdle);
        }

        /// <summary>
        /// Returns the module catalog that will be used to initialize the modules.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="T:Microsoft.Practices.Composite.Modularity.IModuleCatalog"/> that will be used to initialize the modules.
        /// </returns>
        /// <remarks>
        /// When using the default initialization behavior, this method must be overwritten by a derived class.
        /// </remarks>
        protected override IModuleCatalog GetModuleCatalog()
        {
            return new ModuleCatalog()
                .AddModule(typeof(LaJust.PowerMeter.Modules.Receiver.ReceiverModule))
                .AddModule(typeof(LaJust.PowerMeter.Modules.RemoteControl.RemoteControlModule))

                .AddModule(typeof(LaJust.PowerMeter.Modules.Screens.ScreensModule))
                .AddModule(typeof(LaJust.PowerMeter.Modules.Config.ConfigModule))
                .AddModule(typeof(LaJust.PowerMeter.Modules.CountDownClock.CountDownClockModule))
                .AddModule(typeof(LaJust.PowerMeter.Modules.Meter.MeterModule))
                .AddModule(typeof(LaJust.PowerMeter.Modules.MultiMedia.MultiMediaModule))
                .AddModule(typeof(LaJust.PowerMeter.Modules.GameEngine.GameEngineModule))
                ;
        }

        /// <summary>
        /// Configures the default region adapter mappings to use in the application, in order
        /// to adapt UI controls defined in XAML to use a region and register it automatically.
        /// May be overwritten in a derived class to add specific mappings required by the application.
        /// </summary>
        /// <returns>
        /// The <see cref="T:Microsoft.Practices.Composite.Presentation.Regions.RegionAdapterMappings"/> instance containing all the mappings.
        /// </returns>
        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            RegionAdapterMappings regionAdapterMappings = Container.TryResolve<RegionAdapterMappings>();
            IRegionBehaviorFactory factory = Container.TryResolve<IRegionBehaviorFactory>();

            if (regionAdapterMappings != null && factory != null)
            {
                regionAdapterMappings.RegisterMapping(typeof(Transitionals.Controls.TransitionElement), new TransitionElementRegionAdapter(factory));
            }

            return base.ConfigureRegionAdapterMappings(); 
        }

        /// <summary>
        /// Creates the shell or main window of the application.
        /// </summary>
        /// <returns>The shell of the application.</returns>
        /// <remarks>
        /// If the returned instance is a <see cref="T:System.Windows.DependencyObject"/>, the
        /// <see cref="T:Microsoft.Practices.Composite.UnityExtensions.UnityBootstrapper"/> will attach the default <seealso cref="T:Microsoft.Practices.Composite.Regions.IRegionManager"/> of
        /// the application in its <see cref="F:Microsoft.Practices.Composite.Presentation.Regions.RegionManager.RegionManagerProperty"/> attached property
        /// in order to be able to add regions by using the <seealso cref="F:Microsoft.Practices.Composite.Presentation.Regions.RegionManager.RegionNameProperty"/>
        /// attached property from XAML.
        /// </remarks>
        protected override DependencyObject CreateShell()
        {
            Shell shell = Container.Resolve<Shell>();
            shell.Show(); // Needs to be shown before we add external monitor child window

            // Check for multiple monitors
            if (SystemInformation.MonitorCount > 1)
                CreateExternalShell(Screen.AllScreens.Last(), shell);

            // Make sure the main shell window is the one with focus
            shell.Activate();

            return shell as DependencyObject;
        }

        /// <summary>
        /// Setups the external window.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <param name="owner">The owner.</param>
        /// <returns></returns>
        private Window CreateExternalShell(Screen screen, Shell owner)
        {
            ExternalShell externalShell = Container.Resolve<ExternalShell>();

            externalShell.Left = screen.WorkingArea.Left;
            externalShell.Top = screen.WorkingArea.Top;
            externalShell.Width = screen.WorkingArea.Width;
            externalShell.Height = screen.WorkingArea.Height;
            externalShell.Owner = owner;

            externalShell.Show();
            externalShell.WindowState = WindowState.Maximized; //do this after Show() or won't draw on secondary screens
            return externalShell;
        }

        /// <summary>
        /// Populates the configuration.
        /// </summary>
        private void PopulateConfiguration()
        {
            // Check for application upgrade
            string appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (Properties.Settings.Default.ApplicationVersion != appVersion)
            {
                System.Diagnostics.Trace.WriteLine("Upgrading property settings");
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.ApplicationVersion = appVersion;
            }

            // Populate our container configuration object
            Container.Resolve<ConfigProperties>().Populate(Properties.Settings.Default.PropertyValues);
        }

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        internal void SaveConfiguration()
        {
            Container.Resolve<ConfigProperties>().Update(Properties.Settings.Default.PropertyValues);
            Properties.Settings.Default.Save();
        }
    }
}
