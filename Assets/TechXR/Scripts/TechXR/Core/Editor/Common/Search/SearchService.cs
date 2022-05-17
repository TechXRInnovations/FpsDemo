using System.Collections.Generic;
using System.Net;
using co.techxr.unity.network;
using UnityEngine;

namespace TechXR.Core.Editor
{
    /// <summary>
    /// Class contains all the functions related to web/api calls for Search Service
    /// </summary>
    public class SearchService
    {
        #region CONSTANTS
        const string SEARCH_ASSET = "api/search/3DAsset/search";
        #endregion // CONSTANTS
        //
        #region PRIVATE_MEMBERS
        private NetworkService m_NetworkService;
        #endregion // PRIVATE_MEMBERS
        //
        #region PUBLIC_METHODS
        /// <summary>
        /// Get asset data from web
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public List<SearchAssetData> GetAssetData(string searchText)
        {
            // Set Base URL
            Url.setBaseUrl("http://devgateway.techxr.co");


            // Make Parameters
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["searchString"] = searchText;


            // API Call
            m_NetworkService = new NetworkService();
            return m_NetworkService.get<List<SearchAssetData>>(SEARCH_ASSET, parameters);
        }

        /// <summary>
        /// Download File from url
        /// </summary>
        /// <param name="url"></param>
        public void DownloadFile(string url, string path, string fileName, string fileFormat)
        {
            using (WebClient client = new WebClient())
            {
                byte[] data = client.DownloadData(url);
                //client.DownloadFile(url,Application.dataPath + "/" + "d_test.mtl");
                client.DownloadFile(url, path + fileName + "." + fileFormat);
            }
        }
        #endregion // PUBLIC_METHODS
    }
}