using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace thelab.core
{
    [CustomEditor(typeof(EventComponent), true)]
    public class EventComponentInspector : Inspector<EventComponent>
    {
        /// <summary>
        /// Init.
        /// </summary>
        public void OnEnable()
        {
            icon = EditorTools.FindAsset<Texture2D>("thelab/core/assets/texture/icons/ui/event-component","*.psd");
        }

    }

}