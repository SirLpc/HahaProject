using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace thelab.core
{
    [CustomEditor(typeof(DragComponent), true)]
    public class DragComponentInspector : Inspector<DragComponent>
    {
        /// <summary>
        /// Init.
        /// </summary>
        public void OnEnable()
        {
            icon = EditorTools.FindAsset<Texture2D>("thelab/core/assets/texture/icons/ui/drag-component","*.psd");
        }

        /// <summary>
        /// Draws the GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            if(Inspect("alphaOriginal"))    { HasChange("Change Original Alpha"); }
            if(Inspect("alphaCopy"))        { HasChange("Change Copy Alpha"); }
            if(Inspect("move"))             { HasChange("Change Move Flag"); }
            if(Inspect("OnEvent"))       { HasChange("Change Event Handlers"); }
        }

    }

}