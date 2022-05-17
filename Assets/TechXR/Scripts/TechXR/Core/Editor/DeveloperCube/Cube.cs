using UnityEngine;
using UnityEditor;
using TechXR.Core.Sense;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace TechXR.Core.Editor
{
    public class Cube : EditorWindow
    {
        static FixedExpiryLicense fixedExpiryLicense;
        static bool IsLicenseKeyValid = false;

        // max layers and tags allowed
        private const int MAX_LAYERS = 31;
        private const int MAX_TAGS = 10000;

        // layers and tags to be added
        private static List<string> m_Layers = new List<string>() { "Ground" };
        private static List<string> m_Tags = new List<string>() { "XRPlayerController", "SenseController", "SenseManager", "Env", "CharacterBody", "TechXRDeveloperCube", "SenseEventSystem", "GazeImage", "ISphere" };

        // GUI Styling
        private GUIStyle guiStyle = new GUIStyle();


        // For Folding the elements
        protected static bool showUIElements = false;
        protected static bool showPrefabs = false;
        protected static bool showObjects = false;

        // Dropdown Controller Body Options 
        string[] controllerBodyOptions = { "Default", "Hand" };
        static int controllerBodyIndex = 0;

        // Dropdown Player Character Options
        static string[] playerCharacterOptions = { "None", "Default" };
        static int playerCharacterIndex = 0;

        // Dropdown Player Character Options
        static string[] arControllerOptions = { "InsideView", "OutsideView", "IntractableCube" };
        static int arControllerIndex = 0;

        // Dropdown Environment Options
        static int environmentIndex;
        static DirectoryInfo environmentDir = new DirectoryInfo("Assets/TechXR/Prefabs/Environments");
        static FileInfo[] environmentFileInfo;
        static string[] environmentOptions;

        // Dropdown Canvas Type Options
        static int uiElementIndex;
        static DirectoryInfo uiElementDir = new DirectoryInfo("Assets/TechXR/Prefabs/UIElements/UI");
        static FileInfo[] uiElementFilesInfo;
        string[] uiElementOptions;

        // Dropdown 3D Model Options
        static int modelIndex;
        static DirectoryInfo modelsDir = new DirectoryInfo("Assets/TechXR/3DModels");
        static FileInfo[] modelFilesInfo;
        string[] modelOptions;

        // Dropdown Skybox Options
        static int skyboxIndex;
        static DirectoryInfo skyboxDir = new DirectoryInfo("Assets/TechXR/Skybox/Mat");
        static FileInfo[] skyboxFilesInfo;
        static string[] skyboxOptions;

        Texture2D controllerBody;

        Vector2 scrollerPosition;

        // Tab System
        private GUISkin skin;
        private GUIStyle rightTabStyle, leftTabStyle, tabStyle2, tabStyle3;
        static private int selectedTab = 0;
        static string[] tabs = { "AR", "VR" };
        GUISkin originalGUISkin;

        static private int SelectedTabsOfDeveloperSDK = 0;
        static string[] DeveloperSDKOptions = { "DEVELOPER CUBE", "6 DoF CONTROLLER" };

        // Skin color alternate
        Texture2D headerSectionTexture;
        Color headerSectionColor = new Color(0.76f, 0.76f, 0.76f, 1);
        private Rect headerSection;
        //

        float originalValue;

        // Search Variables
        string searchText;
        bool displayAssets;
        Dictionary<string, GUIContent> searchGuisDict;
        Dictionary<string, List<AssetFileInfo>> idFilesDict;
        Dictionary<string, string> idAuthorDict;
        AssetManager assetManager;
        //

        private void OnEnable()
        {
            // save references to skin and style
            skin = (GUISkin)Resources.Load("CustomSkin_Cube");
            rightTabStyle = skin.GetStyle("RightTab");
            leftTabStyle = skin.GetStyle("LeftTab");
            tabStyle2 = skin.GetStyle("Tab2");
            tabStyle3 = skin.GetStyle("Tab3");

            // Update Layers and Tags
            AddLayersAndTags();

            // Update the dropdown lists
            FillEnvironmentOptions();
            FillModelOptions();
            FillSkyboxOptions();
            FillUIElementOptions();

            //
            InitTextures();
            //

            //
            displayAssets = false;
            //
        }

        void InitTextures()
        {
            headerSectionTexture = new Texture2D(1, 1);
            headerSectionTexture.SetPixel(0, 0, headerSectionColor);
            headerSectionTexture.Apply();
        }

        void DrawLayouts()
        {
            headerSection.x = 0;
            headerSection.y = 0;
            headerSection.width = Screen.width;
            headerSection.height = Screen.height;

            GUI.DrawTexture(headerSection, headerSectionTexture);
        }

        [MenuItem("TechXR/Cube_Preferences")]
        public static void ShowWindow()
        {
            fixedExpiryLicense = new FixedExpiryLicense();
            IsLicenseKeyValid = fixedExpiryLicense.VerifyLicenseKey(TechXRConfiguration.Instance.LicenseKey);

            Cube window = (Cube)EditorWindow.GetWindow<Cube>("TechXR Sense Action Panel");
            window.minSize = new Vector2(600, 620);
            window.maxSize = new Vector2(600, 640);

            if (!IsLicenseKeyValid) OpenDevKeyWindow();

            // Give path to the TechXR Icon here
            //var texture = Resources.Load<Texture>("Icon/TechXR");
            //window.titleContent = new GUIContent("Nice icon, eh?", texture, "Just the tip");
        }

        public static void OpenDevKeyWindow()
        {
            SetDevKey devKeyWindow = (SetDevKey)EditorWindow.GetWindow<SetDevKey>("DevKey Window");
        }

        [MenuItem("TechXR/Cube_Set-Up Controller VR Scene")]
        public static void SetUpVRScene()
        {

        }

        // Validate the menu item defined by the function above.
        // The menu item will be disabled if this function returns false.
        [MenuItem("TechXR/Cube_Set-Up Controller VR Scene", true)]
        static bool ValidateSetUpVRScene()
        {
            // Return false if no transform is selected.
            return false;
        }

        [MenuItem("TechXR/Cube_Set-Up Controller AR Scene")]
        public static void SetUpARScene()
        {

        }

        [MenuItem("TechXR/Cube_Set-Up Controller AR Scene", true)]
        static bool ValidateSetUpARScene()
        {
            // Return false if no transform is selected.
            return false;
        }

        [MenuItem("TechXR/Cube_Set-Up Cube VR Scene")]
        public static void SetUpCubeVRScene()
        {
            fixedExpiryLicense = new FixedExpiryLicense();
            IsLicenseKeyValid = fixedExpiryLicense.VerifyLicenseKey(TechXRConfiguration.Instance.LicenseKey);
            if (!IsLicenseKeyValid)
                OpenDevKeyWindow();
            else if (EditorUtility.DisplayDialog("Warning...!!", "TechXR-Default VR Scene will be loaded, it may remove the Existing TechXR-Environment, DeveloperCube and Cameras.", "Ok", "Close"))
            {
                // Reset Dropdown Selection
                controllerBodyIndex = 0;
                playerCharacterIndex = 0;
                environmentIndex = 0;

                // Add Tags & Layers
                AddLayersAndTags();

                // Delete all un-necessary things for AR camera configuration
                GameObject[] grounds = GameObject.FindGameObjectsWithTag("Env");
                foreach (GameObject ground in grounds)
                    DestroyImmediate(ground);

                GameObject[] players = GameObject.FindGameObjectsWithTag("XRPlayerController");
                foreach (GameObject player in players)
                    DestroyImmediate(player);

                // Add Items
                AddSenseCamera();
                AddDeveloperCube("Intractable");
                if (!GameObject.Find("MenuCanvas"))
                    AddCanvas("MenuCanvas");
                AddEnvironmentForVR();
                AddDirectionalLight();
                AddGazeTimer();

                // Camera settings
                Camera.main.clearFlags = CameraClearFlags.Skybox;

                // Notify
                foreach (SceneView scene in SceneView.sceneViews)
                {
                    scene.ShowNotification(new GUIContent("Updated..!!"));
                }

                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }

        [MenuItem("TechXR/Cube_Set-Up Cube AR Scene")]
        public static void SetUpCubeARScene()
        {
            fixedExpiryLicense = new FixedExpiryLicense();
            IsLicenseKeyValid = fixedExpiryLicense.VerifyLicenseKey(TechXRConfiguration.Instance.LicenseKey);
            if (!IsLicenseKeyValid)
                OpenDevKeyWindow();
            else if (EditorUtility.DisplayDialog("Warning...!!", "TechXR-Default AR Scene will be loaded, it may remove the Existing TechXR-Environment, DeveloperCube and Cameras.", "Ok", "Close"))
            {
                // Reset Dropdown Selection
                controllerBodyIndex = 0;
                playerCharacterIndex = 0;
                environmentIndex = 0;


                // Add Items
                AddLayersAndTags();
                AddSenseCamera();
                AddDeveloperCube("InsideView");
                AddDirectionalLight();
                AddGazeTimer();

                // Delete all un-necessary things for AR camera configuration
                GameObject[] grounds = GameObject.FindGameObjectsWithTag("Env");
                foreach (GameObject ground in grounds)
                    DestroyImmediate(ground);

                GameObject[] players = GameObject.FindGameObjectsWithTag("XRPlayerController");
                foreach (GameObject player in players)
                    DestroyImmediate(player);


                // Camera settings
                Camera.main.clearFlags = CameraClearFlags.SolidColor;

                // Notify
                foreach (SceneView scene in SceneView.sceneViews)
                {
                    scene.ShowNotification(new GUIContent("Updated..!!"));
                }

                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }


        [MenuItem("TechXR/Cube_Configuration")]
        public static void ShowTechXRConfiguration()
        {
            if (TechXRConfiguration.Instance)
            {
                // Print the path of the created asset
                string configPath = "Assets/Resources/TechXRConfiguration.asset";
                Debug.Log(configPath);

                // Open the ConfigurationFile
                AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(configPath));
            }
            else
            {
                // Create the asset
                TechXRConfiguration techXRConfiguration = new TechXRConfiguration();
                AssetDatabase.CreateAsset(techXRConfiguration, "Assets/Resources/TechXRConfiguration.asset");

                // Print the path of the created asset
                string path = AssetDatabase.GetAssetPath(techXRConfiguration);
                Debug.Log(path);

                // Open the ConfigurationFile
                AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(path));
            }
        }

        private void OnGUI()
        {
            if (!headerSectionTexture)
            {
                Close();
            }
            DrawLayouts();
            //Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/TechXR/Logo/file.png", typeof(Texture));
            //GUILayout.Box(banner, skin.GetStyle("Logo"));

            //=============Developer Cube Tabs=================
            originalGUISkin = GUI.skin; // store original gui skin
            GUI.skin = skin; // use our custom skin

            // Create toolbar using custom tab style

            if (DeveloperSDKOptions[SelectedTabsOfDeveloperSDK].Contains("CONTROLLER"))
            {
                SelectedTabsOfDeveloperSDK = GUILayout.Toolbar(SelectedTabsOfDeveloperSDK, DeveloperSDKOptions, leftTabStyle);
            }
            else
            {
                SelectedTabsOfDeveloperSDK = GUILayout.Toolbar(SelectedTabsOfDeveloperSDK, DeveloperSDKOptions, rightTabStyle);
            }

            // Set Back original skin
            GUI.skin = originalGUISkin;
            //X============Developer Cube Tabs================X



            //=============AR and VR Tabs======================
            originalGUISkin = GUI.skin; // store original gui skin
            GUI.skin = skin; // use our custom skin

            // Create toolbar using custom tab style
            if (DeveloperSDKOptions[SelectedTabsOfDeveloperSDK].Contains("CONTROLLER"))
            {
                selectedTab = GUILayout.Toolbar(selectedTab, tabs, tabStyle3);
            }
            else
            {
                selectedTab = GUILayout.Toolbar(selectedTab, tabs, tabStyle2);
            }
            // Set Back original skin
            GUI.skin = originalGUISkin;
            //X============AR and VR Tabs=====================X


            // Set GUI Style
            guiStyle.fontSize = 15; // Change the font size
            guiStyle.richText = true; // Bold
            guiStyle.fontStyle = FontStyle.Bold;
            guiStyle.alignment = TextAnchor.MiddleCenter; // Align Text to the center

            // Space
            GUILayout.Space(8);

            // Start designing Window Vertically
            GUILayout.BeginVertical();

            // Add Scroller / Begin scroll view
            scrollerPosition = GUILayout.BeginScrollView(scrollerPosition, false, true);

            IsLicenseKeyValid = fixedExpiryLicense.VerifyLicenseKey(TechXRConfiguration.Instance.LicenseKey);

            EditorGUI.BeginDisabledGroup(!IsLicenseKeyValid);

            // Space
            GUILayout.Space(7);

            originalValue = EditorGUIUtility.labelWidth;

            // Developer Cube
            if (DeveloperSDKOptions[SelectedTabsOfDeveloperSDK].Contains("DEVELOPER"))
            {
                // AR Tab
                if (tabs[selectedTab].Contains("AR"))
                {
                    // Space
                    GUILayout.Space(10);

                    // Show XR Camera
                    ShowXRCamera();


                    // Horizontal Line Separation
                    HorizontalLineSeparation();

                    // Space
                    GUILayout.Space(10);

                    // Show XR Developer Cube
                    ShowXRDeveloperCube();

                    // Space
                    GUILayout.Space(10);

                    // Horizontal Line Separation
                    HorizontalLineSeparation();

                    // Space
                    GUILayout.Space(10);

                    // Show Assets
                    ShowAssets();

                }

                // VR Tab
                if (tabs[selectedTab].Contains("VR"))
                {
                    // Space
                    GUILayout.Space(10);

                    // Show XR Camera
                    ShowXRCamera();

                    // Horizontal Line Separation
                    HorizontalLineSeparation();

                    // Space
                    GUILayout.Space(10);


                    // Show XR Developer Cube
                    ShowXRDeveloperCube();


                    // Space
                    GUILayout.Space(10);


                    // Horizontal Line Separation
                    HorizontalLineSeparation();

                    // Show Configure VR Scene
                    ShowConfigureVRScene();

                    // Space
                    GUILayout.Space(10);

                    // Horizontal Line Separation
                    HorizontalLineSeparation();

                    // Space
                    GUILayout.Space(10);

                    ShowAssets();
                }
            }

            // 6DoF Controller
            if (DeveloperSDKOptions[SelectedTabsOfDeveloperSDK].Contains("CONTROLLER"))
            {

                // AR Tab
                if (tabs[selectedTab].Contains("AR"))
                {
                    EditorGUI.BeginDisabledGroup(true);

                    // Space
                    GUILayout.Space(10);

                    // Show XR Camera
                    ShowXRCamera();


                    // Space
                    GUILayout.Space(10);


                    // Horizontal Line Separation
                    HorizontalLineSeparation();


                    // Show XR Controller
                    ShowXRController();


                    // Space
                    GUILayout.Space(10);


                    // Horizontal Line Separation
                    HorizontalLineSeparation();


                    // Space
                    GUILayout.Space(10);


                    // Show Assets
                    ShowAssets();


                    EditorGUI.EndDisabledGroup();
                }

                // VR Tab
                if (tabs[selectedTab].Contains("VR"))
                {
                    EditorGUI.BeginDisabledGroup(true);

                    // Space
                    GUILayout.Space(10);


                    // Show XR Player
                    ShowXRPlayer();


                    // Horizontal Line Separation
                    HorizontalLineSeparation();


                    // Show Configure VR Controller Scene
                    ShowConfigureVRControllerScene();


                    // Horizontal Line Separation
                    HorizontalLineSeparation();


                    // Show Assets
                    ShowAssets();


                    EditorGUI.EndDisabledGroup();
                }
            }

            EditorGUI.EndDisabledGroup();

            GUILayout.EndScrollView(); // End Scroll View


            // Show Logo and Link
            ShowLogoWithLink();


            // Space
            GUILayout.Space(10);


            GUILayout.EndVertical(); // End Vertical Window design

        }

        void ShowLogoWithLink()
        {
            Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/TechXR/Logo/file.png", typeof(Texture));
            GUILayout.Box(banner, skin.GetStyle("Logo"));

            // Space
            GUILayout.Space(10);

            // Link to TechXR Store
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("To Pre-Book your TechXR 6DoF Controller (SenseXR) Click");
            if (GUILayout.Button("Here", skin.GetStyle("Button")))
            {
                Application.OpenURL("https://forms.gle/jxNumHU3ZwVZVwGi6");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        void ShowConfigureVRControllerScene()
        {
            // Heading
            ShowHeading("Configure VR Scene", skin.GetStyle("Heading"));


            // Space
            GUILayout.Space(15);


            // Add Environment
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (environmentOptions[0] == null) FillEnvironmentOptions();
            EditorGUIUtility.labelWidth = 100;
            GUILayout.Label("Environment :", skin.GetStyle("SubHeading"));
            EditorGUIUtility.labelWidth = originalValue;
            environmentIndex = EditorGUILayout.Popup("", environmentIndex, environmentOptions);
            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {

            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            // Space
            GUILayout.Space(10);


            // Add Skybox
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (environmentOptions[0] == null) FillEnvironmentOptions();
            EditorGUIUtility.labelWidth = 100;
            GUILayout.Label("Skybox         :", skin.GetStyle("SubHeading"));
            EditorGUIUtility.labelWidth = originalValue;
            skyboxIndex = EditorGUILayout.Popup("", skyboxIndex, skyboxOptions);
            if (GUILayout.Button("Set", GUILayout.Width(60)))
            {

            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            // Space
            GUILayout.Space(10);


            // Add Trackable Warning System
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUIUtility.labelWidth = 100;
            GUILayout.Label("Controller Trackable Red Alert Warning System :", skin.GetStyle("SubHeading"));
            EditorGUIUtility.labelWidth = originalValue;
            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {

            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            // Space
            GUILayout.Space(10);


            // Add Bluetooth Warning System
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUIUtility.labelWidth = 100;
            GUILayout.Label("Controller Bluetooth Warning System :", skin.GetStyle("SubHeading"));
            EditorGUIUtility.labelWidth = originalValue;
            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {

            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            // Space
            GUILayout.Space(10);
        }

        void ShowXRPlayer()
        {
            // Heading
            ShowHeading("XR Player", skin.GetStyle("Heading"));

            // Space
            GUILayout.Space(20);


            // Check for the player controller
            GameObject playerController = GameObject.FindWithTag("XRPlayerController");


            // Player Body Options
            GUILayout.BeginHorizontal();
            GUILayout.Space(80);
            GUILayout.Label("Character Type :", skin.GetStyle("SubHeading"));
            // Start a code block to check for GUI changes
            EditorGUI.BeginChangeCheck();


            playerCharacterIndex = EditorGUILayout.Popup("", playerCharacterIndex, playerCharacterOptions);

            // End the code block and update the label if a change occurred
            if (EditorGUI.EndChangeCheck())
            {
                if (playerController)
                {
                    Transform characterBodyContainer = playerController.transform.Find("CharacterBodyContainer");

                    foreach (Transform child in characterBodyContainer)
                    {
                        child.gameObject.SetActive(false);
                    }

                    characterBodyContainer.Find(playerCharacterOptions[playerCharacterIndex]).gameObject.SetActive(true);
                }


            }

            GUILayout.Space(40);
            GUILayout.EndHorizontal();


            // Space
            GUILayout.Space(10);


            // Controller Body Options
            GUILayout.BeginHorizontal();
            GUILayout.Space(80);
            GUILayout.Label("Controller Type :", skin.GetStyle("SubHeading"));


            // Start a code block to check for GUI changes
            EditorGUI.BeginChangeCheck();

            // Dropdown
            controllerBodyIndex = EditorGUILayout.Popup("", controllerBodyIndex, controllerBodyOptions);

            // End the code block and update the label if a change occurred
            if (EditorGUI.EndChangeCheck())
            {
                //if (senseController) AddController();
            }


            if (controllerBodyIndex >= 0)
            {
                string icon = "";

                if (controllerBodyIndex == 0) icon = "Default";
                if (controllerBodyIndex == 1) icon = "Hand";

                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath("Assets/TechXR/Sprites/Icon/" + icon + ".png", typeof(Texture2D));
                controllerBody = obj as Texture2D;
                GUILayout.Label(controllerBody, GUILayout.Width(80), GUILayout.Height(80));
            }
            GUILayout.Space(40);
            GUILayout.EndHorizontal();



            // Add Player
            EditorGUI.BeginDisabledGroup(playerController);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add", GUILayout.Width(80), GUILayout.Height(30)))
            {

            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();


            // HelpBox
            if (playerController)
            {
                // Space
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.Space(50);
                GUILayout.FlexibleSpace();
                EditorGUILayout.HelpBox("XRCamera and the XRController are inside XRPlayerController", MessageType.Info);
                GUILayout.FlexibleSpace();
                GUILayout.Space(50);
                GUILayout.EndHorizontal();
            }

        }

        void ShowXRController()
        {
            // Heading
            ShowHeading("XR Controller", skin.GetStyle("Heading"));


            // Space
            GUILayout.Space(20);


            // Check for DeveloperCube
            GameObject developerCube = GameObject.FindGameObjectWithTag("TechXRDeveloperCube");


            // XRController
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Controller Type :", skin.GetStyle("SubHeading"));
            GUILayout.Space(10);

            // Start a code block to check for GUI changes
            EditorGUI.BeginChangeCheck();

            // Dropdown
            controllerBodyIndex = EditorGUILayout.Popup("", controllerBodyIndex, controllerBodyOptions, GUILayout.Width(100));

            // End the code block and update the label if a change occurred
            if (EditorGUI.EndChangeCheck())
            {
                //if (senseController) AddController();
            }

            GUILayout.Space(15);
            if (controllerBodyIndex >= 0)
            {
                string icon = "";

                if (controllerBodyIndex == 0) icon = "Default";
                if (controllerBodyIndex == 1) icon = "Hand";

                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath("Assets/TechXR/Sprites/Icon/" + icon + ".png", typeof(Texture2D));
                controllerBody = obj as Texture2D;
                GUILayout.Label(controllerBody, GUILayout.Width(80), GUILayout.Height(80));
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            // Add Button
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            //EditorGUI.BeginDisabledGroup(senseController || developerCube);
            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {

            }
            //EditorGUI.EndDisabledGroup();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        void ShowConfigureVRScene()
        {
            // Heading
            ShowHeading("Configure VR Scene", skin.GetStyle("Heading"));

            GameObject developerCube = GameObject.FindGameObjectWithTag("TechXRDeveloperCube");

            // Space
            GUILayout.Space(15);


            // Add Environment
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (environmentOptions[0] == null) FillEnvironmentOptions();
            EditorGUIUtility.labelWidth = 100;
            GUILayout.Label("Environment :", skin.GetStyle("SubHeading"));
            EditorGUIUtility.labelWidth = originalValue;
            environmentIndex = EditorGUILayout.Popup("", environmentIndex, environmentOptions);
            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {
                GameObject[] envs = GameObject.FindGameObjectsWithTag("Env");

                if (envs.Length > 0)
                {
                    if (EditorUtility.DisplayDialog("Warning...!!", "TechXR-Environment already present", "Delete Existing and Add new one", "Close"))
                    {
                        foreach (GameObject e in envs)
                            DestroyImmediate(e);
                        AddEnvironment();
                        this.ShowNotification(new GUIContent("Updated..!!"));
                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    }
                }
                else
                {
                    AddEnvironment();
                    this.ShowNotification(new GUIContent("Updated..!!"));
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
                AddDirectionalLight();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            // Space
            GUILayout.Space(10);


            // Add Skybox
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (environmentOptions[0] == null) FillEnvironmentOptions();
            EditorGUIUtility.labelWidth = 100;
            GUILayout.Label("Skybox         :", skin.GetStyle("SubHeading"));
            EditorGUIUtility.labelWidth = originalValue;
            skyboxIndex = EditorGUILayout.Popup("", skyboxIndex, skyboxOptions);
            if (GUILayout.Button("Set", GUILayout.Width(60)))
            {
                SetSkybox();
                this.ShowNotification(new GUIContent("Updated..!!"));
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

        }

        void ShowAssets()
        {
            // Heading
            ShowHeading("Assets", skin.GetStyle("Heading"));

            GameObject developerCube = GameObject.FindGameObjectWithTag("TechXRDeveloperCube");

            // Space
            GUILayout.Space(15);


            // UI Elements Options
            GUILayout.BeginHorizontal();
            GUILayout.Label("Select UI Element :", skin.GetStyle("SubHeading"));
            GUILayout.Space(20);
            if (uiElementOptions[0] == null) FillUIElementOptions();
            uiElementIndex = EditorGUILayout.Popup("", uiElementIndex, uiElementOptions, GUILayout.Width(100)); // Dropdown
            GUILayout.Space(80);
            if (uiElementIndex >= 0)
            {
                GUILayout.BeginVertical();
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath("Assets/TechXR/Sprites/Icon/" + uiElementOptions[uiElementIndex] + ".png", typeof(Texture2D));

                // Button
                if (GUILayout.Button("Add", GUILayout.Width(75)))
                {
                    AddCanvas(uiElementOptions[uiElementIndex]);
                    this.ShowNotification(new GUIContent("Updated..!!"));
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
                GUILayout.EndVertical();
            }
            GUILayout.Space(20);
            GUILayout.EndHorizontal();


            // Space
            GUILayout.Space(15);


            // 3D Models Options
            GUILayout.BeginHorizontal();
            GUILayout.Label("Select 3D Model  :", skin.GetStyle("SubHeading"));
            GUILayout.Space(20);
            if (modelOptions[0] == null) FillModelOptions();
            modelIndex = EditorGUILayout.Popup("", modelIndex, modelOptions, GUILayout.Width(100)); // Dropdown
            GUILayout.Space(80);
            if (modelIndex >= 0)
            {
                GUILayout.BeginVertical();
                if (GUILayout.Button("Add", GUILayout.Width(75))) // Button
                {
                    UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath("Assets/TechXR/3DModels/" + modelOptions[modelIndex] + "/" + "model.obj", typeof(GameObject));

                    if (!prefab)
                    {
                        prefab = AssetDatabase.LoadAssetAtPath("Assets/TechXR/3DModels/" + modelOptions[modelIndex] + "/" + modelOptions[modelIndex] + ".obj", typeof(GameObject));
                    }

                    GameObject obj = Instantiate(prefab) as GameObject;
                    if (developerCube)
                    {
                        obj.transform.SetParent(developerCube.transform);
                        obj.transform.localPosition = Vector3.zero;
                    }

                    // remove the sphere
                    GameObject i_Sphere = GameObject.FindWithTag("ISphere");
                    if (i_Sphere) i_Sphere.transform.Find("Sphere").gameObject.SetActive(false);

                    // Focus
                    Selection.activeGameObject = obj;
                    SceneView.lastActiveSceneView.FrameSelected();

                    this.ShowNotification(new GUIContent("Updated..!!"));
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
                GUILayout.EndVertical();
            }
            GUILayout.Space(20);
            GUILayout.EndHorizontal();


            // Search Box
            ShowSearchBox();


            // Space
            GUILayout.Space(10);
        }

        void ShowSearchBox()
        {
            // Space
            GUILayout.Space(20);


            // Horizontal Line Separation
            HorizontalLineSeparation();


            // Space
            GUILayout.Space(10);


            // Heading
            ShowHeading("Search Box", skin.GetStyle("Heading"));


            // Space
            GUILayout.Space(10);


            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            //searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"));
            searchText = GUILayout.TextField(searchText, EditorStyles.toolbarSearchField, GUILayout.Width(250));
            //searchString = GUILayout.TextField(searchString, GUILayout.Height(50), GUILayout.Width(200));
            if (GUILayout.Button("Search"))
            {
                SearchAssets(searchText);
                
                // Remove focus if cleared
                //searchText = "";

                GUI.FocusControl(null);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Space
            GUILayout.Space(10);

            //
            if (displayAssets)
            {
                DisplayAssets(searchGuisDict, idAuthorDict);
            }
            //

            // Space
            GUILayout.Space(20);
        }


        /// <summary>
        /// Function to search assets
        /// </summary>
        /// <param name="searchText"></param>
        void SearchAssets(string searchText)
        {
            // If searchText is empty then return
            if (string.IsNullOrEmpty(searchText))
            {
                Debug.Log("No text to search..!");
                return;
            }


            // Populate List of AssetDisplay objecs fromn AssetManager class
            assetManager = new AssetManager();
            Dictionary<string, AssetDisplay> assetDisplayDict = assetManager.Populate(searchText);


            // Get file info with ID's
            idFilesDict = assetManager.GetFileInfo(assetManager.searchResult);


            // Add GUIContent objects
            searchGuisDict = new Dictionary<string, GUIContent>();
            idAuthorDict = new Dictionary<string, string>();
            foreach (KeyValuePair<string, AssetDisplay> d in assetDisplayDict)
            {
                searchGuisDict[d.Key] = d.Value.GUIContent;
                idAuthorDict[d.Key] = d.Value.AuthorCredit.authorName;
            }

            displayAssets = true;
        }

        /// <summary>
        /// Function to display buttons in form of images and call function when button is clicked
        /// </summary>
        /// <param name="var"></param>
        void DisplayAssets(Dictionary<string, GUIContent> dict, Dictionary<string, string> idAuthorDict)
        {
            int count = 0;

            if (dict.Count / 4 <= 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                //
                foreach (KeyValuePair<string, GUIContent> d in dict)
                {
                    if (GUILayout.Button(dict[d.Key], GUILayout.Width(100), GUILayout.Height(100)))
                    {
                        //Debug.Log(d.Key);

                        // Download files by providing file information of asset
                        assetManager.DownloadAsset(idFilesDict[d.Key], idAuthorDict[d.Key]);

                    }
                }
                //
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

            }
            else
            {
                foreach (KeyValuePair<string, GUIContent> d in dict)
                {
                    if (count == 0)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                    }
                    //
                    if (GUILayout.Button(dict[d.Key], GUILayout.Width(100), GUILayout.Height(100)))
                    {
                        //Debug.Log(d.Key);

                        // Download files by providing file information of asset
                        assetManager.DownloadAsset(idFilesDict[d.Key], idAuthorDict[d.Key]);

                    }
                    //
                    if (count == 3)
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        count = 0;
                        continue;
                    }
                    count++;
                }
            }

            if (count != 0)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }


        void ShowXRDeveloperCube()
        {
            // Heading
            ShowHeading("XR Developer Cube", skin.GetStyle("Heading"));

            // Space
            GUILayout.Space(20);

            GameObject developerCube = GameObject.FindGameObjectWithTag("TechXRDeveloperCube");

            // XR Developer Cube
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Cube Type :", skin.GetStyle("SubHeading"));
            GUILayout.Space(10);

            // Start a code block to check for GUI changes
            EditorGUI.BeginChangeCheck();

            // Dropdown
            arControllerIndex = EditorGUILayout.Popup("", arControllerIndex, arControllerOptions, GUILayout.Width(100));

            // End the code block and update the label if a change occurred
            if (EditorGUI.EndChangeCheck())
            {

                if (developerCube)
                {
                    // turn off inside/outside view
                    foreach (string s in arControllerOptions)
                    {
                        Transform go = developerCube.transform.Find(s);
                        if (go) go.gameObject.SetActive(false);
                    }

                    // if intractable cube is selected add laser pointer and input module
                    if (arControllerOptions[arControllerIndex].Contains("Intractable"))
                    {
                        // return if present
                        GameObject i_Sphere = GameObject.FindWithTag("ISphere");
                        if (i_Sphere) return;

                        // add intractable sphere
                        UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath("Assets/TechXR/Prefabs/Intractable Cube/Intractable.prefab", typeof(GameObject));
                        var sphere = Instantiate(prefab) as GameObject;
                        sphere.transform.SetParent(developerCube.transform);
                        sphere.transform.localPosition = Vector3.zero;

                        // add laserpointer if not present
                        LaserPointer laserPointer = developerCube.GetComponent<LaserPointer>();
                        if (!laserPointer)
                            laserPointer = developerCube.AddComponent<LaserPointer>();

                        // Set Color
                        laserPointer.Color = Color.white;

                        // add laserpointerinputmodule in eventsystem, if not prenent add eventsystem
                        GameObject eventSystem = GameObject.FindGameObjectWithTag("SenseEventSystem");
                        if (eventSystem != null)
                        {
                            // add laserpointerinputmodule
                            LaserPointerInputModule lpim = GameObject.FindObjectOfType<LaserPointerInputModule>();
                            if (lpim) lpim.enabled = true;
                            else eventSystem.AddComponent<LaserPointerInputModule>();

                            // turn off StandaloneInputModule
                            StandaloneInputModule standaloneInputModule = eventSystem.GetComponent<StandaloneInputModule>();
                            if (standaloneInputModule) standaloneInputModule.enabled = false;
                        }
                        else
                        {
                            UnityEngine.Object es_prefab = AssetDatabase.LoadAssetAtPath("Assets/TechXR/Prefabs/SenseEventSystem.prefab", typeof(GameObject));
                            var es = Instantiate(es_prefab) as GameObject;
                            es.name = "SenseEventSystem";

                            GameObject te = GetTechXREssentialGO();
                            es.transform.SetParent(te.transform);
                        }
                        AddGazeTimer();
                    }
                    else
                    {
                        // remove the sphere
                        GameObject i_Sphere = GameObject.FindWithTag("ISphere");
                        if (i_Sphere) DestroyImmediate(i_Sphere);

                        // turn on inside/outside view
                        developerCube.transform.Find(arControllerOptions[arControllerIndex]).gameObject.SetActive(true);

                        // remove the laserpointer
                        LaserPointer laserPointer = developerCube.GetComponent<LaserPointer>();
                        if (laserPointer) DestroyImmediate(laserPointer);

                        // remove the LaserPointerInputModule and enable StandaloneInputModule
                        LaserPointerInputModule lpim = GameObject.FindObjectOfType<LaserPointerInputModule>();
                        if (lpim)
                        {
                            GameObject eventSystem = lpim.gameObject;

                            DestroyImmediate(eventSystem.GetComponent<LaserPointerInputModule>());

                            StandaloneInputModule standaloneInputModule = eventSystem.GetComponent<StandaloneInputModule>();
                            if (standaloneInputModule) standaloneInputModule.enabled = true;
                        }
                        // Remove Gaze Timer Game Object
                        GazeTimer gt = FindObjectOfType<GazeTimer>();
                        if (gt) DestroyImmediate(gt.gameObject);
                    }
                    this.ShowNotification(new GUIContent("Developer Cube Updated to " + arControllerOptions[arControllerIndex] + "..!!"));
                }
            }

            GUILayout.Space(15);
            if (arControllerIndex >= 0)
            {
                string icon = arControllerOptions[arControllerIndex];

                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath("Assets/TechXR/Sprites/Icon/" + icon + ".png", typeof(Texture2D));
                controllerBody = obj as Texture2D;
                GUILayout.Label(controllerBody, GUILayout.Width(80), GUILayout.Height(80));
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();



            // Button
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(developerCube);
            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {
                // Instantiate the prefab
                UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath("Assets/TechXR/TechXR_DeveloperCube/Prefabs/DeveloperCube" + ".prefab", typeof(GameObject));
                var devCube = Instantiate(prefab) as GameObject;


                // turn off inside/outside view
                foreach (string s in arControllerOptions)
                {
                    Transform go = devCube.transform.Find(s);
                    if (go) go.gameObject.SetActive(false);
                }

                // if intractable cube is selected add laser pointer and input module
                if (arControllerOptions[arControllerIndex].Contains("Intractable"))
                {
                    // add laserpointer
                    LaserPointer laserPointer = devCube.AddComponent<LaserPointer>();

                    if (!laserPointer)
                        laserPointer = devCube.AddComponent<LaserPointer>();

                    // Set Color
                    laserPointer.Color = Color.white;

                    // laserpointerinputmodule
                    AddEventSystem();

                    AddGazeTimer();

                    // add intractable sphere
                    UnityEngine.Object i_sphere = AssetDatabase.LoadAssetAtPath("Assets/TechXR/Prefabs/Intractable Cube/Intractable.prefab", typeof(GameObject));
                    var sphere = Instantiate(i_sphere) as GameObject;
                    sphere.transform.SetParent(devCube.transform);
                    sphere.transform.localPosition = Vector3.zero;
                }
                else
                {
                    // turn on inside/outside view
                    devCube.transform.Find(arControllerOptions[arControllerIndex]).gameObject.SetActive(true);

                    // remove the sphere
                    GameObject i_Sphere = GameObject.FindWithTag("ISphere");
                    if (i_Sphere) DestroyImmediate(i_Sphere);
                }

                // Focus
                Selection.activeGameObject = devCube;
                SceneView.lastActiveSceneView.FrameSelected();

                AddDirectionalLight();

                this.ShowNotification(new GUIContent("Updated..!!"));
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        void ShowXRCamera()
        {
            // Heading
            ShowHeading("XR Camera", skin.GetStyle("Heading"));

            // Space
            GUILayout.Space(10);

            // Check for camera
            Vuforia.VuforiaBehaviour techxrCamera = GameObject.FindObjectOfType<Vuforia.VuforiaBehaviour>();

            // Camera
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("XRCamera", skin.GetStyle("SubHeading"));
            GUILayout.Space(10);
            EditorGUI.BeginDisabledGroup(techxrCamera);
            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {
                AddSenseCamera();

                if (tabs[selectedTab].Contains("VR"))
                {
                    // Camera settings
                    Camera.main.clearFlags = CameraClearFlags.Skybox;
                }
                else if (tabs[selectedTab].Contains("AR"))
                {
                    // Camera settings
                    Camera.main.clearFlags = CameraClearFlags.SolidColor;
                }
                //
                AddDirectionalLight();
                this.ShowNotification(new GUIContent("Updated..!!"));
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            // HelpBox
            if (techxrCamera)
            {
                // Horizontal Line Separation
                HorizontalLineSeparation();


                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Space(40);
                EditorGUILayout.HelpBox("To switch to the Setreo-mode (Split Screen-mode), Install XR Plugin Management under the Player Settings and select Mock HMD Loader checkbox.", MessageType.Info);
                GUILayout.Space(40);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Show heading on Editor GUI Window
        /// </summary>
        /// <param name="_heading">Heading Name</param>
        void ShowHeading(string _heading, GUIStyle _guiStyle)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(_heading, _guiStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Horizontal line separation on EditorWindow GUI
        /// </summary>
        void HorizontalLineSeparation()
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Space(100);
            GUILayout.Box(GUIContent.none, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
            GUILayout.Space(100);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        static void AddGazeTimer()
        {
            GazeTimer gt = FindObjectOfType<GazeTimer>();
            if (gt == null)
            {
                GameObject gazeTimer = new GameObject();
                gazeTimer.AddComponent<GazeTimer>();
                gazeTimer.name = "GazeTimer";

                GameObject te = GetTechXREssentialGO();
                gazeTimer.transform.SetParent(te.transform);
            }
        }

        /// <summary>
        /// Update the Dropdown List of Environment prefabs present in the Given folder
        /// </summary>
        static void FillEnvironmentOptions()
        {
            environmentFileInfo = environmentDir.GetFiles("*.*");
            environmentOptions = new string[environmentFileInfo.Length];

            int index = 0;
            foreach (FileInfo f in environmentFileInfo)
            {
                environmentOptions[index] = f.Name.Split('.')[0];
                index++;
            }
        }

        /// <summary>
        /// Update the Dropdown List of UI Elements present in the Given folder
        /// </summary>
        void FillUIElementOptions()
        {
            uiElementFilesInfo = uiElementDir.GetFiles("*.*");
            uiElementOptions = new string[uiElementFilesInfo.Length];

            int index = 0;
            foreach (FileInfo f in uiElementFilesInfo)
            {
                uiElementOptions[index] = f.Name.Split('.')[0];
                index++;
            }
        }

        /// <summary>
        /// Update the Dropdown List of 3D Models present in the Given folder
        /// </summary>
        void FillModelOptions()
        {
            modelFilesInfo = modelsDir.GetFiles("*.*");
            modelOptions = new string[modelFilesInfo.Length];

            int index = 0;
            foreach (FileInfo f in modelFilesInfo)
            {
                modelOptions[index] = f.Name.Split('.')[0];
                index++;
            }
        }

        /// <summary>
        /// Update the Dropdown List of Skybox materials present in the Given folder
        /// </summary>
        void FillSkyboxOptions()
        {
            skyboxFilesInfo = skyboxDir.GetFiles("*.*");
            skyboxOptions = new string[skyboxFilesInfo.Length / 2];

            int index = 0;
            foreach (FileInfo f in skyboxFilesInfo)
            {
                if (f.Name.Contains(".mat.")) continue;

                skyboxOptions[index] = f.Name.Split('.')[0];
                index++;
            }
        }

        /// <summary>
        /// Add the Camera to the scene
        /// </summary>
        private static void AddSenseCamera()
        {

            // Destroy All cameras
            Camera[] cameras = FindObjectsOfType<Camera>();
            foreach (Camera cam in cameras)
                DestroyImmediate(cam.gameObject);

            // Instantiate the SenseCamera
            UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath("Assets/TechXR/Prefabs/SenseCamera.prefab", typeof(GameObject));
            var xrcam = Instantiate(prefab) as GameObject;
            xrcam.name = "SenseCamera";

            // SetUp Clear Flags
            if (tabs[selectedTab].Contains("VR"))
            {
                xrcam.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
            }
            else if (tabs[selectedTab].Contains("AR"))
            {
                xrcam.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            }

            // Focus
            Selection.activeGameObject = xrcam;
            SceneView.lastActiveSceneView.FrameSelected();


        }


        /// <summary>
        /// Check if eventsystem is not present in the scene and add
        /// </summary>
        private static void AddEventSystem()
        {
            GameObject eventSystem = GameObject.FindGameObjectWithTag("SenseEventSystem");
            if (eventSystem != null) return;

            UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath("Assets/TechXR/Prefabs/SenseEventSystem.prefab", typeof(GameObject));
            var es = Instantiate(prefab) as GameObject;
            es.name = "SenseEventSystem";

            GameObject te = GetTechXREssentialGO();
            es.transform.SetParent(te.transform);
            AddGazeTimer();
        }

        /// <summary>
        /// Add the Floor prefab for VR Scene
        /// </summary>
        private static void AddEnvironmentForVR()
        {
            FillEnvironmentOptions();
            UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath("Assets/TechXR/Prefabs/Environments/Plane" + ".prefab", typeof(GameObject));
            var env = Instantiate(prefab) as GameObject;
            env.name = environmentOptions[environmentIndex];

            GameObject te = GetTechXREssentialGO();
            env.transform.SetParent(te.transform);
        }

        /// <summary>
        /// Add the Floor prefab
        /// </summary>
        private static void AddEnvironment()
        {
            FillEnvironmentOptions();
            UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath("Assets/TechXR/Prefabs/Environments/" + environmentOptions[environmentIndex] + ".prefab", typeof(GameObject));
            var env = Instantiate(prefab) as GameObject;
            env.name = environmentOptions[environmentIndex];

            GameObject te = GetTechXREssentialGO();
            env.transform.SetParent(te.transform);
        }

        /// <summary>
        /// Set Skybox
        /// </summary>
        private static void SetSkybox()
        {
            Material m = AssetDatabase.LoadAssetAtPath("Assets/TechXR/Skybox/Mat/" + skyboxOptions[skyboxIndex] + ".mat", typeof(Material)) as Material;

            RenderSettings.skybox = m;
        }

        /// <summary>
        /// Add Directional Light
        /// </summary>
        private static void AddDirectionalLight()
        {
            // If Directional light is there then return
            Light dl = GameObject.FindObjectOfType<Light>();
            if (dl) if (dl.type == LightType.Directional) return;

            // If Directional light is not there create new object and give the name
            GameObject lightGameObject = new GameObject("Directional Light");

            // Assign The transform
            lightGameObject.transform.position = new Vector3(0, 3, 0);
            lightGameObject.transform.eulerAngles = new Vector3(50, -30, 0);

            // Add light component and do the settings
            Light lightComp = lightGameObject.AddComponent<Light>();
            lightComp.type = LightType.Directional;
            lightComp.color = new Color(255f / 255f, 244f / 255f, 214f / 255f);
            lightComp.shadows = LightShadows.Soft;
        }

        /// <summary>
        /// Add Canvas
        /// </summary>
        /// <param name="canvasName"></param>
        public static void AddCanvas(string canvasName)
        {
            UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath("Assets/TechXR/Prefabs/UIElements/UI/" + canvasName + ".prefab", typeof(GameObject));
            var xrcanvas = Instantiate(prefab) as GameObject;
            xrcanvas.name = canvasName;

            // Check and assign the MainCamera (Sense Camera) to the Canvas - EventCamera
            if (Camera.main)
                xrcanvas.GetComponent<Canvas>().worldCamera = Camera.main;
            else
                Debug.LogWarning("TechXR : Add SenseCamera to the scene, and assign the SenseCamera to the EventCamera of the Canvas component");

            // Focus
            Selection.activeGameObject = xrcanvas;
            SceneView.lastActiveSceneView.FrameSelected();

            //CheckEventSystem();
        }


        /// <summary>
        /// Get the TechXREssential gameobject
        /// </summary>
        /// <returns></returns>
        private static GameObject GetTechXREssentialGO()
        {
            GameObject go = GameObject.Find("TechXREssentials");
            if (go) return go;

            go = new GameObject("TechXREssentials");
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;

            return go;
        }

        public static void AddDeveloperCube(string cubeType)
        {
            GameObject _go = GameObject.FindWithTag("TechXRDeveloperCube");
            if (_go) return;

            UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath("Assets/TechXR/TechXR_DeveloperCube/Prefabs/DeveloperCube" + ".prefab", typeof(GameObject));
            var devCube = Instantiate(prefab) as GameObject;


            // turn off inside/outside view
            foreach (string s in arControllerOptions)
            {
                Transform go = devCube.transform.Find(s);
                if (go) go.gameObject.SetActive(false);
            }

            // if intractable cube is selected add laser pointer and input module
            if (cubeType == "Intractable")
            {
                // add laserpointer
                LaserPointer laserPointer = devCube.AddComponent<LaserPointer>();

                if (!laserPointer)
                    laserPointer = devCube.AddComponent<LaserPointer>();

                // Set Color
                laserPointer.Color = Color.white;

                // laserpointerinputmodule
                AddEventSystem();

                // add intractable sphere
                UnityEngine.Object i_sphere = AssetDatabase.LoadAssetAtPath("Assets/TechXR/Prefabs/Intractable Cube/Intractable.prefab", typeof(GameObject));
                var sphere = Instantiate(i_sphere) as GameObject;
                sphere.transform.SetParent(devCube.transform);
                sphere.transform.localPosition = Vector3.zero;
            }
            else
            {
                // turn on inside/outside view
                devCube.transform.Find(cubeType).gameObject.SetActive(true);

                // remove the sphere
                GameObject i_Sphere = GameObject.FindWithTag("ISphere");
                if (i_Sphere) DestroyImmediate(i_Sphere);
            }

            // Focus
            Selection.activeGameObject = devCube;
            SceneView.lastActiveSceneView.FrameSelected();

            AddDirectionalLight();
        }


        /// <summary>
        /// Create Layer
        /// </summary>
        /// <param name="name"></param>
        public static void CreateLayer(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentNullException("name", "New layer name string is either null or empty.");

            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layerProps = tagManager.FindProperty("layers");
            var propCount = layerProps.arraySize;

            SerializedProperty firstEmptyProp = null;

            for (var i = 0; i < propCount; i++)
            {
                var layerProp = layerProps.GetArrayElementAtIndex(i);

                var stringValue = layerProp.stringValue;

                if (stringValue == name) return;

                if (i < 8 || stringValue != string.Empty) continue;

                if (firstEmptyProp == null)
                    firstEmptyProp = layerProp;
            }

            if (firstEmptyProp == null)
            {
                UnityEngine.Debug.LogError("Maximum limit of " + propCount + " layers exceeded. Layer \"" + name + "\" not created.");
                return;
            }

            firstEmptyProp.stringValue = name;
            tagManager.ApplyModifiedProperties();

            Debug.Log(name + " Layer is Successfully added to your project..!!");
        }

        /// <summary>
        /// Add layers and tags
        /// </summary>
        private static void AddLayersAndTags()
        {
            AddLayersAndTags(m_Layers, m_Tags);
        }

        /// <summary>
        /// Add the layers and tags from the passed lists
        /// </summary>
        /// <param name="layers"></param>
        /// <param name="tags"></param>
        private static void AddLayersAndTags(List<string> layers, List<string> tags)
        {
            // add layers
            foreach (var item in layers)
            {
                CreateLayer(item);
            }

            // add tags
            foreach (var item in tags)
            {
                AddTag(item);
            }
        }

        /// <summary>
        /// Add tag to the tag list
        /// </summary>
        /// <param name="tagName"></param>
        private static void AddTag(string tagName)
        {
            // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            // Tags Property
            SerializedProperty tagsProp = tagManager.FindProperty("tags");
            if (tagsProp.arraySize >= MAX_TAGS)
            {
                Debug.Log("No more tags can be added to the Tags property. You have " + tagsProp.arraySize + " tags");
            }
            // if not found, add it
            if (!PropertyExists(tagsProp, 0, tagsProp.arraySize, tagName))
            {
                int index = tagsProp.arraySize;
                // Insert new array element
                tagsProp.InsertArrayElementAtIndex(index);
                SerializedProperty sp = tagsProp.GetArrayElementAtIndex(index);
                // Set array element to tagName
                sp.stringValue = tagName;
                Debug.Log("Tag: " + tagName + " has been added");
                // Save settings
                tagManager.ApplyModifiedProperties();
            }
            else
            {
                //Debug.Log("Tag: " + tagName + " already exists");
            }
        }


        /// <summary>
        /// Checks if the value exists in the property.
        /// </summary>
        /// <returns><c>true</c>, if exists was propertyed, <c>false</c> otherwise.</returns>
        /// <param name="property">Property.</param>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        /// <param name="value">Value.</param>
        private static bool PropertyExists(SerializedProperty property, int start, int end, string value)
        {
            for (int i = start; i < end; i++)
            {
                SerializedProperty t = property.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }
    }
}