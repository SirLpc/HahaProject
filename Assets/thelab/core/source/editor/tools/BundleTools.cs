using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace thelab.core {

    #region enum BundlePackState

    /// <summary>
    /// Bundle generation state.
    /// </summary>
    public enum BundlePackState {
        /// <summary>
        /// Bundling start
        /// </summary>
        Start = 0,
        /// <summary>
        /// Bundling finish
        /// </summary>
        Complete,
    }

    #endregion
    
    /// <summary>
    /// Class that offers extra functionalities for editor tasks.
    /// </summary>
    public class BundleTools
    {    
        #region static
    
        [MenuItem("Assets/Bundle/Build/android")]
        static void OnPackFolderAndroid() {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);        
            if(File.Exists(path)) return; //not a folder
            Build("",BuildAssetBundleOptions.None,BuildTarget.Android);        
        }

        [MenuItem("Assets/Bundle/Build/ios")]
        static void OnPackFolderIOS() {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if(File.Exists(path)) return; //not a folder        
            Build("",BuildAssetBundleOptions.None,BuildTarget.iOS);
        }

        [MenuItem("Assets/Bundle/Build/web")]
        static void OnPackFolderWeb() {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if(File.Exists(path)) return; //not a folder
            Build("",BuildAssetBundleOptions.None,BuildTarget.WebPlayer);
        }

        [MenuItem("Assets/Bundle/Build/win32")]
        static void OnPackFolderWin32() {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if(File.Exists(path)) return; //not a folder
            Build("",BuildAssetBundleOptions.None,BuildTarget.StandaloneWindows);
        }

        [MenuItem("Assets/Bundle/Build/win64")]
        static void OnPackFolderWin64() {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if(File.Exists(path)) return; //not a folder
            Build("",BuildAssetBundleOptions.None,BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Assets/Bundle/Build/osx")]
        static void OnPackFolderOSX() {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if(File.Exists(path)) return; //not a folder
            Build("",BuildAssetBundleOptions.None,BuildTarget.StandaloneOSXUniversal);
        }

        [MenuItem("Assets/Bundle/Build/linux")]
        static void OnPackFolderLinux() {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if(File.Exists(path)) return; //not a folder
            Build("",BuildAssetBundleOptions.None,BuildTarget.StandaloneLinuxUniversal);
        } 

        [MenuItem("Assets/Bundle/Tag All")]
        static void OnTagFolder() { OnTagFolder(false); }

        [MenuItem("Assets/Bundle/Tag Clear")]
        static void OnTagFolderClear() { OnTagFolder(true); }

        /// <summary>
        /// Tags a folder
        /// </summary>
        /// <param name="p_clear"></param>
        static void OnTagFolder(bool p_clear) {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if(File.Exists(path)) return; //not a folder 
            TagFolder(path,"$",p_clear);
        }
        
        #endregion

        #region AssetBundle

        /// <summary>
        /// Order the asset bundle builds and direct the result to the chosen folder.
        /// </summary>
        /// <param name="p_target_path"></param>
        /// <param name="p_options"></param>
        /// <param name="p_target"></param>
        static public void Build(string p_target_path,BuildAssetBundleOptions p_options,BuildTarget p_target,System.Action<BundlePackState> p_callback=null)
        {
            if(p_target_path=="") {

                string last_path = EditorPrefs.GetString("bundle.build.last_path","");
                p_target_path = EditorUtility.OpenFolderPanel("Save " + p_target.ToString() + " Bundle",last_path,"");
                if(p_target_path != "") {

                    p_target_path += "/" + p_target.ToString().ToLower();
                    last_path = p_target_path;
                    EditorPrefs.SetString("bundle.build.last_path",last_path);

                }

            }
        
            if(p_target_path == "") return;

            string[] paths = p_target_path.Split('/');
            if(paths.Length>0) {
                string p = "";
                for(int i=0;i<paths.Length;i++) {
                    if(i > 0) p += "/";
                    p += paths[i];
                    if(!Directory.Exists(p)) Directory.CreateDirectory(p);
                }
            }
            if(p_callback != null) p_callback(BundlePackState.Start);
            BuildPipeline.BuildAssetBundles(p_target_path,p_options,p_target);
            if(p_callback != null) p_callback(BundlePackState.Complete);        
        }

        /// <summary>
        /// Automatically tag a folder's children based on the folder prefix.
        /// </summary>
        /// <param name="p_clear"></param>
        static public void TagFolder(string p_path,string p_prefix="$",bool p_clear=false) {       

            string[] files = GetPrefixedAssetsPath(p_path,p_prefix);
            string bundle  = "";
        
            for(int i = 0; i < files.Length; i++) {            
                string it  = files[i];
                int i0 = it.IndexOf(p_prefix);
                if(i0>=0) {
                    int i1 = it.IndexOf('/',i0);
                    bundle = it.Substring(i0,i1-i0).Replace("$","");                
                }
                if(bundle == "") continue;
                it = it.Substring(0,it.LastIndexOf(p_prefix+ bundle))+bundle;            
                string tag = p_clear ? "" : it.Replace(p_path + "/","");
                Tag(files[i],tag);
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Tags an asset by its path.
        /// </summary>
        /// <param name="p_path"></param>
        static public void Tag(string p_path,string p_tag) {
            AssetImporter imp = AssetImporter.GetAtPath(p_path);            
            if(imp==null) {
                Debug.LogWarning("BundleTool> Asset [" + p_path + "] don't exists!");
                return;
            }
            imp.assetBundleName = p_tag;
        }

        /// <summary>
        /// Tags an asset for bundling.
        /// </summary>
        /// <param name="p_path"></param>
        /// <param name="p_tag"></param>
        static public void Tag(Object p_target,string p_tag) {
            string path = AssetDatabase.GetAssetPath(p_target);
            Tag(path,p_tag);
        }

        /// <summary>
        /// Returns a list of paths of assets inside prefixed folders.
        /// </summary>
        /// <param name="p_path"></param>
        /// <param name="p_prefix"></param>
        /// <returns></returns>
        static public string[] GetPrefixedAssetsPath(string p_path,string p_prefix="$") {

            List<string> res   = new List<string>();
            List<string> files = new List<string>(Directory.GetFiles(p_path,"*",SearchOption.AllDirectories));

            //remove metas from files.
            for(int i = 0; i < files.Count; i++) {
                files[i] = files[i].Replace("\\","/");
                string it = files[i];
                if(it.IndexOf('.') < 0) continue;
                string ext = it.Substring(it.LastIndexOf('.')).ToLower();
                if(ext == ".meta") files.RemoveAt(i--);
            }
        
            for(int i = 0; i < files.Count; i++) {
                if(files[i].IndexOf(p_prefix) >= 0) res.Add(files[i]);                    
            }

            return res.ToArray();
        }

        #endregion
        
    }
}
