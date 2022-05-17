using UnityEngine;
using UnityEditor;
using TechXR.Core.Sense;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor.AI;

namespace TechXR.Core.Editor
{
    public class TechXR : EditorWindow
    {
        // License Key
        static FixedExpiryLicense fixedExpiryLicense;
        static bool IsLicenseKeyValid = false;

        // GUI Styling
        private GUIStyle guiStyle = new GUIStyle();

        // Dropdown Controller Body Options 
        string[] controllerBodyOptions = { "Default", "Hand" };
        static int controllerBodyIndex = 0;

        // Dropdown Player Character Options
        static string[] playerCharacterOptions = { "None", "Default" };
        static int playerCharacterIndex = 0;

        // Dropdown Player Character Options
        static string[] arControllerOptions = { "InsideView", "OutsideView" };
        static int arControllerIndex = 0;

        // Dropdown Environment Options
        static int environmentIndex;
        static string[] environmentOptions;

        // Dropdown Canvas Type Options
        static int uiElementIndex;
        string[] uiElementOptions;

        // Dropdown 3D Model Options
        static int modelIndex;
        string[] modelOptions;

        // Dropdown Skybox Options
        static int skyboxIndex;
        static string[] skyboxOptions;

        // Variable for Image
        Texture2D controllerBody;
        Texture2D uiElement;

        // Vector2 Variable for Scroller Position
        Vector2 scrollerPosition;

        // Tab System fields
        private GUISkin skin;
        private GUIStyle tabStyle;
        static private int selectedTab = 0;
        static string[] tabs = { "AR", "VR", "FPS" };
        GUISkin originalGUISkin;

        //
        GUIManager gUIManager;
        static LayersAndTagsManager m_LayersAndTagsManager;
        static PopulateGUIFields m_PopulateGUIFields;
        static TechXRSceneManager m_TechXRSceneManager;
        //

        // OnEnable will call everytime when window will open
        private void OnEnable()
        {
            //
            gUIManager = new GUIManager();
            m_LayersAndTagsManager = new LayersAndTagsManager();
            m_PopulateGUIFields = new PopulateGUIFields();
            m_TechXRSceneManager = new TechXRSceneManager();
            //

            // save references to skin and style
            skin = (GUISkin)Resources.Load("CustomSkin");
            tabStyle = skin.GetStyle("Tab");

            // Update Layers and Tags
            m_LayersAndTagsManager.UpdateLayersAndTags();

            // Update the dropdown lists
            environmentOptions = m_PopulateGUIFields.PopulateEnvironmentOptions();
            uiElementOptions = m_PopulateGUIFields.PopulateUIElementOptions();
            modelOptions = m_PopulateGUIFields.PopulateModelOptions();
            skyboxOptions = m_PopulateGUIFields.PopulateSkyboxOptions();
        }

        [MenuItem("TechXR/Preferences")]
        public static void ShowWindow()
        {
            TechXR window = (TechXR)EditorWindow.GetWindow<TechXR>("TechXR Sense Action Panel");
            window.minSize = new Vector2(500, 400);
            window.maxSize = new Vector2(600, 620);

            // Check License
            fixedExpiryLicense = new FixedExpiryLicense();
            IsLicenseKeyValid = fixedExpiryLicense.VerifyLicenseKey(TechXRConfiguration.Instance.LicenseKey);
            if (!IsLicenseKeyValid) OpenDevKeyWindow();

            // Give path to the TechXR Icon here
            //var texture = Resources.Load<Texture>("Icon/TechXR");
            //window.titleContent = new GUIContent("Nice icon, eh?", texture, "Just the tip");
        }

        [MenuItem("TechXR/Set-Up VR Scene")]
        public static void SetUpVRScene()
        {
            // Check License
            fixedExpiryLicense = new FixedExpiryLicense();
            IsLicenseKeyValid = fixedExpiryLicense.VerifyLicenseKey(TechXRConfiguration.Instance.LicenseKey);

            if (!IsLicenseKeyValid) OpenDevKeyWindow();

            else if (EditorUtility.DisplayDialog("Warning...!!", "TechXR-Default VR Scene will be loaded, it may remove the Existing Camera, XRPlayers, XRController and DeveloperCube.", "Ok", "Close"))
            {

                // Reset Dropdown Selection
                controllerBodyIndex = 0;
                playerCharacterIndex = 0;
                environmentIndex = 0;

                // Make instances
                m_TechXRSceneManager = new TechXRSceneManager();
                m_LayersAndTagsManager = new LayersAndTagsManager();
                m_PopulateGUIFields = new PopulateGUIFields();

                // Add Layers & Tags
                m_LayersAndTagsManager.UpdateLayersAndTags();

                // Delete TechXR-Environments
                GameObject[] envs = GameObject.FindGameObjectsWithTag("Env");
                foreach (GameObject e in envs)
                    DestroyImmediate(e);

                // Add Items
                Dictionary<string, Action> functionMap = new Dictionary<string, Action>()
                {
                    { "XRPlayerController", AddXRPlayerController_SetUpScene },
                    { "Environment", AddEnvironment_SetUpScene },
                    { "DirectionalLight", m_TechXRSceneManager.AddDirectionalLight}
                };

                foreach (var item in functionMap)
                {
                    item.Value();
                }

                // Camera settings
                Camera.main.clearFlags = CameraClearFlags.Skybox;

                // Notify
                foreach (SceneView scene in SceneView.sceneViews)
                {
                    scene.ShowNotification(new GUIContent("Updated..!!"));
                }

                // Mark Scene Dirty
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }

        private static void AddEnvironment_SetUpScene()
        {
            environmentOptions = m_PopulateGUIFields.PopulateEnvironmentOptions();
            m_TechXRSceneManager.AddEnvironment(environmentOptions, 0, "");
        }

        private static void AddXRPlayerController_SetUpScene()
        {
            m_TechXRSceneManager.AddXRPlayerController(playerCharacterOptions, 0, 0);
        }

        [MenuItem("TechXR/Set-Up AR Scene")]
        public static void SetUpARScene()
        {
            // Check License
            fixedExpiryLicense = new FixedExpiryLicense();
            IsLicenseKeyValid = fixedExpiryLicense.VerifyLicenseKey(TechXRConfiguration.Instance.LicenseKey);

            if (!IsLicenseKeyValid) OpenDevKeyWindow();

            else if (EditorUtility.DisplayDialog("Warning...!!", "TechXR-Default AR Scene will be loaded, it may remove the Existing TechXR-Environment, XRPlayer, XRController, DeveloperCube and Cameras.", "Ok", "Close"))
            {
                // Reset Dropdown Selection
                controllerBodyIndex = 0;
                playerCharacterIndex = 0;
                environmentIndex = 0;

                // Add Layers & Tags
                m_LayersAndTagsManager.UpdateLayersAndTags();

                // Delete all un-necessary things for AR camera configuration
                GameObject[] grounds = GameObject.FindGameObjectsWithTag("Env");
                foreach (GameObject ground in grounds)
                    DestroyImmediate(ground);

                GameObject[] players = GameObject.FindGameObjectsWithTag("XRPlayerController");
                foreach (GameObject player in players)
                    DestroyImmediate(player);

                // Add Items
                m_TechXRSceneManager.AddSenseCamera("AR");
                m_TechXRSceneManager.AddController(0);
                m_TechXRSceneManager.AddSenseManager();
                m_TechXRSceneManager.AddDirectionalLight();

                // Camera settings
                Camera.main.clearFlags = CameraClearFlags.SolidColor;

                // Notify
                foreach (SceneView scene in SceneView.sceneViews)
                {
                    scene.ShowNotification(new GUIContent("Updated..!!"));
                }

                // Mark Scene Dirty
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }

        /*
        [MenuItem("TechXR/Set-Up FPS Game")]
        public static void SetUpFPSScene()
        {
            if (EditorUtility.DisplayDialog("Warning...!!", "TechXR-Default FPS Scene will be loaded, it may remove active GameObjects from your scene. Make a new scene or Click on Set-Up", "Set-Up", "Close"))
            {
                // Remove all the Items from Scene
                GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
                foreach (UnityEngine.Object go in allObjects)
                    DestroyImmediate(go);

                // Instantiate the FPS prefab
                UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath("Assets/TechXR/FPS/Prefabs/FPS.prefab", typeof(GameObject));
                var fps = Instantiate(prefab) as GameObject;
                fps.name = "FPS";

                // Set the Skybox Material
                Material m = AssetDatabase.LoadAssetAtPath("Assets/TechXR/FPS/Skybox/Material/FPS.mat", typeof(Material)) as Material;
                RenderSettings.skybox = m;

                // Notify
                foreach (SceneView scene in SceneView.sceneViews)
                {
                    scene.ShowNotification(new GUIContent("Updated..!!"));
                }

                // Mark Scene Dirty
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }
        */


        /*// Item ports the current scene to the new oculus scene if oculus sdk is imported, the function was made only for testing, need to repair a lot
        [MenuItem("TechXR/Port To Oculus")]
        static void PortToOculus()
        {
            // Duplicate Scene
            string[] path = EditorApplication.currentScene.Split(char.Parse("/"));
            path[path.Length - 1] = "Oculus_" + path[path.Length - 1];
            EditorApplication.SaveScene(string.Join("/", path), true);
            Debug.Log("Saved Scene");

            // Open Scene
            EditorApplication.OpenScene(string.Join("/", path));

            // Add scene to Build Setting
            var original = EditorBuildSettings.scenes;
            var newSettings = new EditorBuildSettingsScene[original.Length + 1];
            System.Array.Copy(original, newSettings, original.Length);
            var sceneToAdd = new EditorBuildSettingsScene(string.Join("/", path), true);
            newSettings[newSettings.Length - 1] = sceneToAdd;
            EditorBuildSettings.scenes = newSettings;

            // Instantiate the Oculus PlayerPrefab
            UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Oculus/VR/Prefabs/OVRPlayerController.prefab", typeof(GameObject));
            GameObject player = Instantiate(prefab) as GameObject;
            Vector3 pos = player.transform.position;
            pos.y = 1f;
            player.transform.position = pos;
            player.name = "OVRPlayerController";

            // Move the Container
            GameObject container = GameObject.Find("SenseContainer");
            container.transform.parent = GameObject.Find("RightHandAnchor").transform;
            container.transform.localPosition = Vector3.zero;

            // Add Controller Prefabs
            GameObject lha = GameObject.Find("LeftHandAnchor");
            GameObject rha = GameObject.Find("RightHandAnchor");

            UnityEngine.Object lc = AssetDatabase.LoadAssetAtPath("Assets/Oculus/VR/Meshes/OculusTouchForQuest2/OculusTouchForQuest2_Left.fbx", typeof(GameObject));
            GameObject left = Instantiate(lc) as GameObject;
            UnityEngine.Object rc = AssetDatabase.LoadAssetAtPath("Assets/Oculus/VR/Meshes/OculusTouchForQuest2/OculusTouchForQuest2_Right.fbx", typeof(GameObject));
            GameObject right = Instantiate(rc) as GameObject;

            left.transform.SetParent(lha.transform);
            right.transform.SetParent(rha.transform);

            Vector3 defaultpos = Vector3.zero;
            left.transform.localPosition = defaultpos;
            right.transform.localPosition = defaultpos;

            // Destroy
            DestroyImmediate(GameObject.FindGameObjectWithTag("XRPlayerController"));
        }
        */

        [MenuItem("TechXR/Configuration")]
        public static void ShowTechXRConfiguration()
        {
            // If config file exists then open the file else create a new one and open 
            if (File.Exists("Assets/Resources/TechXRConfiguration.asset"))
            {
                // Open the ConfigurationFile
                AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath("Assets/Resources/TechXRConfiguration.asset"));
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
            originalGUISkin = GUI.skin; // store original gui skin
            GUI.skin = skin; // use our custom skin

            // Create toolbar using custom tab style
            selectedTab = GUILayout.Toolbar(selectedTab, tabs, tabStyle);

            // Set Back original skin
            GUI.skin = originalGUISkin;

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

            // Check For License Key
            IsLicenseKeyValid = fixedExpiryLicense.VerifyLicenseKey(TechXRConfiguration.Instance.LicenseKey);

            // Disable Group if key is Invalid
            EditorGUI.BeginDisabledGroup(!IsLicenseKeyValid);

            // Space
            GUILayout.Space(7);

            float originalValue = EditorGUIUtility.labelWidth;


            // ================== AR Tab ======================
            if (tabs[selectedTab].Contains("AR"))
            {
                GUILayout.Space(10);


                // Show XRCamera
                ShowXRCamera("AR");


                GUILayout.Space(10);


                // Horizontal Line Separation
                gUIManager.HorizontalLineSeparation();


                // Show Controller
                ShowController();


                // Check for controller
                SenseController senseController = GameObject.FindObjectOfType<SenseController>();
                // Check for DeveloperCube
                GameObject developerCube = GameObject.FindGameObjectWithTag("TechXRDeveloperCube");
                //


                // Horizontal Line Separation
                gUIManager.HorizontalLineSeparation();


                GUILayout.Space(10);


                // Show Developer Cube
                ShowDeveloperCube();


                // Horizontal Line Separation
                gUIManager.HorizontalLineSeparation();


                // Show Assets
                ShowAssets();


                GUILayout.Space(10);

            }
            // ================== AR Tab ======================

            // ==================VR Tab========================
            if (tabs[selectedTab].Contains("VR"))
            {
                GUILayout.Space(10);


                // Show XRPlayer
                ShowXRPlayer();



                // Horizontal Line Separation
                gUIManager.HorizontalLineSeparation();


                // Show Configure VR Scene
                ShowConfigureVRScene();


                // Horizontal Line Separation
                gUIManager.HorizontalLineSeparation();


                // Show Assets
                ShowAssets();


                GUILayout.Space(10);
            }
            //==================VR Tab========================

            // ==================FPS Tab========================
            if (tabs[selectedTab].Contains("FPS"))
            {

                GUILayout.Space(10);
                // Show Heading
                gUIManager.ShowHeading("FPS Skybox", guiStyle);

                // Show Skybox
                ShowSkybox();

              

                // Horizontal Line Separation
                gUIManager.HorizontalLineSeparation();

                GUILayout.Space(10);

                // Show Heading
                gUIManager.ShowHeading("FPS Environment", guiStyle);

                // Show Environment
                ShowEnvironment("FPS");
                ////https://drive.google.com/uc?export=download&id=1OoXMZlXDtxj4wh8cx-2CKog2XSVqakJg
                //UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath("Assets/TechXR/Sprites/Icon/OutsideView.png", typeof(Texture2D));
                //controllerBody = obj as Texture2D;
                //GUILayout.Label(controllerBody, GUILayout.Width(80), GUILayout.Height(80));

                // Horizontal Line Separation
                gUIManager.HorizontalLineSeparation();

                GUILayout.Space(20);

                // Show FPSPlayer
                ShowFPSPlayer();

                

                GUILayout.Space(10);


                // Horizontal Line Separation
                gUIManager.HorizontalLineSeparation();


                GUILayout.Space(10);


                // Show Nav Mesh
                ShowNavMesh();


                GUILayout.Space(10);


                // Horizontal Line Separation
                gUIManager.HorizontalLineSeparation();


                GUILayout.Space(10);


                // Show Enemy Manager
                ShowEnemyManager();

            }


            // End License Key Disabled Group Section
            EditorGUI.EndDisabledGroup();


            GUILayout.EndScrollView(); // End Scroll View


            // Show Logo and Link
            ShowLogoWithLink();


            GUILayout.Space(10);

           
            GUILayout.EndVertical(); // End Vertical Window design

        }


        /// <summary>
        /// Open Dev Key Window
        /// </summary>
        public static void OpenDevKeyWindow()
        {
            SetDevKey devKeyWindow = (SetDevKey)EditorWindow.GetWindow<SetDevKey>("DevKey Window");
        }


        /// <summary>
        /// Show Logo of TechXR With the Link
        /// </summary>
        void ShowLogoWithLink()
        {
            // Get Image
            Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/TechXR/Logo/file.png", typeof(Texture));

            // Display Image
            GUILayout.Box(banner, skin.GetStyle("Logo"));
            GUILayout.Space(10);


            // Link to TechXR Store
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("To Pre-Book your TechXR 6DoF Controller (SenseXR) Click");
            if (GUILayout.Button("Here", skin.GetStyle("HyperButton")))
            {
                Application.OpenURL("https://forms.gle/jxNumHU3ZwVZVwGi6");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        

        /// <summary>
        /// Function to Show UIElements on Editor GUI Window
        /// </summary>
        void ShowUIElements()
        {
            // ------------------------ DROPDOWN -------------------------
            GUILayout.BeginHorizontal();
            GUILayout.Space(60);
            if (uiElementOptions.Length <= 0) uiElementOptions = m_PopulateGUIFields.PopulateUIElementOptions();
            uiElementIndex = EditorGUILayout.Popup("Select UI Element : ", uiElementIndex, uiElementOptions, GUILayout.Width(300)); // Dropdown
            GUILayout.Space(80);
            if (uiElementIndex >= 0)
            {
                GUILayout.BeginVertical();
                // ------------------------ BUTTON -------------------------
                if (GUILayout.Button("Add", GUILayout.Width(75)))
                {
                    // Add and Notify
                    m_TechXRSceneManager.AddCanvas(uiElementOptions[uiElementIndex]);
                    this.ShowNotification(new GUIContent("Updated..!!"));

                    // Mark Scene Dirty
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
                GUILayout.EndVertical();
            }
            GUILayout.Space(20);
            GUILayout.EndHorizontal();
        }


        /// <summary>
        /// Function to Show 3D Models Option on Editor GUI Window
        /// </summary>
        void Objects3D()
        {
            // ------------------------ DROPDOWN -------------------------
            GUILayout.BeginHorizontal();
            GUILayout.Space(60);
            if (modelOptions.Length <= 0) modelOptions = m_PopulateGUIFields.PopulateModelOptions();
            modelIndex = EditorGUILayout.Popup("Select 3D Model : ", modelIndex, modelOptions, GUILayout.Width(300)); // Dropdown
            GUILayout.Space(80);
            if (modelIndex >= 0)
            {
                GUILayout.BeginVertical();
                //---------------------------------BUTTON---------------------------------
                if (GUILayout.Button("Add", GUILayout.Width(75))) // Button
                {
                    // Find Prefab
                    UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath("Assets/TechXR/3DModels/" + modelOptions[modelIndex] + "/" + "model.obj", typeof(GameObject));
                    if (!prefab)
                    {
                        prefab = AssetDatabase.LoadAssetAtPath("Assets/TechXR/3DModels/" + modelOptions[modelIndex] + "/" + modelOptions[modelIndex] + ".obj", typeof(GameObject));
                    }
                    // Instantiate Object
                    GameObject obj = Instantiate(prefab) as GameObject;
                    // Focus
                    Selection.activeGameObject = obj;
                    SceneView.lastActiveSceneView.FrameSelected();
                    // Notify
                    this.ShowNotification(new GUIContent("Updated..!!"));

                    // Mark Scene Dirty
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }

                GUILayout.EndVertical();
            }
            GUILayout.Space(20);
            GUILayout.EndHorizontal();
        }


        /// <summary>
        /// Function to Show Assets on Editor GUI Window
        /// </summary>
        void ShowAssets()
        {
            // Heading
            gUIManager.ShowHeading("Assets", guiStyle);


            GUILayout.Space(15);


            // Show UI Elements Options
            ShowUIElements();


            GUILayout.Space(15);


            // Show 3D Models Options
            Objects3D();
        }


        /// <summary>
        /// Function to Environment options on Editor GUI Window
        /// </summary>
        void ShowEnvironment(string mode)
        {
            float originalValue = EditorGUIUtility.labelWidth;

            // ------------------------ DROPDOWN -------------------------
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (environmentOptions.Length <= 0) environmentOptions = m_PopulateGUIFields.PopulateEnvironmentOptions();
            //
            EditorGUIUtility.labelWidth = 100;
            GUILayout.Label("Environment : ");
            EditorGUIUtility.labelWidth = originalValue;
            //
            environmentIndex = EditorGUILayout.Popup("", environmentIndex, environmentOptions); // DROPDOWN
            // ------------------------ BUTTON -------------------------
            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {
                GameObject[] envs = GameObject.FindGameObjectsWithTag("Env");

                // If Environment is there Disply Warning if yes then Add the same ; else Add
                if (envs.Length > 0)
                {
                    if (EditorUtility.DisplayDialog("Warning...!!", "TechXR-Environment already present", "Delete Existing and Add new one", "Close"))
                    {
                        // Destroy all
                        foreach (GameObject e in envs)
                            DestroyImmediate(e);

                        // Add Environment and Notify
                        environmentOptions = m_PopulateGUIFields.PopulateEnvironmentOptions();
                        m_TechXRSceneManager.AddEnvironment(environmentOptions, environmentIndex, mode);
                        this.ShowNotification(new GUIContent("Updated..!!"));

                        // Mark Scene Dirty
                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    }
                }
                else
                {
                    // Add Env and Notify
                    environmentOptions = m_PopulateGUIFields.PopulateEnvironmentOptions();
                    m_TechXRSceneManager.AddEnvironment(environmentOptions, environmentIndex, mode);
                    this.ShowNotification(new GUIContent("Updated..!!"));

                    // Mark Scene Dirty
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
                // Add Directional Light
                m_TechXRSceneManager.AddDirectionalLight();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


        /// <summary>
        /// Function to Show Skybox options on Editor GUI Window
        /// </summary>
        void ShowSkybox()
        {
            float originalValue = EditorGUIUtility.labelWidth;

            // ------------------------ DROPDOWN -------------------------
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (skyboxOptions.Length <= 0) skyboxOptions = m_PopulateGUIFields.PopulateSkyboxOptions();
            EditorGUIUtility.labelWidth = 100;
            GUILayout.Label("Skybox          : ");
            EditorGUIUtility.labelWidth = originalValue;
            skyboxIndex = EditorGUILayout.Popup("", skyboxIndex, skyboxOptions); // DROPDOWN


            //---------------------------------BUTTON---------------------------------
            if (GUILayout.Button("Set", GUILayout.Width(60)))
            {
                // Set Skybox and notify
                m_TechXRSceneManager.SetSkybox(skyboxOptions, skyboxIndex);
                this.ShowNotification(new GUIContent("Updated..!!"));

                // Mark Scene Dirty
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


        /// <summary>
        /// Function to Show XR Controller Attachment option on Editor GUI Window
        /// </summary>
        void XRControllerAttachmentFeature()
        {
            float originalValue = EditorGUIUtility.labelWidth;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            //
            EditorGUIUtility.labelWidth = 100;
            GUILayout.Label("XR Controller Attachment Feature :           ");
            EditorGUIUtility.labelWidth = originalValue;
            //
            SenseXRTrackingStatus trackingStatus = FindObjectOfType<SenseXRTrackingStatus>();
            EditorGUI.BeginDisabledGroup(trackingStatus);

            //---------------------------------BUTTON---------------------------------
            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {
                GameObject pc = GameObject.FindWithTag("XRPlayerController");
                if (!pc)
                {
                    if (EditorUtility.DisplayDialog("Warning...!!", "TechXR :: No XRPlayer in the Scene , First add the XRPlayer", "Ok", "Close")) { }
                    Debug.Log("TechXR :: No XRPlayer in the Scene , First add the XRPlayer");
                }
                else
                {
                    // Add and Notify
                    m_TechXRSceneManager.AddXRControllerAttachmentFeature();
                    this.ShowNotification(new GUIContent("Updated..!!"));

                    // Mark Scene Dirty
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
            }
            EditorGUI.EndDisabledGroup();
            //
            EditorGUI.BeginDisabledGroup(!trackingStatus);

            //---------------------------------BUTTON---------------------------------
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                GameObject pc = GameObject.FindWithTag("XRPlayerController");
                if (!pc)
                {
                    if (EditorUtility.DisplayDialog("Warning...!!", "TechXR :: No XRPlayer in the Scene , First add the XRPlayer", "Ok", "Close")) { }
                    Debug.Log("TechXR :: No XRPlayer in the Scene , First add the XRPlayer");
                }
                else
                {
                    // Remove and Notify
                    m_TechXRSceneManager.RemoveXRControllerAttachmentFeature();
                    this.ShowNotification(new GUIContent("Updated..!!"));

                    // Mark Scene Dirty
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


        /// <summary>
        /// Function to Show Bluetooth Warning System option on Editor GUI Window
        /// </summary>
        void ShowBluetoothWarningSystem()
        {
            float originalValue = EditorGUIUtility.labelWidth;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            //
            EditorGUIUtility.labelWidth = 100;
            GUILayout.Label("Controller Bluetooth Warning System :     ");
            EditorGUIUtility.labelWidth = originalValue;
            //
            SenseXRConnectivityStatus bt = FindObjectOfType<SenseXRConnectivityStatus>();
            EditorGUI.BeginDisabledGroup(bt);

            //---------------------------------BUTTON---------------------------------
            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {
                GameObject sc = GameObject.FindWithTag("SenseController");
                if (!sc)
                {
                    if (EditorUtility.DisplayDialog("Warning...!!", "TechXR :: No XRController in the Scene , First add the XRController/XRPlayer", "Ok", "Close")) { }
                    Debug.Log("TechXR :: No XRController in the Scene , First add the XRController/XRPlayer");
                }
                else
                {
                    // Add and Notify
                    m_TechXRSceneManager.AddBluetoothWarningSystem();
                    this.ShowNotification(new GUIContent("Updated..!!"));

                    // Mark Scene Dirty
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
            }
            EditorGUI.EndDisabledGroup();
            //
            EditorGUI.BeginDisabledGroup(!bt);

            //---------------------------------BUTTON---------------------------------
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                GameObject sc = GameObject.FindWithTag("SenseController");
                if (!sc)
                {
                    if (EditorUtility.DisplayDialog("Warning...!!", "TechXR :: No XRController in the Scene , First add the XRController/XRPlayer", "Ok", "Close")) { }
                    Debug.Log("TechXR :: No XRController in the Scene , First add the XRController/XRPlayer");
                }
                else
                {
                    // Add and Notify
                    m_TechXRSceneManager.RemoveBluetoothWarningSystem();
                    this.ShowNotification(new GUIContent("Updated..!!"));

                    // Mark Scene Dirty
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Function to Show Configure VR Scene options on Editor GUI Window
        /// </summary>
        void ShowConfigureVRScene()
        {
            // Heading
            gUIManager.ShowHeading("Configure VR Scene", guiStyle);


            // Space
            GUILayout.Space(15);


            // Show Environment
            ShowEnvironment("VR");


            // Space
            GUILayout.Space(10);


            // Show Skybox
            ShowSkybox();


            // Space
            GUILayout.Space(10);


            // Show XR Controller Attachment Feature
            XRControllerAttachmentFeature();


            // Space
            GUILayout.Space(10);


            // Show Bluetooth Warning System
            ShowBluetoothWarningSystem();

        }

        /// <summary>
        /// Function to Show XRPlayer Component on Editor GUI Window
        /// </summary>
        void ShowXRPlayer()
        {

            // Heading
            gUIManager.ShowHeading("XR Player", guiStyle);

            // Space
            GUILayout.Space(20);


            // Check for the player controller
            GameObject playerController = GameObject.FindWithTag("XRPlayerController");

            //--------------------------DROPDOWN----------------------------
            // Player Body Options
            GUILayout.BeginHorizontal();
            GUILayout.Space(80);
            // Start a code block to check for GUI changes
            EditorGUI.BeginChangeCheck();
            playerCharacterIndex = EditorGUILayout.Popup("Character Type : ", playerCharacterIndex, playerCharacterOptions); // DROPDOWN
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


            //--------------------------DROPDOWN----------------------------
            // Controller Body Options
            GUILayout.BeginHorizontal();
            GUILayout.Space(80);
            // Check for controller
            SenseController senseController = GameObject.FindObjectOfType<SenseController>();
            // Start a code block to check for GUI changes
            EditorGUI.BeginChangeCheck();
            controllerBodyIndex = EditorGUILayout.Popup("Controller Type : ", controllerBodyIndex, controllerBodyOptions); // Dropdown
            // End the code block and update the label if a change occurred
            if (EditorGUI.EndChangeCheck())
            {
                if (senseController) m_TechXRSceneManager.AddController(controllerBodyIndex);
            }

            //----------------------------ICON------------------------------
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



            //----------------------------BUTTON--------------------------
            // Add Player
            EditorGUI.BeginDisabledGroup(playerController);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add", GUILayout.Width(80), GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Warning...!!", "Existing Camera and Controller/DeveloperCube will be removed and a Player will be added to the scene.", "Ok", "Close"))
                {
                    m_TechXRSceneManager.AddXRPlayerController(playerCharacterOptions, playerCharacterIndex, controllerBodyIndex);
                    m_TechXRSceneManager.AddDirectionalLight();
                    this.ShowNotification(new GUIContent("Updated..!!"));

                    // Mark Scene Dirty
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();


            //---------------------HelpBox-------------------------
            if (playerController)
            {
                // Space
                GUILayout.Space(10);

                // Show Help Box
                gUIManager.ShowHelpBox("XRCamera and the XRController are inside XRPlayerController");
            }
        }


        /// <summary>
        /// Function to Show FPSPlayer Component on Editor GUI Window
        /// </summary>
        void ShowFPSPlayer()
        {
            // Heading
            gUIManager.ShowHeading("FPS Player", guiStyle);


            // Space
            GUILayout.Space(20);

            //---------------------- BUTTON -----------------------
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add", GUILayout.Width(80), GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Warning...!!", "Existing Camera and Controller/DeveloperCube will be removed and a FPSPlayer will be added to the scene.", "Ok", "Close"))
                {
                    m_TechXRSceneManager.AddFPSPlayerController();
                    m_TechXRSceneManager.AddDirectionalLight();
                    this.ShowNotification(new GUIContent("Updated..!!"));

                    // Mark Scene Dirty
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

        }


        /// <summary>
        /// Function to Build Nav Mesh
        /// </summary>
        void ShowNavMesh()
        {
            // Heading
            gUIManager.ShowHeading("Make Environment AI Ready", guiStyle);


            // Space
            GUILayout.Space(20);

            //---------------------- BUTTON -----------------------
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add", GUILayout.Width(80), GUILayout.Height(30)))
            {
                NavMeshBuilder.BuildNavMesh(); // Bake the Nav Mesh Map here
                this.ShowNotification(new GUIContent("Updated..!!"));

                // Mark Scene Dirty
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


        /// <summary>
        /// Function to show Enemy Component
        /// </summary>
        void ShowEnemyManager()
        {
            // Heading
            gUIManager.ShowHeading("Enemy Manager", guiStyle);


            // Space
            GUILayout.Space(20);

            //---------------------- BUTTON -----------------------
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add", GUILayout.Width(80), GUILayout.Height(30)))
            {
                // Add the prefab here
                m_TechXRSceneManager.AddEnemyManager();
                this.ShowNotification(new GUIContent("Updated..!!"));

                // Mark Scene Dirty
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


        /// <summary>
        /// Function to Show DeveloperCube Component on Editor GUI Window
        /// </summary>
        void ShowDeveloperCube()
        {
            // Heading
            gUIManager.ShowHeading("XR Developer Cube", guiStyle);

            // Space
            GUILayout.Space(20);


            // Check for controller
            SenseController senseController = GameObject.FindObjectOfType<SenseController>();
            // Check for DeveloperCube
            GameObject developerCube = GameObject.FindGameObjectWithTag("TechXRDeveloperCube");


            //--------------------------DROPDOWN-----------------------------
            // XR Developer Cube
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("        Cube Type :");
            GUILayout.Space(10);
            // Start a code block to check for GUI changes
            EditorGUI.BeginChangeCheck();
            arControllerIndex = EditorGUILayout.Popup("", arControllerIndex, arControllerOptions, GUILayout.Width(100)); // Dropdown
            // End the code block and update the label if a change occurred
            if (EditorGUI.EndChangeCheck())
            {
                if (developerCube)
                {
                    foreach (string s in arControllerOptions)
                        developerCube.transform.Find(s).gameObject.SetActive(false);

                    developerCube.transform.Find(arControllerOptions[arControllerIndex]).gameObject.SetActive(true);

                    // Focus
                    Selection.activeGameObject = developerCube;
                    SceneView.lastActiveSceneView.FrameSelected();

                    // Notify
                    this.ShowNotification(new GUIContent("Developer Cube Updated to " + arControllerOptions[arControllerIndex]));

                    // Mark Scene Dirty
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
            }
            //
            GUILayout.Space(15);

            //----------------------ICON----------------------
            if (arControllerIndex >= 0)
            {
                string icon = arControllerOptions[arControllerIndex];

                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath("Assets/TechXR/Sprites/Icon/" + icon + ".png", typeof(Texture2D));
                controllerBody = obj as Texture2D;
                GUILayout.Label(controllerBody, GUILayout.Width(80), GUILayout.Height(80));
            }
            //
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            // Space
            //GUILayout.Space(15);


            //---------------------Button------------------------
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(senseController || developerCube);
            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {
                // Instantiate the prefab
                UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath("Assets/TechXR/TechXR_DeveloperCube/Prefabs/DeveloperCube" + ".prefab", typeof(GameObject));
                var devCube = Instantiate(prefab) as GameObject;

                foreach (string s in arControllerOptions)
                    devCube.transform.Find(s).gameObject.SetActive(false);

                devCube.transform.Find(arControllerOptions[arControllerIndex]).gameObject.SetActive(true);

                // Focus
                Selection.activeGameObject = devCube;
                SceneView.lastActiveSceneView.FrameSelected();

                m_TechXRSceneManager.AddDirectionalLight();

                this.ShowNotification(new GUIContent("Updated..!!"));

                // Mark Scene Dirty
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Function to Show Controller Component on Editor GUI Window
        /// </summary>
        void ShowController()
        {
            // Heading
            gUIManager.ShowHeading("XR Controller", guiStyle);


            // Space
            GUILayout.Space(20);


            // Check for controller
            SenseController senseController = GameObject.FindObjectOfType<SenseController>();
            // Check for DeveloperCube
            GameObject developerCube = GameObject.FindGameObjectWithTag("TechXRDeveloperCube");


            //-------------------------DROPDOWN--------------------
            // XRController
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Controller Type : ");
            GUILayout.Space(10);

            // Start a code block to check for GUI changes
            EditorGUI.BeginChangeCheck();
            controllerBodyIndex = EditorGUILayout.Popup("", controllerBodyIndex, controllerBodyOptions, GUILayout.Width(100)); // Dropdown
            // End the code block and update the label if a change occurred
            if (EditorGUI.EndChangeCheck())
            {
                if (senseController) m_TechXRSceneManager.AddController(controllerBodyIndex);
            }

            //---------------- ICON --------------
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


            //------------Button-------------------
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(senseController || developerCube);
            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {
                m_TechXRSceneManager.AddController(controllerBodyIndex);
                m_TechXRSceneManager.AddSenseManager();
                m_TechXRSceneManager.AddDirectionalLight();
                this.ShowNotification(new GUIContent("Updated..!!"));

                // Mark Scene Dirty
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            // Space
            //GUILayout.Space(10);

            //-----------------HelpBox---------------
            if (senseController || developerCube)
            {
                // Horizontal Line Separation
                gUIManager.HorizontalLineSeparation();

                // Help Box
                gUIManager.ShowHelpBox("Either Controller or DeveloperCube can be added at a time");
            }
        }

        /// <summary>
        /// Function to Show Camera Component on Editor GUI Window
        /// </summary>
        void ShowXRCamera(string mode)
        {
            //--------------Heading------------------
            gUIManager.ShowHeading("XR Camera", guiStyle);

            GUILayout.Space(10);

            // Check for camera
            Vuforia.VuforiaBehaviour xrCamera = GameObject.FindObjectOfType<Vuforia.VuforiaBehaviour>();


            //--------------Button----------------------
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("XRCamera");
            GUILayout.Space(10);
            EditorGUI.BeginDisabledGroup(xrCamera);
            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {
                m_TechXRSceneManager.AddSenseCamera(mode);
                m_TechXRSceneManager.AddDirectionalLight();
                this.ShowNotification(new GUIContent("Updated..!!"));

                // Mark Scene Dirty
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}