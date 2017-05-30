using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

namespace thelab.core
{

    #region FloatTween

    /// <summary>
    /// Extension of the Tween class to handle 'T' typed 'from' and 'to' values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    internal class FloatTween : Tween<float>
    {
        /// <summary>
        /// Creates a new Activity.
        /// </summary>
        /// <param name="p_threaded">Flag that indicates if this thread will run on a thread.</param>
        internal FloatTween(object p_target, string p_property,float p_to, float p_duration, float p_delay, Easing p_easing, bool p_threaded) : base(p_target,p_property,p_to,p_duration, p_delay, p_easing, p_threaded){ }
        internal FloatTween(object p_target, string p_property, float p_to, float p_duration, float p_delay, Easing p_easing) : this(p_target, p_property, p_to, p_duration, p_delay, p_easing, false) { }
        internal FloatTween(object p_target, string p_property, float p_to, float p_duration,Easing p_easing) : this(p_target, p_property, p_to, p_duration, 0f, p_easing, false) { }
        internal FloatTween(object p_target, string p_property, float p_to, Easing p_easing) : this(p_target, p_property, p_to, DefaultDuration, 0f, p_easing, false) { }
        public override void Lerp(float p_ratio) { Reflection.Set(target, property, (float)(from + ((to - from) * easing(p_ratio)))); }
    }

    #endregion

    #region IntTween

    /// <summary>
    /// Extension of the Tween class to handle 'T' typed 'from' and 'to' values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class IntTween : Tween<int>
    {
        /// <summary>
        /// Creates a new Activity.
        /// </summary>
        /// <param name="p_threaded">Flag that indicates if this thread will run on a thread.</param>
        public IntTween(object p_target, string p_property, int p_to, float p_duration, float p_delay, Easing p_easing, bool p_threaded) : base(p_target, p_property, p_to, p_duration, p_delay, p_easing, p_threaded) { }
        public IntTween(object p_target, string p_property, int p_to, float p_duration, float p_delay, Easing p_easing) : this(p_target, p_property, p_to, p_duration, p_delay, p_easing, false) { }
        public IntTween(object p_target, string p_property, int p_to, float p_duration, Easing p_easing) : this(p_target, p_property, p_to, p_duration, 0f, p_easing, false) { }
        public IntTween(object p_target, string p_property, int p_to, Easing p_easing) : this(p_target, p_property, p_to, DefaultDuration, 0f, p_easing, false) { }
        public override void Lerp(float p_ratio) { Reflection.Set(target, property, (int)(from + ((to - from) * easing(p_ratio)))); }
    }

    #endregion

    #region Vector2Tween

    /// <summary>
    /// Extension of the Tween class to handle 'T' typed 'from' and 'to' values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class Vector2Tween : Tween<Vector2>
    {
        /// <summary>
        /// Creates a new Activity.
        /// </summary>
        /// <param name="p_threaded">Flag that indicates if this thread will run on a thread.</param>
        public Vector2Tween(object p_target, string p_property, Vector2 p_to, float p_duration, float p_delay, Easing p_easing, bool p_threaded) : base(p_target, p_property, p_to, p_duration, p_delay, p_easing, p_threaded) { }
        public Vector2Tween(object p_target, string p_property, Vector2 p_to, float p_duration, float p_delay, Easing p_easing) : this(p_target, p_property, p_to, p_duration, p_delay, p_easing, false) { }
        public Vector2Tween(object p_target, string p_property, Vector2 p_to, float p_duration, Easing p_easing) : this(p_target, p_property, p_to, p_duration, 0f, p_easing, false) { }
        public Vector2Tween(object p_target, string p_property, Vector2 p_to, Easing p_easing) : this(p_target, p_property, p_to, DefaultDuration, 0f, p_easing, false) { }
        public override void Lerp(float p_ratio) { Reflection.Set(target, property, (Vector2)(from + ((to - from) * easing(p_ratio)))); }
    }

    #endregion

    #region Vector3Tween

    /// <summary>
    /// Extension of the Tween class to handle 'T' typed 'from' and 'to' values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class Vector3Tween : Tween<Vector3>
    {
        /// <summary>
        /// Creates a new Activity.
        /// </summary>
        /// <param name="p_threaded">Flag that indicates if this thread will run on a thread.</param>
        public Vector3Tween(object p_target, string p_property, Vector3 p_to, float p_duration, float p_delay, Easing p_easing, bool p_threaded) : base(p_target, p_property, p_to, p_duration, p_delay, p_easing, p_threaded) { }
        public Vector3Tween(object p_target, string p_property, Vector3 p_to, float p_duration, float p_delay, Easing p_easing) : this(p_target, p_property, p_to, p_duration, p_delay, p_easing, false) { }
        public Vector3Tween(object p_target, string p_property, Vector3 p_to, float p_duration, Easing p_easing) : this(p_target, p_property, p_to, p_duration, 0f, p_easing, false) { }
        public Vector3Tween(object p_target, string p_property, Vector3 p_to, Easing p_easing) : this(p_target, p_property, p_to, DefaultDuration, 0f, p_easing, false) { }
        public override void Lerp(float p_ratio) { Reflection.Set(target, property, (Vector3)(from + ((to - from) * easing(p_ratio)))); }
    }

    #endregion

    #region Vector4Tween

    /// <summary>
    /// Extension of the Tween class to handle 'T' typed 'from' and 'to' values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class Vector4Tween : Tween<Vector4>
    {
        /// <summary>
        /// Creates a new Activity.
        /// </summary>
        /// <param name="p_threaded">Flag that indicates if this thread will run on a thread.</param>
        public Vector4Tween(object p_target, string p_property, Vector4 p_to, float p_duration, float p_delay, Easing p_easing, bool p_threaded) : base(p_target, p_property, p_to, p_duration, p_delay, p_easing, p_threaded) { }
        public Vector4Tween(object p_target, string p_property, Vector4 p_to, float p_duration, float p_delay, Easing p_easing) : this(p_target, p_property, p_to, p_duration, p_delay, p_easing, false) { }
        public Vector4Tween(object p_target, string p_property, Vector4 p_to, float p_duration, Easing p_easing) : this(p_target, p_property, p_to, p_duration, 0f, p_easing, false) { }
        public Vector4Tween(object p_target, string p_property, Vector4 p_to, Easing p_easing) : this(p_target, p_property, p_to, DefaultDuration, 0f, p_easing, false) { }
        public override void Lerp(float p_ratio) { Reflection.Set(target, property, (Vector4)(from + ((to - from) * easing(p_ratio)))); }
    }

    #endregion

    #region ColorTween

    /// <summary>
    /// Extension of the Tween class to handle 'T' typed 'from' and 'to' values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class ColorTween : Tween<Color>
    {
        /// <summary>
        /// Creates a new Activity.
        /// </summary>
        /// <param name="p_threaded">Flag that indicates if this thread will run on a thread.</param>
        public ColorTween(object p_target, string p_property, Color p_to, float p_duration, float p_delay, Easing p_easing, bool p_threaded) : base(p_target, p_property, p_to, p_duration, p_delay, p_easing, p_threaded) { }
        public ColorTween(object p_target, string p_property, Color p_to, float p_duration, float p_delay, Easing p_easing) : this(p_target, p_property, p_to, p_duration, p_delay, p_easing, false) { }
        public ColorTween(object p_target, string p_property, Color p_to, float p_duration, Easing p_easing) : this(p_target, p_property, p_to, p_duration, 0f, p_easing, false) { }
        public ColorTween(object p_target, string p_property, Color p_to, Easing p_easing) : this(p_target, p_property, p_to, DefaultDuration, 0f, p_easing, false) { }
        public override void Lerp(float p_ratio) { Reflection.Set(target, property, (Color)(from + ((to - from) * easing(p_ratio)))); }
    }

    #endregion

    #region QuaternionTween

    /// <summary>
    /// Extension of the Tween class to handle 'T' typed 'from' and 'to' values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class QuaternionTween : Tween<Quaternion>
    {
        /// <summary>
        /// Creates a new Activity.
        /// </summary>
        /// <param name="p_threaded">Flag that indicates if this thread will run on a thread.</param>
        public QuaternionTween(object p_target, string p_property, Quaternion p_to, float p_duration, float p_delay, Easing p_easing, bool p_threaded) : base(p_target, p_property, p_to, p_duration, p_delay, p_easing, p_threaded) { }
        public QuaternionTween(object p_target, string p_property, Quaternion p_to, float p_duration, float p_delay, Easing p_easing) : this(p_target, p_property, p_to, p_duration, p_delay, p_easing, false) { }
        public QuaternionTween(object p_target, string p_property, Quaternion p_to, float p_duration, Easing p_easing) : this(p_target, p_property, p_to, p_duration, 0f, p_easing, false) { }
        public QuaternionTween(object p_target, string p_property, Quaternion p_to, Easing p_easing) : this(p_target, p_property, p_to, DefaultDuration, 0f, p_easing, false) { }
        public override void Lerp(float p_ratio) { Reflection.Set(target, property, Quaternion.Slerp(from, to, easing(progress))); }
    }

    #endregion

    #region TransformTween

    /// <summary>
    /// Extension of the Tween class to handle 'T' typed 'from' and 'to' values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class TransformTween : Tween<Transform>
    {
        /// <summary>
        /// Initial Transform data.
        /// </summary>
        private Vector3 p;
        private Quaternion r;
        private Vector3 s;

        /// <summary>
        /// Creates a new Activity.
        /// </summary>
        /// <param name="p_threaded">Flag that indicates if this thread will run on a thread.</param>
        public TransformTween(object p_target, string p_property, Transform p_to, float p_duration, float p_delay, Easing p_easing, bool p_threaded) : base(p_target, p_property, p_to, p_duration, p_delay, p_easing, p_threaded) { }
        public TransformTween(object p_target, string p_property, Transform p_to, float p_duration, float p_delay, Easing p_easing) : this(p_target, p_property, p_to, p_duration, p_delay, p_easing, false) { }
        public TransformTween(object p_target, string p_property, Transform p_to, float p_duration, Easing p_easing) : this(p_target, p_property, p_to, p_duration, 0f, p_easing, false) { }
        public TransformTween(object p_target, string p_property, Transform p_to, Easing p_easing) : this(p_target, p_property, p_to, DefaultDuration, 0f, p_easing, false) { }

        /// <summary>
        /// Init callback.
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();
            if(valid) { p = from.position; r = from.rotation; s = from.localScale; }
        }

        /// <summary>
        /// Interpolates the data.
        /// </summary>
        /// <param name="p_ratio"></param>
        public override void Lerp(float p_ratio) 
        {
            Transform t   = from;
            Vector3    p1 = to.position;
            Quaternion r1 = to.rotation;
            Vector3    s1 = to.localScale;
            float      e  = easing(progress);
            t.position   = p + (p1 - p) * e;
            t.localScale = s + (s1 - s) * e;
            t.rotation = Quaternion.Slerp(r, r1, e);            
        }
    }

    #endregion

    #region RectTween

    /// <summary>
    /// Extension of the Tween class to handle 'T' typed 'from' and 'to' values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class RectTween : Tween<Rect>
    {
        /// <summary>
        /// Creates a new Activity.
        /// </summary>
        /// <param name="p_threaded">Flag that indicates if this thread will run on a thread.</param>
        public RectTween(object p_target, string p_property, Rect p_to, float p_duration, float p_delay, Easing p_easing, bool p_threaded) : base(p_target, p_property, p_to, p_duration, p_delay, p_easing, p_threaded) { }
        public RectTween(object p_target, string p_property, Rect p_to, float p_duration, float p_delay, Easing p_easing) : this(p_target, p_property, p_to, p_duration, p_delay, p_easing, false) { }
        public RectTween(object p_target, string p_property, Rect p_to, float p_duration, Easing p_easing) : this(p_target, p_property, p_to, p_duration, 0f, p_easing, false) { }
        public RectTween(object p_target, string p_property, Rect p_to, Easing p_easing) : this(p_target, p_property, p_to, DefaultDuration, 0f, p_easing, false) { }
        public override void Lerp(float p_ratio) 
        {
            Rect f = from;
            float r = easing(p_ratio);
            f.xMin += (to.xMin - f.xMin) * r;
            f.xMax += (to.xMax - f.xMax) * r;
            f.yMin += (to.yMin - f.yMin) * r;
            f.yMax += (to.yMax - f.yMax) * r;
            Reflection.Set(target, property, f);            
        }
    }

    #endregion


}