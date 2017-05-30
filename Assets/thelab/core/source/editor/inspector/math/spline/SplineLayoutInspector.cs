using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace thelab.core {

    /// <summary>
    /// Class that implements the inspector of the OrbitWASDInput component.
    /// </summary>
    [CustomEditor(typeof(SplineLayout),true)]
    public class SplineLayoutInspector : Inspector<SplineLayout> {

        /// <summary>
        /// Last spline rev.
        /// </summary>
        private int m_last_rev;

        /// <summary>
        /// Init.
        /// </summary>
        public void OnEnable() {            
            //icon = EditorTools.FindAsset<Texture2D>("thelab/core/assets/texture/icons/math/orbit-wasd-input-icon","*.psd");
            m_last_rev = -1;
        }

        /// <summary>
        /// Renders the GUI.
        /// </summary>
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            if(!target.spline) return;

            if(m_last_rev != target.spline.m_rev) { m_last_rev = target.spline.m_rev; target.Refresh(); }


            if(GUILayout.Button("Refresh")) {
                target.Refresh();
            }
        }



    }

}