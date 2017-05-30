using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace thelab.core
{
    [CustomEditor(typeof(ImageClip), true)]
    public class ImageClipInspector : Inspector<ImageClip> {
        
        /// <summary>
        /// Last sampled time.
        /// </summary>
        private float m_last_time;

        /// <summary>
        /// Init.
        /// </summary>
        public void OnEnable() {
            icon = EditorTools.FindAsset<Texture2D>("thelab/core/assets/texture/icons/ui/image-clip","*.psd");
            m_last_time = Time.realtimeSinceStartup;
        }
        
        /// <summary>
        /// Draws the GUI.
        /// </summary>
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            float vf;
            int vi;

            bool refresh = false;

            vi = EditorGUILayout.IntSlider("Frame",target.frame,0,target.count);
            if(HasChange("Clip Frame")) {
                if(!target.playing) target.frame = vi;
                refresh=true;
            }

            vf = EditorGUILayout.Slider("Time",target.elapsed,0f,target.duration);
            if(HasChange("Clip Time")) {
                if(!target.playing) target.elapsed = vf;
                refresh=true;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Playback",GUILayout.Width(80f));
            if(GUILayout.RepeatButton("Play"))  {  target.Play(-1f); }
            if(GUILayout.RepeatButton("Pause")) {  target.PauseSwitch(); refresh=true; }
            if(GUILayout.RepeatButton("Stop"))  {  target.Stop(); refresh=true; }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Step",GUILayout.Width(80f));
            if(GUILayout.Button("<")) { target.frame--; refresh = true; }
            if(GUILayout.Button(">")) { target.frame++; refresh = true; }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Sort",GUILayout.Width(80f));
            
            if(GUILayout.Button("Forward"))  { refresh = true; target.SortForward();  }
            if(GUILayout.Button("Backward")) { refresh = true; target.SortBackward(); }
            if(GUILayout.Button("Random"))   { refresh = true; target.SortRandom();   }
            
            EditorGUILayout.EndHorizontal();

            if(target.playing) refresh=true;

            float dt = Time.realtimeSinceStartup - m_last_time;
            m_last_time = Time.realtimeSinceStartup;
            if(dt>=(1f/30f))dt = 1f/30f;

            if(!Application.isPlaying) {
                bool can_step = target.playing && (!target.paused);                
                if(can_step) target.Step(dt);
            }
            if(refresh) {                
                ArrayList l = SceneView.sceneViews;
                foreach(object it in l) {
                    SceneView v = (SceneView)it;
                    if(v!=null) v.camera.Render();
                }
                Repaint();
            }



        }

        protected int CompareFramesForward(Sprite a,Sprite b)  { return !a ? 1 : (!b ? -1 :  string.Compare(a.name, b.name)); }
        protected int CompareFramesBackward(Sprite a,Sprite b) { return !a ? 1 : (!b ? -1 : -string.Compare(a.name, b.name)); }
        protected int CompareFramesRandom(Sprite a,Sprite b) { return UnityEngine.Random.Range(-1,1); }
       

    }

}