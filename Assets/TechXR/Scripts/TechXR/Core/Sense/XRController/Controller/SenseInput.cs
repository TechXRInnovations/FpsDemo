using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TechXR.Core.Utils;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Main input manager class
    /// </summary>
    public class SenseInput : MonoBehaviour
    {
        // static reference
        public static SenseInput Instance;
        // Joystick button states
        // up
        public static bool ButtonA_Up;
        public static bool ButtonB_Up;
        public static bool ButtonC_Up;
        public static bool ButtonD_Up;
        public static bool ButtonE_Up;
        public static bool ButtonF_Up;
        // down
        public static bool ButtonA_Down;
        public static bool ButtonB_Down;
        public static bool ButtonC_Down;
        public static bool ButtonD_Down;
        public static bool ButtonE_Down;
        public static bool ButtonF_Down;
        // pressed
        public static bool ButtonA_Press;
        public static bool ButtonB_Press;
        public static bool ButtonC_Press;
        public static bool ButtonD_Press;
        public static bool ButtonE_Press;
        public static bool ButtonF_Press;
        // movement inputs
        public static float HorizontalInput;
        public static float VerticalInput;

        //public static bool 
        #region PUBLIC_MEMBERS
        /// <summary>
        /// Enable/Disable the joystick basedd movements
        /// </summary>
        public bool JoystickMovement { get; set; }
        /// <summary>
        /// Enable/Disable the teleport option
        /// </summary>
        public bool TeleportMovement { get; set; }
        #endregion // PUBLIC_MEMBERS
        //
        #region PRIVATE_MEMBERS
        private string[] m_ButtonName = { "A", "B", "C", "D", "E", "F" };
        private bool[] m_ButtonInput = { ButtonA_Down, ButtonB_Down, ButtonC_Down, ButtonD_Down, ButtonE_Down, ButtonF_Down, ButtonA_Press, ButtonB_Press, ButtonC_Press, ButtonD_Press, ButtonE_Press, ButtonF_Press, ButtonA_Up, ButtonB_Up, ButtonC_Up, ButtonD_Up, ButtonE_Up, ButtonF_Up };
        private List<Dictionary<string, bool>> m_InputMap = new List<Dictionary<string, bool>>();
        #endregion // PRIVATE_MEMBERS;
        //
        #region MONOBEHAVIOUR_METHODS
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance);
            }
            Instance = this;
            //
            JoystickMovement = true;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // Change the mapping to suit the requiremet

            // Joystick Key Down --------------------------------------------->

            //if (Input.GetKeyDown(KeyCode.Joystick1Button0))
            if (Input.GetKeyDown(KeyCode.C))
            {
                ResetButtonState();
                ButtonC_Down = true;
                //EventManager.Instance.TriggerEvent(SenseEvent.BUTTON_C_DOWN);
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                ResetButtonState();
                ButtonA_Down = true;
                //EventManager.Instance.TriggerEvent(SenseEvent.BUTTON_A_DOWN);
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button2))
            //else if (Input.GetKeyDown(KeyCode.B))
            {
                ResetButtonState();
                ButtonB_Down = true;
                //EventManager.Instance.TriggerEvent(SenseEvent.BUTTON_B_DOWN);
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button3))
            {
                ResetButtonState();
                ButtonD_Down = true;
                //EventManager.Instance.TriggerEvent(SenseEvent.BUTTON_D_DOWN);
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button4))
            {
                ResetButtonState();
                ButtonE_Down = true;
                //EventManager.Instance.TriggerEvent(SenseEvent.BUTTON_E_DOWN);
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button5))
            {
                ResetButtonState();
                ButtonF_Down = true;
                //EventManager.Instance.TriggerEvent(SenseEvent.BUTTON_F_DOWN);
            }

            // Joystick Key Up ------------------------------------------------>

            //if (Input.GetKeyUp(KeyCode.Joystick1Button0))
            if (Input.GetKeyUp(KeyCode.C))
            {
                ResetButtonState();
                ButtonC_Up = true;
                //EventManager.Instance.TriggerEvent(SenseEvent.BUTTON_C_UP);
            }
            else if (Input.GetKeyUp(KeyCode.Joystick1Button1))
            {
                ResetButtonState();
                ButtonA_Up = true;
                //EventManager.Instance.TriggerEvent(SenseEvent.BUTTON_A_UP);
            }
            else if (Input.GetKeyUp(KeyCode.Joystick1Button2))
            //else if (Input.GetKeyUp(KeyCode.B))
            {
                ResetButtonState();
                ButtonB_Up = true;
                //EventManager.Instance.TriggerEvent(SenseEvent.BUTTON_B_UP);
            }
            else if (Input.GetKeyUp(KeyCode.Joystick1Button3))
            {
                ResetButtonState();
                ButtonD_Up = true;
                //EventManager.Instance.TriggerEvent(SenseEvent.BUTTON_D_UP);
            }
            else if (Input.GetKeyUp(KeyCode.Joystick1Button4))
            {
                ResetButtonState();
                ButtonE_Up = true;
                //EventManager.Instance.TriggerEvent(SenseEvent.BUTTON_E_UP);
            }
            else if (Input.GetKeyUp(KeyCode.Joystick1Button5))
            {
                ResetButtonState();
                ButtonF_Up = true;
                //EventManager.Instance.TriggerEvent(SenseEvent.BUTTON_F_UP);
            }

            // Joystick Key Held Down --------------------------------------------------->

            if (Input.GetKey(KeyCode.Joystick1Button0))
            {
                ResetButtonState();
                ButtonC_Press = true;
                //EventManager.Instance.TriggerEvent(SenseEvent.BUTTON_C_PRESS_CONTINUE);
            }
            else if (Input.GetKey(KeyCode.Joystick1Button1))
            {
                ResetButtonState();
                ButtonA_Press = true;
                //EventManager.Instance.TriggerEvent(SenseEvent.BUTTON_A_PRESS_CONTINUE);
            }
            else if (Input.GetKey(KeyCode.Joystick1Button2))
            {
                ResetButtonState();
                ButtonB_Press = true;
                //EventManager.Instance.TriggerEvent(SenseEvent.BUTTON_B_PRESS_CONTINUE);
            }
            else if (Input.GetKey(KeyCode.Joystick1Button3))
            {
                ResetButtonState();
                ButtonD_Press = true;
                //EventManager.Instance.TriggerEvent(SenseEvent.BUTTON_D_PRESS_CONTINUE);
            }
            else if (Input.GetKey(KeyCode.Joystick1Button4))
            {
                ResetButtonState();
                ButtonE_Press = true;
                //EventManager.Instance.TriggerEvent(SenseEvent.BUTTON_E_PRESS_CONTINUE);
            }
            else if (Input.GetKey(KeyCode.Joystick1Button5))
            {
                ResetButtonState();
                ButtonF_Press = true;
                //EventManager.Instance.TriggerEvent(SenseEvent.BUTTON_F_PRESS_CONTINUE);
            }

            // joystick movement inputs
            HorizontalInput = JoystickMovement ? Input.GetAxis("Horizontal") : 0;
            VerticalInput = JoystickMovement ? Input.GetAxis("Vertical") : 0;
        }
        #endregion // MONOBEHAVIOUR_METHODS
        //
        #region PRIVATE_METHODS
        /// <summary>
        /// Reset all the button state variables
        /// </summary>
        private void ResetButtonState()
        {
            // set all the button states to false
            ButtonA_Down = ButtonA_Press = ButtonA_Press
                = ButtonB_Down = ButtonB_Press = ButtonB_Up
                = ButtonC_Down = ButtonC_Press = ButtonC_Up
                = ButtonD_Down = ButtonD_Press = ButtonD_Up
                = ButtonE_Down = ButtonE_Press = ButtonE_Up
                = ButtonF_Down = ButtonF_Press = ButtonF_Up
                = false;
        }
        #endregion // PRIVATE_METHODS
        //
        #region PUBLIC_METHODS
        public static bool GetButtonDown(string button)
        {
            switch (button.ToLower())
            {
                case "a":
                    return Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.Alpha1);
                case "b":
                    return Input.GetKeyDown(KeyCode.JoystickButton2) || Input.GetKeyDown(KeyCode.Alpha2);
                case "c":
                    return Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.Alpha3);
                case "d":
                    return Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.Alpha4);
                case "l":
                    return Input.GetKeyDown(KeyCode.JoystickButton4) || Input.GetKeyDown(KeyCode.L);
                case "u":
                    return Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetKeyDown(KeyCode.U);

                default: return false;
            }
        }

        public static bool GetButton(string button)
        {
            switch (button.ToLower())
            {
                case "a":
                    return Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.Alpha1);
                case "b":
                    return Input.GetKey(KeyCode.JoystickButton2) || Input.GetKey(KeyCode.Alpha2);
                case "c":
                    return Input.GetKey(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.Alpha3);
                case "d":
                    return Input.GetKey(KeyCode.JoystickButton3) || Input.GetKey(KeyCode.Alpha4);
                case "l":
                    return Input.GetKey(KeyCode.JoystickButton4) || Input.GetKey(KeyCode.L);
                case "u":
                    return Input.GetKey(KeyCode.JoystickButton5) || Input.GetKey(KeyCode.U);

                default: return false;
            }
        }

        public static bool GetButtonUp(string button)
        {
            switch (button.ToLower())
            {
                case "a":
                    return Input.GetKeyUp(KeyCode.JoystickButton1) || Input.GetKeyUp(KeyCode.Alpha1);
                case "b":
                    return Input.GetKeyUp(KeyCode.JoystickButton2) || Input.GetKeyUp(KeyCode.Alpha2);
                case "c":
                    return Input.GetKeyUp(KeyCode.JoystickButton0) || Input.GetKeyUp(KeyCode.Alpha3);
                case "d":
                    return Input.GetKeyUp(KeyCode.JoystickButton3) || Input.GetKeyUp(KeyCode.Alpha4);
                case "l":
                    return Input.GetKeyUp(KeyCode.JoystickButton4) || Input.GetKeyUp(KeyCode.L);
                case "u":
                    return Input.GetKeyUp(KeyCode.JoystickButton5) || Input.GetKeyUp(KeyCode.U);

                default: return false;
            }
        }
        #endregion //PUBLIC_METHODS
    }
}

