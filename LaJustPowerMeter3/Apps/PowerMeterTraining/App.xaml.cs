// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// --------------------------------------------------------------------------------------------------------------------

namespace Shell
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Handles the application exception.
        /// </summary>
        /// <param name="ex">
        /// The exception.
        /// </param>
        private static void HandleException(Exception ex)
        {
            if (ex != null)
            {
                try
                {
                    Trace.TraceError(ex.ToString());
                    Debug.Flush();
                    Trace.Flush();
                }
                finally
                {
                    MessageBox.Show(
                        ex.GetBaseException().Message,
                        "Application Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Exclamation,
                        MessageBoxResult.OK);
                    Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// Handles the Startup event of the Application.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.StartupEventArgs"/> instance containing the event data.
        /// </param>
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            // Check to see if application is already running and terminate if so
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                Environment.Exit(1);
            }

            // Log application version
            Trace.Write("Application Startup. Version " + Assembly.GetExecutingAssembly().GetName().Version);

            // Enable visual styles for windows forms
            System.Windows.Forms.Application.EnableVisualStyles();

            // Check there is a valid license or evaluation for the application
#if LICENSED
            this.CheckLicense();
#endif

            // Show Splash Screen
            new SplashScreen("Resources/SplashScreen.jpg").Show(true);

            var bootstrapper = new Bootstrapper();
#if DEBUG
            bootstrapper.Run();
#else
            try
            {
                bootstrapper.Run();
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
#endif
        }

        /// <summary>
        /// Handles the Exit event of the Application.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.ExitEventArgs"/> instance containing the event data.
        /// </param>
        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            Trace.TraceInformation("Program completed. Application exit code " + e.ApplicationExitCode);
            try
            {
                Debug.Flush();
                Trace.Flush();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Unhandled exception handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.UnhandledExceptionEventArgs"/> instance containing the event data.
        /// </param>
        private void ApplicationDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        #region Licensing Code
#if LICENSED
        private void CheckLicense()
        {
            const int TRIAL_DAYS = 30;

            const string LICENSE_PARAMETERS =
                @"<AuthenticatedLicenseParameters>
	              <EncryptedLicenseParameters>
	                <ProductName>LaJust Power Meter</ProductName>
	                <RSAKeyValue>
	                  <Modulus>n66mRwUlIE5rGPYLtrCrdDR1yGuOZbyALMEKvO6X2fIqWkTgVtaCKHVnjzxVG1JSOlngHuCtoOypeuL0pNKyxj4/0TGny7mYS5Eas2J0NnDEH2P75FsVzGQ7ESNyns/uXEmtCgzVCtfktW3X7bNQgi3ycb1ZCwJVjVaoQGiuUyM=</Modulus>
	                  <Exponent>AQAB</Exponent>
	                </RSAKeyValue>
	                <DesignSignature>X0XJLQFa08k1f6BD9Zte2OyJyW/mk0dro0EYPN9nDM5egOzn1ljCBBGWdPEZmPHz++G7w1uNvTMI/KcIlgUTa/oTJAbd/kOXbDIDliZX1NAPkxtgAdhxiZL/TL3rx9faMdWg78C4jucprOmTCHHAjz1xHtBRoOWbQhV3jT9LhX0=</DesignSignature>
	                <RuntimeSignature>L5hUaIqMKt3xY0jOl8oKcoPnDzLfuZIV5/gHKJt/cqrm6ufnfmynGjNiL24nt2Lqczt+P3HeUSIxnikJJtJ+bRI5d6ljiRuP2vB1yu73gEAU1cF24HKz3v+xMsYmRBdVvuShKCvpLB2JFAjZE9HejFr73p+pCIlsZyZjAn2H930=</RuntimeSignature>
	                <KeyStrength>7</KeyStrength>
	                <ChecksumProductInfo>True</ChecksumProductInfo>
	                <TextEncoding>Base32</TextEncoding>
	                <ShortSerialNo>False</ShortSerialNo>
	              </EncryptedLicenseParameters>
	              <AuthenticationServerURL>http://authsvc.lajust.net/LicenseService/AuthenticationService.asmx</AuthenticationServerURL>
	              <ServerRSAKeyValue>
	                <Modulus>zpt4gdTSKUZTSpbfEL02AJKqNbfeJZ79Xe6Z76dCzX+ntPQL+lusGJ1TUbGpY6SPdLHc++8Dvnl5F+8mT2nBhBNUYtVCohE3GrQin1B/zi42PUvgy/lx4jbW1GfG2blYYqwgp/TOXciQo+aHcuhgpX8w+1azd6zD/gb/yo8asPc=</Modulus>
	                <Exponent>AQAB</Exponent>
	              </ServerRSAKeyValue>
	            </AuthenticatedLicenseParameters>";

            // Check for machine names checksums that do not require license files
            // We don't put the actual machine name in the code for security reasons
            switch (LicenseUtilities.Checksum(Environment.MachineName.ToUpper()))
            {
                case "766": // "LAJUST-MSI" MSI 2200AE All In One Machine
                case "754": // "LAJUST-PC"  Dell Studio One All In One Machine
                    return;
            }

            // The license file, the directory will automatically get created if required
            string licenseFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\LaJust\LaJustPowerMeter.lic";

            // Check if there is a valid license for the application
            MyAuthenticatedLicenseProvider licenseProvider = new MyAuthenticatedLicenseProvider();
            AuthenticatedLicense license = licenseProvider.GetLicense(LICENSE_PARAMETERS, licenseFile, true);

            // Allow forcing of the license dialog using command line option
            if (Environment.CommandLine.ToUpper().Contains("/LICENSE"))
            {
                MyAuthenticatedLicenseInstallForm licenseForm = new MyAuthenticatedLicenseInstallForm();
                license = licenseForm.ShowDialog(Solution.Product, licenseFile, license);
            }

            // If there is no installed license then display the evaluation dialog until
            // the user installs a license or selects Exit or Continue
            EvaluationMonitor evaluationMonitor = new IsolatedStorageEvaluationMonitor(LicenseUtilities.Checksum(Solution.Version), false, false);
            EvaluationDialog evaluationDialog = new EvaluationDialog(evaluationMonitor, Solution.Product) { TrialDays = TRIAL_DAYS, ExtendedTrialDays = TRIAL_DAYS };

            while (license == null || license.Status != AuthenticatedLicenseStatus.Valid)
            {
                EvaluationDialogResult dialogResult = evaluationDialog.ShowDialog();
                if (dialogResult == EvaluationDialogResult.Exit)
                {
                    Environment.Exit(0);
                }
                else if (dialogResult == EvaluationDialogResult.Continue)
                {
                    break; // exit the loop 
                }
                else if (dialogResult == EvaluationDialogResult.InstallLicense)
                {
                    MyAuthenticatedLicenseInstallForm licenseForm = new MyAuthenticatedLicenseInstallForm();
                    license = licenseForm.ShowDialog(Solution.Product, licenseFile, license);
                }
            }

            // Reset the evaluation monitor if there is a currently valid license, this allows
            // for another 30 days trial if something should happen to the license
            if (license != null && license.Status == AuthenticatedLicenseStatus.Valid)
            {
                evaluationMonitor.Reset(true);
            }
        }
#endif

        #endregion
    }
}