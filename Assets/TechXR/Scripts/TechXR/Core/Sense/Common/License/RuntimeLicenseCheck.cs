using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vuforia;

namespace TechXR.Core.Sense
{
    internal class RuntimeLicenseCheck
    {
        public static bool IsValid;
        static ILicense license;

        //
        [RuntimeInitializeOnLoadMethod]
        public static bool CheckLicenseOnRuntime()
        {
            license = new FixedExpiryLicense();
            IsValid = license.VerifyLicenseKey(TechXRConfiguration.Instance.LicenseKey);
            if (!IsValid)
            {
                throw new System.Exception("TechXR :: Error, Enter the Valid License Key");
            }

            return true;
        }
        //
        
    }
}