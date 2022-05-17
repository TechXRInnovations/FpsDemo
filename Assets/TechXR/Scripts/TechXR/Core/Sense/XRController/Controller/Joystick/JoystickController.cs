using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Manager class for on-screen joystick for mobile devices
    /// </summary>
    internal class JoystickController : MonoBehaviour
    {
        #region PUBLIC_MEMBERS
        /// <summary>
        /// Multiplier factor to control the speed of movement
        /// </summary>
        public float MoveMultiplier = 250f;
        /// <summary>
        /// Multiplier factor to control the speed of rotation
        /// </summary>
        public float RotationMultiplier = 0.01f;
        /// <summary>
        /// Max rotation along the X-Axis
        /// </summary>
        public float RotationXMax = 30f;
        #endregion // PUBLIC_MEMBERS
        //
        #region PRIVATE_MEMBERS
        private float m_DeltaTime;
        //
        private Vector3 m_MoveAmount;
        private Vector3 m_MoveVelocity;
        private Vector3 m_MoveDirAmount;
        //
        Quaternion m_RotationMinX;
        Quaternion m_RotationMaxX;
        
        //private bool m_JoyStickActive;
        //
        [SerializeField]
        private VariableJoystick m_MoveJoystick;
        [SerializeField]
        private VariableJoystick m_DirJoystick;
        [SerializeField]
        private GameObject m_Player;

        private bool m_VerticalFlag = new bool();
        #endregion // PRIVATE_MEMBERS;
        //
        #region MONOBEHAVIOUR_METHODS
        // Start is called before the first frame update
        void Start()
        {
            m_DeltaTime = 0.15f;
            m_MoveAmount = Vector3.zero;
            m_MoveVelocity = Vector3.one * MoveMultiplier;
            m_MoveDirAmount = Vector3.zero;
            //
            m_RotationMinX = Quaternion.Euler(new Vector3(-30f, 0, 0));
            m_RotationMaxX = Quaternion.Euler(new Vector3(30f, 0, 0));
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 moveDir = m_Player.transform.forward * m_MoveJoystick.Vertical + m_Player.transform.right * m_MoveJoystick.Horizontal;
            m_MoveAmount = Vector3.SmoothDamp(m_MoveAmount, moveDir * 15f, ref m_MoveVelocity, m_DeltaTime);
            //print("Vertical: " + m_DirJoystick.Vertical + ", horizontal: " + m_DirJoystick.Horizontal);
            float rotationX = RotationXMax * -m_DirJoystick.Vertical;
            
            float rotationY = Mathf.Abs(m_DirJoystick.Horizontal) > 0.2f ? (1.2f * m_DirJoystick.Horizontal) : 0;
            float rotationZ = 0;

            print("Angle: " + Mathf.Atan2(Mathf.Abs(m_DirJoystick.Horizontal), Mathf.Abs(m_DirJoystick.Vertical)) * 180 / Mathf.PI);
            m_VerticalFlag = (Mathf.Atan2(Mathf.Abs(m_DirJoystick.Horizontal), Mathf.Abs(m_DirJoystick.Vertical)) * 180 / Mathf.PI) <= 30f ? true : false;

            m_MoveDirAmount = new Vector3(rotationX, rotationY, rotationZ);
        }

        private void FixedUpdate()
        {
            // move
            Move();
            // rotate
            Rotate();
        }
        #endregion // MONOBEHAVIOUR_METHODS
        //
        #region PRIVATE_METHODS
        /// <summary>
        /// Move player with joystick input
        /// </summary>
        private void Move()
        {
            Vector3 initPosition = m_Player.transform.position;
            Vector3 finalPosition = initPosition + m_MoveAmount;

            m_Player.transform.position = Vector3.Lerp(initPosition, finalPosition, Time.fixedDeltaTime);
        }

        /// <summary>
        /// Rotate player with joystick input
        /// </summary>
        private void Rotate()
        {
            if ((m_DirJoystick.Horizontal == 0 && m_DirJoystick.Vertical == 0))// || (Mathf.Abs(m_DirJoystick.Horizontal) >= 0.1f))
            {
                Quaternion currentRotation = m_Player.transform.localRotation;
                Quaternion homeRotation = currentRotation;

                Vector3 angles = homeRotation.eulerAngles;

                angles.x = angles.z = 0;

                homeRotation.eulerAngles = angles;
                
                m_Player.transform.localRotation = Quaternion.Slerp(currentRotation, homeRotation, Time.fixedDeltaTime);
            }
            else //if ((Mathf.Abs(m_DirJoystick.Vertical) > 0.1f) || (Mathf.Abs(m_DirJoystick.Horizontal) > 0.1f))
            {
                float angleX = m_MoveDirAmount.x;
                float angleY = m_Player.transform.localEulerAngles.y;

                Vector3 angles = new Vector3(angleX, angleY, 0);

                Quaternion currentRotation = m_Player.transform.localRotation;

                m_Player.transform.localRotation = Quaternion.Slerp(currentRotation, Quaternion.Euler(angles), Time.fixedDeltaTime);

                m_Player.transform.Rotate(Vector3.up, m_MoveDirAmount.y);
            }
            
        }
        #endregion // PRIVATE_METHODS
        //
        #region PUBLIC_METHODS
        /// <summary>
        /// Set player object reference
        /// </summary>
        /// <param name="player">The gameobject to be set as the main player object</param>
        public void SetPlayerReference(GameObject player)
        {
            m_Player = player;
        }
        #endregion //PUBLIC_METHODS
    }
}

