using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace thelab.core
{
    /// <summary>
    /// Inspector for all Tag related classes.
    /// </summary>
    [CustomEditor(typeof(Tag), true)]
    public class TagInspector : Inspector<Tag>
    {    
        /// <summary>
        /// Init.
        /// </summary>
        public void OnEnable()
        {            
            icon = EditorTools.FindAsset<Texture2D>("thelab/core/assets/texture/icons/tag/icon","*.psd");            
        }
        
        /// <summary>
        /// Draws the inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {            
            string s;

            s = EditorGUILayout.TextField("Label",target.label);
            if(HasChange("Change Tag Label")) { target.label = s; }

            Type t = target.GetType();
            if(t == typeof(Tag)) return;
            t = t.BaseType;
            Type[] generics = t.GetGenericArguments();            
            if(generics.Length <= 0) return;
            Inspect("tags");            
        }
        
    }

    

}