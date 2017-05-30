using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


namespace thelab.core
{
    /// <summary>
    /// Organize gameobjects in the scene.
    /// </summary>
    public class TagContext
    {

        [MenuItem("GameObject/Tag/String",false,0)]
        static void OnAddString() { OnAdd<StringTag>(); }

        [MenuItem("GameObject/Tag/Int",false,0)]
        static void OnAddInt() { OnAdd<IntTag>(); }

        [MenuItem("GameObject/Tag/Object",false,0)]
        static void OnAddObject() { OnAdd<ObjectTag>(); }

        [MenuItem("GameObject/Tag/Script",false,0)]
        static void OnAddScript() { OnAdd<ScriptTag>(); }
        
        /// <summary>
        /// Adds a Tag component of given type in the current selection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        static void OnAdd<T>() where T : MonoBehaviour
        {
            GameObject[] list = Selection.gameObjects;
            foreach(GameObject it in list)
            {
                it.AddComponent<T>();
            }
        }

    }
}