using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TechXR.Core.Sense
{
    public interface IObject
    {
        /// <summary>
        /// Get the Object global position
        /// </summary>
        /// <returns></returns>
        Vector3 GetObjectPosition();

        /// <summary>
        /// Get the Object local position
        /// </summary>
        /// <returns></returns>
        Vector3 GetObjectLocalPosition();

        /// <summary>
        /// Get the Object rotation
        /// </summary>
        /// <returns></returns>
        Quaternion GetObjectRotation();

        /// <summary>
        /// Get the Object local rotation
        /// </summary>
        /// <returns></returns>
        Quaternion GetObjectLocalRotation();
    }
}
