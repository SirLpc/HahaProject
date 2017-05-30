using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace thelab.core {

        #region struct ClipLoopInterval 

        /// <summary>
        /// Loop interval descriptior.
        /// </summary>
        [System.Serializable] 
        public struct ClipLoopInterval {

            /// <summary>
            /// Flag that tells the loop interval is in seconds otherwise in frames.
            /// </summary>
            public bool time;

            /// <summary>
            /// Start
            /// </summary>
            public float start;

            /// <summary>
            /// End
            /// </summary>
            public float end;

            /// <summary>
            /// Number of repetions.
            /// </summary>
            public int count;

            /// <summary>
            /// Flag that tells the time switch on loop will be reversing playback.
            /// </summary>
            public bool pingpong;
            
        }

        #endregion

    /// <summary>
    /// Component that implements a Texture based movie clip.
    /// </summary>
    public class MovieClip<T,U> : MonoBehaviour {
        
        /// <summary>
        /// Reference to the target to be animated.
        /// </summary>
        public U target;

        /// <summary>
        /// List of frames
        /// </summary>
        public T[] frames;

        /// <summary>
        /// List of loop intervals.
        /// </summary>        
        public ClipLoopInterval[] loops;
        
        /// <summary>
        /// Frames per seconds.
        /// </summary>
        public int fps = 60;

        /// <summary>
        /// Flag that tells this movie will start playing.
        /// </summary>
        public bool playOnAwake;

        /// <summary>
        /// Flag that indicates the movie is playing.
        /// </summary>
        public bool playing;

        /// <summary>
        /// Flag that tells this clip is paused.
        /// </summary>
        public bool paused;

        /// <summary>
        /// Flag that indicates this clip will use loops.
        /// </summary>
        public bool loop;

        /// <summary>
        /// Flag that tells the playback will be reversed.
        /// </summary>
        public bool reverse;

        /// <summary>
        /// Get/Set the current frame.
        /// </summary>
        public int frame {
            get { return ToFrame(m_elapsed); }
            set { elapsed = ToTime(value); }
        }

        /// <summary>
        /// Number of frames.
        /// </summary>
        public int count {
            get { return frames==null ? 0 : (frames.Length); }
        }
        
        /// <summary>
        /// Elapsed time.
        /// </summary>
        public float elapsed {
            get { return m_elapsed; }
            set { m_elapsed = value; Refresh(); }
        }
        private float m_elapsed;

        /// <summary>
        /// Duration of the clip in seconds.
        /// </summary>
        public float duration {
            get { return ToTime(count); }
        }

        /// <summary>
        /// Playback speed.
        /// </summary>
        public float speed = 1f;
        
        /// <summary>
        /// Seconds per frame.
        /// </summary>
        protected float m_spf { get { return fps<=0 ? 0f : (1f/(float)fps); } }

        /// <summary>
        /// Number of iterations of the current loop.
        /// </summary>
        protected int m_loop_count;
        
        /// <summary>
        /// Internal reverse for ping pong.
        /// </summary>
        protected bool m_internal_reverse;

        /// <summary>
        /// CTOR.
        /// </summary>
        virtual protected void Awake() {
            m_internal_reverse=false;
            if(playOnAwake) Play(-1f);
            Refresh();
        }

        /// <summary>
        /// Start playing the clip.
        /// </summary>
        /// <param name="p_time"></param>
        public void Play(float p_time=0f) {
            if(playing) return;
            playing = true;
            m_internal_reverse  = false;
            m_loop_count        = 0;
            if(p_time>=0f) elapsed = p_time;
        }

        /// <summary>
        /// Stops the playback.
        /// </summary>
        public void Stop() {
            if(!playing) return;            
            m_loop_count        = 0;
            m_internal_reverse  = false;
            playing = false;
            elapsed=0f;
        }

        /// <summary>
        /// Pauses the playback.
        /// </summary>
        public void Pause() { paused=true; }

        /// <summary>
        /// Unpauses the playback.
        /// </summary>
        public void Unpause() { paused=false; }

        /// <summary>
        /// Switches the pause state.
        /// </summary>
        public void PauseSwitch() {
            paused = !paused;
        }
        
        /// <summary>
        /// Set the frames.
        /// </summary>
        /// <param name="p_frames"></param>
        public void Set(T[] p_frames) {
            frames = p_frames;
        }

        /// <summary>
        /// Sorts the frames in forward direction.
        /// </summary>
        public void Sort(System.Comparison<T> p_func) {
            List<T> l = new List<T>(frames);
            l.Sort(p_func);
            frames = l.ToArray();
            l.Clear();
        }

        /// <summary>
        /// Sorts frames forward if they are Unity Objects.
        /// </summary>
        public void SortForward() {
            Sort(delegate(T a, T b) {
                string na="";
                string nb="";
                if(a is Object) { Object oa = (Object)(object)a; na=oa.name; }
                if(b is Object) { Object ob = (Object)(object)b; nb=ob.name; }
                return string.Compare(na,nb);
            });
        }

        /// <summary>
        /// Sorts frames backward if they are Unity Objects.
        /// </summary>
        public void SortBackward() {
            Sort(delegate(T a, T b) {
                string na="";
                string nb="";
                if(a is Object) { Object oa = (Object)(object)a; na=oa.name; }
                if(b is Object) { Object ob = (Object)(object)b; nb=ob.name; }
                return -string.Compare(na,nb);
            });
        }

        /// <summary>
        /// Sorts frames backward if they are Unity Objects.
        /// </summary>
        public void SortRandom() {
            Sort(delegate(T a, T b) { return Random.Range(-1,1); });
        }

        /// <summary>
        /// Updates internal states.
        /// </summary>
        protected void Refresh() {
            int f = frame;
            f = Mathf.Clamp(f,0,count-1);
            OnFrame(frames[f]);
        }
        
        /// <summary>
        /// Steps the animation.
        /// </summary>
        /// <param name="p_dt"></param>
        internal void Step(float p_dt) {
            float dt = p_dt * speed;
            if(reverse) dt = -dt;
            if(m_internal_reverse) dt = -dt;
            float t0 = elapsed;
            float t1 = t0 + dt;
            int loop_id = 0;
            bool has_loop = loop;
            ClipLoopInterval l = GetLoop(t0,out loop_id);
            if(loops.Length>0) if(loop_id<0) has_loop=false;

            if(has_loop) {
                
                float cap    = l.time ? duration : count;
                float lstart = Mathf.Clamp(Mathf.Min(l.start,l.end),0f,cap);
                float lend   = Mathf.Clamp(Mathf.Max(l.start,l.end),0f,cap);

                float l0 = l.time ? lstart : ToTime((int)lstart);
                float l1 = l.time ? lend   : ToTime((int)lend);
                
                bool repeat = false;

                bool inside = (t0>=l0) ? ((t0<=l1) ? true : false) : false;

                int out_of_loop = 0;                    

                if(inside) {                        
                    out_of_loop = (t1<l0) ? -1 : ((t1>l1) ? 1 : 0);                        
                    if(out_of_loop!=0) {
                        m_loop_count++;
                        repeat = (m_loop_count<l.count);
                    }
                }
                    
                if(repeat) {
                    if(l.pingpong) {
                        m_internal_reverse = !m_internal_reverse;
                    }
                    else {
                        t1 = out_of_loop<0 ? l1 : l0; 
                    }
                }
            } 
                       
            elapsed = Mathf.Clamp(t1,0f,duration);
        }
        
        /// <summary>
        /// Updates the playback.
        /// </summary>
        virtual internal void Update() {
            if(paused)      return;
            if(!playing)    return;
            float dt = Time.deltaTime;
            Step(dt);            
        }

        /// <summary>
        /// Callback called when a frame is set during animation.
        /// </summary>
        /// <param name="p_frame"></param>
        virtual protected void OnFrame(T p_frame) { }

        /// <summary>
        /// Retrieves a loop interval based on a given time.
        /// </summary>
        /// <param name="p_time"></param>
        /// <returns></returns>
        protected ClipLoopInterval GetLoop(float p_time,out int p_index) {
            p_index = -1;
            for(int i=0;i<loops.Length;i++) {
                ClipLoopInterval it = loops[i];
                float pos = it.time ? p_time : ((float)ToFrame(p_time));
                if(it.start<=pos) {
                    if(it.end>pos) {
                        p_index = i;
                        return it;
                    }
                }
            }
            ClipLoopInterval full = new ClipLoopInterval();
            full.time  = true;
            full.start = 0f;
            full.end   = duration;
            full.count = 0xffffff;
            return full;
        }

        /// <summary>
        /// Retrieves a loop interval based on a given time.
        /// </summary>
        /// <param name="p_time"></param>
        /// <returns></returns>
        protected ClipLoopInterval GetLoop(float p_time) {
            int pos = 0;
            return GetLoop(p_time,out pos);
        }

        /// <summary>
        /// Converts time in seconds to frame.
        /// </summary>
        /// <param name="p_time"></param>
        /// <returns></returns>
        protected int ToFrame(float p_time) { return Mathf.FloorToInt(((float)fps)*p_time); }

        /// <summary>
        /// Converts frame to time in seconds.
        /// </summary>
        /// <param name="p_time"></param>
        /// <returns></returns>
        protected float ToTime(int p_frame) { return ((float)p_frame) * m_spf; }

    }

}