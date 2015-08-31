// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomInstaller.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// --------------------------------------------------------------------------------------------------------------------

namespace Shell
{
    using System.Collections;
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    /// <summary>
    /// Custom Installer to pre-compile the assemblies to native code using ngen.exe
    /// </summary>
    [RunInstaller(true)]
    public partial class CustomInstaller : Installer
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomInstaller"/> class.
        /// </summary>
        public CustomInstaller()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// When overridden in a derived class, completes the install transaction.
        /// </summary>
        /// <param name="savedState">
        /// An <see cref="T:System.Collections.IDictionary"/> that contains the state of the computer after all the installers in the collection have run.
        /// </param>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="savedState"/> parameter is null.
        /// -or-
        /// The saved-state <see cref="T:System.Collections.IDictionary"/> might have been corrupted.
        /// </exception>
        /// <exception cref="T:System.Configuration.Install.InstallException">
        /// An exception occurred during the <see cref="M:System.Configuration.Install.Installer.Commit(System.Collections.IDictionary)"/> phase of the installation. This exception is ignored and the installation continues. However, the application might not function correctly after the installation is complete.
        /// </exception>
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            var process = new Process();
            process.StartInfo.FileName = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "ngen.exe");

            // get the assembly (exe) path and filename.
            string assemblyPath = this.Context.Parameters["assemblypath"];

            // add the argument to the filename as the assembly path.
            // Use quotes--important if there are spaces in the name.
            // Use the "install" verb and ngen.exe will compile all deps.
            process.StartInfo.Arguments = "install \"" + assemblyPath + "\"";

            // start ngen. it will do its magic.
            process.Start();
            process.WaitForExit();
        }

        /// <summary>
        /// When overridden in a derived class, performs the installation.
        /// </summary>
        /// <param name="stateSaver">
        /// An <see cref="T:System.Collections.IDictionary"/> used to save information needed to perform a commit, rollback, or uninstall operation.
        /// </param>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="stateSaver"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Exception">
        /// An exception occurred in the <see cref="E:System.Configuration.Install.Installer.BeforeInstall"/> event handler of one of the installers in the collection.
        /// -or-
        /// An exception occurred in the <see cref="E:System.Configuration.Install.Installer.AfterInstall"/> event handler of one of the installers in the collection.
        /// </exception>
        [SecurityPermission(SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        /// <summary>
        /// When overridden in a derived class, restores the pre-installation state of the computer.
        /// </summary>
        /// <param name="savedState">
        /// An <see cref="T:System.Collections.IDictionary"/> that contains the pre-installation state of the computer.
        /// </param>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="savedState"/> parameter is null.
        /// -or-
        /// The saved-state <see cref="T:System.Collections.IDictionary"/> might have been corrupted.
        /// </exception>
        /// <exception cref="T:System.Configuration.Install.InstallException">
        /// An exception occurred during the <see cref="M:System.Configuration.Install.Installer.Rollback(System.Collections.IDictionary)"/> phase of the installation. This exception is ignored and the rollback continues. However, the computer might not be fully reverted to its initial state after the rollback completes.
        /// </exception>
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);

            var process = new Process();
            process.StartInfo.FileName = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "ngen.exe");

            // get the assembly (exe) path and filename.
            string assemblyPath = this.Context.Parameters["assemblypath"];

            // add the argument to the filename as the assembly path.
            // Use quotes--important if there are spaces in the name.
            // Use the "install" verb and ngen.exe will compile all deps.
            process.StartInfo.Arguments = "rollback \"" + assemblyPath + "\"";

            // start ngen. it will do its magic.
            process.Start();
            process.WaitForExit();
        }

        /// <summary>
        /// When overridden in a derived class, removes an installation.
        /// </summary>
        /// <param name="savedState">
        /// An <see cref="T:System.Collections.IDictionary"/> that contains the state of the computer after the installation was complete.
        /// </param>
        /// <exception cref="T:System.ArgumentException">
        /// The saved-state <see cref="T:System.Collections.IDictionary"/> might have been corrupted.
        /// </exception>
        /// <exception cref="T:System.Configuration.Install.InstallException">
        /// An exception occurred while uninstalling. This exception is ignored and the uninstall continues. However, the application might not be fully uninstalled after the uninstallation completes.
        /// </exception>
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            var process = new Process();
            process.StartInfo.FileName = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "ngen.exe");

            // get the assembly (exe) path and filename.
            string assemblyPath = this.Context.Parameters["assemblypath"];

            // add the argument to the filename as the assembly path.
            // Use quotes--important if there are spaces in the name.
            // Use the "install" verb and ngen.exe will compile all deps.
            process.StartInfo.Arguments = "uninstall \"" + assemblyPath + "\"";

            // start ngen. it will do its magic.
            process.Start();
            process.WaitForExit();
        }

        #endregion
    }
}