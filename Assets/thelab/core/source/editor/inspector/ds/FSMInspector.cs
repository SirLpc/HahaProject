using System;
using System.Collections;
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
    /// Inspector for all DictionaryBehaviour related classes.
    /// </summary>
    [CustomEditor(typeof(FSM), true)]
    public class FSMInspector : Inspector<FSM>
    {    
        /// <summary>
        /// Init.
        /// </summary>
        public void OnEnable()
        {            
            //icon = EditorTools.FindAsset<Texture2D>("thelab/core/assets/texture/icons/dictionary/icon","*.psd");            
        }
        
        /// <summary>
        /// Draws the inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {   
            Type t = target.GetType();
            if(t == typeof(FSM)) return;
            t = t.BaseType;
            Type[] generics = t.GetGenericArguments();            
            if(generics.Length <= 0) return;
            
            Type state_type = target.GetStateType();
            
            bool b;

            object previous = Reflection.Get(target,"previous");
            object state    = Reflection.Get(target,"state");
            object o;
            

            //float lw = EditorGUIUtility.labelWidth;
            
            GUILayout.Space(3f);
            
            EditorGUILayout.FloatField("Time",target.time);
            
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Previous");
            InspectState(previous,state_type);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Current");
            Inspect("state");
            o = InspectState(state,state_type);
            if(HasChange("Change State"))
            {                
                Reflection.Set(target,"state",o);                
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();

            if(Inspect("OnChangeEvent")) { HasChange("OnChange Handler"); }
            if(Inspect("OnStateEvent"))  { HasChange("OnState Handler"); }
            
            if(GUILayout.Button("Clear")) target.Clear();

            IList l;
            
            l = (IList)Reflection.Get(target,"transitions");
            
            b = prefs.Get<bool>("fsm.transition.foldout",false);
            if(b = EditorGUILayout.Foldout(b,"Transitions"))
            {   
                if(l!=null) for(int i=0;i<l.Count;i++) InspectLog(l[i],state_type);
            }
            prefs.Set<bool>("fsm.transition.foldout",b);

            
            l = (IList)Reflection.Get(target,"log");
            
            b = prefs.Get<bool>("fsm.log.foldout",false);
            if(b = EditorGUILayout.Foldout(b,"Log"))
            {   
                if(l!=null) for(int i=0;i<l.Count;i++) InspectLog(l[i],state_type);
            }
            prefs.Set<bool>("fsm.log.foldout",b);
                        
            serializedObject.UpdateIfDirtyOrScript();
        }

        /// <summary>
        /// Inspect a log entry.
        /// </summary>
        /// <param name="p_log"></param>
        /// <param name="p_type"></param>
        public void InspectLog(object p_log,Type p_type)
        {
            EditorGUILayout.BeginHorizontal();                        
            object ls = Reflection.Get(p_log,"state");
            float  lt = Reflection.Get<float>(p_log,"time");                        
            GUILayout.Space(5f);
            InspectState(ls,p_type);
            EditorGUILayout.LabelField(lt+"",GUILayout.Width(80f));                         
            EditorGUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// Inspects a state instance of the FSM.
        /// </summary>
        /// <param name="p_state"></param>
        /// <param name="p_type"></param>
        /// <returns></returns>
        public object InspectState(object p_state,Type p_type)
        {
            object v = p_state;            
            if(p_type.IsEnum)            return EditorGUILayout.EnumPopup((Enum)v);
            if(p_type == typeof(string)) return EditorGUILayout.TextField((string)p_state);
            if(p_type == typeof(int))    return EditorGUILayout.IntField((int)p_state);
            if(p_type.IsSubclassOf(typeof(UnityEngine.Object))) return EditorGUILayout.ObjectField((UnityEngine.Object)p_state,p_type,true);
            
            //TBD 
            
            return null;
        }

    }

    

}