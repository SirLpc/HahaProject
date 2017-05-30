using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace thelab.core
{
    [CustomEditor(typeof(DropComponent), true)]
    public class DropComponentInspector : Inspector<DropComponent>
    {
        /// <summary>
        /// Init.
        /// </summary>
        public void OnEnable()
        {
            icon = EditorTools.FindAsset<Texture2D>("thelab/core/assets/texture/icons/ui/drop-component","*.psd");
        }

        /// <summary>
        /// Draws the GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            if(Inspect("OnEvent")) { HasChange("Change Event Handlers"); }
        }

    }

}