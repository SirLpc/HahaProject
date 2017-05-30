using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace thelab.core
{
    [CustomEditor(typeof(Container), false)]
    public class ContainerInspector : Inspector<Container>
    {   

        /// <summary>
        /// Init.
        /// </summary>
        virtual public void OnEnable()
        {
            icon = EditorTools.FindAsset<Texture2D>("thelab/core/assets/texture/icons/ui/container","*.psd");
        }

        /// <summary>
        /// Draws the inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {            
            float v=0f;
            bool b = false;
            
            EditorGUIUtility.labelWidth = 20f;

            float lw = 60f;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("position",GUILayout.Width(lw));
            v = EditorGUILayout.FloatField("x", target.x); if(HasChange("Set X")) { target.x = v; }
            v = EditorGUILayout.FloatField("y", target.y); if(HasChange("Set Y")) { target.y = v; }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("size",GUILayout.Width(lw));
            v = EditorGUILayout.FloatField("w", target.width);  if(HasChange("Set Width")) { target.width   = v; }
            v = EditorGUILayout.FloatField("h", target.height); if(HasChange("Set Height")) { target.height = v; }
            EditorGUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = 50f;

            v = EditorGUILayout.FloatField("rotation", target.rotation);  if(HasChange("Set Rotation")) { target.rotation = v; }

            EditorGUILayout.BeginHorizontal();
            v = EditorGUILayout.Slider("alpha",target.alpha,-0.1f,1f);  if(HasChange("Set Alpha")) { target.alpha = v; }
            b = EditorGUILayout.Toggle("visible",target.visible); if(HasChange("Set Visibility")) { target.visible = b; }
            EditorGUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = 0f;

            b = EditorGUILayout.Toggle("mouseEnabled",target.mouseEnabled); if(HasChange("Set Mouse Interactive")) { target.mouseEnabled = b; }
        }

    }

}