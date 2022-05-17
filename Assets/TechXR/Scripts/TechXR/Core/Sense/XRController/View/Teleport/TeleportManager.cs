using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Manager class for the teleport functionality
    /// </summary>
    internal class TeleportManager : MonoBehaviour
    {
        #region PUBLIC_MEMBERS
        /// <summary>
        /// Teleport mode on/off
        /// </summary>
        public bool TeleportMode { get; set; }
        #endregion // PUBLIC_MEMBERS
        //
        #region PRIVATE_MEMBERS
        [SerializeField]
        private Teleporter VRTeleporter;
        //[SerializeField]
        //private LaserPointer LaserPointerObj;
        private string m_TelportButton;
        private bool m_DisplayFlag = new bool();
        private CharacterController m_CharacterController;
        #endregion // PRIVATE_MEMBERS;
        //
        #region MONOBEHAVIOUR_METHODS
        private void Awake()
        {
            TeleportMode = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            m_CharacterController = this.transform.root.GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (m_DisplayFlag)
            {
                // change the key bindings to whatever is suitable
                //if (Input.GetKeyDown(KeyCode.Joystick1Button6))
                //if (Input.GetKeyDown(KeyCode.T))
                if(SenseInput.GetButtonDown(m_TelportButton))
                {
                    // display teleporter
                    VRTeleporter.ToggleDisplay(true);
                    // turn off the characher controller
                    if (m_CharacterController) m_CharacterController.enabled = false;
                    // toggle VR pointer display off
                    //InputManager._instance.TogglePointerDisplay(false);
                    SenseManager._instance.SetPointerDisplayMode(Defs.PointerDisplayMode.Teleporter);
                }
                // change the key bindings to whatever is suitable
                //if (Input.GetKeyUp(KeyCode.Joystick1Button6))
                //if (Input.GetKeyUp(KeyCode.T))
                if (SenseInput.GetButtonUp(m_TelportButton))
                {
                    // teleport to the target and toggle teleporter display off
                    VRTeleporter.Teleport();
                    VRTeleporter.ToggleDisplay(false);
                    // turn on the character controller
                    if (m_CharacterController) m_CharacterController.enabled = true;
                    // toggle VR pointer display on
                    SenseManager._instance.SetPointerDisplayMode(Defs.PointerDisplayMode.LaserPointer);
                }
            }
        }
        #endregion // MONOBEHAVIOUR_METHODS
        //
        #region PRIVATE_METHODS
        #endregion // PRIVATE_METHODS
        //
        #region PUBLIC_METHODS
        /// <summary>
        /// Toggle teleporter mode on/off
        /// </summary>
        /// <param name="flag">True:Show. False:Hide</param>
        public void ToggleDisplay(bool flag)
        {
            m_DisplayFlag = flag;
        }

        /// <summary>
        /// Set teleport button name
        /// </summary>
        /// <param name="_buttonName">A-F</param>
        public void SetTeleportInput(string _buttonName)
        {
            m_TelportButton = _buttonName;
        }
        #endregion //PUBLIC_METHODS
    }
}

