namespace LaJust.PowerMeter.UI.Helpers
{
    using System;
    using System.IO;
    using System.IO.IsolatedStorage;
    using Infralution.Licensing;
    using System.ComponentModel;
    using LaJust.PowerMeter.Common;

    internal class MyAuthenticatedLicenseInstallForm : AuthenticatedLicenseInstallForm
    {
        /// <summary>
        /// Gets the license provider.
        /// </summary>
        /// <returns></returns>
        protected override AuthenticatedLicenseProvider GetLicenseProvider()
        {
            return new MyAuthenticatedLicenseProvider();
        }
    }

    internal class MyAuthenticatedLicenseProvider : AuthenticatedLicenseProvider
    {
        /// <summary>
        /// Gets the license checking for blacklisted serial numbers
        /// </summary>
        /// <param name="licenseFile">The license file.</param>
        /// <param name="validateLicense">if set to <c>true</c> [validate license].</param>
        /// <returns></returns>
        public override AuthenticatedLicense GetLicense(string licenseFile, bool validateLicense)
        {
            AuthenticatedLicense license = base.GetLicense(licenseFile, validateLicense);

            // Check for blacklisted serial numbers
            //if (license != null) switch (license.SerialNo)
            //{
            //    case 1:
            //        ShowError("{0} License Error", "Please contact " + Solution.Company + " to update your license.");
            //        return null;
            //}

            return license;
        }

        /// <summary>
        /// Gets the computer ID.
        /// </summary>
        public override string GetComputerID()
        {
            Guid computerId = GenerateKey("id.dat");
            return LicenseUtilities.ToBase32(computerId.ToByteArray());
        }

        /// <summary>
        /// Generates the key.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <returns></returns>
        private Guid GenerateKey(string keyFileName)
        {
            Guid key;
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetMachineStoreForAssembly())
            {
                if (isoStore.GetFileNames(keyFileName).Length > 0)
                {
                    using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(keyFileName, FileMode.Open, isoStore))
                    using (StreamReader reader = new StreamReader(isoStream)) key = new Guid(reader.ReadToEnd());
                }
                else
                {
                    key = Guid.NewGuid();
                    using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(keyFileName, FileMode.OpenOrCreate, isoStore))
                    using (StreamWriter writer = new StreamWriter(isoStream)) writer.WriteLine(key.ToString());
                }
            }
            return key;
        }
    }
}
