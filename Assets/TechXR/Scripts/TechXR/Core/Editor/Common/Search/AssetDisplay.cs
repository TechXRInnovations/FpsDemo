using System.Net;
using UnityEngine;

namespace TechXR.Core.Editor
{
    /// <summary>
    /// Manages all the fuctionalities related to the Display section of the Assets
    /// </summary>
    public class AssetDisplay
    {
        #region PUBLIC_MEMBERS
        public AuthorInfo AuthorCredit { get; set; }
        public Texture2D Texture { get; set; }
        public GUIContent GUIContent { get; set; }
        public SearchAssetData SearchAssetData { get; set; } = new SearchAssetData();
        #endregion // PUBLIC_MEMBERS
        //
        #region PUBLIC_METHODS

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="searchAssetData"></param>
        public AssetDisplay(SearchAssetData searchAssetData)
        {
            this.SearchAssetData = searchAssetData;
        }

        /// <summary>
        /// Make GUI content from textures
        /// </summary>
        /// <param name="_thumbnails"></param>
        /// <returns></returns>
        public GUIContent MakeSearchGui(Texture2D _thumbnail)
        {
            GUIContent gUIContent = new GUIContent(_thumbnail);
            return gUIContent;
        }

        /// <summary>
        /// Get Texture2D from url
        /// </summary>
        /// <param name="thumbnailUrl"></param>
        /// <returns></returns>
        public Texture2D GetTexture(string thumbnailUrl)
        {
            Texture2D texture;
            using (WebClient client = new WebClient())
            {
                byte[] data = client.DownloadData(thumbnailUrl);
                texture = new Texture2D(1, 1);
                ImageConversion.LoadImage(texture, data);
            }
            return texture;
        }
        #endregion // PUBLIC_METHODS
    }
}