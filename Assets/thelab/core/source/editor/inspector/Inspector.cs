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
    /// Callback called when a property will be inspected.
    /// </summary>
    /// <param name="p_property"></param>
    /// <param name="p_options"></param>
    public delegate void InspectCallback(SerializedProperty p_property,GUILayoutOption[] p_options);

    /// <summary>
    /// Base class for all inspector scripts.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Inspector<T> : Editor where T : Object
    {

        #region class Prefs

        /// <summary>
        /// Class that wraps the features of editor prefs.
        /// </summary>
        public class Prefs
        {
            /// <summary>
            /// Target object related to the prefs.
            /// </summary>
            public T target;

            /// <summary>
            /// CTOR.
            /// </summary>
            /// <param name="p_target"></param>
            public Prefs(T p_target) { target = p_target; }

            /// <summary>
            /// Returns a value from the prefs.
            /// </summary>
            /// <typeparam name="U"></typeparam>
            /// <param name="p_key"></param>
            /// <param name="p_default"></param>
            /// <returns></returns>
            public U Get<U>(string p_key,U p_default,bool p_global=false) {
                string sufix = p_global ? "" : ("."+target.GetInstanceID());
                string k = p_key +sufix;
                if(typeof(U)==typeof(string)) return (U)(object)EditorPrefs.GetString(k,(string)(object)p_default);
                if(typeof(U)==typeof(int))    return (U)(object)EditorPrefs.GetInt(k,(int)(object)p_default);
                if(typeof(U)==typeof(float))  return (U)(object)EditorPrefs.GetFloat(k,(float)(object)p_default);
                if(typeof(U)==typeof(bool))   return (U)(object)EditorPrefs.GetBool(k,(bool)(object)p_default);
                return p_default;
            }

            /// <summary>
            /// Returns a value from the prefs.
            /// </summary>
            /// <typeparam name="U"></typeparam>
            /// <param name="p_key"></param>
            /// <returns></returns>
            public U Get<U>(string p_key) { return Get<U>(p_key,default(U)); }

            /// <summary>
            /// Sets a value in the prefs.
            /// </summary>
            /// <typeparam name="U"></typeparam>
            /// <param name="p_key"></param>
            /// <param name="p_value"></param>
            public void Set<U>(string p_key,U p_value,bool p_global=false) {
                string sufix = p_global ? "" : ("."+target.GetInstanceID());
                string k = p_key +sufix;
                if(typeof(U)==typeof(string)) EditorPrefs.SetString(k,(string)(object)p_value);
                if(typeof(U)==typeof(int))    EditorPrefs.SetInt(k,(int)(object)p_value);
                if(typeof(U)==typeof(float))  EditorPrefs.SetFloat(k,(float)(object)p_value);
                if(typeof(U)==typeof(bool))   EditorPrefs.SetBool(k,(bool)(object)p_value);                
            }
        }

        #endregion

        /// <summary>
        /// Reference to the desired object.
        /// </summary>
        new public T target { get { return (T)base.target; } }

        /// <summary>
        /// Reference to the desired objects.
        /// </summary>
        new public T[] targets { get { T[] res = new T[base.targets.Length]; for(int i=0;i<res.Length;i++) res[i]=(T)base.targets[i]; return res; } }
         
        /// <summary>
        /// Inspector icon.
        /// </summary>
        public Texture2D icon { get { return m_icon; } set { m_icon = value; EditorTools.SetAssetIcon(target,m_icon); } }
        private Texture2D m_icon;

        /// <summary>
        /// Reference to this inspector's target prefs.
        /// </summary>
        public Prefs prefs { get { return m_prefs==null ? (m_prefs = new Prefs(target)) : m_prefs; } }
        private Prefs m_prefs;
        
        /// <summary>
        /// Flag that tells something changed.
        /// </summary>
        private bool m_control_changed;
        
        /// <summary>
        /// Prepare the drawing of a property field and calls a callback to let the user handle it.
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_undo_label"></param>
        /// <param name="p_options"></param>
        /// <returns></returns>
        public bool Inspect(SerializedObject p_target,string p_property,InspectCallback p_callback,params GUILayoutOption[] p_options) {
            m_control_changed = false;
            SerializedObject t   = p_target;
            SerializedProperty p = t.FindProperty(p_property);
            if(p == null) return false;
            EditorGUI.BeginChangeCheck();                        
            p_callback(p,p_options);
            if(EditorGUI.EndChangeCheck()) {                
                HasChange("Change "+p_property);
                t.ApplyModifiedProperties();
                m_control_changed = true;
            }            
            return m_control_changed;
        }

        /// <summary>
        /// Draws the field for changing a given property.
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_undo_label"></param>
        /// <param name="p_options"></param>
        /// <returns></returns>
        public bool Inspect(SerializedObject p_target,string p_property,params GUILayoutOption[] p_options) {
            return Inspect(p_target,p_property,delegate(SerializedProperty p,GUILayoutOption[] opts){
                EditorGUILayout.PropertyField(p,true,opts);
            });            
        }

        /// <summary>
        /// Draws the field for changing a given property.
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_undo_label"></param>
        /// <param name="p_options"></param>
        /// <returns></returns>
        public bool Inspect(Object p_target,string p_property,InspectCallback p_callback,params GUILayoutOption[] p_options) {
            return Inspect(new SerializedObject(p_target),p_property,p_callback,p_options);            
        }
        
        /// <summary>
        /// Draws the field for changing a given property.
        /// </summary>
        /// <param name="p_property"></param>
        /// <param name="p_undo_label"></param>
        /// <param name="p_options"></param>
        /// <returns></returns>
        public bool Inspect(Object p_target,string p_property,params GUILayoutOption[] p_options) {
            return Inspect(new SerializedObject(p_target),p_property,p_options);
        }

        /// <summary>
        /// Draws the field for changing a given property.
        /// </summary>
        /// <param name="p_property"></param>
        /// <param name="p_undo_label"></param>
        /// <param name="p_options"></param>
        /// <returns></returns>
        public bool Inspect(string p_property,params GUILayoutOption[] p_options) {
            return Inspect(serializedObject,p_property,p_options);
        }

        /// <summary>
        /// Returns a flag indicating if some previous operation incurred in changes.
        /// </summary>
        /// <param name="p_label"></param>
        /// <returns></returns>
        public bool HasChange(string p_label="",Object p_target=null) {
            bool changed = GUI.changed || m_control_changed;
            m_control_changed = false;
            GUI.changed = false;
            if(changed){
                Object t = p_target ? p_target : target;
                Undo.RegisterCompleteObjectUndo(t,p_label);
            }
            return changed;
        }
        
    }

    

}