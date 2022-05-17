using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TechXR.Core.Sense;

namespace TechXR.Core.Utils
{
    /// <summary>
    /// EventManager :: Singleton Class for adding/removing eventlisteners and dispatching events
    /// </summary>
    internal class EventManager : Singleton<EventManager>
    {
        #region public variables
        //Sense Event delegate
        //public delegate void SenseEventDelegate(params object[] args);
        #endregion
        //
        #region private variables
        //Game Events map
        private Dictionary<SenseEvent, SenseEventDelegate> mEventRegistry = new Dictionary<SenseEvent, SenseEventDelegate>();

        private string strEventKey;
        private SenseEventDelegate d;

        #endregion

        /// <summary>
        /// Initialization
        /// </summary>
        void Awake() { }

        /// <summary>
        /// Listener function of the instance created event
        /// </summary>
        public void OnInstanceCreated() { }

        /// <summary>
        /// Register the event with the delegate
        /// </summary>
        /// <param name="a_eEvent">The event to listen to</param>
        /// <param name="a_delListener">The callback function to call when the event is triggered</param>
        public void RegisterEvent(SenseEvent a_eEvent, SenseEventDelegate a_delListener)
        {
            if (!mEventRegistry.ContainsKey(a_eEvent))
            {
                mEventRegistry.Add(a_eEvent, a_delListener);
                return;
            }

            mEventRegistry[a_eEvent] -= a_delListener;
            mEventRegistry[a_eEvent] += a_delListener;

        }

        /// <summary>
        /// Remove the event from the event register
        /// </summary>
        /// <param name="a_eEvent">The event attached to the callback function</param>
        /// <param name="a_delListener">The callback function listening to the event</param>
        public void DeRegisterEvent(SenseEvent a_eEvent, SenseEventDelegate a_delListener)
        {
            if (!mEventRegistry.ContainsKey(a_eEvent))
                return;

            mEventRegistry[a_eEvent] -= a_delListener;
        }

        /// <summary>
        /// Event trigger function
        /// </summary>
        /// <param name="a_eEvent">The event to dispatch</param>
        /// <param name="args">The optional array of arugments to dispatch along with the event</param>
        public void TriggerEvent(SenseEvent a_eEvent, params object[] args)
        {
            strEventKey = a_eEvent.ToString();

            if (mEventRegistry.TryGetValue(a_eEvent, out d))
            {
                if (d != null)
                {
                    //trigger the delegate
                    d(args);
                }
                else
                    Debug.Log("Could not trigger event! Event: " + strEventKey);
            }
            d = null;
        }

        /// <summary>
        /// Destroy object
        /// </summary>
        void OnDestroy()
        {
            //base.OnDestroy();
            mEventRegistry.Clear();
        }
    }
}

