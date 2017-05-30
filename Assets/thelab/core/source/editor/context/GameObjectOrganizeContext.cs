using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

namespace thelab.core
{
    /// <summary>
    /// Organize gameobjects in the scene.
    /// </summary>
    public class GameObjectOrganizeContext
    {

        [MenuItem("GameObject/Organize/Distribute",false,0)]
        static void OnDistribute()
        {
            GameObject[] list = Selection.gameObjects;
            Transform first = list[0].transform;
            Transform last = list[0].transform;
            float m0 = first.localPosition.magnitude;
            float m1 = first.localPosition.magnitude;
            for(int i = 1; i < list.Length; i++)
            {
                Transform t = list[i].transform;
                float m = t.localPosition.magnitude;
                if(m < m0)
                { first = t; m0 = m; }
                if(m > m1)
                { last = t; m1 = m; }
            }
            Vector3 p0 = first.localPosition;
            Vector3 p1 = last.localPosition;
            Vector3 vec = p1 - p0;
            float c = (float)list.Length;
            float d = vec.magnitude;
            vec.Normalize();
            float step = d / (c - 1f);
            for(int i = 0; i < list.Length; i++)
            {
                float si = (float)i;
                list[i].transform.localPosition = p0 + (vec * (si * step));
            }
        }

        [MenuItem("GameObject/Organize/Orient/Forward",false,0)]
        static void OnOrientForward()
        { OrientGroup(true); }

        [MenuItem("GameObject/Organize/Orient/Backward",false,0)]
        static void OnOrientBackward()
        { OrientGroup(false); }

        static void OrientGroup(bool p_forward)
        {
            List<Transform> list = GetOrderedSelection(!p_forward);
            if(list.Count <= 0)
                return;
            Transform p = list[0].parent;
            for(int i = 1; i < list.Count; i++)
            {
                Transform t0 = list[i - 1];
                Transform t1 = list[i];
                t0.LookAt(t1,p.up);
                if(i >= (list.Count - 1))
                { Vector3 dir = t1.position - t0.position; dir.Normalize(); t1.LookAt(t1.position + dir,p.up); }
            }
        }

        /// <summary>
        /// Orders the transform selection based on the child indexes.
        /// </summary>
        /// <param name="p_reverse"></param>
        /// <returns></returns>
        static List<Transform> GetOrderedSelection(bool p_reverse = false)
        {
            List<Transform> list = new List<Transform>(Selection.transforms);
            list.Sort(delegate (Transform a,Transform b)
            { return a.GetSiblingIndex() < b.GetSiblingIndex() ? -1 : 1; });
            if(p_reverse)
                list.Reverse();
            return list;
        }

        /// <summary>
        /// Iterate the transforms in selection.
        /// </summary>
        /// <param name="p_callback"></param>
        static void IterateTransforms(Action<Transform> p_callback,bool p_reverse = false)
        {
            List<Transform> list = new List<Transform>(Selection.transforms);
            list.Sort(delegate (Transform a,Transform b)
            { return a.GetSiblingIndex() < b.GetSiblingIndex() ? -1 : 1; });
            for(int i = 0; i < list.Count; i++)
                p_callback(list[i]);
        }

        [MenuItem("GameObject/Organize/Distribute",true,0)]
        static bool OnDistributeValidate()
        { return (Selection.gameObjects.Length > 1); }


        [MenuItem("GameObject/Organize/Index Naming",false,0)]
        static void OnIndexNaming()
        {
            GameObject[] list = Selection.gameObjects;
            for(int i = 0; i < list.Length; i++)
                list[i].gameObject.name = list[i].transform.GetSiblingIndex() + "";
        }


    }
}