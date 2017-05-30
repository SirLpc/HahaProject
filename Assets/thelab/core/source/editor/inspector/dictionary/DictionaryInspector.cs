using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace thelab.core
{
    /// <summary>
    /// Inspector for all DictionaryBehaviour related classes.
    /// </summary>
    [CustomEditor(typeof(DictionaryBehaviour), true)]
    public class DictionaryInspector : Inspector<DictionaryBehaviour>
    {    
        /// <summary>
        /// Init.
        /// </summary>
        public void OnEnable() {            
            icon = EditorTools.FindAsset<Texture2D>("thelab/core/assets/texture/icons/dictionary/icon","*.psd");            
        }
        
        /// <summary>
        /// Draws the inspector.
        /// </summary>
        public override void OnInspectorGUI() {   
            Type t = target.GetType();
            if(t == typeof(DictionaryBehaviour)) return;
            t = t.BaseType;
            Type[] generics = t.GetGenericArguments();            
            if(generics.Length <= 0) return;
            
            float lw = EditorGUIUtility.labelWidth;
            
            GUILayout.Space(2f);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(16f));

            GUILayout.Space(20f);
            
            GUIStyle bt = new GUIStyle(GUI.skin.button);            
            bt.fontSize = 11;            
            bt.normal.textColor = Color.white;
            bt.active.textColor = Color.white;
            
            for(int i=-1; i < target.Count; i++) {
                GUI.backgroundColor = i<0 ? new Color(0f,0.5f,0f) : new Color(1f,0f,0f);
                bt.contentOffset = i<0 ? new Vector2(-1f,-1f) : new Vector2(-1f,-1f);
                if(GUILayout.Button(i<0 ? "+" : "x",bt,GUILayout.Height(16f))) {
                    if(i<0) target.Add(); else target.RemoveAt(i);
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                }
                GUILayout.Space(-1f);
            }
            EditorGUILayout.EndVertical();

            GUI.backgroundColor = Color.white;
            

            EditorGUIUtility.labelWidth = 10f;
            
            GUILayout.Space(10f);

            EditorGUILayout.BeginVertical();
            Inspect("keys");
            EditorGUILayout.EndVertical();

            GUILayout.Space(10f);

            EditorGUILayout.BeginVertical();
            Inspect("values");
            EditorGUILayout.EndVertical();

            EditorGUIUtility.labelWidth = lw;

            EditorGUILayout.EndHorizontal();
            
        }
        
    }

    

}