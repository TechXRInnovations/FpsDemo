using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TechXR.Core.Utils;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Singleton manager class for managing the XR objects and functionalities
    /// </summary>
    internal class SenseManager : MonoBehaviour
    {
        /// <summary>
        /// Static class reference
        /// </summary>
        public static SenseManager _instance;
        //
        #region PUBLIC_MEMBERS
        /// <summary>
        /// Current pointer display mode
        /// - LaserPointer
        /// - Teleporter
        /// </summary>
        public Defs.PointerDisplayMode CurrentPointerMode { get; set; }
        #endregion // PUBLIC_MEMBERS
        //
        #region PRIVATE_MEMBERS
        [SerializeField]
        private SenseController m_SenseController;
        [SerializeField]
        private LaserPointer m_LaserPointer;
        [SerializeField]
        private LaserPointerInputModule m_LaserPointerInput;
        [SerializeField]
        private StandaloneInputModule m_StandaloneInput;
        [SerializeField]
        private TeleportManager m_TeleportManager;
        [SerializeField]
        private GameObject Joystick;
        // Input types
        private enum InputType
        {
            RayCast,
            Joystick,
            MouseKeyboard
        }
        [SerializeField]
        private InputType m_InputType;
        //
        private Dictionary<InputType, Action> m_InputMap = new Dictionary<InputType, Action>();
        #endregion // PRIVATE_MEMBERS;
        //
        #region MONOBEHAVIOUR_METHODS
        // Awake
        private void Awake()
        {
            m_InputMap.Add(InputType.Joystick, SetJoystickMode);
            m_InputMap.Add(InputType.RayCast, SetRaycastMode);
            m_InputMap.Add(InputType.MouseKeyboard, SetMouseKeyboardMode);
            //
            if (_instance != null)
            {
                Destroy(_instance);
            }
            _instance = this;
            //
            CurrentPointerMode = Defs.PointerDisplayMode.LaserPointer;
            // populate class references
            PopulateClassObjects();
        }
        // Start is called before the first frame update
        void Start()
        {
            // set the input type
            m_InputMap[m_InputType]();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        #endregion // MONOBEHAVIOUR_METHODS
        //
        #region PRIVATE_METHODS
        /// <summary>
        /// Set controls to Raycast mode
        /// </summary>
        private void SetRaycastMode()
        {
            // switch the event system input module
            m_LaserPointerInput.enabled = true;
            m_StandaloneInput.enabled = false;
            // toggle the laser pointer display off
            if(m_SenseController) m_SenseController.ToggleDisplay(true);
            // toggle the teleport display on
            if(m_TeleportManager) m_TeleportManager.ToggleDisplay(true);
            // display the joystick canvas
            Joystick.SetActive(false);
        }

        /// <summary>
        /// Set controls to Joystick mode
        /// </summary>
        private void SetJoystickMode()
        {
            // switch the event system input module
            m_LaserPointerInput.enabled = false;
            m_StandaloneInput.enabled = true;
            // toggle the laser pointer display off
            m_SenseController.ToggleDisplay(false);
            // toggle the teleport display on
            if (m_TeleportManager) m_TeleportManager.ToggleDisplay(false);
            // display the joystick canvas
            Joystick.SetActive(true);
        }

        /// <summary>
        /// Set controls to mouse-keyboard mode
        /// </summary>
        private void SetMouseKeyboardMode()
        {
            // switch the event system input module
            m_LaserPointerInput.enabled = true;
            m_StandaloneInput.enabled = false;
            // toggle the laser pointer display off
            m_SenseController.ToggleDisplay(true);
            // toggle the teleport display on
            if (m_TeleportManager) m_TeleportManager.ToggleDisplay(true);
            // display the joystick canvas
            Joystick.SetActive(false);
        }
        #endregion // PRIVATE_METHODS
        //
        #region PUBLIC_METHODS
        /// <summary>
        /// Toggle VR Pointer display
        /// </summary>
        /// <param name="flag">True: Show. False: Hide</param>
        public void TogglePointerDisplay(bool flag)
        {
            m_SenseController.ToggleDisplay(flag);
        }

        /// <summary>
        /// Toggle the Controller Body Display on/off
        /// </summary>
        /// <param name="flag">True: Show. False: Hide</param>
        public void ToggleControllerBodyDisplay(bool flag)
        {
            m_SenseController.ToggleControllerBodyDisplay(flag);
        }

        /// <summary>
        /// Set interaction mode:
        /// - LaserPointer
        /// - Teleporter
        /// </summary>
        /// <param name="mode">Defs.PointerDisplayMode.X</param>
        public void SetPointerDisplayMode(Defs.PointerDisplayMode mode)
        {
            m_LaserPointer.SetPointerDisplayMode(mode);
        }

        /// <summary>
        /// Set laser pointer color
        /// </summary>
        /// <param name="color"></param>
        public void SetPointerColor(Color color)
        {
            m_LaserPointer.SetColor(color);
        }

        /// <summary>
        /// Get the current gameobject returned by the laser pointer
        /// </summary>
        /// <returns></returns>
        public GameObject GetCurrentGameObject()
        {
            return m_LaserPointer.GetCurrentGameObject();
        }

        /// <summary>
        /// Get the RaycastHit returned by the laser pointer
        /// </summary>
        /// <returns></returns>
        public RaycastHit GetRaycastHit()
        {
            return m_LaserPointer.GetRaycastHit();
        }

        /// <summary>
        /// Get the controller object attached to the laser pointer
        /// </summary>
        /// <returns></returns>
        public GameObject GetController()
        {
            return m_LaserPointer.gameObject;
        }

        /// <summary>
        /// Toggle teleport mode
        /// </summary>
        /// <param name="flag"></param>
        public void ToggleTeleportMode(bool flag)
        {
            if (m_TeleportManager) m_TeleportManager.enabled = flag;
        }

        /// <summary>
        /// Set Teleport Input button
        /// </summary>
        /// <param name="_buttonName">A-F</param>
        public void SetTeleportInput(string _buttonName)
        {
            if (m_TeleportManager) m_TeleportManager.SetTeleportInput(_buttonName);
        }
        /// <summary>
        /// Called from the editor. Auto-populates the class objects
        /// </summary>
        public void PopulateClassObjects()
        {
            //print("SenseXRManager :: Populating objects");
            // get object references
            GameObject SenseXR = GameObject.Find("SenseController");
            GameObject SenseEventSystem = GameObject.Find("SenseEventSystem");
            GameObject TeleportManager = GameObject.Find("TeleportManager");
            GameObject XRPlayer = GameObject.Find("XRPlayerController");
            JoystickController joystickController = Joystick.GetComponent<JoystickController>();

            // populate laser pointer
            if (SenseXR)
            {
                //m_LaserPointer = SenseXR.GetComponent<LaserPointer>();
                m_LaserPointer = SenseXR.GetComponentInChildren<LaserPointer>();
                m_SenseController = SenseXR.GetComponent<SenseController>();
            }
            // populate event systems
            if (SenseEventSystem)
            {
                m_LaserPointerInput = SenseEventSystem.GetComponent<LaserPointerInputModule>();
                m_StandaloneInput = SenseEventSystem.GetComponent<StandaloneInputModule>();
            }
            // populate teleport manager
            if (TeleportManager)
            {
                m_TeleportManager = TeleportManager.GetComponent<TeleportManager>();
            }
            // populate player reference
            if (XRPlayer && joystickController)
            {
                joystickController.SetPlayerReference(XRPlayer);
            }
        }
        #endregion //PUBLIC_METHODS
    }
}

