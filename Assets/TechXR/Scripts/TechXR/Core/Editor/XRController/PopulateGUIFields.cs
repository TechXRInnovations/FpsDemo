using System.IO;

namespace TechXR.Core.Editor
{
    public class PopulateGUIFields
    {
        // Dropdown Environment Options
        DirectoryInfo environmentDir = new DirectoryInfo("Assets/TechXR/Prefabs/Environments");
        FileInfo[] environmentFileInfo;
        string[] environmentOptions;

        // Dropdown Canvas Type Options
        DirectoryInfo uiElementDir = new DirectoryInfo("Assets/TechXR/Prefabs/UIElements/UI");
        FileInfo[] uiElementFilesInfo;
        string[] uiElementOptions;

        // Dropdown 3D Model Options
        DirectoryInfo modelsDir = new DirectoryInfo("Assets/TechXR/3DModels");
        FileInfo[] modelFilesInfo;
        string[] modelOptions;

        // Dropdown Skybox Options
        DirectoryInfo skyboxDir = new DirectoryInfo("Assets/TechXR/Skybox/Mat");
        FileInfo[] skyboxFilesInfo;
        string[] skyboxOptions;

        /// <summary>
        /// Update the Dropdown List of Environment prefabs present in the Given folder
        /// </summary>
        public string[] PopulateEnvironmentOptions()
        {
            environmentFileInfo = environmentDir.GetFiles("*.*");
            environmentOptions = new string[environmentFileInfo.Length];

            int index = 0;
            foreach (FileInfo f in environmentFileInfo)
            {
                environmentOptions[index] = f.Name.Split('.')[0];
                index++;
            }

            return environmentOptions;
        }

        /// <summary>
        /// Update the Dropdown List of UI Elements present in the Given folder
        /// </summary>
        public string[] PopulateUIElementOptions()
        {
            uiElementFilesInfo = uiElementDir.GetFiles("*.*");
            uiElementOptions = new string[uiElementFilesInfo.Length];

            int index = 0;
            foreach (FileInfo f in uiElementFilesInfo)
            {
                uiElementOptions[index] = f.Name.Split('.')[0];
                index++;
            }

            return uiElementOptions;
        }

        /// <summary>
        /// Update the Dropdown List of 3D Models present in the Given folder
        /// </summary>
        public string[] PopulateModelOptions()
        {
            modelFilesInfo = modelsDir.GetFiles("*.*");
            modelOptions = new string[modelFilesInfo.Length];

            int index = 0;
            foreach (FileInfo f in modelFilesInfo)
            {
                modelOptions[index] = f.Name.Split('.')[0];
                index++;
            }

            return modelOptions;
        }

        /// <summary>
        /// Update the Dropdown List of Skybox materials present in the Given folder
        /// </summary>
        public string[] PopulateSkyboxOptions()
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

            return skyboxOptions;
        }
    }
}