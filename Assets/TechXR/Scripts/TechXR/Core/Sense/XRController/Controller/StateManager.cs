using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TechXR.Core.Utils
{
    /// <summary>
    /// Singleton manager class for managing application and module states
    /// </summary>
    public class StateManager : Singleton<StateManager>
    {
        #region Public_Members
        /// <summary>
        /// Grab objects and bring them close
        /// </summary>
        public static string GRAB_MODE_NEAR = "GrabNear";
        /// <summary>
        /// Grab objects from a distance
        /// </summary>
        public static string GRAB_MODE_FAR = "GrabFar";
        // Application mode
        //public string ApplicationMode { get; set; }
        // Controller grab mode
        public string GrabMode { get; set; }
        #endregion
        //
        #region Private_Members
        #endregion
        //
        #region Private_Methods
        #endregion
        //
        #region Public_Methods
        #endregion
    }
}

