using System;

namespace TechXR.Core.Sense
{
    /// <summary>
    /// Sense event delegate
    /// </summary>
    /// <param name="args"></param>
    public delegate void SenseEventDelegate(params object[] args);

    /// <summary>
    /// Events to be registered by the XR Manager
    /// </summary>
    public enum SenseEvent
    {
        // Object Grabbed
        OBJECT_GRABBED,
        // Object Released
        OBJECT_RELEASED,
        // Object Clicked
        OBJECT_CLICKED,
        // Data loading complete
        DATA_LOADING_COMPLETE,
        // Joystick button 0
        BUTTON_C_UP, BUTTON_C_DOWN, BUTTON_C_PRESS_CONTINUE,
        // Joystick button 1
        BUTTON_A_UP, BUTTON_A_DOWN, BUTTON_A_PRESS_CONTINUE,
        // Joystick button 2
        BUTTON_B_UP, BUTTON_B_DOWN, BUTTON_B_PRESS_CONTINUE,
        // Joystick button 3
        BUTTON_D_UP, BUTTON_D_DOWN, BUTTON_D_PRESS_CONTINUE,
        // Joystick button 4
        BUTTON_E_UP, BUTTON_E_DOWN, BUTTON_E_PRESS_CONTINUE,
        // Joystick button 5
        BUTTON_F_UP, BUTTON_F_DOWN, BUTTON_F_PRESS_CONTINUE
    }
}