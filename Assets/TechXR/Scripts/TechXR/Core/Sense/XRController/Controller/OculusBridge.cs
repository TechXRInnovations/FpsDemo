using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TechXR.Core.Utils;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Main entry point into the Oculus functionalities
    /// Implements the IXR interface
    /// </summary>
    internal class OculusBridge : Singleton<OculusBridge>, IXR
    {
        public void AddEventListener(SenseEvent e, SenseEventDelegate listenerFunction)
        {
            throw new System.NotImplementedException();
        }

        public GameObject GetController()
        {
            throw new System.NotImplementedException();
        }

        public GameObject GetCurrentObject()
        {
            throw new System.NotImplementedException();
        }

        public RaycastHit GetRaycastHit()
        {
            throw new System.NotImplementedException();
        }

        public Vector3 GetXRLocalPosition()
        {
            throw new System.NotImplementedException();
        }

        public Quaternion GetXRLocalRotation()
        {
            throw new System.NotImplementedException();
        }

        public Vector3 GetXRPosition()
        {
            throw new System.NotImplementedException();
        }

        public Quaternion GetXRRotation()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveEventListener(SenseEvent e, SenseEventDelegate listenerFunction)
        {
            throw new System.NotImplementedException();
        }

        public void SetObjectInteractionMode(GameObject gameObject, bool interactive)
        {
            throw new System.NotImplementedException();
        }

        public void SetPointerColor(Color color)
        {
            throw new System.NotImplementedException();
        }

        public void SetPointerDisplayMode(Defs.PointerDisplayMode mode)
        {
            throw new System.NotImplementedException();
        }

        public void ToggleControllerBodyDisplay(bool flag)
        {
            throw new System.NotImplementedException();
        }

        public void TogglePointerDisplay(bool flag)
        {
            throw new System.NotImplementedException();
        }

        public void TriggerEvent(SenseEvent e)
        {
            throw new System.NotImplementedException();
        }

        public void TriggerEvent(SenseEvent e, params object[] args)
        {
            throw new System.NotImplementedException();
        }
    }
}
