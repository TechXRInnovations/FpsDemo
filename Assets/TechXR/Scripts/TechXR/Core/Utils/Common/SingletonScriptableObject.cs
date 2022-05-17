using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TechXR.Core.Utils
{
    /// <summary>
    /// Class template for Scriptable Singleton classes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    T[] assets = Resources.LoadAll<T>("");
                    if (assets == null || assets.Length < 1)
                    {
                        throw new System.Exception("Could not find any singleton scriptable object instances in the resources.");
                    }
                    else
                    {
                        Debug.LogWarning("Multiple instances of the singleton scriptable object found in the resources.");
                    }
                    instance = assets[0];
                    (instance).OnInitialize();
                    //instance = Resources.Load<T>(typeof(T).ToString());
                    //(instance as SingletonScriptableObject<T>).OnInitialize();
                }
                return instance;
            }
        }

        // Optional overridable method for initializing the instance.
        protected virtual void OnInitialize() { }

    }
}
