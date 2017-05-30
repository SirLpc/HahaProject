using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace thelab.core {

    [CustomEditor(typeof(UIScreen), true)]
    public class UIScreenInspector : ContainerInspector
    {   

        /// <summary>
        /// Reference to the desired object.
        /// </summary>
        new public UIScreen target { get { return (UIScreen)base.target; } }

        /// <summary>
        /// Reference to the desired objects.
        /// </summary>
        new public UIScreen[] targets { get { UIScreen[] res = new UIScreen[base.targets.Length]; for(int i=0;i<res.Length;i++) res[i]=(UIScreen)base.targets[i]; return res; } }

        /// <summary>
        /// Init.
        /// </summary>
        override public void OnEnable()
        {
            icon = EditorTools.FindAsset<Texture2D>("thelab/core/assets/texture/icons/ui/screen","*.psd");
        }

        /// <summary>
        /// Draws the inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {   
            base.OnInspectorGUI();            

            float v = 0f;
            UIScreen.Mode md = UIScreen.Mode.Custom;
            
            v = EditorGUILayout.Slider("transition",target.transition,0f,1f);            
            if(HasChange("Screen Transition Change")) { target.transition = v; }
            
            md = (UIScreen.Mode)EditorGUILayout.EnumPopup("Mode",target.mode);            
            if(HasChange("Screen Mode Change")) { target.mode = md; }

            switch(target.mode) {
                case UIScreen.Mode.Animation:
                Inspect("clip");
                break;
            }

            Inspect("OnEvent");

            if(Application.isPlaying) {
                EditorGUILayout.BeginHorizontal();
                if(GUILayout.Button("Show")) target.Show(0.5f);
                if(GUILayout.Button("Hide")) target.Hide(0.5f);
                v = prefs.Get<float>("screen.transition",0f);
                if(GUILayout.Button("Fade")) target.Fade(v,0.5f);                
                v = EditorGUILayout.FloatField(v);
                if(HasChange("Transition Test Value")) { prefs.Set<float>("screen.transition",v); }
                EditorGUILayout.EndHorizontal();
            }
            
            DrawDefaultInspector();
            
        }

    }

}