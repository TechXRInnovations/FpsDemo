using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Sense controller manager class
    /// </summary>
    public class SenseController : MonoBehaviour
    {
        #region PUBLIC_MEMBERS
        [HideInInspector]
        public enum XRButton { A, B, C, D, L, U }
        [HideInInspector]
        public bool Teleport;
        [HideInInspector]
        public XRButton TeleportButton;
        #endregion // PUBLIC_MEMBERS
        //
        #region PRIVATE_MEMBERS
        [SerializeField]
        private LaserPointer m_LaserPointer;
        [SerializeField]
        private GameObject[] m_PointerOptions;
        [SerializeField]
        private bool JoystickMovement = true;
        //
        private GameObject m_CurrentPointer;
        #endregion // PRIVATE_MEMBERS;
        //
        #region MONOBEHAVIOUR_METHODS
        private void Awake()
        {
            //m_CurrentPointer = m_PointerOptions.Length > 0 ? m_PointerOptions[0] : null;
        }
        // Start is called before the first frame update
        void Start()
        {
            // initialize joystick movement mode
            ToggleJoystickMovement(JoystickMovement);
            // initialize teleport mode
            ToggleTeleportMovement(Teleport);
            // set the teleport button
            SetTeleportInput(TeleportButton.ToString());
            // set current pointer reference
            foreach (GameObject item in m_PointerOptions)
            {
                if (item.activeSelf)
                {
                    m_CurrentPointer = item;
                    break;
                }
            }
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
        /// Toggle joystick movement
        /// </summary>
        /// <param name="flag">True: Enable. False: Disable</param>
        public void ToggleJoystickMovement(bool flag)
        {
            SenseInput.Instance.JoystickMovement = flag;
        }

        /// <summary>
        /// Toggle teleport movement
        /// </summary>
        /// <param name="flag">True: Enable. False: Disable</param>
        public void ToggleTeleportMovement(bool flag)
        {
            SenseManager._instance.ToggleTeleportMode(flag);
        }

        /// <summary>
        /// Map the selected input for Teleportation
        /// </summary>
        /// <param name="_buttonName"></param>
        public void SetTeleportInput(string _buttonName)
        {
            SenseManager._instance.SetTeleportInput(_buttonName);
        }

        /// <summary>
        /// Set the current pointer type:
        /// 0: Default Pointer
        /// 1: Hand pointer
        /// </summary>
        /// <param name="index">0: Default pointer. 1: Hand pointer</param>
        public void SetPointerType(int index)
        {
            // toggle the current pointer off
            if (m_CurrentPointer)
            {
                m_CurrentPointer.SetActive(false);
            }

            // If any pointer is active turn it off
            foreach (GameObject pointer in m_PointerOptions)
                if(pointer.activeSelf) pointer.SetActive(false);

            if (index < m_PointerOptions.Length)
            {
                m_CurrentPointer = m_PointerOptions[index];
                m_CurrentPointer.SetActive(true);
            }
            else
            {
                Debug.LogError("Pointer type not defined for pointer index: " + index);
            }
        }

        /// <summary>
        /// Toggle the Controller Body Display on/off
        /// </summary>
        /// <param name="flag">True: Show. False: Hide</param>
        public void ToggleControllerBodyDisplay(bool flag)
        {
            if (m_CurrentPointer) m_CurrentPointer.SetActive(flag);
        }

        /// <summary>
        /// Toggle the pointer display on/off
        /// </summary>
        /// <param name="flag">True: Show. False: Hide</param>
        public void ToggleDisplay(bool flag)
        {
            /*
            if (m_CurrentPointer == null)
            {
                // set current pointer reference
                foreach (GameObject item in m_PointerOptions)
                {
                    if (item.activeSelf)
                    {
                        m_CurrentPointer = item;
                        break;
                    }
                }
            }

            // Set controller vody and laser pointer to flag
            m_CurrentPointer.SetActive(flag); // Gives error if new pointer is not assigned
            */

            m_LaserPointer.ToggleDisplay(flag);
        }
        #endregion //PUBLIC_METHODS
    }
}

