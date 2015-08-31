// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyAuthenticatedLicenseInstallForm.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// --------------------------------------------------------------------------------------------------------------------

namespace Shell
{
    using Infralution.Licensing;

    /// <summary>
    /// Custom Authenticated License Install Form that returns our custom license provider
    /// </summary>
    internal class MyAuthenticatedLicenseInstallForm : AuthenticatedLicenseInstallForm
    {
        #region Methods

        /// <summary>
        /// Gets the license provider.
        /// </summary>
        /// <returns>
        /// The license provider
        /// </returns>
        protected override AuthenticatedLicenseProvider GetLicenseProvider()
        {
            return new MyAuthenticatedLicenseProvider();
        }

        #endregion
    }
}