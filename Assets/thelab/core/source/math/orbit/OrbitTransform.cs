using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Assembly-CSharp-Editor")]

namespace thelab.core {

    /// <summary>
    /// Class that implements an orbit camera that rotates around a pivot and keeps a defined distance from it.
    /// </summary>
    public class OrbitTransform : MonoBehaviour {

        /// <summary>
        /// Smoothing flags.
        /// </summary>
        public enum Transition {
            None = 0, 

            Snap = 585,
            Lerp = 1170,
            Move = 2340,

            DistanceSnap = 1,
            DistanceLerp = 2,
            DistanceMove = 4,
            DistanceMask = 7,

            AngleSnap    = 8,
            AngleLerp    = 16,
            AngleMove    = 32,
            AngleMask    = 56,

            AnchorSnap   = 64,
            AnchorLerp   = 128,
            AnchorMove   = 256,
            AnchorMask   = 448,

            AnchorRotationSnap   = 512,
            AnchorRotationLerp   = 1024,
            AnchorRotationMove   = 2048,
            AnchorRotationMask   = 3584,
            
        }

        /// <summary>
        /// Numeric data for the orbit attribs.
        /// </summary>
        [System.Serializable]
        public struct Data {
            public float distance;
            public float angle;
            public float anchor;
            public float rotation;
        }

        /// <summary>
        /// Distance from pivot.
        /// </summary>
        public float distance {
            get { return m_distance; }
            set { m_next_distance = value; Refresh(); }
        }
        [SerializeField]
        private float m_distance;
        [SerializeField]
        private float m_next_distance;
        
        /// <summary>
        /// World position of the pivot.
        /// </summary>
        public Vector3 anchor {
            get { return m_anchor;  }
            set { m_next_anchor = value; Refresh(); }
        }
        [SerializeField]
        private Vector3 m_anchor;
        [SerializeField]
        private Vector3 m_next_anchor;
        
        /// <summary>
        /// Local position of the pivot.
        /// </summary>
        public Vector3 localAnchor {
            get { return transform.parent ? transform.parent.InverseTransformPoint(anchor) : anchor;  }
            set { Vector3 p = value; p = transform.parent ? transform.parent.TransformPoint(p) : p; anchor = p; }
        }

        /// <summary>
        /// Orbit angle of the camera.
        /// </summary>
        public Vector2 angle {
            get { return m_angle;  }
            set { m_next_angle = value; Refresh(); }
        }
        [SerializeField]
        private Vector2 m_angle;
        [SerializeField]
        private Vector2 m_next_angle;

        /// <summary>
        /// Starting rotation
        /// </summary>
        public Quaternion anchorRotation {
            get { return m_anchorRotation;  }
            set { m_next_anchorRotation = value; Refresh(); }
        }
        [SerializeField]
        private Quaternion m_anchorRotation;
        [SerializeField]
        private Quaternion m_next_anchorRotation;

        /// <summary>
        /// Starting rotation in euler angles.
        /// </summary>
        public Vector3 anchorEulerAngles {
            get { return m_anchorRotation.eulerAngles;  }
            set { anchorRotation = Quaternion.Euler(value); Refresh(); }
        }
        
        /// <summary>
        /// Smooth flag
        /// </summary>
        public Transition transition;
        
        /// <summary>
        /// List of transition flags to apply.
        /// </summary>    
        [SerializeField]
        internal Transition[] transitions;

        /// <summary>
        /// Speed of smoothing.
        /// </summary>
        public Data speed;

        /// <summary>
        /// Internal flag for first run.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        internal bool m_init = false;

        /// <summary>
        /// Internal flag for dirty data.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        internal bool m_dirty = false;

        /// <summary>
        /// First run to setup using base values.
        /// </summary>
        internal void Init() {
            if(m_init) return;
            m_init           = true;
            m_anchor         = transform.position;
            m_anchorRotation = transform.localRotation;
            m_distance       = 1f;
            m_angle          = Vector2.zero;

            m_dirty          = true;

            transition       = Transition.Snap;

            speed = new Data();
            speed.distance  = 1f;
            speed.angle     = 1f;
            speed.distance  = 1f;
            speed.anchor    = 1f;
            speed.rotation  = 1f;

            Refresh();
        }

        /// <summary>
        /// CTOR.
        /// </summary>
        virtual protected void Awake() {
            //transition = Transition.None;
            //SetTransition(true,transitions);
        }

        /// <summary>
        /// Resets this orbit's orientation and snap the current rotation as anchorRotation.
        /// </summary>
        public void Snap(bool p_position=true,bool p_angle=true) {
            Transition t = transition;

            transition = Transition.Snap;

            if(p_angle) {
                m_anchorRotation = m_next_anchorRotation  = transform.localRotation;                
                m_angle = m_next_angle = Vector2.zero;                
            }

            if(p_position) {
                m_next_distance = m_distance;
                m_anchor = m_next_anchor = transform.position - (transform.forward * -m_distance);                
            }            
            
            transition = t;
        }
        
        /// <summary>
        /// Checks if a given smoothing is enabled
        /// </summary>
        /// <param name="p_flag"></param>
        /// <returns></returns>
        public bool IsTransitionEnabled(Transition p_flag) { return (p_flag & transition)!=0; }

        /// <summary>
        /// Set the flags in the smooth enum.
        /// </summary>
        /// <param name="p_flags"></param>
        public void SetTransition(bool p_value,params Transition[] p_flags) {
            Transition f = transition;
            for(int i=0;i<p_flags.Length;i++) f = p_value ? (f | p_flags[i]) : (f & (~p_flags[i]));
            transition = f;
        }

        /// <summary>
        /// Updates the orbit transform.
        /// </summary>
        protected void Refresh() {
            //if(Application.isPlaying) { m_dirty = true; return; }
            m_dirty = true;
            ApplyRefresh();
        }

        /// <summary>
        /// Applies the orbit data.
        /// </summary>
        protected void ApplyRefresh() {
            if(!m_dirty) return;
            
            Vector3 v_up     = Vector3.up;
            Vector3 v_right  = Vector3.right;

            Transition csflag   = transition;
            
            //If not playing snap all
            if(!Application.isPlaying) csflag = Transition.Snap;

            m_dirty = false;

            Transition sflag    = Transition.None;            
            float bias      = 0.01f;

            sflag = csflag & Transition.DistanceMask;
            
            switch(sflag) {
                case Transition.DistanceSnap: m_distance = m_next_distance;                                                               break;
                case Transition.DistanceLerp: m_distance = Mathf.Lerp(m_distance,m_next_distance,Time.deltaTime * speed.distance);        break;
                case Transition.DistanceMove: m_distance = Mathf.MoveTowards(m_distance,m_next_distance,Time.deltaTime * speed.distance); break;
            }
            if(IsDirty(m_distance,m_next_distance,bias)) { m_dirty=true; } else { m_distance=m_next_distance;  }

            sflag = csflag & Transition.AngleMask;
            switch(sflag) {
                case Transition.AngleSnap: m_angle = m_next_angle;                                                           break;
                case Transition.AngleLerp: m_angle = Vector2.Lerp(m_angle,m_next_angle,Time.deltaTime * speed.angle);        break;
                case Transition.AngleMove: m_angle = Vector2.MoveTowards(m_angle,m_next_angle,Time.deltaTime * speed.angle); break;
            }
            if(IsDirty(m_angle,m_next_angle,bias)) { m_dirty=true; } else { m_angle=m_next_angle;  }

            sflag = csflag & Transition.AnchorMask;
            switch(sflag) {
                case Transition.AnchorSnap: m_anchor = m_next_anchor;                                                          break;
                case Transition.AnchorLerp: m_anchor = Vector3.Lerp(m_anchor,m_next_anchor,Time.deltaTime * speed.angle);        break;
                case Transition.AnchorMove: m_anchor = Vector3.MoveTowards(m_anchor,m_next_anchor,Time.deltaTime * speed.angle); break;
            }
            if(IsDirty(m_anchor,m_next_anchor,bias)) { m_dirty=true; } else { m_anchor=m_next_anchor;  }

            //Safeguard
            if(Mathf.Abs(m_next_anchorRotation.w) <= Mathf.Epsilon) {                
                m_next_anchorRotation.w = 0.00001f;
            }

            sflag = csflag & Transition.AnchorRotationMask;
            switch(sflag) {
                case Transition.AnchorRotationSnap: m_anchorRotation = m_next_anchorRotation;                                                                         break;
                case Transition.AnchorRotationLerp: m_anchorRotation = Quaternion.Lerp(m_anchorRotation,m_next_anchorRotation,Time.deltaTime * speed.angle);          break;
                case Transition.AnchorRotationMove: m_anchorRotation = Quaternion.RotateTowards(m_anchorRotation,m_next_anchorRotation,Time.deltaTime * speed.angle); break;
            }
            if(IsDirty(m_anchorRotation,m_next_anchorRotation,bias)) { m_dirty=true; } else { m_anchorRotation=m_next_anchorRotation;  }
            
            Quaternion q = Quaternion.AngleAxis(m_angle.x, v_up);
            q = m_anchorRotation * q * Quaternion.AngleAxis(m_angle.y, v_right);        
            transform.localRotation = q;
            
            Vector3 ap = m_anchor;            
            transform.position = ap + transform.forward * -m_distance;
            
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Update() {
            if(m_dirty)ApplyRefresh();
        }

        /// <summary>
        /// Draws this element's gizmos.
        /// </summary>
        protected void OnDrawGizmos() {            
            Gizmos.color = new Color(0.5f,0.5f,1f);
            Gizmos.DrawSphere(anchor,0.08f);
            Gizmos.color = new Color(0.3f,0.8f,0.3f,0.5f);
            Gizmos.DrawLine(anchor,transform.position);
        }

        /// <summary>
        /// Helper to check if 2 values are 'bias' different.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="bias"></param>
        /// <returns></returns>
        private bool IsDirty(float a,float b,float bias) { return Mathf.Abs(a-b) >= bias; }

        /// <summary>
        /// Helper to check if 2 values are 'bias' different.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="bias"></param>
        /// <returns></returns>
        private bool IsDirty(Vector3 a,Vector3 b,float bias) {
            if(IsDirty(a.x,b.x,bias)) return true;
            if(IsDirty(a.y,b.y,bias)) return true;
            if(IsDirty(a.z,b.z,bias)) return true;            
            return false;
        }

        /// <summary>
        /// Helper to check if 2 values are 'bias' different.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="bias"></param>
        /// <returns></returns>
        private bool IsDirty(Quaternion a,Quaternion b,float bias) {
            if(IsDirty(a.x,b.x,bias)) return true;
            if(IsDirty(a.y,b.y,bias)) return true;
            if(IsDirty(a.z,b.z,bias)) return true;            
            if(IsDirty(a.w,b.w,bias)) return true;
            return false;
        }
        
    }

}