using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TechXR.Core.Sense
{
    public interface IXR
    {
        /// <summary>
        /// Get the current object hit by the raycast. Returns null if there is none.
        /// </summary>
        /// <returns></returns>
        GameObject GetCurrentObject();

        /// <summary>
        /// Get the controller gameobject on which contains the laser pointer
        /// </summary>
        /// <returns></returns>
        GameObject GetController();

        /// <summary>
        /// Get the controller global position
        /// </summary>
        /// <returns></returns>
        Vector3 GetXRPosition();

        /// <summary>
        /// Get the controller local position
        /// </summary>
        /// <returns></returns>
        Vector3 GetXRLocalPosition();

        /// <summary>
        /// Get the controller rotation
        /// </summary>
        /// <returns></returns>
        Quaternion GetXRRotation();

        /// <summary>
        /// Get the controller local rotation
        /// </summary>
        /// <returns></returns>
        Quaternion GetXRLocalRotation();

        /// <summary>
        /// Set raycast active-inactive state for the object
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="interactive"></param>
        void SetObjectInteractionMode(GameObject gameObject, bool interactive);

        /// <summary>
        /// Add an event listener to the function
        /// </summary>
        /// <param name="e"></param>
        /// <param name="listenerFunction"></param>
        void AddEventListener(SenseEvent e, SenseEventDelegate listenerFunction);

        /// <summary>
        /// Remove event listener from a function
        /// </summary>
        /// <param name="e"></param>
        /// <param name="listenerFunction"></param>
        void RemoveEventListener(SenseEvent e, SenseEventDelegate listenerFunction);
        //bool addListener_FireButton(Notify notify);
        //bool removeListener_FireButton();

        /// <summary>
        /// Trigger the event e with no arguments
        /// </summary>
        /// <param name="e"></param>
        void TriggerEvent(SenseEvent e);

        /// <summary>
        /// Trigger the event e
        /// </summary>
        /// <param name="e">event</param>
        /// <param name="args">More than 1 arguments</param>
        void TriggerEvent(SenseEvent e, params object[] args);

        /// <summary>
        /// Toggle VR Pointer display
        /// </summary>
        /// <param name="flag"></param>
        void TogglePointerDisplay(bool flag);

        /// <summary>
        /// Toggle the current active controller skin/prefab
        /// </summary>
        /// <param name="flag"></param>
        void ToggleControllerBodyDisplay(bool flag);

        /// <summary>
        /// Toggle display mode between pointer and teleportation
        /// </summary>
        /// <param name="mode"></param>
        void SetPointerDisplayMode(Defs.PointerDisplayMode mode);

        /// <summary>
        /// Set laser pointer color
        /// </summary>
        /// <param name="color"></param>
        void SetPointerColor(Color color);

        /// <summary>
        /// Get Raycast Hit handle
        /// </summary>
        RaycastHit GetRaycastHit();
    }
}
