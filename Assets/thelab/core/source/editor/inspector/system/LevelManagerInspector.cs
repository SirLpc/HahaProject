using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace thelab.core {

    /// <summary>
    /// Inspector for all Tag related classes.
    /// </summary>
    [CustomEditor(typeof(LevelManager), true)]
    public class LevelManagerInspector : Inspector<LevelManager>
    {    
        /// <summary>
        /// Init.
        /// </summary>
        public void OnEnable()
        {            
            icon = EditorTools.FindAsset<Texture2D>("thelab/core/assets/texture/icons/system/level-manager","*.psd");            
        }
        
        /// <summary>
        /// Draws the inspector.
        /// </summary>
        public override void OnInspectorGUI() {            

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Level Id",GUILayout.Width(100f));
            EditorGUILayout.LabelField(target.levelId+"");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Level Name",GUILayout.Width(100f));
            EditorGUILayout.LabelField(target.levelName);
            GUILayout.EndHorizontal();

            Inspect("OnEvent");

            string inspect_name = prefs.Get<string>("inspect.name","");
            bool inspect_async  = prefs.Get<bool>("inspect.async",false);
            bool inspect_add    = false;
            bool will_load      = false;

            if(!Application.isPlaying) return;
            
            EditorGUILayout.LabelField("Inspect");
            EditorGUILayout.Separator();
            
            inspect_async = GUILayout.Toggle(inspect_async,"Async");
            if(HasChange("Level Inspect Async")) { prefs.Set<bool>("inspect.async",inspect_async); }

            GUILayout.BeginHorizontal();            
            EditorGUILayout.LabelField("Name",GUILayout.Width(50f));
            inspect_name = EditorGUILayout.TextField(inspect_name);
            if(HasChange("Level Inspect Name")) { prefs.Set<string>("inspect.name",inspect_name); }
            if(GUILayout.Button("Add",GUILayout.Width(40f))) { will_load = true; inspect_add = true; }
            if(GUILayout.Button("Load",GUILayout.Width(40f))) { will_load = true; inspect_add = false; }
            GUILayout.EndHorizontal();

            if(will_load) {
                if(inspect_async) {
                    if(inspect_add) { target.AddLevelAsync(inspect_name); } else {  target.LoadLevelAsync(inspect_name); }
                }
                else {
                    if(inspect_add) { target.AddLevel(inspect_name); } else {  target.LoadLevel(inspect_name); }
                }
            }
            
            EditorGUILayout.LabelField("Loaded Levels");
            EditorGUILayout.Separator();
            for(int i=0;i<target.levelNames.Count;i++) {
                string n = target.levelNames[i];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(n);
                if(GUILayout.Button("x",GUILayout.Width(25f))) {
                    target.Unload(n);
                }
                EditorGUILayout.EndHorizontal();
            }

            Dictionary<string,AsyncOperation> loaders = Reflection.Get<Dictionary<string,AsyncOperation>>(target,"m_loaders");
            
            if(loaders.Count > 0) {
                EditorGUILayout.LabelField("Loading");                
                foreach(KeyValuePair<string,AsyncOperation> it in loaders) {
                    string n          = it.Key;
                    AsyncOperation op = it.Value;            
                    float p = op==null ? 1f : op.progress;    
                    Rect r = GUILayoutUtility.GetRect(Screen.width,18f);
                    EditorGUI.ProgressBar(r,p,n);
                    GUILayout.Space(5f);
                }
            }
            
        }
        
    }

    

}