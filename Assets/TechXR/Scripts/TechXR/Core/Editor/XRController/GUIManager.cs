using UnityEditor;
using UnityEngine;

namespace TechXR.Core.Editor
{
    public class GUIManager
    {
        /// <summary>
        /// Show heading on Editor GUI Window
        /// </summary>
        /// <param name="_heading">Heading Name</param>
        public void ShowHeading(string _heading, GUIStyle guiStyle)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(_heading, guiStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Horizontal line separation on EditorWindow GUI
        /// </summary>
        public void HorizontalLineSeparation()
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Space(100);
            GUILayout.Box(GUIContent.none, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
            GUILayout.Space(100);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        /// <summary>
        /// Help Box with an info message
        /// </summary>
        /// <param name="message"></param>
        public void ShowHelpBox(string message)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(50);
            GUILayout.FlexibleSpace();
            EditorGUILayout.HelpBox(message, MessageType.Info);
            GUILayout.FlexibleSpace();
            GUILayout.Space(50);
            GUILayout.EndHorizontal();
        }
    }
}