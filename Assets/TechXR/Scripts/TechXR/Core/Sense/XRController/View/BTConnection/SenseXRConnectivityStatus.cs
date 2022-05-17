using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Class to Check the Connection of XR Controller
    /// </summary>
    public class SenseXRConnectivityStatus : MonoBehaviour
    {
        #region PUBLIC MEMBERS
        //public Image WarningImage;
        public GameObject XRController;
        [HideInInspector]
        public GameObject DisconnectedInfo;
        #endregion // Public Members
        //
        #region PRIVATE MEMBERS
        private bool m_IsConnected;
        //private Color m_WarningImageColor;
        private bool m_IsWarned;
        #endregion // Private Members
        //
        #region MONOBEHAVIOUR METHODS
        private void Start()
        {
            // Assign the Fields
            XRController = GameObject.FindWithTag("SenseController");
            /*GameObject warningCanvas = GameObject.Find("Warning Canvas");

            if (warningCanvas != null)
                WarningImage = warningCanvas.transform.Find("BluetoothWarningImage").GetComponent<Image>();
            else
                Debug.LogError("TechXR :: Warning Image not Found");

            m_WarningImageColor = WarningImage.color;

            // Make Image Transparent
            Color imgColor = WarningImage.color;
            imgColor.a = 0;
            WarningImage.color = imgColor;*/

            // Turn off the "Disconnected info"
            if (XRController)
            {
                Transform btCanvas = XRController.transform.Find("Bluetooth Canvas");
                if (btCanvas)
                {
                    DisconnectedInfo = btCanvas.gameObject;
                    DisconnectedInfo.SetActive(false);
                }
                else Debug.LogWarning("GameObject named Bluetooth Canvas not found..!!");
            }
            else Debug.LogWarning("XR Controller not found...!!");
        }
        //
        void Update()
        {
            // Check for the connection
            m_IsConnected = CheckSenseXRConnection();

            // If Cotroller is not connected and is not warned, then warn the user
            if (!m_IsConnected) if(!m_IsWarned) Warn();

            // If Controller is connected and user is already warned, reset the warning system
            if(m_IsConnected && m_IsWarned)
            {
                /*Color transparent = new Color(1, 1, 1, 0);
                WarningImage.color = Color.Lerp(WarningImage.color, transparent, 20 * Time.deltaTime);*/

                if (DisconnectedInfo) DisconnectedInfo.SetActive(false);

                m_IsWarned = false;
            }
        }
        #endregion // Monobehaviour Methods
        //
        #region PRIVATE METHODS
        /// <summary>
        /// Warn the user if the controller is not connected via Bluetooth
        /// </summary>
        private void Warn()
        {
            /*Color c = m_WarningImageColor;
            c.a = 0.26f;
            WarningImage.color = c;*/

            if(DisconnectedInfo) DisconnectedInfo.SetActive(true);

            m_IsWarned = true;
        }
        /// <summary>
        /// Check if the Controller is connected return true else return false
        /// </summary>
        /// <returns></returns>
        private bool CheckSenseXRConnection()
        {
            string[] inputDevices = Input.GetJoystickNames();
            
            for (int i = inputDevices.Length - 1; i >= 0 ; i--)
                if (inputDevices[i] != "") return true;

            return false;
        }
        #endregion // Private Methods
    }
}
