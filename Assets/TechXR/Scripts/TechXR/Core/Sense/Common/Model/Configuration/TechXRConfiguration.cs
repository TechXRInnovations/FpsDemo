using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TechXR.Core.Utils;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Global settings for TechXR which are used for all scenes
    /// </summary>
    public class TechXRConfiguration : SingletonScriptableObject<TechXRConfiguration>
    {
        #region PUBLIC_FIELDS
        public string LicenseKey { get { return m_AppLicenseKey; } set { m_AppLicenseKey = value; } }
        #endregion // PUBLIC_FIELDS
        //
        #region PRIVATE_FIELDS
        [SerializeField] 
        [TextArea(5,10)]
        private string m_AppLicenseKey;
        #endregion // PRIVATE_FIELDS
    }
}
