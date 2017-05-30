using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace thelab.core {
   
    /// <summary>
    /// Class that offers extra functionalities for editor tasks.
    /// </summary>
    public class EditorTools
    {

        #region Directory

        /// <summary>
        /// Checks if the given unity object is a folder in project panel.
        /// </summary>
        /// <param name="p_target"></param>
        /// <returns></returns>
        static public bool IsFolderAsset(Object p_target)
        {            
            if (!p_target) return false;
            string path = AssetDatabase.GetAssetPath(p_target);
            if (path.Length <= 0)       return false;
            return Directory.Exists(path);            
        }

        /// <summary>
        /// Validates the path and create the needed folders.
        /// </summary>
        /// <param name="p_path"></param>
        static public void PathAssert(string p_path)
        {
            p_path = p_path.Replace('\\','/');
            string[] paths = p_path.Split('/');
            if(paths.Length > 0)
            {
                string p = "";
                for(int i = 0; i < paths.Length; i++)
                {
                    if(i > 0) p += "/";
                    p += paths[i];
                    if(!Directory.Exists(p)) Directory.CreateDirectory(p);
                }
            }
        }

        #endregion

        #region GUI

        /// <summary>
        /// Sets a given Object icon.
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_icon"></param>
        static public void SetAssetIcon(Object p_target,Texture2D p_icon)
        {
            /*
            System.Type t = typeof(EditorGUIUtility);
            BindingFlags bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
            object[] args = new object[] { p_target,p_icon };
            t.InvokeMember("SetIconForObject",bindingFlags,null,null,args);
            //*/
            Reflection<EditorGUIUtility>.Invoke("SetIconForObject",new object[] { p_target,p_icon });
        }

        #endregion

        #region Find

        /// <summary>
        /// Searches the 'Assets' folder for assets matching the pattern.
        /// </summary>
        /// <param name="p_pattern"></param>
        static public List<T> FindAssets<T>(string p_query,string p_pattern="*") where T : Object
        {
            List<T> res = new List<T>();
            List<string> files = new List<string>(Directory.GetFiles("Assets/",p_pattern,SearchOption.AllDirectories));
            
            for(int i=0;i<files.Count;i++)
            {
                string f = files[i];
                f = f.Replace('\\','/');                
                if(f.IndexOf(p_query)<0)continue;
                T it = AssetDatabase.LoadAssetAtPath<T>(f);
                if(!it) continue;
                res.Add(it);
            }            
            return res;
        }

        /// <summary>
        /// Searches the 'Assets' folder for the first occurence matching the pattern.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_pattern"></param>
        /// <returns></returns>
        static public T FindAsset<T>(string p_query,string p_pattern="*") where T : Object {
            List<T> res = FindAssets<T>(p_query,p_pattern);
            if(res.Count<=0)return default(T);
            return res[0];
        }

        /// <summary>
        /// Searches a given importer located on the desired path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_path"></param>
        /// <returns></returns>
        static public T FindImporter<T>(string p_path) where T : AssetImporter {            
            return (T)AssetImporter.GetAtPath(p_path);
        }

        /// <summary>
        /// Searches a given importer located for the informed asset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_asset"></param>
        /// <returns></returns>
        static public T FindImporter<T>(Object p_asset) where T : AssetImporter {
            string path = AssetDatabase.GetAssetPath(p_asset);                
            return (T)AssetImporter.GetAtPath(path);
        }



        #endregion

        #region Delay

        /// <summary>
        /// Delays the call of a given method.
        /// </summary>
        /// <param name="p_callback"></param>
        /// <param name="p_time"></param>
        static public void Delay(System.Action p_callback,float p_time)
        {
            EditorApplication.CallbackFunction cb = null;
            float t = Time.realtimeSinceStartup;
            cb = delegate ()
            {
                if((Time.realtimeSinceStartup - t) < p_time)
                    return;
                p_callback();
                EditorApplication.update -= cb;
            };
            EditorApplication.update += cb;
        }

        /// <summary>
        /// Delay the change of a property.
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_value"></param>
        static public void DelaySet(object p_target,string p_property,object p_value,float p_time) { Delay(delegate () { Reflection.Set(p_target,p_property,p_value); },p_time); }

        /// <summary>
        /// Delay the change of a static property.
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_value"></param>
        static public void DelaySet<T>(string p_property,object p_value,float p_time) { Delay(delegate () { Reflection<T>.Set(p_property,p_value); },p_time); }

        /// <summary>
        /// Delay the invokation of a function.
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_value"></param>
        static public void Delay(object p_target,string p_method,object[] p_args,float p_time) { Delay(delegate () { Reflection.Invoke(p_target,p_method,p_args); },p_time); }

        /// <summary>
        /// Delay the invokation of a static function.
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_value"></param>
        static public void Delay<T>(string p_method,object[] p_args,float p_time) { Delay(delegate () { Reflection<T>.Invoke(p_method,p_args); },p_time); }

        #endregion
    }
}
