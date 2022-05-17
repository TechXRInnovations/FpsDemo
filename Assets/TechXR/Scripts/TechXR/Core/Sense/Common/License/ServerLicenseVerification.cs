using System.Collections.Generic;
using co.techxr.unity.network;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Class to check if the license is valid
    /// </summary>
    public class ServerLicenseVerification : ILicense
    {
        #region CONST
        const string LICENSE_CHECK = "api/developer/license/validate";
        #endregion // CONST
        //
        #region PRIVATE_MEMBERS
        private NetworkService m_NetworkService = new NetworkService();
        #endregion // PRIVATE_MEMBERS
        //
        #region PUBLIC_METHODS
        public bool VerifyLicenseKey(string licenseKey)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["license"] = licenseKey;

            LicenseVerificationDetails lvd = m_NetworkService.get<LicenseVerificationDetails>(LICENSE_CHECK, parameters);

            return lvd.status;
        }
        #endregion // PUBLIC_METHODS
    }
}