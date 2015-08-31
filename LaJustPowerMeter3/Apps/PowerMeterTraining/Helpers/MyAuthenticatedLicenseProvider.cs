// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyAuthenticatedLicenseProvider.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// --------------------------------------------------------------------------------------------------------------------

namespace Shell
{
    using System;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Security.Permissions;

    using Infralution.Licensing;

    /// <summary>
    /// Custom Authenticated License Provider that generates a unique Computer Id
    /// </summary>
    internal class MyAuthenticatedLicenseProvider : AuthenticatedLicenseProvider
    {
        #region Public Methods

        /// <summary>
        /// Gets the computer ID.
        /// </summary>
        /// <returns>
        /// A string identifying the computer
        /// </returns>
        /// <remarks>
        /// This id is included in the authenticated license and checked when the license
        /// is validated.   This prevents a license being authenticated on one machine and
        /// then copied to another.
        /// </remarks>
        public override string GetComputerID()
        {
            var computerId = this.GenerateKey("id.dat");
            return LicenseUtilities.ToBase32(computerId.ToByteArray());
        }

        /// <summary>
        /// Gets the license checking for blacklisted serial numbers
        /// </summary>
        /// <param name="licenseFile">
        /// The license file.
        /// </param>
        /// <param name="validateLicense">
        /// if set to <c>true</c> [validate license].
        /// </param>
        /// <returns>
        /// Authenticated License
        /// </returns>
        public override AuthenticatedLicense GetLicense(string licenseFile, bool validateLicense)
        {
            var license = base.GetLicense(licenseFile, validateLicense);

            // Check for blacklisted serial numbers
            ////if (license != null) switch (license.SerialNo)
            ////{
            ////    case 1:
            ////        ShowError("{0} License Error", "Please contact " + Solution.Company + " to update your license.");
            ////        return null;
            ////}
            return license;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generates the key.
        /// </summary>
        /// <param name="keyFileName">
        /// Name of the key file.
        /// </param>
        /// <returns>
        /// The unique key for this computer
        /// </returns>
        [IsolatedStorageFilePermission(SecurityAction.Demand)]
        private Guid GenerateKey(string keyFileName)
        {
            Guid key;
            using (var isoStore = IsolatedStorageFile.GetMachineStoreForAssembly())
            {
                if (isoStore.GetFileNames(keyFileName).Length > 0)
                {
                    using (var isoStream = new IsolatedStorageFileStream(keyFileName, FileMode.Open, isoStore))
                    using (var reader = new StreamReader(isoStream))
                    {
                        key = new Guid(reader.ReadToEnd());
                    }
                }
                else
                {
                    key = Guid.NewGuid();
                    using (var isoStream = new IsolatedStorageFileStream(keyFileName, FileMode.OpenOrCreate, isoStore))
                    using (var writer = new StreamWriter(isoStream))
                    {
                        writer.WriteLine(key.ToString());
                    }
                }
            }

            return key;
        }

        #endregion
    }
}