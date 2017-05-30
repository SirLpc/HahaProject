using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace thelab.core {

    /// <summary>
    /// Class that implements the inspector of the OrbitTransform component.
    /// </summary>
    [CustomEditor(typeof(OrbitTransform),true)]
    public class OrbitTransformInspector : Inspector<OrbitTransform> {

        /// <summary>
        /// Init.
        /// </summary>
        public void OnEnable() {            
            icon = EditorTools.FindAsset<Texture2D>("thelab/core/assets/texture/icons/math/orbit-transform-icon","*.psd");
            target.Init();
        }

        /// <summary>
        /// Handles the editor gui draw.
        /// </summary>
        public override void OnInspectorGUI() {
        
            float v;
            Vector2 v2;
            float lw;
            
            lw = EditorGUIUtility.labelWidth;
            
            EditorGUIUtility.labelWidth = 70f;

            v = EditorGUILayout.FloatField("distance",target.distance);
            if(HasChange("Orbit Distance")) {
                target.distance = v;
            }
            
            v2 = EditorGUILayout.Vector2Field("angle",target.angle);
            if(HasChange("Orbit Angle")) {
                target.angle = v2;
            }
            
            Inspect("speed");

            EditorGUIUtility.labelWidth = lw;

            if(Inspect("transitions")) {                                
                target.transition = OrbitTransform.Transition.None;
                target.SetTransition(true,target.transitions);
            }
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Snap");
            if(GUILayout.Button("Anchor")) { target.Snap(true,false); }   
            if(GUILayout.Button("Angle"))  { target.Snap(false,true); }   
            if(GUILayout.Button("All"))    { target.Snap(true,true); }   
            EditorGUILayout.EndHorizontal();

        }

    }

}