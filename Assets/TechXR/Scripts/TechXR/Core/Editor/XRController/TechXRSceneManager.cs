using TechXR.Core.Sense;
using UnityEditor;
using UnityEngine;
using UnityEditor.AI;

namespace TechXR.Core.Editor
{
    public class TechXRSceneManager
    {
        const string CAM_PREFAB_PATH = "Assets/TechXR/Prefabs/SenseCamera.prefab";
        const string CONTROLLER_PREFAB_PATH = "Assets/TechXR/Prefabs/ControllerContainer.prefab";
        const string EVENT_SYSTEM_PREFAB_PATH = "Assets/TechXR/Prefabs/SenseEventSystem.prefab";
        const string SENSE_MANAGER_PREFAB_PATH = "Assets/TechXR/Prefabs/SenseManager.prefab";
        const string UI_CANVAS_PREFAB_PATH = "Assets/TechXR/Prefabs/UIElements/UI/";
        const string SKYBOX_PATH = "Assets/TechXR/Skybox/Mat/";
        const string XR_PLAYER_PATH = "Assets/TechXR/Prefabs/T_XRPlayerController.prefab";
        const string ENVIRONMENTS_PATH = "Assets/TechXR/Prefabs/Environments/";
        const string FPS_PLAYER_PATH = "Assets/TechXR/FPS/Prefabs/FPSPlayerController.prefab";
        const string ENEMY_MANAGER_PATH = "Assets/TechXR/FPS/Prefabs/EnemyManager.prefab";
        const string OBJECT_POOLING_MANAGER_PATH = "Assets/TechXR/FPS/Prefabs/ObjectPoolingManager.prefab";

        /// <summary>
        /// Add the Camera to the scene
        /// </summary>
        public void AddSenseCamera(string mode)
        {
            // Destroy All cameras
            Camera[] cameras = Object.FindObjectsOfType<Camera>();
            foreach (Camera cam in cameras)
                Object.DestroyImmediate(cam.gameObject);

            // Instantiate the SenseCamera
            UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath(CAM_PREFAB_PATH, typeof(GameObject));
            var xrcam = Object.Instantiate(prefab) as GameObject;
            xrcam.name = "SenseCamera";

            // SetUp Clear Flags
            if (mode.Contains("VR"))
            {
                xrcam.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
            }
            else if (mode.Contains("AR"))
            {
                xrcam.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            }

            // Focus
            Selection.activeGameObject = xrcam;
            SceneView.lastActiveSceneView.FrameSelected();
        }

        /// <summary>
        /// Add Directional Light
        /// </summary>
        public void AddDirectionalLight()
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
        /// Add XR Controller to the scene
        /// </summary>
        public void AddController(int controllerBodyIndex)
        {

            // Return if SenseController is present
            GameObject[] controllers = GameObject.FindGameObjectsWithTag("SenseController");

            if (controllers.Length > 0)
            {
                controllers[0].GetComponent<SenseController>().SetPointerType(controllerBodyIndex);
                Debug.Log("SenseController is already present in your scene..!");
                return;
            }

            // Instantiate the XR Controller
            //UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath("Assets/TechXR/Prefabs/SenseController.prefab", typeof(GameObject));
            Object prefab = AssetDatabase.LoadAssetAtPath(CONTROLLER_PREFAB_PATH, typeof(GameObject));
            var xr = Object.Instantiate(prefab) as GameObject;
            //xr.name = "SenseController";
            xr.name = "ControllerContainer";

            // Set Active the GameObject
            //xr.GetComponent<SenseController>().SetPointerType(controllerBodyIndex);
            xr.GetComponentInChildren<SenseController>().SetPointerType(controllerBodyIndex);

            AddEventSystem();
        }

        /// <summary>
        /// Check if eventsystem is not present in the scene and add
        /// </summary>
        public void AddEventSystem()
        {
            GameObject eventSystem = GameObject.FindGameObjectWithTag("SenseEventSystem");
            if (eventSystem != null) return;

            Object prefab = AssetDatabase.LoadAssetAtPath(EVENT_SYSTEM_PREFAB_PATH, typeof(GameObject));
            var es = Object.Instantiate(prefab) as GameObject;
            es.name = "SenseEventSystem";

            GameObject te = GetTechXREssentialGO();
            es.transform.SetParent(te.transform);
        }

        /// <summary>
        /// Get the TechXREssential gameobject
        /// </summary>
        /// <returns></returns>
        public GameObject GetTechXREssentialGO()
        {
            GameObject go = GameObject.Find("TechXREssentials");
            if (go) return go;

            go = new GameObject("TechXREssentials");
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;

            return go;
        }

        /// <summary>
        /// Add SenseManager prefab
        /// </summary>
        public void AddSenseManager()
        {
            if (GameObject.FindGameObjectWithTag("SenseManager") != null) return;

            Object prefab = AssetDatabase.LoadAssetAtPath(SENSE_MANAGER_PREFAB_PATH, typeof(GameObject));
            var senseManager = Object.Instantiate(prefab) as GameObject;
            senseManager.name = "SenseManager";

            GameObject te = GetTechXREssentialGO();
            senseManager.transform.SetParent(te.transform);
        }

        /// <summary>
        /// Add Canvas
        /// </summary>
        /// <param name="canvasName"></param>
        public void AddCanvas(string canvasName)
        {
            Object prefab = AssetDatabase.LoadAssetAtPath(UI_CANVAS_PREFAB_PATH + canvasName + ".prefab", typeof(GameObject));
            var xrcanvas = Object.Instantiate(prefab) as GameObject;
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
        /// Set Skybox
        /// </summary>
        public void SetSkybox(string[] skyboxOptions, int skyboxIndex)
        {
            Material m = AssetDatabase.LoadAssetAtPath(SKYBOX_PATH + skyboxOptions[skyboxIndex] + ".mat", typeof(Material)) as Material;

            RenderSettings.skybox = m;
        }

        /// <summary>
        /// Add the Player to the scene
        /// </summary>
        public void AddXRPlayerController(string[] playerCharacterOptions, int playerCharacterIndex, int controllerBodyIndex)
        {
            // Destroy All Players
            GameObject[] players = GameObject.FindGameObjectsWithTag("XRPlayerController");
            foreach (GameObject p in players)
                Object.DestroyImmediate(p);

            // Delete all Cameras
            DeleteCameras();

            // Delete all XRControllers
            //GameObject[] controllers = GameObject.FindGameObjectsWithTag("SenseController");
            //foreach (GameObject controller in controllers)
            //    DestroyImmediate(controller);

            // Delete all XRControllers
            Vuforia.CylinderTargetBehaviour[] controllers = Object.FindObjectsOfType<Vuforia.CylinderTargetBehaviour>();
            foreach (Vuforia.CylinderTargetBehaviour controller in controllers)
                Object.DestroyImmediate(controller.gameObject);

            // Delete all DeveloperCube
            GameObject[] devCubes = GameObject.FindGameObjectsWithTag("TechXRDeveloperCube");
            foreach (GameObject devCube in devCubes)
                Object.DestroyImmediate(devCube);

            // Instantiate the XRPlayerController
            Object prefab = AssetDatabase.LoadAssetAtPath(XR_PLAYER_PATH, typeof(GameObject));
            var playerController = Object.Instantiate(prefab) as GameObject;
            playerController.name = "XRPlayerController";

            // Turn on the Player Body
            playerController.transform.Find("CharacterBodyContainer").Find(playerCharacterOptions[playerCharacterIndex]).gameObject.SetActive(true);

            // Set active the controller body
            playerController.GetComponentInChildren<SenseController>().SetPointerType(controllerBodyIndex);

            // Focus
            Selection.activeGameObject = playerController;
            SceneView.lastActiveSceneView.FrameSelected();

            /*
                        // Add warning System
                        if(m_TrackableWarningSystem)
                        {
                            var xrcanvas = Instantiate(Resources.Load("CanvasPrefabs/Warning Canvas") as GameObject);
                            xrcanvas.name = "Warning Canvas";

                            DestroyImmediate(playerController.GetComponentInChildren<DefaultTrackableEventHandler>());

                            GameObject controller = playerController.GetComponentInChildren<SenseController>().gameObject;
                            controller.AddComponent<SenseXRTrackingStatus>();
                        }
            */

            // Adding SenseManager and EventSystem
            AddEventSystem();
            AddSenseManager();
        }

        /// <summary>
        /// Delete All Cameras from the scene
        /// </summary>
        private static void DeleteCameras()
        {
            Camera[] cameras = Object.FindObjectsOfType<Camera>();
            foreach (Camera cam in cameras)
                Object.DestroyImmediate(cam.gameObject);
        }

        /// <summary>
        /// Add XR Controller Attachment Feature
        /// </summary>
        public void AddXRControllerAttachmentFeature()
        {
            GameObject playerController = GameObject.FindWithTag("XRPlayerController");

            if (!playerController)
            {
                Debug.Log("TechXR :: No XRPlayer in the Scene , Add the XRPlayer First");
            }
            else
            {
                Object.DestroyImmediate(playerController.GetComponentInChildren<DefaultTrackableEventHandler>());

                //GameObject controller = playerController.GetComponentInChildren<SenseController>().gameObject;
                GameObject controller = playerController.GetComponentInChildren<Vuforia.CylinderTargetBehaviour>().gameObject;
                controller.AddComponent<SenseXRTrackingStatus>();
            }
        }

        /// <summary>
        /// Remove XR Controller Attachment Feature
        /// </summary>
        public void RemoveXRControllerAttachmentFeature()
        {
            GameObject playerController = GameObject.FindWithTag("XRPlayerController");

            if (!playerController)
            {
                Debug.Log("TechXR :: No XRPlayer in the Scene , Check the XRPlayer Checkbox");
            }
            else
            {
                Object.DestroyImmediate(playerController.GetComponentInChildren<SenseXRTrackingStatus>());

                //GameObject controller = playerController.GetComponentInChildren<SenseController>().gameObject;
                GameObject controller = playerController.GetComponentInChildren<Vuforia.CylinderTargetBehaviour>().gameObject;
                controller.AddComponent<DefaultTrackableEventHandler>();
            }
        }

        /// <summary>
        /// Add Bluetooth warning system
        /// </summary>
        public void AddBluetoothWarningSystem()
        {
            /*GameObject warningCanvas = GameObject.Find("Warning Canvas");
            if (warningCanvas)
                warningCanvas.transform.Find("BluetoothWarningImage").gameObject.SetActive(true);
            else
            {
                UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath("Assets/TechXR/Prefabs/UIElements/Optional/Warning Canvas.prefab", typeof(GameObject));
                var xrcanvas = Instantiate(prefab) as GameObject;
                xrcanvas.name = "Warning Canvas";

                xrcanvas.transform.Find("BluetoothWarningImage").gameObject.SetActive(true);
            }*/

            GameObject btWarning = GameObject.Find("Bluetooth Warning");
            if (!btWarning)
            {
                GameObject go = new GameObject("Bluetooth Warning");
                go.AddComponent<SenseXRConnectivityStatus>();

                GameObject te = GetTechXREssentialGO();
                go.transform.SetParent(te.transform);
            }
        }

        /// <summary>
        /// Remove Bluetooth warning system
        /// </summary>
        public void RemoveBluetoothWarningSystem()
        {
            SenseXRConnectivityStatus btObj = Object.FindObjectOfType<SenseXRConnectivityStatus>();
            if (btObj)
                Object.DestroyImmediate(btObj.gameObject);
        }

        /// <summary>
        /// Add the Floor prefab
        /// </summary>
        public void AddEnvironment(string[] environmentOptions, int environmentIndex, string mode)
        {
            Object prefab = AssetDatabase.LoadAssetAtPath(ENVIRONMENTS_PATH + environmentOptions[environmentIndex] + ".prefab", typeof(GameObject));
            var env = Object.Instantiate(prefab) as GameObject;
            env.name = environmentOptions[environmentIndex];

            GameObject te = GetTechXREssentialGO();
            env.transform.SetParent(te.transform);

            if (mode.Contains("FPS"))
            {
                //NavMeshBuilder.BuildNavMesh(); // Bake the Nav Mesh Map here
            }
        }

        /// <summary>
        /// Add the FPSPlayer to the scene
        /// </summary>
        public void AddFPSPlayerController()
        {
            // Destroy All Players
            GameObject[] players = GameObject.FindGameObjectsWithTag("XRPlayerController");
            foreach (GameObject p in players)
                Object.DestroyImmediate(p);

            // Delete all Cameras
            DeleteCameras();

            // Delete all XRControllers
            Vuforia.CylinderTargetBehaviour[] controllers = Object.FindObjectsOfType<Vuforia.CylinderTargetBehaviour>();
            foreach (Vuforia.CylinderTargetBehaviour controller in controllers)
                Object.DestroyImmediate(controller.gameObject);

            // Delete all DeveloperCube
            GameObject[] devCubes = GameObject.FindGameObjectsWithTag("TechXRDeveloperCube");
            foreach (GameObject devCube in devCubes)
                Object.DestroyImmediate(devCube);

            // Instantiate the XRPlayerController
            Object prefab = AssetDatabase.LoadAssetAtPath(FPS_PLAYER_PATH, typeof(GameObject));
            var playerController = Object.Instantiate(prefab) as GameObject;
            playerController.name = "FPSPlayerController";

            // Focus
            Selection.activeGameObject = playerController;
            SceneView.lastActiveSceneView.FrameSelected();

            // Adding SenseManager and EventSystem
            AddEventSystem();
            AddSenseManager();
            AddObjectPoolingManager();
        }

        /// <summary>
        /// Function to add EnemyManager Prefab
        /// </summary>
        public void AddEnemyManager()
        {
            Object prefab = AssetDatabase.LoadAssetAtPath(ENEMY_MANAGER_PATH, typeof(GameObject));
            var enemyManager = Object.Instantiate(prefab) as GameObject;
            enemyManager.name = "EnemyManager";
        }

        /// <summary>
        /// Function to add object pooling manager prefab
        /// </summary>
        public void AddObjectPoolingManager()
        {
            Object prefab = AssetDatabase.LoadAssetAtPath(OBJECT_POOLING_MANAGER_PATH, typeof(GameObject));
            var objectPoolingManager = Object.Instantiate(prefab) as GameObject;
            objectPoolingManager.name = "ObjectPoolingManager";
        }
    }
}