using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Class to return the Instance for the selected device
    /// This is the Main Entry Point to the IXR interface
    /// </summary>
    public class ControllerFactory
    {
        #region PUBLIC_STATIC_MEMBERS
        public static bool IsOculus = false;
        #endregion // PUBLIC_STATIC_MEMBERS
        //
        #region PRIVATE_STATIC_MEMBERS
        private static ILicense m_License;
        #endregion // PRIVATE_STATIC_MEMBERS
        //
        #region PUBLIC_STATIC_METHODS
        /// <summary>
        /// Return null if license key is invalid else return the Instance for the Device
        /// </summary>
        /// <returns></returns>
        public static IXR GetIXR()
        {
            //if (!m_License.VerifyLicenseKey(TechXRConfiguration.Instance.LicenseKey))
            //{
            //    return null;
            //}

            if (!IsOculus)
            {
                return SenseXR.Instance;
            }

            return OculusBridge.Instance;
        }
        #endregion // PUBLIC_STATIC_METHODS

    }
}