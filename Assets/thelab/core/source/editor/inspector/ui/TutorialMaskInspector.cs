using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace thelab.core
{
    [CustomEditor(typeof(TutorialMask), true)]
    public class TutorialMaskInspector : Inspector<TutorialMask>
    {
        /// <summary>
        /// Target size percent for debug.
        /// </summary>
        public float maskTargetSizeScale = 1f;
        public RectTransform maskTarget;

        /// <summary>
        /// Init.
        /// </summary>
        public void OnEnable()
        {
            target.Invoke("Init", 0f);
        }

        /// <summary>
        /// Draws the inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            float v=0f;
            RectTransform rt=null;

            EditorGUIUtility.labelWidth = 47f;

            v = EditorGUILayout.FloatField("x", target.x);
            if (GUI.changed) { GUI.changed = false; Undo.RecordObject(target, "Mask X"); target.x = v; }

            v = EditorGUILayout.FloatField("y", target.y);
            if (GUI.changed) { GUI.changed = false; Undo.RecordObject(target, "Mask Y");  target.y = v; }

            v = EditorGUILayout.FloatField("w", target.width);
            if (GUI.changed) { GUI.changed = false; Undo.RecordObject(target, "Mask Width");  target.width = v; }

            v = EditorGUILayout.FloatField("h", target.height);
            if (GUI.changed) { GUI.changed = false; Undo.RecordObject(target, "Mask Height");  target.height = v; }

            EditorGUILayout.BeginHorizontal();
                rt = (RectTransform)EditorGUILayout.ObjectField("Target",maskTarget, typeof(RectTransform), true);
                if (GUI.changed) { GUI.changed = false; maskTarget = rt; Undo.RecordObject(target, "Mask Target"); target.Mask(maskTarget, maskTargetSizeScale); }

                v = EditorGUILayout.FloatField("scale", maskTargetSizeScale);
                if (GUI.changed) { GUI.changed = false; maskTargetSizeScale = v; Undo.RecordObject(target, "Mask Target"); target.Mask(maskTarget, maskTargetSizeScale); }
            EditorGUILayout.EndHorizontal();
        }

    }

}