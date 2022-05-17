using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Class for checking the License Validation
    /// </summary>
    public class LicenseValidity : MonoBehaviour
    {
        #region PUBLIC_FIELDS
        public string URL;
        #endregion // PUBLIC_FIELDS
        //
        #region PRIVATE_FIELDS
        private bool m_Valid;
        #endregion // PRIVATE_FIELDS
        //
        #region PRIVATE_METHODS
        /// <summary>
        /// Post Data to the Given URL
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private IEnumerator Post(string url, string key)
        {
            var request = new UnityEngine.Networking.UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(key);

            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            // For Json
            //request.SetRequestHeader("Content-Type", "application/json");
            // For Plain text
            request.SetRequestHeader("Content-Type", "text/plain");

            yield return request.SendWebRequest();

            Debug.Log("Status Code: " + request.responseCode); // 200 : Success
            Debug.Log(request.downloadHandler.text); // Response Text from Server-Side

            if (request.downloadHandler.text == "true") m_Valid = true;

            else if (request.downloadHandler.text == "false")
            {
                m_Valid = false;

                // If license key is not valid delete our Vuforia-Database

                /*
                File.Delete("Assets/Resources/qwerty.asset"); // For Deleting the File
                FileUtil.DeleteFileOrDirectory("Assets/StreamingAssets"); // For Del the folder
                */
            }
        }
        #endregion // PRIVATE_METHODS
        //
        #region PUBLIC_METHODS
        /// <summary>
        /// Check the App License Validity
        /// </summary>
        public void CheckLicenseValidity()
        {
            if (!string.IsNullOrEmpty(TechXRConfiguration.Instance.LicenseKey))
            { 
                StartCoroutine(Post(URL, TechXRConfiguration.Instance.LicenseKey)); 
            }

            else Debug.LogError("TechXR :: Please Fill up the License Key");
        }
        #endregion // PUBLIC_METHODS
    }
}
