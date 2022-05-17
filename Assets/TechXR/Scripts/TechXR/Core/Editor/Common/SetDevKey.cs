using UnityEngine;
using UnityEditor;
using TechXR.Core.Sense;

namespace TechXR.Core.Editor
{

    [InitializeOnLoad]
    public class SetDevKey : EditorWindow
    {
        static string DevKey;
        static bool IsEmpty;
        static FixedExpiryLicense fixedExpiryLicense;
        bool showWarning = false;

        static SetDevKey()
        {
            EditorApplication.update += RunOnce;
        }

        static void RunOnce()
        {
            DevKey = TechXRConfiguration.Instance.LicenseKey;
            fixedExpiryLicense = new FixedExpiryLicense();

            if (string.IsNullOrEmpty(DevKey))
            {
                IsEmpty = true;
                SetDevKey window = (SetDevKey)EditorWindow.GetWindow<SetDevKey>("DevKey Window");
                window.minSize = new Vector2(500, 100);
                window.maxSize = new Vector2(600, 100);
            }
            else
                IsEmpty = false;
            EditorApplication.update -= RunOnce;
        }

        private void OnGUI()
        {
            // Space
            GUILayout.Space(8);


            // Start designing Window Vertically
            GUILayout.BeginVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Enter Dev Key : ");
            DevKey = EditorGUILayout.TextField("", DevKey, GUILayout.Width(400));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("OK"))
            {
                TechXRConfiguration.Instance.LicenseKey = DevKey;
                EditorUtility.SetDirty(TechXRConfiguration.Instance); // Save configuration asset file
                if (fixedExpiryLicense.VerifyLicenseKey(DevKey))
                {
                    showWarning = false;
                    // Notify
                    if (EditorWindow.HasOpenInstances<TechXR>())
                    {
                        TechXR.GetWindow<TechXR>().ShowNotification(new GUIContent("DevKey is Valid till 31st July 2022!!"));
                    }
                    else
                    {
                        foreach (SceneView scene in SceneView.sceneViews)
                        {
                            scene.ShowNotification(new GUIContent("DevKey is Valid till 31st July 2022!!"));
                        }
                    }
                    this.Close();
                }
                else
                {
                    showWarning = true;
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (showWarning)
            {
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Wrong Key Entered..!! Please Enter a valid key");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndVertical(); // End Vertical Window design

        }
    }
}