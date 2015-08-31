/*
 * © Copyright 2010 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.UI
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;
    using LaJust.PowerMeter.Common;
    using LaJust.PowerMeter.Common.Events;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Static reference to our bootstrapper
        /// </summary>
        private static Bootstrapper _bootstrapper;

        /// <summary>
        /// Handles the Startup event of the Application.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.StartupEventArgs"/> instance containing the event data.</param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Check to see if application is already running and terminate if so
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
                Environment.Exit(1);

            Trace.Write("Application Startup. Version " + Assembly.GetExecutingAssembly().GetName().Version);

            // Enable visual styles for windows forms
            System.Windows.Forms.Application.EnableVisualStyles();

            // Show Splash Screen
            new SplashScreen("Resources/SplashScreen.jpg").Show(true);

            // Hook the Power Mode event to publish notification of power changes
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;

            try
            {
                _bootstrapper = new Bootstrapper();
                _bootstrapper.Run();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Handles the Exit event of the Application.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.ExitEventArgs"/> instance containing the event data.</param>
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                if (_bootstrapper != null && _bootstrapper.Container != null)
                {
                    _bootstrapper.Container.Resolve<IEventAggregator>().GetEvent<ProcessEvent>().Publish(ProcessEventType.ApplicationShutdown);
                    _bootstrapper.SaveConfiguration();
                    _bootstrapper.Container.Dispose();
                }
                Trace.TraceInformation("Program completed");
                Debug.Flush();
                Trace.Flush();
            }
            catch { }
        }

        /// <summary>
        /// Unhandled exception handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        /// <summary>
        /// Handles the application exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        private static void HandleException(Exception ex)
        {
            if (ex == null) return;
            try
            {
                Trace.TraceError(ex.ToString());
                Debug.Flush();
                Trace.Flush();
                MessageBox.Show(ex.GetBaseException().Message, "Application Error",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);
            }
            finally
            {
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Handles the PowerModeChanged event of the SystemEvents control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Win32.PowerModeChangedEventArgs"/> instance containing the event data.</param>
        protected void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (_bootstrapper != null && _bootstrapper.Container != null && e.Mode == PowerModes.Resume)
                _bootstrapper.Container.Resolve<IEventAggregator>().GetEvent<ProcessEvent>()
                    .Publish(ProcessEventType.SystemResumed);
        }
    }
}
