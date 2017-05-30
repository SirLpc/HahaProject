using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


namespace thelab.core
{
    /// <summary>
    /// Organize gameobjects in the scene.
    /// </summary>
    public class DictionaryContext
    {

        [MenuItem("GameObject/Dictionary/String > String",false,0)]    static void OnAddStringString() { OnAdd<DictionaryStringString>(); }        
        [MenuItem("GameObject/Dictionary/String > Int",false,0)]       static void OnAddStringInt()    { OnAdd<DictionaryStringInt>(); }
        [MenuItem("GameObject/Dictionary/String > Object",false,0)]    static void OnAddStringObject() { OnAdd<DictionaryStringObject>(); }
        [MenuItem("GameObject/Dictionary/String > Float",false,0)]     static void OnAddStringFloat()    { OnAdd<DictionaryStringFloat>(); }
        [MenuItem("GameObject/Dictionary/String > Curve",false,0)]     static void OnAddStringCurve()    { OnAdd<DictionaryStringCurve>(); }

        [MenuItem("GameObject/Dictionary/Int > String",false,0)]    static void OnAddIntString() { OnAdd<DictionaryIntString>(); }        
        [MenuItem("GameObject/Dictionary/Int > Int",false,0)]       static void OnAddIntInt()    { OnAdd<DictionaryIntInt>(); }
        [MenuItem("GameObject/Dictionary/Int > Object",false,0)]    static void OnAddIntObject() { OnAdd<DictionaryIntObject>(); }
        [MenuItem("GameObject/Dictionary/Int > Float",false,0)]     static void OnAddIntFloat()  { OnAdd<DictionaryIntFloat>(); }
        [MenuItem("GameObject/Dictionary/Int > Curve",false,0)]     static void OnAddIntCurve()    { OnAdd<DictionaryIntCurve>(); }

        [MenuItem("GameObject/Dictionary/Object > String",false,0)]    static void OnAddObjectString() { OnAdd<DictionaryObjectString>(); }        
        [MenuItem("GameObject/Dictionary/Object > Int",false,0)]       static void OnAddObjectInt()    { OnAdd<DictionaryObjectInt>(); }
        [MenuItem("GameObject/Dictionary/Object > Object",false,0)]    static void OnAddObjectObject() { OnAdd<DictionaryObjectObject>(); }
        [MenuItem("GameObject/Dictionary/Object > Float",false,0)]     static void OnAddObjectFloat()  { OnAdd<DictionaryObjectFloat>(); }
        [MenuItem("GameObject/Dictionary/Object > Curve",false,0)]     static void OnAddObjectCurve()    { OnAdd<DictionaryObjectCurve>(); }

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