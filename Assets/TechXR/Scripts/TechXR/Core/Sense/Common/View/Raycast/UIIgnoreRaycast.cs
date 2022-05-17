using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    /// <summary>
    /// Ignore Raycast layer
    /// </summary>
    [AddComponentMenu("UI/Raycast Filters/Ignore Raycast Filter")]
    internal class UIIgnoreRaycast : MonoBehaviour, ICanvasRaycastFilter
    {
        #region PUBLIC_MEMBERS
        #endregion // PUBLIC_MEMBERS
        //
        #region PRIVATE_MEMBERS
        #endregion // PRIVATE_MEMBERS;
        //
        #region MONOBEHAVIOUR_METHODS
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        #endregion // MONOBEHAVIOUR_METHODS
        //
        #region PRIVATE_METHODS
        #endregion // PRIVATE_METHODS
        //
        #region PUBLIC_METHODS
        /// <summary>
        /// Check for valid raycast location
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="eventCamera"></param>
        /// <returns></returns>
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return false;
        }
        #endregion //PUBLIC_METHODS
    }
}

