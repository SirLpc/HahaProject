using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace thelab.core {

    /// <summary>
    /// Class that implements the inspector of the AudioComponent.
    /// </summary>
    [CustomEditor(typeof(AudioComponent),true)]
    public class AudioComponentInspector : Inspector<AudioComponent> {

        /// <summary>
        /// Init.
        /// </summary>
        public void OnEnable() {            
            icon = EditorTools.FindAsset<Texture2D>("thelab/core/assets/texture/icons/media/audio-component","*.psd");            
        }
            
        /// <summary>
        /// Draws the inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            //Get the target's type
            Type t = target.GetType();

            if(!target.source) return;
            if(!target.source.clip) return;

            //Validate if it is an 'AudioComponent' at its roots.
            while (t.Name.IndexOf("AudioComponent") != 0) {  t = t.BaseType; }
            
            if (t.GetGenericArguments().Length<=0) {             
                Inspect("type");
            }
            else {
                //Get the <T> argument Type.
                t = t.GetGenericArguments()[0];

                

                //Process it Enum contents
                Array el = Enum.GetValues(t);
                int sid = -1;
                for(int i=0;i<el.Length;i++) {
                    int en = (int)el.GetValue(i);
                    if (en == target.type) { sid = i; break; }
                }
                Enum prev = (sid>=0) ? (Enum)el.GetValue(sid) : default(Enum);
        
                //Inspect its Enum contents.
                Enum next = EditorGUILayout.EnumPopup("Type",prev);

                int id_prev = ((int)(object)prev);
                int id_next = ((int)(object)next);

                if(id_next != id_prev) {                
                    target.type = id_next;
                }

                HasChange("Change Audio Type");
            }


            AudioSource src = target ? target.source : null;
            
            if(src) {
            
                float v;
                
                v = EditorGUILayout.Slider("Volume",src.volume,0f,1f);
                if(HasChange("Volume Change",src)) { src.volume = v; }

                v = EditorGUILayout.Slider("Pitch",src.pitch,0f,3f);
                if(HasChange("Pitch Change",src)) { src.pitch = v; }
                
                float d = 0f;
                if(src.clip) d = src.clip.length;

                if(d>0f) {
    
                    v = EditorGUILayout.Slider("Time",src.time,0f,d);
                    
                    if(Event.current.type== EventType.Used) 
                    if(HasChange("Time Change",src)) { src.time = v; }
                                    
                    EditorGUILayout.BeginHorizontal();
                    if(GUILayout.Button("Play"))  { src.Play();  }
                    if(GUILayout.Button("Pause")) { src.Pause(); }
                    if(GUILayout.Button("Stop"))  { src.Stop(); src.time=0f; }
                    EditorGUILayout.EndHorizontal();
                    
                    if(Application.isPlaying) {

                        v = prefs.Get<float>("fade.volume",src.volume);
                        
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Fade");
                        if(GUILayout.Button("In"))  { target.Fade(v,1f);  }
                        if(GUILayout.Button("Out")) { target.Fade(0f,1f); }                    
                        v = EditorGUILayout.FloatField(v);
                        if(HasChange("Fade Volume")) { prefs.Set<float>("fade.volume",v); }
                        EditorGUILayout.EndHorizontal();
                        
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Pitch");

                        if(GUILayout.Button("Slow"))    { target.FadePitch(prefs.Get<float>("fade.pitch.slow",src.pitch),1f); }
                        if(GUILayout.Button("Default")) { target.FadePitch(1f,1f); }                
                        if(GUILayout.Button("Fast"))    { target.FadePitch(prefs.Get<float>("fade.pitch.fast",src.pitch),1f); }

                        v = prefs.Get<float>("fade.pitch.slow",src.pitch);
                        v = EditorGUILayout.FloatField(v);
                        if(HasChange("Fade Slow Pitch")) { prefs.Set<float>("fade.pitch.slow",v); }

                        v = prefs.Get<float>("fade.pitch.fast",src.pitch);
                        v = EditorGUILayout.FloatField(v);
                        if(HasChange("Fade Fast Pitch")) { prefs.Set<float>("fade.pitch.fast",v); }
                            
                        EditorGUILayout.EndHorizontal();

                    }



                    if(src.isPlaying) Repaint();

                }

            }
        
        }

    }

}