using UnityEngine;
using System.Collections;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Manually manage the orientation with the device gyroscope data
    /// </summary>
    public class GyroScript : MonoBehaviour
    {
        public Transform PlayerBody;

        // STATE
        private float _initialYAngle = 0f;
        private float _appliedGyroYAngle = 0f;
        private float _calibrationYAngle = 0f;
        private Transform _rawGyroRotation;
        private float _tempSmoothing;

        // SETTINGS
        [SerializeField] private float _smoothing = 0.1f;

        private IEnumerator Start()
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                GameObject[] playerBody = GameObject.FindGameObjectsWithTag("CharacterBody");
                foreach (GameObject pb in playerBody)
                    if (pb.activeSelf) PlayerBody = pb.transform;

                GameObject CharacterBodyContainer = GameObject.Find("CharacterBodyContainer");
                if (PlayerBody == null) 
                    if(CharacterBodyContainer!=null) PlayerBody = CharacterBodyContainer.transform;

                Input.gyro.enabled = true;
                Application.targetFrameRate = 60;
                _initialYAngle = transform.eulerAngles.y;

                _rawGyroRotation = new GameObject("GyroRaw").transform;
                _rawGyroRotation.position = transform.position;
                _rawGyroRotation.rotation = transform.rotation;
            }

            // Wait until gyro is active, then calibrate to reset starting rotation.
            yield return new WaitForSeconds(1);


            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) StartCoroutine(CalibrateYAngle());

        }

        private void Update()
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                ApplyGyroRotation();
                ApplyCalibration();

                transform.rotation = Quaternion.Slerp(transform.rotation, _rawGyroRotation.rotation, _smoothing);

                // Rotate Player body around y-axis
                Quaternion temp = transform.rotation;
                temp.x = 0f;
                temp.z = 0f;

                PlayerBody.rotation = Quaternion.Slerp(PlayerBody.rotation, temp, _smoothing);
            }
        }

        private IEnumerator CalibrateYAngle()
        {
            _tempSmoothing = _smoothing;
            _smoothing = 1;
            _calibrationYAngle = _appliedGyroYAngle - _initialYAngle; // Offsets the y angle in case it wasn't 0 at edit time.
            yield return null;
            _smoothing = _tempSmoothing;
        }

        private void ApplyGyroRotation()
        {
            _rawGyroRotation.rotation = Input.gyro.attitude;
            _rawGyroRotation.Rotate(0f, 0f, 180f, Space.Self); // Swap "handedness" of quaternion from gyro.
            _rawGyroRotation.Rotate(90f, 180f, 0f, Space.World); // Rotate to make sense as a camera pointing out the back of your device.
            _appliedGyroYAngle = _rawGyroRotation.eulerAngles.y; // Save the angle around y axis for use in calibration.
        }

        private void ApplyCalibration()
        {
            _rawGyroRotation.Rotate(0f, -_calibrationYAngle, 0f, Space.World); // Rotates y angle back however much it deviated when calibrationYAngle was saved.
        }

        /// <summary>
        /// Enable and callibrate the Y-angle
        /// </summary>
        /// <param name="value"></param>
        public void SetEnabled(bool value)
        {
            enabled = true;
            StartCoroutine(CalibrateYAngle());
        }
    }
}