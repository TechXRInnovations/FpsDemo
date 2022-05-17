using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TechXR.Core.Utils;
using UnityEngine.UI;

namespace TechXR.Core.Sense
{
    public class GazeTimer : MonoBehaviour
    {
        public static GazeTimer Instance => _instance;
        private static GazeTimer _instance;
        #region PUBLIC_FIELDS
        public float TotalTime = 2f;
        #endregion // PUBLIC_FIELDS
        //
        #region PRIVATE_FIELDS
        private Image m_GazeImg;
        private LaserPointer m_LaserPointer;
        private bool m_GazeStatus;
        private float m_GazeTimer;
        #endregion // PRIVATE_FIELDS
        //
        #region MONOBEHAVIOUR_METHODS
        private void Awake()
        {
            if (_instance != null)
            {
                Debug.LogWarning("Trying to create more than one instance..!");
                Destroy(_instance);
            }
            _instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            m_LaserPointer = GameObject.FindObjectOfType<LaserPointer>();

            if (!m_LaserPointer)
                Debug.LogWarning("LaserPointer not found...!!");

            GameObject gazeImageObject = GameObject.FindWithTag("GazeImage");
            if (gazeImageObject) m_GazeImg = gazeImageObject.GetComponent<Image>();

            if (!m_GazeImg)
                Debug.LogWarning("GazeImage not found...!!");
        }
        // Update is called once per frame
        void Update()
        {
            if (m_GazeStatus && RuntimeLicenseCheck.IsValid)
            {
                m_GazeTimer += Time.deltaTime;
                m_GazeImg.fillAmount = m_GazeTimer / TotalTime;

                if (m_GazeImg.fillAmount == 1)
                {
                    m_LaserPointer.ButtonState = true;
                    m_GazeStatus = false;
                    m_GazeImg.fillAmount = 0;
                }
            }
        }
        #endregion // MONOBEHAVIOUR_METHODS
        //
        #region PUBLIC_METHODS
        public void GazeOn()
        {
            m_GazeStatus = true;
        }
        //
        public void GazeOff()
        {
            m_GazeStatus = false;
            m_GazeTimer = 0;
            m_GazeImg.fillAmount = 0;
            m_LaserPointer.ButtonState = false;
        }
        #endregion // PUBLIC_METHODS
    }
}