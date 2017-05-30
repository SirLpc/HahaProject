using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace thelab.core {

    /// <summary>
    /// Class that implements the inspector of the SplineLayoutElement component.
    /// </summary>
    [CustomEditor(typeof(SplineLayoutElement),true)]
    public class SplineLayoutElementInspector : Inspector<SplineLayoutElement> {
        
        /// <summary>
        /// Reference to the containing layout.
        /// </summary>
        public SplineLayout layout;

        /// <summary>
        /// Init.
        /// </summary>
        public void OnEnable() {            
            //icon = EditorTools.FindAsset<Texture2D>("thelab/core/assets/texture/icons/math/orbit-wasd-input-icon","*.psd");
            Transform p = target.transform.parent;
            while(p) {
                layout = p.gameObject.GetComponent<SplineLayout>();
                if(layout) return;
                p = p.parent;
            }                
        }

        /// <summary>
        /// Renders the GUI.
        /// </summary>
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            if(GUI.changed) {               
                if(layout) layout.Refresh();
            }
                        
        }



    }

}