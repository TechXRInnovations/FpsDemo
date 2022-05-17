using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TechXR.Core.Utils;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Main entry point into the SenseXR functionalities
    /// Implements the IXR interface
    /// </summary>
    internal class SenseXR : Singleton<SenseXR>, IXR
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
        /// Get the current object under raycast
        /// </summary>
        /// <returns>GameObject</returns>
        public GameObject GetCurrentObject()
        {
            return SenseManager._instance.GetCurrentGameObject();
        }

        /// <summary>
        /// Get the controller object reference
        /// </summary>
        /// <returns>GameObject</returns>
        public GameObject GetController()
        {
            return SenseManager._instance.GetController();
        }

        /// <summary>
        /// Get global position of the controller
        /// </summary>
        /// <returns>Vector3</returns>
        public Vector3 GetXRPosition()
        {
            return SenseManager._instance.GetController().transform.position;
        }

        /// <summary>
        /// Get local position of the controller
        /// </summary>
        /// <returns>Vector3</returns>
        public Vector3 GetXRLocalPosition()
        {
            return SenseManager._instance.GetController().transform.localPosition;
        }

        /// <summary>
        /// Get global rotation of the controller
        /// </summary>
        /// <returns>Quaternion</returns>
        public Quaternion GetXRRotation()
        {
            return SenseManager._instance.GetController().transform.rotation;
        }

        /// <summary>
        /// Get local rotation of the controller
        /// </summary>
        /// <returns>Quaternion</returns>
        public Quaternion GetXRLocalRotation()
        {
            return SenseManager._instance.GetController().transform.localRotation;
        }

        /// <summary>
        /// Make the object interactive/non-interactive
        /// gameObject: Target GameObject
        /// True for interactive. False for non-interactive
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="interactive"></param>
        public void SetObjectInteractionMode(GameObject gameObject, bool interactive)
        {
            /*int layer = interactive ? LayerMask.NameToLayer(Defs.DEFAULT_LAYER) : LayerMask.NameToLayer(Defs.IGNORE_RAYCAST_LAYER);

            gameObject.layer = layer;*/

            // Above logic is not working, so need to be implemented like this for now
            if (interactive) gameObject.layer = 0;
            else gameObject.layer = 2;
        }

        /// <summary>
        /// Add event listener to a function
        /// e: Event to listen to
        /// listenerFunction: Callback function for the event
        /// </summary>
        /// <param name="e"></param>
        /// <param name="listenerFunction"></param>
        public void AddEventListener(SenseEvent e, SenseEventDelegate listenerFunction)
        {
            EventManager.Instance.RegisterEvent(e, listenerFunction);
        }

        /// <summary>
        /// Remove the event listener from a function
        /// e: Event attached to the callback function
        /// listenerFunction: Callback function listening to the event
        /// </summary>
        /// <param name="e"></param>
        /// <param name="listenerFunction"></param>
        public void RemoveEventListener(SenseEvent e, SenseEventDelegate listenerFunction)
        {
            EventManager.Instance.DeRegisterEvent(e, listenerFunction);
        }

        /// <summary>
        /// Trigger the event without any parameter
        /// e: Event to dispatch
        /// </summary>
        /// <param name="e"></param>
        public void TriggerEvent(SenseEvent e)
        {
            EventManager.Instance.TriggerEvent(e);
        }

        /// <summary>
        /// Trigger the event with some parameters
        /// Array of arguments to dispatch along with the event
        /// </summary>
        /// <param name="e">Event to dispatch</param>
        /// <param name="args"></param>
        public void TriggerEvent(SenseEvent e, params object[] args)
        {
            EventManager.Instance.TriggerEvent(e, args);
        }
        /// <summary>
        /// Show or Hide the VR Pointer
        /// True: Display
        /// False: Hide
        /// </summary>
        /// <param name="flag"></param>
        public void TogglePointerDisplay(bool flag)
        {
            SenseManager._instance.TogglePointerDisplay(flag);
        }

        /// <summary>
        /// Toggle the Controller Body Display on/off
        /// </summary>
        /// <param name="flag">True: Show. False: Hide</param>
        public void ToggleControllerBodyDisplay(bool flag)
        {
            SenseManager._instance.ToggleControllerBodyDisplay(flag);
        }

        /// <summary>
        /// Toggle display mode between pointer and teleportation
        /// PointerDisplayMode.LaserPointer for LaserPointer. 
        /// PointerDisplayMode.Teleporter for Teleporter
        /// </summary>
        /// <param name="mode"></param>
        public void SetPointerDisplayMode(Defs.PointerDisplayMode mode)
        {
            SenseManager._instance.SetPointerDisplayMode(mode);
        }

        /// <summary>
        /// Set laser pointer color
        /// </summary>
        /// <param name="color"></param>
        public void SetPointerColor(Color color)
        {
            SenseManager._instance.SetPointerColor(color);
        }

        /// <summary>
        /// Get RaycastHit Handle
        /// </summary>
        public RaycastHit GetRaycastHit()
        {
            return SenseManager._instance.GetRaycastHit();
        }
        #endregion //PUBLIC_METHODS
    }
}

