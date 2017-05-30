using UnityEngine;
using System.Collections;

namespace thelab.core
{
    
    /// <summary>
    /// Class that implements a spline system using the underlying hierarchy.
    /// </summary>
    public class SplineComponent : MonoBehaviour {
       
        /// <summary>
        /// Type of curve to be used.
        /// </summary>
        public SplineType type = SplineType.Catmull;

        /// <summary>
        /// Flag that enabled/disable the gizmo rendering.
        /// </summary>
        public bool gizmo = true;
        public bool gizmoAxis = false;

        
        /// <summary>
        /// Splines to interpolate properties.
        /// </summary>        
        public Spline<Vector3> positions { get { bool d = (m_positions == null); m_positions = d ? (m_positions = new Spline<Vector3>(type, 0)) : m_positions; if (d)Refresh(); return m_positions; } }
        private Spline<Vector3> m_positions;
        public Spline<Vector3> rotations { get { bool d = (m_rotations == null); m_rotations = d ? (m_rotations = new Spline<Vector3>(type, 0)) : m_rotations; if (d)Refresh(); return m_rotations; } }
        private Spline<Vector3> m_rotations;
        public Spline<Vector3> ups { get { bool d = (m_ups == null); m_ups = d ? (m_ups = new Spline<Vector3>(type, 0)) : m_ups; if (d)Refresh(); return m_ups; } }
        private Spline<Vector3> m_ups;
        public Spline<Vector3> scales { get { bool d = (m_scales == null); m_scales = d ? (m_scales = new Spline<Vector3>(type, 0)) : m_scales; if (d)Refresh(); return m_scales; } }
        private Spline<Vector3> m_scales;

        internal int m_rev=0;

        /// <summary>
        /// Init.
        /// </summary>
        protected void Awake() {
            Refresh();
        }

        /// <summary>
        /// Fills the splines with new values.
        /// </summary>
        public void Refresh() {

            Transform[] t = new Transform[transform.childCount];
            
            for (int i = 0; i < t.Length; i++) t[i] = transform.GetChild(i);
            Vector3[]    vp = new Vector3[t.Length];    for (int i = 0; i < t.Length; i++) vp[i] = t[i].position;   positions.values = vp;
            Vector3[]    vr = new Vector3[t.Length];    for (int i = 0; i < t.Length; i++) vr[i] = t[i].localEulerAngles;   rotations.values = vr;
            Vector3[]    vs = new Vector3[t.Length];    for (int i = 0; i < t.Length; i++) vs[i] = t[i].localScale; scales.values    = vs;
            Vector3[]    us = new Vector3[t.Length];    for (int i = 0; i < t.Length; i++) us[i] = t[i].up; ups.values = us;

            m_rev++;
        }

        /// <summary>
        /// Renders the spline gizmos.
        /// </summary>
        void OnDrawGizmos() {            

            if(gizmo) {

                Spline<Vector3> ps = positions;
                Spline<Vector3> us = ups;
                
                if (ps.values.Length <= 1) return;
                Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
                for (int i = 1; i < ps.values.Length; i++) {
                    Vector3 p0 = ps.values[i - 1];
                    Vector3 p1 = ps.values[i];
                    Gizmos.DrawLine(p0,p1);
                }

                float sample_count = (ps.length / 10f) * 500f;
                float step = ps.length / Mathf.Min(500f, sample_count);
                int k = 0;
                for (float p = step; p <= ps.length;p+=step) {
                    Vector3 p0 = ps.Get(p-step);
                    Vector3 p1 = ps.Get(p);
                    bool f;
                    f = ((k / 3) & 1) == 0;
                    Color c = f ? Color.yellow : Color.black;
                    Gizmos.color = c;
                    Gizmos.DrawLine(p0, p1);

                    f = (k%2) == 0;
                    if(f) {
                        if (gizmoAxis) {
                            Vector3 vz = (p1 - p0).normalized;
                            Vector3 vy = us.GetNormalized(p / ps.length, true).normalized;
                            Vector3 vx = Vector3.Cross(vy, vz);
                            Vector3 vp;
                            Vector3 mid = (p0 + p1) * 0.5f;
                            vp = mid + vy * 0.3f; Gizmos.color = Color.green; Gizmos.DrawLine(mid, vp);
                            vp = mid + vx * 0.3f; Gizmos.color = Color.red; Gizmos.DrawLine(mid, vp);
                        }
                    }

                    k++;
                }

                Refresh();
            }
        }
    }
}