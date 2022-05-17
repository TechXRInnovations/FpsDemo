using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Linq;
using System.ComponentModel;
using TechXR.Core.Sense;

namespace TechXR.Core.Utils
{
    /// <summary>
    /// Singleton manager class for loading external data
    /// </summary>
    internal class DataManager : Singleton<DataManager>
    {
        #region Public_Members
        #endregion
        //
        #region Private_Members
        // Config loaded counter
        private int m_DataLoadedCounter = 0;
        // json data list
        private Dictionary<string, string> m_Data = new Dictionary<string, string>();
        // data path list
        private List<string> m_Paths = new List<string>();
        // local vs remote data flag
        private bool m_LocalData = new bool();
        // load count
        private uint m_LoadCount;
        #endregion
        //
        #region private methods
        /// <summary>
        /// Load application data
        /// </summary>
        private void LoadData()
        {
            // if local data
            if (m_LocalData)
            {
                foreach (var item in m_Paths)
                {
                    StartCoroutine(GetDataLocal(item));
                }
            }
            // if remote data
            else
            {
                foreach (var item in m_Paths)
                {
                    StartCoroutine(GetDataRemote(item));
                }
            }
        }

        /// <summary>
        /// Load data from the local file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private IEnumerator GetDataLocal(string fileName)
        {
            yield return new WaitForSeconds(0.1f);

            // load the local file as a textasset
            TextAsset file = Resources.Load(fileName) as TextAsset;

            // get loaded data
            string jsonString = file.ToString();
            // add to the results list
            m_Data.Add(fileName, jsonString);
            // call data loadig complete handler
            DataLoadedHandler();
        }

        /// <summary>
        /// Load data from the remote url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private IEnumerator GetDataRemote(string url)
        {
            yield return new WaitForSeconds(0.1f);

            UnityWebRequest www = UnityWebRequest.Get(url);

            yield return www.SendWebRequest();

            if (www.isHttpError || www.isNetworkError)
            {
                print("Error Loading Data from url: " + url + ", error: "+ www.error);
            }
            else
            {
                // get downloaded data
                string jsonString = www.downloadHandler.text;
                // add to the results list
                m_Data.Add(url, jsonString);
                // call data loadig complete handler
                DataLoadedHandler();
            }
        }

        /// <summary>
        /// Data loading complete handler
        /// </summary>
        private void DataLoadedHandler()
        {
            // increment the data loaded counter
            m_DataLoadedCounter++;
            // if all the data paths have been loaded
            if (m_DataLoadedCounter >= m_Paths.Count)
            {
                //trigger the config loaded event
                EventManager.Instance.TriggerEvent(SenseEvent.DATA_LOADING_COMPLETE);
            }
        }
        #endregion
        //
        #region public methods
        /// <summary>
        /// Initialize data loading
        /// </summary>
        /// <param name="filePaths">A list of file paths or URLs from where data is to be loaded</param>
        /// <param name="local">Boolean flag to detenmine if the file to be loaded is on the local machine or online on a remote system</param>
        public void Init(List<string> filePaths, bool local = false)
        {
            // set the path list
            m_Paths = new List<string>(filePaths);
            // set the local/remote data flag
            m_LocalData = local;
            // reset data loaded counter
            m_DataLoadedCounter = 0;
            // load game data
            LoadData();
        }
        #endregion
    }
}
