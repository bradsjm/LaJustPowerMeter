// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bootstrapper.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// --------------------------------------------------------------------------------------------------------------------

namespace Shell
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Threading;

    using Infrastructure;

    using Microsoft.Practices.Prism.Logging;
    using Microsoft.Practices.Prism.MefExtensions;
    using Microsoft.Practices.Prism.Regions;
    using Microsoft.Practices.ServiceLocation;

    using Application = System.Windows.Application;

    /// <summary>
    /// MefBootstrapper enables all of the Prism Library services by default.
    /// </summary>
    internal sealed class Bootstrapper : MefBootstrapper
    {
        /// <summary>
        /// Configures the <see cref="AggregateCatalog"/> used by MEF.
        /// </summary>
        /// <remarks>
        /// The base implementation does nothing.
        /// </remarks>
        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();

            // Add the application assembly and application references to catalog
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            this.AggregateCatalog.Catalogs.Add(new DirectoryCatalog("."));

            // These modules are discovered by inspecting the "Modules" directory.
            this.AggregateCatalog.Catalogs.Add(new DirectoryCatalog("Modules"));
        }

        /// <summary>
        /// Configures the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer"/>.
        /// May be overwritten in a derived class to add specific type mappings required by the application.
        /// </summary>
        /// <remarks>
        /// The base implementation registers all the types direct instantiated by the bootstrapper with the container.
        /// If the method is overwritten, the new implementation should call the base class version.
        /// </remarks>
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            this.Container.ComposeExportedValue(this.Container);

            // Ensure we properly dispose of objects in the container at application exit
            Application.Current.Exit += (sender, e) => this.Container.Dispose();
        }

        /// <summary>
        /// Configures the <see cref="T:Microsoft.Practices.Prism.Regions.IRegionBehaviorFactory"/>.
        /// This will be the list of default behaviors that will be added to a region.
        /// </summary>
        /// <returns>IRegionBehaviorFactory containing additional default region registrations</returns>
        protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            var factory = base.ConfigureDefaultRegionBehaviors();
            factory.AddIfMissing("AutoPopulateExportedViewsBehavior", typeof(AutoPopulateExportedViewsBehavior));
            return factory;
        }

        /// <summary>
        /// Creates the shell or main window of the application.
        /// </summary>
        /// <returns>The shell of the application.</returns>
        /// <remarks>
        /// If the returned instance is a <see cref="T:System.Windows.DependencyObject"/>, the
        /// <see cref="T:Microsoft.Practices.Prism.Bootstrapper"/> will attach the default <seealso cref="T:Microsoft.Practices.Prism.Regions.IRegionManager"/> of
        /// the application in its <see cref="F:Microsoft.Practices.Prism.Regions.RegionManager.RegionManagerProperty"/> attached property
        /// in order to be able to add regions by using the <seealso cref="F:Microsoft.Practices.Prism.Regions.RegionManager.RegionNameProperty"/>
        /// attached property from XAML.
        /// </remarks>
        protected override DependencyObject CreateShell()
        {
            return this.Container.GetExportedValue<MainView>();
        }

        /// <summary>
        /// Initializes the modules. May be overwritten in a derived class to use a custom Modules Catalog
        /// </summary>
        protected override void InitializeModules()
        {
            base.InitializeModules();

            // Active the event page before we show the interface
            var regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
            var page = regionManager.Regions[RegionNames.PrimaryPageRegion].GetView(PageNames.Event);
            regionManager.Regions[RegionNames.PrimaryPageRegion].Activate(page);

            // Notification to application upon main window closing
            Application.Current.MainWindow.Closed +=
                (s, e) => ServiceLocator.Current.GetInstance<ApplicationEvents.ApplicationClosing>().Publish(e);

            this.Logger.Log("Showing main window", Category.Debug, Priority.Low);
            Application.Current.MainWindow.Show();
        }

        /// <summary>
        /// Initializes the shell.
        /// </summary>
        /// <remarks>
        /// The base implemention ensures the shell is composed in the container.
        /// </remarks>
        protected override void InitializeShell()
        {
            base.InitializeShell();
            Application.Current.MainWindow = (MainView)this.Shell;

            // Check for multiple monitors
            // if (System.Windows.Forms.SystemInformation.MonitorCount > 1)
            // {
            // this.InitializeExternalShell(System.Windows.Forms.Screen.AllScreens.Last(), Application.Current.MainWindow);
            // }
        }

        /// <summary>
        /// Setup the external window for secondary monitor on a new dispatcher thread.
        /// </summary>
        /// <param name="screen">
        /// The screen to put the secondary shell on.
        /// </param>
        /// <param name="owner">
        /// The main application window owner handle.
        /// </param>
        private void InitializeExternalShell(Screen screen, Window owner)
        {
            var thread = new Thread(
                () =>
                    {
                        var externalShell = this.Container.GetExportedValue<ExternalView>();
                        externalShell.Closed += (s, e) => externalShell.Dispatcher.InvokeShutdown();
                        externalShell.SizeToScreen(screen);
                        externalShell.Show();
                        externalShell.WindowState = WindowState.Maximized;

                        // owner.Activated += (s,e) => externalShell.Dispatcher.BeginInvoke((Action)externalShell.Show);
                        owner.Closed += (s, e) => externalShell.Dispatcher.BeginInvoke((Action)externalShell.Close);

                        RegionManager.SetRegionManager(
                            externalShell, this.Container.GetExportedValue<IRegionManager>().CreateRegionManager());
                        Dispatcher.Run();
                    });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}