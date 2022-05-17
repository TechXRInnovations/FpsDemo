using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Class to trace the Tracking of XR Controller
    /// </summary>
    public class SenseXRTrackingStatus : DefaultTrackableEventHandler
    {
        #region PUBLIC MEMBERS
        #endregion // Public Members
        //
        #region PRIVATE MEMBERS
        private bool m_IsTrackingLost;
        #endregion // Private Members
        //
        #region MONOBEHAVIOUR METHODS
        protected override void Start()
        {
            base.Start();
        }
        //
        void Update()
        {
            if (m_IsTrackingLost)
            {
                // Out of Camera Field Range


                // If tracking is lost lerp the controller to the assigned position
                this.gameObject.transform.localPosition = Vector3.Lerp(this.gameObject.transform.localPosition, new Vector3(0.112f, -0.044f, 0.221f), Time.deltaTime * 5f);
            }
        }
        #endregion // MONOBEHAVIOUR METHODS
        //
        #region PROTECTED METHODS
        protected override void OnTrackingFound()
        {
            Debug.Log("Unity AR - Tracking image is found.");
            m_IsTrackingLost = false;
        }
        protected override void OnTrackingLost()
        {
            Debug.Log("Unity AR - Tracking image is lost.");
            m_IsTrackingLost = true;

            //Quaternion q = Camera.main.transform.rotation;
            Quaternion q = this.transform.parent.rotation; // Rotation of MainCamera
            this.gameObject.transform.rotation = q;

            // Set offset of y-axis to 90 degree
            Vector3 rot = this.gameObject.transform.eulerAngles;
            rot.y = 90f;
            this.gameObject.transform.eulerAngles = rot;
            //
        }
        #endregion // PROTECTED METHODS
        //
        #region PRIVATE METHODS
        #endregion // Private Methods
    }
}
