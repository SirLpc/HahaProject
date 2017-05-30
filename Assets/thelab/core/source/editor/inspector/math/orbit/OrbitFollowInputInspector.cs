using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace thelab.core {

    /// <summary>
    /// Class that implements the inspector of the OrbitFollowInput component.
    /// </summary>
    [CustomEditor(typeof(OrbitFollowInput),true)]
    public class OrbitFollowInputInspector : Inspector<OrbitFollowInput> {

        /// <summary>
        /// Init.
        /// </summary>
        public void OnEnable() {            
            icon = EditorTools.FindAsset<Texture2D>("thelab/core/assets/texture/icons/math/orbit-follow-input-icon","*.psd");
        }
        
    }

}