using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace thelab.core {
     
    #region enum SplineType

    /// <summary>
    /// Type of spline sampling method.
    /// </summary>
    public enum SplineType {
        Linear=0,        
        Catmull,
        BezierCubic,
    }

    #endregion

    /// <summary>
    /// Class that implements a set of points and interpolation algorithm.
    /// </summary>
    public class Spline<T> {        

        /// <summary>
        /// List of typed values that makes the spline.
        /// </summary>
        public T[] values {

            get { return m_values; }
            set {

                T[] l = (value==null) ? new T[0] : value;                
                m_values  = new T[l.Length];
                m_lengths = new float[l.Length];
                m_weights = new float[l.Length];
                for (int i = 0; i < l.Length; i++) { m_values[i]  = l[i]; }
                Refresh();

            }
        }
        private T[] m_values;
        
        
        /// <summary>
        /// List of weights for each node.
        /// </summary>
        public float[] weights { get { return m_weights; } }
        private float[] m_weights;

        /// <summary>
        /// List of lengths for each section.
        /// </summary>
        public float[] lengths { get { return m_lengths; } }
        private float[] m_lengths;

        /// <summary>
        /// Returns the length of the spline.
        /// </summary>
        public float length { get; private set; }

        /// <summary>
        /// Precision of the length calculation.
        /// </summary>
        public float precision { get { return m_precision; } set { m_precision = value; Refresh(); } }
        private float m_precision;

        /// <summary>
        /// The type of value being manipulated.
        /// </summary>
        public MathType data { get; private set; }

        /// <summary>
        /// Sampling method for this spline.
        /// </summary>
        public SplineType type { get; set; }

        /// <summary>
        /// Samples to be used for the interpolation method.
        /// </summary>
        private T[] m_samples;
        

        /// <summary>
        /// Creates a new spline with 'length' elements. The precision argument specify the lenght calculation's precision.
        /// </summary>
        /// <param name="p_length"></param>
        public Spline(SplineType p_type, int p_length,float p_precision) {

            m_values    = new T[p_length];
            m_weights   = new float[p_length];
            m_lengths = new float[p_length];
            m_samples   = new T[8];
            m_precision = p_precision;
            type        = p_type;
            if (typeof(T) == typeof(float))         data = MathType.Float;     else
            if (typeof(T) == typeof(Color))         data = MathType.Color;     else
            if (typeof(T) == typeof(Vector3))       data = MathType.Vector3;   else
            if (typeof(T) == typeof(Vector2))       data = MathType.Vector2;   else
            if (typeof(T) == typeof(Quaternion))    data = MathType.Quaternion;else
            if (typeof(T) == typeof(Transform))     data = MathType.Transform; else                
            if (typeof(T) == typeof(Rect))          data = MathType.Rect;      else
            if (typeof(T) == typeof(Vector4))       data = MathType.Vector4;   else
            if (typeof(T) == typeof(int))           data = MathType.Int;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="p_length"></param>
        public Spline(SplineType p_type,int p_length) : this(p_type,p_length, 0.01f) {  }
        
        /// <summary>
        /// Samples a value from the spline using normalized coordinates between [0,1]
        /// </summary>
        /// <param name="p_ratio"></param>
        public T GetNormalized(float p_ratio,bool p_use_weights=true) {

            T[] v     = m_values;
            if (v.Length <= 1) return v[0];
            float[] w  = m_weights;
            float r    = Mathf.Clamp01(p_ratio);
          
            if (!p_use_weights) {

                int len = values.Length - 1;
                float p = len * p_ratio;
                float f = p - Mathf.Floor(p);
                SetSamples((int)p);
                return Spline.Lerp<T>(data, type, f, m_samples);

            }
            else {

                int p = 0;                
                for (int i = 1; i < w.Length; i++)
                {
                    if (w[i] >= r) { p = i; break; }                    
                }
                int i0 = Mathf.Clamp(p - 1, 0, w.Length - 1);
                int i1 = Mathf.Clamp(p, 0, w.Length - 1);
                
                float w0 = w[i0];
                float w1 = w[i1];
                float dw = w1 - w0;
                r = Mathf.Clamp01(Mathf.Abs(dw)<=0f ? 1f : ((r - w0) / dw));
                SetSamples((int)(i0));
                return Spline.Lerp<T>(data, type, r, m_samples);

            }
        }

        /// <summary>
        /// Samples the spline using the distance relative from the spline start.
        /// </summary>
        /// <param name="p_distance"></param>
        /// <returns></returns>
        public T Get(float p_distance) { 
            float d = Mathf.Clamp(p_distance, 0f, length);
            return GetNormalized(length<=0f ? 0f : (d/length)); 
        }


        /// <summary>
        /// Get closest node given a distance and its index.
        /// </summary>
        /// <param name="p_distance"></param>
        /// <returns></returns>
        public T GetClosestNode(float p_distance,out int p_index) {
            p_index = -1;
            if(m_values.Length<=0) return default(T);
            T c = Get(p_distance);
            T v = m_values[0];
            T res = v;
            p_index = 0;
            float d = Distance(c,v);
            for(int i=1;i<m_values.Length;i++) {
                v = m_values[i];
                float nd = Distance(c,v);
                if(nd < d) {
                    res = v;
                    d = nd;
                    p_index = i;
                }
            }
            return res;
        }

        /// <summary>
        /// Get closest node given a distance and its index.
        /// </summary>
        /// <param name="p_distance"></param>
        /// <returns></returns>
        public T GetClosestNode(float p_distance) {
            int idx = 0;
            return GetClosestNode(p_distance,out idx);
        }


        /// <summary>
        /// Samples the best next value using as starting point the position.
        /// </summary>
        /// <param name="p_position"></param>
        /// <param name="p_speed"></param>
        /// <returns></returns>
        public float MoveTowards(ref T p_current,float p_position,float p_speed,float p_threshold=0.1f) {            
            float s = p_speed;            
            T c     = p_current;                        
            T tp    = Get(Mathf.Clamp(p_position, 0f, length));
            float d = Distance(c,tp);
            float dt = Mathf.Max(p_threshold - d, 0f);
            switch(data)
            {
                case MathType.Float:      { float v0 = C<float>(c); p_current = C<T>(v0+s); }                              break;
                case MathType.Vector2:    { Vector2 v0 = C<Vector2>(c), v1 = C<Vector2>(tp); p_current = C<T>(v0 + ((v1 - v0).normalized * s)); }   break;
                case MathType.Vector3:    { Vector3 v0 = C<Vector3>(c), v1 = C<Vector3>(tp); p_current = C<T>(v0 + ((v1 - v0).normalized * s)); }   break;
                case MathType.Vector4:    { Vector4 v0 = C<Vector4>(c), v1 = C<Vector4>(tp); p_current = C<T>(v0 + ((v1 - v0).normalized * s)); }   break;
                case MathType.Color:      { Vector4 v0 = C<Vector4>(c), v1 = C<Vector4>(tp); p_current = C<T>(v0 + ((v1 - v0).normalized * s)); }      break;
                case MathType.Quaternion:
                    {
                        Quaternion v0 = C<Quaternion>(c), v1 = C<Quaternion>(tp);
                        Quaternion vc = C<Quaternion>(c);
                        vc.x = v0.x + (v1.x - v0.x) * s;
                        vc.y = v0.y + (v1.y - v0.y) * s;
                        vc.z = v0.z + (v1.z - v0.z) * s;
                        vc.w = v0.w + (v1.w - v0.w) * s;
                        p_current = C<T>(vc);
                    }
                    break;
            }
            return dt;
        }

        /// <summary>
        /// Resizes the spline to the new 'length'.
        /// It transfers the old values up to the new list capacity.
        /// </summary>
        /// <param name="p_length"></param>
        public void Resize(int p_length) {
            T[] nl  = new T[p_length];
            int len = Mathf.Min(nl.Length, m_values.Length);
            for (int i = 0; i < len; i++) nl[i] = m_values[i];
            values = nl;            
        }

        /// <summary>
        /// Updates internal structures of the spline.
        /// </summary>
        private void Refresh() {
            length     = 0;            
            T[] l      = m_values;
            if (l.Length <= 0) return;
            float[] ll = m_lengths;
            float[] wl = m_weights;
            ll[0] = wl[0] = 0f;
            if (l.Length <= 1) return;
            
            float v_size = (float)l.Length;            
            float step   = 1f / (v_size-1f);

            
            for (int i = 1; i < l.Length;i++)
            {
                float fi = (float)i;
                float r0 = (fi-1f)*step;
                float r1 = fi*step;
                //T va = values[i - 1];
                //T vb = values[i];
                float d = 
                //Distance(va, vb);
                MeasureStep(r0,r1, 0.05f);
                length  += d;
                ll[i]    = length;                
                
            }            
            float ws = 0f;            
            for (int i = 0; i < l.Length; i++)
            {
                ws = (length <= 0f) ? 0f : (ll[i] / length);
                wl[i] = ws;
            }
        }

        /// <summary>
        /// Updates the sample list needed for the current spline type.
        /// </summary>
        /// <param name="p_index"></param>
        /// <returns></returns>
        private void SetSamples(int p_index) {
            int len = m_values.Length;

            if (type == SplineType.BezierCubic) {
                p_index = Mathf.FloorToInt(p_index / 3);
            }

            int p   = Mathf.Clamp(p_index, 0, len - 1);
            int pp1 = Mathf.Min(p + 1, len - 1);
            T[] l = m_values, s = m_samples;

            if (type == SplineType.Linear) { s[0] = l[p]; s[1] = l[pp1]; return; }
                        
            int pm1 = Mathf.Max(p - 1, 0);
            
            //if (type == SplineType.Bezier) { s[0] = l[pm1]; s[1] = l[p]; s[2] = l[pp1]; return; }

            int pp2 = Mathf.Min(p + 2, len - 1);

            if (type == SplineType.Catmull) { s[0] = l[pm1]; s[1] = l[p]; s[2] = l[pp1]; s[3] = l[pp2]; return; }
            
            int pp3 = Mathf.Min(p + 3, len - 1);
            //int pm2 = Mathf.Max(p - 2, 0);

            if (type == SplineType.BezierCubic) 
            { 
                s[0] = l[p]; s[1] = l[pp1]; s[2] = l[pp2]; s[3] = l[pp3]; 
                return; 
            }

            
        }

        /// <summary>
        /// Recursive step for measuring the spline.
        /// </summary>
        /// <param name="ra"></param>
        /// <param name="rb"></param>
        /// <returns></returns>
        private float MeasureStep(float ra,float rb,float prec) {   

            if((rb-ra)>prec) {
                float d  = (rb-ra)*0.5f;
                float ma = MeasureStep(ra, rb - d,prec);
                float mb = MeasureStep(ra + d, rb,prec);
                return ma + mb;
            }
            else {
                T va = GetNormalized(ra,false);
                T vb = GetNormalized(rb,false);
                float d = Distance(va, vb);                
                return d;
            }            
        }

        /// <summary>
        /// Returns the distance of two values.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private float Distance(T a,T b) {

            switch(data)
            {                
                case MathType.Float:      { float   va = C<float>(a),   vb = C<float>(b);   return Mathf.Abs(vb - va); }
                case MathType.Vector2:    { Vector2 va = C<Vector2>(a), vb = C<Vector2>(b); return Vector2.Distance(vb,va); }
                case MathType.Vector3:    { Vector3 va = C<Vector3>(a), vb = C<Vector3>(b); return Vector3.Distance(vb, va); }
                case MathType.Vector4:    { Vector4 va = C<Vector4>(a), vb = C<Vector4>(b); return Vector4.Distance(vb, va); }
                case MathType.Quaternion: {

                    Quaternion qa = C<Quaternion>(a), 
                               qb = C<Quaternion>(b);
                    Vector4 va = new Vector4(qa.x, qa.y, qa.z, qa.w);
                    Vector4 vb = new Vector4(qb.x, qb.y, qb.z, qb.w);
                    return  Vector4.Distance(vb, va); 
                }
                case MathType.Color: { return Vector4.Distance(C<Color>(a), C<Color>(b)); }
            }
            return 0f;
        }
        
        /// <summary>
        /// Helper.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="n"></param>
        /// <returns></returns>
        private V C<V>(object n) { return (V)(object)n; }        
    }

    #region class Spline

    /// <summary>
    /// Class that handles the math stuff for splines.
    /// </summary>
    public class Spline
    {
        #region Linear

        static public float      Linear(float      v0, float      v1, float r) { return v0 + (v1 - v0) * r; }
        static public Vector2    Linear(Vector2    v0, Vector2    v1, float r) { return v0 + (v1 - v0) * r; }
        static public Vector3    Linear(Vector3    v0, Vector3    v1, float r) { return v0 + (v1 - v0) * r; }        
        static public Vector4    Linear(Vector4    v0, Vector4    v1, float r) { return v0 + (v1 - v0) * r; }
        static public Color      Linear(Color      v0, Color      v1, float r) { return v0 + (v1 - v0) * r; }
        static public Quaternion Linear(Quaternion v0, Quaternion v1, float r) { return Quaternion.Slerp(v0,v1,r); }

        #endregion

        #region Catmull

        static public float      Catmull(float      v0, float      v1, float       v2, float      v3, float r) { float r2 = r*r, r3 = r2*r;     return 0.5f * ((2f * v1) + (-v0 + v2) * r + (2f * v0 - 5f * v1 + 4f * v2 - v3) * r2 + (-v0 + 3f * v1 - 3f * v2 + v3) * r3); }
        static public Vector2    Catmull(Vector2    v0, Vector2    v1, Vector2     v2, Vector2    v3, float r) { float r2 = r * r, r3 = r2 * r; return 0.5f * ((2f * v1) + (-v0 + v2) * r + (2f * v0 - 5f * v1 + 4f * v2 - v3) * r2 + (-v0 + 3f * v1 - 3f * v2 + v3) * r3); }
        static public Vector3    Catmull(Vector3    v0, Vector3    v1, Vector3     v2, Vector3    v3, float r) { float r2 = r * r, r3 = r2 * r; return 0.5f * ((2f * v1) + (-v0 + v2) * r + (2f * v0 - 5f * v1 + 4f * v2 - v3) * r2 + (-v0 + 3f * v1 - 3f * v2 + v3) * r3); }
        static public Vector4    Catmull(Vector4    v0, Vector4    v1, Vector4     v2, Vector4    v3, float r) { float r2 = r * r, r3 = r2 * r; return 0.5f * ((2f * v1) + (-v0 + v2) * r + (2f * v0 - 5f * v1 + 4f * v2 - v3) * r2 + (-v0 + 3f * v1 - 3f * v2 + v3) * r3); }
        static public Color      Catmull(Color      v0, Color      v1, Color       v2, Color      v3, float r) { return (Color)Catmull((Vector4)v0,(Vector4)v1,(Vector4)v2,(Vector4)v3,r); }
        static public Quaternion Catmull(Quaternion v0, Quaternion v1, Quaternion  v2, Quaternion v3, float r) { Quaternion q; q.x = Catmull(v0.x, v1.x, v2.x, v3.x, r); q.y = Catmull(v0.y, v1.y, v2.y, v3.y, r); q.z = Catmull(v0.z, v1.z, v2.z, v3.z, r); q.w = Catmull(v0.w, v1.w, v2.w, v3.w, r); return q; }

        #endregion

        #region BezierCubic

        static public float      Bezier3(float v0, float v1, float v2, float v3, float r)                     { float omr  = (1.0f - r), omr2 = omr*omr, omr3 = omr2 * omr, r2   = r*r, r3 = r2*r; return (omr3 * v0) + (3.0f * omr2 * r * v1) + (3.0f * omr * r2 * v2) + (r3 * v3); }
        static public Vector2    Bezier3(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, float r)             { float omr  = (1.0f - r), omr2 = omr*omr, omr3 = omr2 * omr, r2   = r*r, r3 = r2*r; return (omr3 * v0) + (3.0f * omr2 * r * v1) + (3.0f * omr * r2 * v2) + (r3 * v3); }
        static public Vector3    Bezier3(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, float r)             { float omr  = (1.0f - r), omr2 = omr*omr, omr3 = omr2 * omr, r2   = r*r, r3 = r2*r; return (omr3 * v0) + (3.0f * omr2 * r * v1) + (3.0f * omr * r2 * v2) + (r3 * v3); }
        static public Vector4    Bezier3(Vector4 v0, Vector4 v1, Vector4 v2, Vector4 v3, float r)             { float omr  = (1.0f - r), omr2 = omr*omr, omr3 = omr2 * omr, r2   = r*r, r3 = r2*r; return (omr3 * v0) + (3.0f * omr2 * r * v1) + (3.0f * omr * r2 * v2) + (r3 * v3); }
        static public Color      Bezier3(Color v0, Color v1, Color v2, Color v3, float r)                     { return (Color)Bezier3((Vector4)v0, (Vector4)v1, (Vector4)v2, (Vector4)v3, r); }
        static public Quaternion Bezier3(Quaternion v0, Quaternion v1, Quaternion v2, Quaternion v3, float r) { Quaternion q; q.x = Bezier3(v0.x, v1.x, v2.x, v3.x, r); q.y = Bezier3(v0.y, v1.y, v2.y, v3.y, r); q.z = Bezier3(v0.z, v1.z, v2.z, v3.z, r); q.w = Bezier3(v0.w, v1.w, v2.w, v3.w, r); return q; }

        #endregion

        #region Lerp
        static public float      Lerp(SplineType p_type, float r, params float[]      v) { switch (p_type) { case SplineType.Catmull: return Catmull(v[0], v[1], v[2], v[3], r);  case SplineType.BezierCubic: return Bezier3(v[0], v[1], v[2], v[3], r); } return Linear(v[0], v[1], r); }
        static public Vector2    Lerp(SplineType p_type, float r, params Vector2[]    v) { switch (p_type) { case SplineType.Catmull: return Catmull(v[0], v[1], v[2], v[3], r);  case SplineType.BezierCubic: return Bezier3(v[0], v[1], v[2], v[3], r); } return Linear(v[0], v[1], r); }
        static public Vector3    Lerp(SplineType p_type, float r, params Vector3[]    v) { switch (p_type) { case SplineType.Catmull: return Catmull(v[0], v[1], v[2], v[3], r);  case SplineType.BezierCubic: return Bezier3(v[0], v[1], v[2], v[3], r); } return Linear(v[0], v[1], r); }
        static public Vector4    Lerp(SplineType p_type, float r, params Vector4[]    v) { switch (p_type) { case SplineType.Catmull: return Catmull(v[0], v[1], v[2], v[3], r);  case SplineType.BezierCubic: return Bezier3(v[0], v[1], v[2], v[3], r); } return Linear(v[0], v[1], r); }
        static public Color      Lerp(SplineType p_type, float r, params Color[]      v) { switch (p_type) { case SplineType.Catmull: return Catmull(v[0], v[1], v[2], v[3], r);  case SplineType.BezierCubic: return Bezier3(v[0], v[1], v[2], v[3], r); } return Linear(v[0], v[1], r); }
        static public Quaternion Lerp(SplineType p_type, float r, params Quaternion[] v) { switch (p_type) { case SplineType.Catmull: return Catmull(v[0], v[1], v[2], v[3], r);  case SplineType.BezierCubic: return Bezier3(v[0], v[1], v[2], v[3], r); } return Linear(v[0], v[1], r); }
        static public T Lerp<T>(MathType p_data,SplineType p_type, float r, params T[] v) 
        {
            switch (p_data) 
            {
                case MathType.Float:      return (T)(object)Lerp(p_type,r, (float)(object)v[0], (float)(object)v[1], (float)(object)v[2], (float)(object)v[3]);
                case MathType.Vector2:    return (T)(object)Lerp(p_type,r, (Vector2)(object)v[0], (Vector2)(object)v[1], (Vector2)(object)v[2], (Vector2)(object)v[3]);
                case MathType.Vector3:    return (T)(object)Lerp(p_type,r, (Vector3)(object)v[0], (Vector3)(object)v[1], (Vector3)(object)v[2], (Vector3)(object)v[3]);
                case MathType.Vector4:    return (T)(object)Lerp(p_type,r, (Vector4)(object)v[0], (Vector4)(object)v[1], (Vector4)(object)v[2], (Vector4)(object)v[3]);
                case MathType.Color:      return (T)(object)Lerp(p_type,r, (Color)(object)v[0], (Color)(object)v[1], (Color)(object)v[2], (Color)(object)v[3]);
                case MathType.Quaternion: return (T)(object)Lerp(p_type, r, (Quaternion)(object)v[0], (Quaternion)(object)v[1], (Quaternion)(object)v[2], (Quaternion)(object)v[3]); 
            }
            return default(T);
        }
        #endregion
    }

    #endregion

}
