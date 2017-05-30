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
    [CustomEditor(typeof(OrbitWASDInput),true)]
    public class OrbitWASDInputInspector : Inspector<OrbitWASDInput> {

        /// <summary>
        /// Init.
        /// </summary>
        public void OnEnable() {            
            icon = EditorTools.FindAsset<Texture2D>("thelab/core/assets/texture/icons/math/orbit-wasd-input-icon","*.psd");
        }
        
    }

}