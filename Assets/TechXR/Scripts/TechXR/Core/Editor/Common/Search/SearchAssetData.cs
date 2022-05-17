using System;
using System.Collections.Generic;

[Serializable]
public class SearchAssetData
{
    public string id { get; set; }
    public string displayName { get; set; }
    public string thumbnailUrl { get; set; }
    public List<AssetFileInfo> resources { get; set; } = new List<AssetFileInfo>();
    public AuthorInfo authorCredit { get; set; }
    public string score { get; set; }
}
