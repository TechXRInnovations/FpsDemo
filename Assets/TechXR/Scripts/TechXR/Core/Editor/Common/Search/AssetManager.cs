using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TechXR.Core.Editor
{
    /// <summary>
    /// The class Manages and Provides all the assets present in TechXR Database
    /// </summary>
    public class AssetManager
    {
        public List<SearchAssetData> searchResult;
        #region PRIVATE_MEMBERS
        private SearchService m_SearchService;
        #endregion // PRIVATE_MEMBERS
        //
        #region PUBLIC_METHODS
        /// <summary>
        /// Populate members of this class
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns>Returns a dictionary with ID to AssetFileInfo Mapping</returns>
        public Dictionary<string,AssetDisplay> Populate(string searchText)
        {
            // Get Asset data fom SearchService class
            m_SearchService = new SearchService();
            searchResult = m_SearchService.GetAssetData(searchText);

            Debug.Log("Search -> "+ searchResult.Count);


            // Add and return list
            Dictionary<string, AssetDisplay> assetDisplayDict = new Dictionary<string, AssetDisplay>();
            for (int i = 0; i < searchResult.Count; i++)
            {
                AssetDisplay ad = new AssetDisplay(searchResult[i]);

                // Do the necessary calls from SearchService class
                // Initialise all the fields of AssetDisplay Object
                ad.Texture = ad.GetTexture(searchResult[i].thumbnailUrl);
                ad.GUIContent = ad.MakeSearchGui(ad.Texture);
                ad.AuthorCredit = searchResult[i].authorCredit;

                //assetDisplay.Add(ad);
                assetDisplayDict[searchResult[i].id] = ad;
            }
            return assetDisplayDict;
        }

        /// <summary>
        /// Returns a dictionary mapped with ID's and File informaion of assets
        /// </summary>
        /// <param name="searchAssetList"></param>
        /// <returns></returns>
        public Dictionary<string, List<AssetFileInfo>> GetFileInfo(List<SearchAssetData> searchAssetList)
        {
            Dictionary<string, List<AssetFileInfo>> searchFileInfo = new Dictionary<string, List<AssetFileInfo>>();

            // Assign Files with IDs
            foreach (SearchAssetData s in searchAssetList)
                searchFileInfo[s.id] = s.resources;

            return searchFileInfo;
        }

        /// <summary>
        /// Download all the necessary files of Asset
        /// </summary>
        /// <param name="assetFileInfo"></param>
        public void DownloadAsset(List<AssetFileInfo> assetFileInfo, string authorCredit)
        {

            // Make a Folder 3DModels if not exists
            string path = Application.dataPath + "/3DModels/" ;
            MakeDirectory(path);


            // Make a Folder with Random Name
            string randStr = GenerateRandomString(7);
            string filePath = path + randStr + "/";
            MakeDirectory(filePath);



            // Check if the asset contains the models in the FBX and OBJ format
            bool fbx = assetFileInfo.Exists(a => a.fileFormat == "FBX");
            bool obj = assetFileInfo.Exists(a => a.fileFormat == "OBJ");
            //Debug.Log("FBX->"+fbx+" "+"OBJ-->"+obj);



            // Local variable for storing format name
            string format;



            // If FBX format is available download FBX File, if not then download OBJ file
            if (fbx)
            {
                // Get the file info of FBX and download the FBX model
                AssetFileInfo model = assetFileInfo.Find(a => a.fileFormat == "FBX");
                format = "fbx";
                DownloadFile(model, filePath, "model", format);


                // Download the license file of the asset
                AssetFileInfo license = assetFileInfo.Find(a => a.fileFormat == "MD");
                DownloadFile(license, filePath, "license", "md");


                // Refresh
                AssetDatabase.Refresh();


                // Instantiate Object
                Instantiate3DObject("Assets/3DModels/" + randStr + "/" + "model", format);
            }
            else if (obj)
            {

                // Get the file info of OBJ and download the OBJ model
                AssetFileInfo model = assetFileInfo.Find(a => a.fileFormat == "OBJ");
                format = "obj";
                DownloadFile(model, filePath, "model", format);


                // Download Material file of this Object
                AssetFileInfo material = assetFileInfo.Find(a => a.fileFormat == "MTL");
                DownloadFile(material, filePath, "materials", "mtl");


                // Download the license file of the asset
                AssetFileInfo license = assetFileInfo.Find(a => a.fileFormat == "MD");
                DownloadFile(license, filePath, "license", "md");


                // Refresh
                AssetDatabase.Refresh();


                // Instantiate Object
                Instantiate3DObject("Assets/3DModels/" + randStr + "/" + "model", format);

            }
            else
            {
                // Delete the Directory Made
                //Directory.Delete(filePath);
                //AssetDatabase.Refresh();

                Debug.Log("No Asset Available..!!");
            }


            // Give Author Credits
            if (authorCredit != null) Debug.Log("AuthorName : " + authorCredit);
            else Debug.Log("No Author Credits provided");

        }


        /// <summary>
        /// Function Takes a path and creates a Directory
        /// </summary>
        /// <param name="path"></param>
        public void MakeDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                AssetDatabase.Refresh();
            }
        }


        /// <summary>
        /// Download file by giving AssetInfo,path,name and format
        /// </summary>
        /// <param name="model"></param>
        /// <param name="filePath"></param>
        /// <param name="name"></param>
        /// <param name="format"></param>
        public void DownloadFile(AssetFileInfo model, string filePath, string name, string format)
        {
            if (model != null)
            {
                m_SearchService.DownloadFile(model.url, filePath, name, format);
            }
            else
            {
                Debug.Log(format + " format is not available for this Object");
            }
        }


        /// <summary>
        /// Instantiate the 3D Object given in the path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="format"></param>
        public void Instantiate3DObject(string path, string format)
        {
            //Debug.Log("----"+path + "." + format+"----");

            Object prefab = AssetDatabase.LoadAssetAtPath(path + "." + format, typeof(GameObject));
            GameObject obj = UnityEngine.Object.Instantiate(prefab) as GameObject;

            // Focus
            Selection.activeGameObject = obj;
            SceneView.lastActiveSceneView.FrameSelected();

            // Highlight Asset
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path + "." + format);
        }


        /// <summary>
        /// Generate random string of given Length
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string GenerateRandomString(int length)
        {
            System.Random random = new System.Random();
            var rString = "";
            for (var i = 0; i < length; i++)
            {
                rString += ((char)(random.Next(1, 26) + 64)).ToString().ToLower();
            }

            return rString;
        }
        #endregion // PUBLIC_METHODS
    }
}