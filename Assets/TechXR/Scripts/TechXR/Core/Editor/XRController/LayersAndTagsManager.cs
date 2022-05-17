using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TechXR.Core.Editor
{
    public class LayersAndTagsManager
    {
        // max layers and tags allowed
        private const int MAX_LAYERS = 31;
        private const int MAX_TAGS = 10000;

        // layers and tags to be added
        private List<string> m_Layers = new List<string>() { "Ground", "SenseXR" };
        private List<string> m_Tags = new List<string>() { "XRPlayerController", "SenseController", "SenseManager", "Env", "CharacterBody", "TechXRDeveloperCube", "SenseEventSystem" };


        /// <summary>
        /// Update Layers and Tags
        /// </summary>
        public void UpdateLayersAndTags()
        {
            AddLayersAndTags(m_Layers, m_Tags);
        }

        /// <summary>
        /// Add the layers and tags from the passed lists
        /// </summary>
        /// <param name="layers"></param>
        /// <param name="tags"></param>
        public void AddLayersAndTags(List<string> layers, List<string> tags)
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
        /// Create Layer
        /// </summary>
        /// <param name="name"></param>
        public void CreateLayer(string name)
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
        /// Add tag to the tag list
        /// </summary>
        /// <param name="tagName"></param>
        private void AddTag(string tagName)
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
        private bool PropertyExists(SerializedProperty property, int start, int end, string value)
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