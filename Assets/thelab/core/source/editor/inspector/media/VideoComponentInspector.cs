using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace thelab.core {

    /// <summary>
    /// Class that implements the inspector of the VideoComponent.
    /// </summary>
    [CustomEditor(typeof(VideoComponent),true)]
    public class VideoComponentInspector : Inspector<VideoComponent> {

        /// <summary>
        /// Init.
        /// </summary>
        public void OnEnable() {            
            icon = EditorTools.FindAsset<Texture2D>("thelab/core/assets/texture/icons/media/video-component","*.psd");            
        }
        
    }

}