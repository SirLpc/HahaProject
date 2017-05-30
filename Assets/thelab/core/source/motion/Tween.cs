using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

namespace thelab.core
{
    /// <summary>
    /// Delegate that is called during animation to be used as interpolation factor of the animated property.
    /// </summary>
    /// <param name="p_args"></param>
    /// <returns></returns>
    public delegate float Easing(float p_args);

    #region class Tween<T>

    /// <summary>
    /// Extension of the Tween class to handle 'T' typed 'from' and 'to' values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class Tween<T> : Tween
    {
        /// <summary>
        /// Starting property value. Sampled after 'OnStart' is called.
        /// </summary>
        public T from;

        /// <summary>
        /// Final property value.
        /// </summary>
        public T to;

        /// <summary>
        /// Data format being tweened.
        /// </summary>
        internal Format format;

        /// <summary>
        /// Creates a new Tween
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_to"></param>
        /// <param name="p_duration"></param>
        /// <param name="p_delay"></param>
        /// <param name="p_easing"></param>
        /// <param name="p_threaded"></param>
        public Tween(object p_target, string p_property,T p_to, float p_duration, float p_delay, Easing p_easing, bool p_threaded) : base(p_target,p_property,p_duration, p_delay, p_easing, p_threaded)
        {
            to = p_to;
            if (typeof(T) == typeof(float))         format = Format.Float;     else
            if (typeof(T) == typeof(Color))         format = Format.Color;     else
            if (typeof(T) == typeof(Vector3))       format = Format.Vector3;   else
            if (typeof(T) == typeof(Vector2))       format = Format.Vector2;   else
            if (typeof(T) == typeof(Quaternion))    format = Format.Quaternion;else
            if (typeof(T) == typeof(Transform))     format = Format.Transform; else
            if (typeof(T) == typeof(Rect))          format = Format.Rect;      else
            if (typeof(T) == typeof(Vector4))       format = Format.Vector4;   else
            if (typeof(T) == typeof(int))           format = Format.Int;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_to"></param>
        /// <param name="p_duration"></param>
        /// <param name="p_delay"></param>
        /// <param name="p_easing"></param>
        public Tween(object p_target, string p_property, T p_to, float p_duration, float p_delay, Easing p_easing) : this(p_target, p_property, p_to, p_duration, p_delay, p_easing, false) { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_to"></param>
        /// <param name="p_duration"></param>
        /// <param name="p_easing"></param>
        public Tween(object p_target, string p_property, T p_to, float p_duration,Easing p_easing) : this(p_target, p_property, p_to, p_duration, 0f, p_easing, false) { }
        
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_to"></param>
        /// <param name="p_easing"></param>
        public Tween(object p_target, string p_property, T p_to, Easing p_easing) : this(p_target, p_property, p_to,DefaultDuration, 0f, p_easing, false) { }

        /// <summary>
        /// Initialize the tween fetching the data from 'target'.
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();
            if (valid)
            {
                KillDuplicates(this);
                switch (type)
                {
                    case Target.Transform:
                    case Target.Component:
                    case Target.Default:
                        from = Reflection.Get<T>(target, property);
                        break;

                    case Target.Material:
                        Material m = (Material)target;
                        switch(format)
                        {
                            case Format.Float:   from = (T)(object)m.GetFloat(property);  break;
                            case Format.Int:     from = (T)(object)m.GetInt(property);    break;
                            case Format.Color:   from = (T)(object)m.GetColor(property);  break;
                            case Format.Vector4: from = (T)(object)m.GetVector(property); break;
                        }
                        break;
                }
            }
        }

    }

    #endregion

    #region class Tween

    /// <summary>
    /// Class that handles a single activity running in the main pool or a thread.
    /// </summary>
    [System.Serializable]
    public class Tween : Timer
    {
        #region static

        /// <summary>
        /// Default duration when none is specified.
        /// </summary>
        internal const float DefaultDuration = 0.25f;

        /// <summary>
        /// Returns all tweens.
        /// </summary>
        static public List<Tween> all {
            get {
                List<Activity> l = ActivityManager.instance.activities;
                List<Tween> res = new List<Tween>();
                for (int i = 0; i < l.Count; i++) {
                    if (!(l[i] is Tween)) continue;
                    res.Add((Tween)l[i]);
                }
                return res;
            }
        }

        /// <summary>
        /// Returns all active tweens (the ones where the 'delay' has already passed.
        /// </summary>
        static public List<Tween> allRunning {
            get {
                List<Activity> l = ActivityManager.instance.activities;
                List<Tween> res = new List<Tween>();
                for (int i = 0; i < l.Count; i++) {
                    if (!(l[i] is Tween)) continue;
                    Tween t = (Tween)l[i];
                    if (t.elapsed >= 0f) res.Add(t);
                }
                return res;
            }
        }

        /// <summary>
        /// Creates and execute a new Tween.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_to"></param>
        /// <param name="p_duration"></param>
        /// <param name="p_delay"></param>
        /// <param name="p_easing"></param>
        /// <param name="p_threaded"></param>
        /// <returns></returns>
        static public Tween Add<T>(object p_target,string p_property,T p_to,float p_duration,float p_delay,Easing p_easing,bool p_threaded)
        {
            Tween t=null;
            if (typeof(T) == typeof(float))         t = new FloatTween(p_target,p_property,(float)(object)p_to,p_duration,p_delay,p_easing,p_threaded); else
            if (typeof(T) == typeof(Color))         t = new ColorTween(p_target,p_property,(Color)(object)p_to,p_duration,p_delay,p_easing,p_threaded); else
            if (typeof(T) == typeof(Vector3))       t = new Vector3Tween(p_target,p_property,(Vector3)(object)p_to,p_duration,p_delay,p_easing,p_threaded); else
            if (typeof(T) == typeof(Vector2))       t = new Vector2Tween(p_target, p_property, (Vector2)(object)p_to, p_duration, p_delay, p_easing, p_threaded); else
            if (typeof(T) == typeof(Quaternion))    t = new QuaternionTween(p_target,p_property,(Quaternion)(object)p_to,p_duration,p_delay,p_easing,p_threaded); else
            if (typeof(T) == typeof(Transform))     t = new TransformTween(p_target,p_property,(Transform)(object)p_to,p_duration,p_delay,p_easing,p_threaded); else
            if (typeof(T) == typeof(Rect))          t = new RectTween(p_target,p_property,(Rect)(object)p_to,p_duration,p_delay,p_easing,p_threaded); else
            if (typeof(T) == typeof(Vector4))       t = new Vector4Tween(p_target,p_property,(Vector4)(object)p_to,p_duration,p_delay,p_easing,p_threaded); else
            if (typeof(T) == typeof(int))           t = new IntTween(p_target,p_property,(int)(object)p_to,p_duration,p_delay,p_easing,p_threaded);
            if (t != null) t.Start(); 
            else
            {
                Debug.LogWarning("Tween> Failed to create tween! type[" + typeof(T).Name + "] target[" + p_target.GetType().Name + "]");
            }
            return t;
        }

        /// <summary>
        /// Creates and execute a new Tween.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_to"></param>
        /// <param name="p_duration"></param>
        /// <param name="p_delay"></param>
        /// <param name="p_easing"></param>
        /// <returns></returns>
        static public Tween Add<T>(object p_target, string p_property, T p_to, float p_duration, float p_delay, Easing p_easing) { return Add<T>(p_target, p_property, p_to, p_duration, p_delay, p_easing, false); }

        /// <summary>
        /// Creates and execute a new Tween.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_to"></param>
        /// <param name="p_duration"></param>
        /// <param name="p_easing"></param>
        /// <returns></returns>
        static public Tween Add<T>(object p_target, string p_property, T p_to, float p_duration, Easing p_easing) { return Add<T>(p_target, p_property, p_to, p_duration, 0f, p_easing, false); }

        /// <summary>
        /// Creates and execute a new Tween.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_to"></param>
        /// <param name="p_easing"></param>
        /// <returns></returns>
        static public Tween Add<T>(object p_target, string p_property, T p_to, Easing p_easing) { return Add<T>(p_target, p_property, p_to, DefaultDuration, 0f, p_easing, false); }

        /// <summary>
        /// Stops the Tweens matching the criteria. If no criteria is informed, all Tweens will be killed.
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        static public void Kill(object p_target = null, string p_property = "") {
            List<Activity> l = ActivityManager.instance.activities;
            for (int i = 0; i < l.Count; i++) {
                if (!(l[i] is Tween)) continue;
                Tween t = l[i] as Tween;
                if (p_target != null) if (t.target != p_target) continue;
                if (p_property != "") if (t.property != p_property) continue;
                t.Stop();
            }
        }

        /// <summary>
        /// Kill duplicates tweens already running.
        /// </summary>
        /// <param name="p_target"></param>
        static public void KillDuplicates(Tween p_target) {
            List<Activity> l = ActivityManager.instance.activities;
            Tween t0 = p_target;
            for (int i = 0; i < l.Count; i++) {
                if (!(l[i] is Tween)) continue;
                Tween t1 = l[i] as Tween;
                if (t0 == t1) continue;
                if (t0.target != t1.target) continue;
                if (t0.property != t1.property) continue;
                if (t1.elapsed >= 0f) t1.Stop();
            }
        }

        #endregion
        
        #region enum Target
        
        /// <summary>
        /// Enumeration to define the tween target type.
        /// </summary>
        internal enum Target
        {        
            Default=0,            
            Transform,
            Component,
            Material
        }

        #endregion

        #region enum Format

        /// <summary>
        /// Enumeration to define the type format being tweened.
        /// </summary>
        internal enum Format
        {
            Float=0,
            Int,
            Vector2,
            Vector3,
            Vector4,
            Color,
            Quaternion,
            Transform,
            Rect,
        }

        #endregion

        /// <summary>
        /// Default easing.
        /// </summary>
        internal Easing Linear = delegate(float r) { return r; };

        /// <summary>
        /// Tween target.
        /// </summary>
        public object target;

        /// <summary>
        /// Type of the target.
        /// </summary>
        internal Target type;

        /// <summary>
        /// Property to tween.
        /// </summary>
        public string property;

        /// <summary>
        /// Checks if the tween is possible.
        /// </summary>
        public bool valid;

        /// <summary>
        /// Easing function.
        /// </summary>
        public Easing easing;

        /// <summary>
        /// Callback called when the tween is finished.
        /// </summary>
        public System.Action<Tween> onComplete;

        /// <summary>
        /// Creates a new Tween.
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_duration"></param>
        /// <param name="p_delay"></param>
        /// <param name="p_easing"></param>
        /// <param name="p_threaded"></param>
        public Tween(object p_target,string p_property,float p_duration,float p_delay,Easing p_easing,bool p_threaded) : base(p_duration,p_delay,0,p_threaded)
        {
            target   = p_target;
            property = p_property;            
            easing = p_easing == null ? Linear : p_easing;
            type = Target.Default;
            if (target is Transform) type = Target.Transform; else
            if (target is Component) type = Target.Component; else
            if (target is Material)  type = Target.Material;
            valid = true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_duration"></param>
        /// <param name="p_delay"></param>
        /// <param name="p_easing"></param>
        public Tween(object p_target, string p_property, float p_duration, float p_delay, Easing p_easing) : this(p_target, p_property, p_duration, p_delay, p_easing, false) { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_duration"></param>
        /// <param name="p_easing"></param>
        public Tween(object p_target, string p_property, float p_duration, Easing p_easing) : this(p_target, p_property, p_duration, 0f, p_easing, false) { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_easing"></param>
        public Tween(object p_target, string p_property, Easing p_easing) : this(p_target, p_property, DefaultDuration, 0f, p_easing, false) { }

        /// <summary>
        /// Callback called when the tween starts.
        /// </summary>
        protected override void OnStart() {   
            bool instance_ok = target!=null;
            bool property_ok = false;
            switch(type)
            {       
                case Target.Default:
                case Target.Transform:
                case Target.Component:                                        
                    property_ok = Reflection.Contains(target, property);
                    break;
                case Target.Material:
                    Material m = (Material)target;
                    property_ok = m.HasProperty(property);
                    break;
            }
            valid = property_ok && instance_ok;
            if (!valid) {
                Stop();
                Debug.LogWarning(GetType().Name + "> Invalid! instance[" + instance_ok + "] property[" + property + "][" + property_ok + "]");
            }
        }

        /// <summary>
        /// Handles the Timer execution.
        /// </summary>
        protected override void OnExecute() {
            Lerp(progress * 0.999f);
            base.OnExecute();
        }

        /// <summary>
        /// Callback called when execution is finished.
        /// </summary>
        protected override void OnComplete() {            
            Lerp(1f);
            if(onComplete!=null) onComplete(this);
        }

        /// <summary>
        /// Callback called when interpolation is needed.
        /// </summary>
        /// <param name="p_ratio"></param>
        virtual public void Lerp(float p_ratio) { }
    }

    #endregion
}