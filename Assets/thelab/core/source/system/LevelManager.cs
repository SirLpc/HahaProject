using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace thelab.core {
    
    /// <summary>
    /// Class that wraps level handling functionalities.
    /// </summary>
    public class LevelManager : MonoBehaviour {
    
        #if UNITY_5_3_OR_NEWER

        /// <summary>
        /// Currently active scene.
        /// </summary>
        public Scene level {
            get {
                return SceneManager.GetActiveScene();
            }
        }    
        
        /// <summary>
        /// Loaded levels.
        /// </summary>
        public List<Scene> levels {
            get {
                List<Scene> res = new List<Scene>();                
                for(int i=0;i<levelNames.Count;i++) { Scene it = SceneManager.GetSceneByName(levelNames[i]); if(it.IsValid()) res.Add(it); }
                return res;
            }
        }
           
        #endif

        /// <summary>
        /// Currently active scene id
        /// </summary>
        public int levelId {
            get {
                #if UNITY_5_3_OR_NEWER
                return level.buildIndex;
                #else
                return Application.loadedLevel;
                #endif                
            }
        }

        /// <summary>
        /// Currently active scene name
        /// </summary>
        public string levelName {
            get {
                #if UNITY_5_3_OR_NEWER
                return level.name;
                #else
                return Application.loadedLevelName;
                #endif                
            }
        }

        /// <summary>
        /// All loaded levels.
        /// </summary>
        public List<string> levelNames;


        /// <summary>
        /// Manager event handler.
        /// </summary>
        public LevelManagerCallback OnEvent;

        /// <summary>
        /// Table of loaders.
        /// </summary>
        internal Dictionary<string,AsyncOperation> m_loaders;
        internal Dictionary<string,Action<LevelEvent>> m_callbacks;
        internal List<string> m_gc;

        /// <summary>
        /// CTOR.
        /// </summary>
        protected void Awake() {
            m_loaders = new Dictionary<string, AsyncOperation>();
            m_callbacks = new Dictionary<string, Action<LevelEvent>>();
            m_gc = new List<string>();
            
            levelNames = new List<string>();

        }

        /// <summary>
        /// Loads a level synchronously.
        /// </summary>
        /// <param name="p_name"></param>
        public void LoadLevel(string p_name) {

            SendEvent(p_name,LevelEventType.Progress,0f);
            SendEvent(p_name,LevelEventType.Progress,1f);
            SendEvent(p_name,LevelEventType.Complete,1f);            

            #if UNITY_5_3_OR_NEWER
            SceneManager.LoadScene(p_name, LoadSceneMode.Single);
            #else
            Application.LoadLevel(p_name);
            #endif
        }

        /// <summary>
        /// Adds a level synchronously
        /// </summary>
        /// <param name="p_name"></param>
        public void AddLevel(string p_name) {

            SendEvent(p_name,LevelEventType.Progress,0f);            
            #if UNITY_5_3_OR_NEWER
            SceneManager.LoadScene(p_name, LoadSceneMode.Additive);            
            #else
            Application.LoadLevel(p_name); 
            #endif
            levelNames.Add(p_name);
            SendEvent(p_name,LevelEventType.Progress,1f);
            SendEvent(p_name,LevelEventType.Complete,1f);            
        }

        /// <summary>
        /// Loads a level asynchronously.
        /// </summary>
        /// <param name="p_name"></param>
        /// <param name="p_callback"></param>
        public void LoadLevelAsync(string p_name,Action<LevelEvent> p_callback=null) {
            AsyncOperation op = null;
            SendEvent(p_name,LevelEventType.Progress,0f,p_callback);
            #if UNITY_5_3_OR_NEWER
            op = SceneManager.LoadSceneAsync(p_name, LoadSceneMode.Single);
            #else
            op = Application.LoadLevelAsync(p_name);
            #endif            
            m_loaders[p_name]   = op;
            m_callbacks[p_name] = p_callback;                        
        }

        /// <summary>
        /// Adds a level asynchronously.
        /// </summary>
        /// <param name="p_name"></param>
        /// <param name="p_callback"></param>
        public void AddLevelAsync(string p_name,Action<LevelEvent> p_callback=null) {
            AsyncOperation op = null;
            SendEvent(p_name,LevelEventType.Progress,0f,p_callback);
            #if UNITY_5_3_OR_NEWER
            op = SceneManager.LoadSceneAsync(p_name, LoadSceneMode.Additive);
            #else
            op = Application.LoadLevelAdditiveAsync(p_name);
            #endif            
            m_loaders[p_name]   = op;
            m_callbacks[p_name] = p_callback;                        
        }

        /// <summary>
        /// Unloads a scene loaded by this manager.
        /// </summary>
        public void Unload(string p_name) {        
            if(levelNames.IndexOf(p_name)<0) return;   
            levelNames.Remove(p_name);
            #if UNITY_5_3_OR_NEWER
            SceneManager.UnloadScene(p_name);
            #else            
            Application.UnloadLevel(p_name);
            #endif                        
        }

        /// <summary>
        /// Unload all levels loaded with this manager.
        /// </summary>
        public void UnloadAll() {
            for(int i=0;i<levelNames.Count;i++) { Unload(levelNames[i]); }
        }
        
        /// <summary>
        /// Updates the loading status of async ops.
        /// </summary>
        protected void Update() {
        
            if(m_loaders==null)     m_loaders   = new Dictionary<string, AsyncOperation>();
            if(m_callbacks==null)   m_callbacks = new Dictionary<string, Action<LevelEvent>>();

            //Iterate active level loadings
            foreach(KeyValuePair<string,AsyncOperation> it in m_loaders) {
                string n              = it.Key;    
                AsyncOperation op     = it.Value;                
                Action<LevelEvent> cb = m_callbacks.ContainsKey(n) ? m_callbacks[n] : null;

                if(op==null) continue;

                SendEvent(n, LevelEventType.Progress,op.progress*0.999f,cb);
                
                if(op.isDone) {
                    if(levelNames.IndexOf(n)<0) {
                        levelNames.Add(n);
                        SendEvent(n, LevelEventType.Progress,1f,cb);                    
                        SendEvent(n, LevelEventType.Complete,1f,cb);                    
                        //Add key to be removed later
                        //m_gc.Add(n);
                    }
                }
            }

            //Remove loading steps already completed.
            if(m_gc.Count>0) {
                for(int i=0;i<m_gc.Count;i++) {
                    m_loaders.Remove(m_gc[i]);
                    m_callbacks.Remove(m_gc[i]);
                }
                m_gc.Clear();
            }

        }
	    
        /// <summary>
        /// Dispatches a LevelEvent to the desired targets.
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_progress"></param>
        /// <param name="p_callback"></param>
        private void SendEvent(string p_name,LevelEventType p_type,float p_progress=0f,Action<LevelEvent> p_callback=null) {
            LevelEvent ev = new LevelEvent(p_name,p_type,this,p_progress);
            if(OnEvent!=null)OnEvent.Invoke(ev);
            if(p_callback!=null) p_callback(ev);
        }

        //*/

    }

    /// <summary>
    /// Delegate that describes the event method.
    /// </summary>
    /// <param name="p_event"></param>
    public delegate void LevelManagerDelegate(LevelEvent p_event);
    
    /// <summary>
    /// Class that extends UnityEvent for editor purposes.
    /// </summary>
    [System.Serializable]
    public class LevelManagerCallback : UnityEvent<LevelEvent> { }

    #region enum LevelEventType

    /// <summary>
    /// Level Event Type enumeration.
    /// </summary>
    public enum LevelEventType
    {
        /// <summary>
        /// None
        /// </summary>
        Progress = 0,
        /// <summary>
        /// Down
        /// </summary>
        Complete     
    }

    #endregion

    #region class Event

    /// <summary>
    /// Class that implements LevelEvent data.
    /// </summary>
    [System.Serializable]
    public class LevelEvent {
        /// <summary>
        /// Event type.
        /// </summary>
        public LevelEventType type;

        /// <summary>
        /// Target.
        /// </summary>
        public LevelManager target;

        /// <summary>
        /// Name of the level.
        /// </summary>
        public string name;

        /// <summary>
        /// Progress of loading.
        /// </summary>
        public float progress;

        /// <summary>
        /// CTOR.
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_manager"></param>
        public LevelEvent(string p_name,LevelEventType p_type,LevelManager p_target,float p_progress=0f) {
            name     = p_name;
            type     = p_type;
            target   = p_target;
            progress = p_progress;
        }

    }

    #endregion    


}
