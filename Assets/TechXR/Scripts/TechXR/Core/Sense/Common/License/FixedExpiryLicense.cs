using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vuforia;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Class to check if the license is valid for a fixed time period
    /// </summary>
    public class FixedExpiryLicense : ILicense
    {
        #region PRIVATE_MEMBERS
        private System.DateTime m_StartDate;
        private System.DateTime m_Today;
        private bool m_IsValid;
        //
        private List<string> m_DevKeys = new List<string>()
        {
            "EIzzDxO7wU","1y2hm9V9DT","MRNBJ81gWr", "QBpl2VsV6w", "4249iSxxn5", "hnRZ05nUb0", "5b4qb5mi5m", "I9ocufbX1p",
            "4pATMfpd8p", "CtLdrx20CS"
        };
        #endregion // PRIVATE_MEMBERS
        //
        #region PUBLIC_METHODS
        public bool VerifyLicenseKey(string licenseKey)
        {
            //VuforiaARController.Instance.RegisterVuforiaStartedCallback(DeactivateDataSet);
            return CheckKey(licenseKey) && CheckTimePeriod();
        }
        #endregion // PUBLIC_METHODS
        //
        #region PRIVATE_METHODS
        private bool CheckKey(string licenseKey)
        {
            foreach (string key in m_DevKeys)
                if (key.Equals(licenseKey))
                    return true;

            return false;
        }
        //
        private void DeactivateDataSet()
        {
            TrackerManager trackerManager = (TrackerManager)TrackerManager.Instance;
            ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();

            IEnumerable<DataSet> datasets = objectTracker.GetDataSets();
            IEnumerable<DataSet> activeDataSets = objectTracker.GetActiveDataSets();
            List<DataSet> activeDataSetsToBeRemoved = activeDataSets.ToList();

            //Loop through all the active datasets and deactivate them.
            foreach (DataSet ads in activeDataSetsToBeRemoved)
            {
                objectTracker.DeactivateDataSet(ads);
            }
        }
        //
        private bool CheckTimePeriod()
        {
            SetStartDate();

            double daysPassed = GetDaysPassed();
            double daysRemaining = GetRemainingDays();

            if (daysPassed >= 180.0 || daysRemaining <= 0.0)
            {
                return false;
            }

            return true;
        }
        //
        private void SetStartDate()
        {
            if (PlayerPrefs.HasKey("DateInitialized")) //if we have the start date saved, we'll use that
                m_StartDate = System.Convert.ToDateTime(PlayerPrefs.GetString("DateInitialized"));
            else //otherwise...
            {
                m_StartDate = System.DateTime.Now; //save the start date ->
                PlayerPrefs.SetString("DateInitialized", m_StartDate.ToString());
            }
        }
        //
        private double GetDaysPassed()
        {
            m_Today = System.DateTime.Now;

            //days between today and start date -->
            System.TimeSpan elapsed = m_Today.Subtract(m_StartDate);

            double days = elapsed.TotalDays;

            //Debug.Log("No. of Days Passed ----> " + days);

            return days;
        }
        //
        private static double GetRemainingDays()
        {
            var dt = System.DateTime.Now;

            var targetDate = new System.DateTime(2022, 07, 31, 0, 0, 0);

            //Debug.Log("Now -> " + System.DateTime.Now + " Target ->" + targetDate + "Total :=> " + System.Math.Abs((targetDate - dt).TotalDays));

            return System.Math.Abs((targetDate - dt).TotalDays);
        }
        //
        #endregion // PRIVATE_METHODS
    }
}