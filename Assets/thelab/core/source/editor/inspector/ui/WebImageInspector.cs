using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace thelab.core
{
    [CustomEditor(typeof(WebImage), true)]
    public class WebImageInspector : Inspector<WebImage> {
        /// <summary>
        /// Init.
        /// </summary>
        public void OnEnable() {
            //icon = EditorTools.FindAsset<Texture2D>("thelab/core/assets/texture/icons/ui/drop-component","*.psd");
        }

        /// <summary>
        /// Draws the GUI.
        /// </summary>
        public override void OnInspectorGUI() {            

            Inspect("url");
            Inspect("loadOnStart");
            Inspect("OnEvent");

        }

    }

}