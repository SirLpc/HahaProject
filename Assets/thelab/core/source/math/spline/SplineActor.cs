using UnityEngine;
using System.Collections;

namespace thelab.core
{
    /// <summary>
    /// Class that allow the GameObject to follow the path of a spline component.
    /// </summary>
    public class SplineActor : MonoBehaviour {
        #region enum Mode

        /// <summary>
        /// Enumeration that describes the properties to be interpolated.
        /// </summary>
        public enum Mode
        {
            Position = 1,
            Rotation = 2,
            Scale = 4,
            PositionRotation = 3,
            PositionScale = 5,
            RotationScale = 6,
            All = 7
        }

        #endregion

        /// <summary>
        /// Reference to the spline.
        /// </summary>
        public SplineComponent spline;

        /// <summary>
        /// Positio of the actor relative to the spline's start.
        /// </summary>
        public float position
        {
            get { return m_position; }
            set { m_position = value; Move(value); }
        }        
        private float m_position;

        /// <summary>
        /// Progress of the actor movement.
        /// </summary>
        public float progress {
            get { if (spline) { return spline.positions.length <= 0f ? 0f : (m_position/spline.positions.length); } return 0f; }
            set { if (spline) { position = spline.positions.length * Mathf.Clamp01(value); } }
        }

        /// <summary>
        /// Sampling wrap mode.
        /// </summary>
        public WrapMode wrap = WrapMode.Clamp;

        /// <summary>
        /// Properties that will be affected.
        /// </summary>
        public Mode mode = Mode.Position;

        /// <summary>
        /// Flag that indicates this actor will move backwards.
        /// </summary>
        public bool reverse;

        /// <summary>
        /// Flag that indicates if the transform will be oriented by the forward movement.
        /// </summary>
        public bool orient = true;

        /// <summary>
        /// Flag that indicates this actor will use the forward vector as velocity.
        /// </summary>
        public bool useForward;

        /// <summary>
        /// Movement speed.
        /// </summary>
        public float speed = 1f;

        /// <summary>
        /// Rotation speed.
        /// </summary>
        public float angularSpeed = 1f;

        /// <summary>
        /// Margin to update the position handler.
        /// </summary>
        public float threshold = 0.25f;

        /// <summary>
        /// Flag that indicates the actor will be snapped to the spline at start.
        /// </summary>
        public bool snap;

        /// <summary>
        /// Automatically move the actor.
        /// </summary>
        public bool auto;

        /// <summary>
        /// Sets the target's position to the assigned spline's position.
        /// </summary>
        /// <param name="p_position"></param>
        public void Move(float p_position) {
            if (!spline) return;
            if ((mode & Mode.Position) != 0) { transform.position         = spline.positions.Get(p_position); }
            if ((mode & Mode.Rotation) != 0) { transform.localEulerAngles = spline.rotations.Get(p_position); }
            if ((mode & Mode.Scale) != 0)    { transform.localScale          = spline.scales.Get(p_position); }
        }

        /// <summary>
        /// Moves this actor in the overral direction of the current spline.
        /// </summary>
        /// <param name="p_position"></param>
        /// <param name="p_speed"></param>
        public void MoveTowards(float p_speed) {
            if (!spline) return;
            Spline<Vector3> vs;                                    
            Transform t = transform;

            vs = spline.positions;

            if (!reverse) {
                if (Mathf.Abs(position - vs.length) <= 0.01f) {
                    if (wrap == WrapMode.Loop)      m_position = 0f; else
                    if (wrap == WrapMode.PingPong)  reverse = true;                    
                    return;
                }
            }
            else {
                if (Mathf.Abs(position) <= 0.01f) {
                    if (wrap == WrapMode.Loop) m_position = vs.length; else
                    if (wrap == WrapMode.PingPong) reverse = false;
                    return;
                }
            }
            
            float r = progress;

            float dt = 0f;

            if ((mode & Mode.Position) != 0) {
                
                Vector3 pos = t.position;
                Vector3 v   = pos;
                dt = vs.MoveTowards(ref v, position, p_speed, threshold);

                if (useForward) {
                    t.position += t.forward * speed;
                }
                else {
                    t.position = v;
                }

                if (orient) {
                    vs = spline.ups;
                    Vector3 vz = (v - pos).normalized;
                    if (vz.magnitude > 0.001f)
                    {
                        Vector3 vy = vs.GetNormalized(r, false);
                        t.rotation = Quaternion.RotateTowards(t.rotation, Quaternion.LookRotation(vz, vy), 360f * angularSpeed * Time.deltaTime);
                    }
                }

            }
            if ((mode & Mode.Rotation) != 0) { vs = spline.rotations; t.localEulerAngles = vs.GetNormalized(r); }
            if ((mode & Mode.Scale) != 0) { vs = spline.scales;       t.localScale = vs.GetNormalized(r,false); }

            m_position += reverse ? -dt : dt;
            m_position = Mathf.Clamp(m_position, 0f, spline.positions.length);            
        }

        /// <summary>
        /// Init.
        /// </summary>
        void Start() {
            if (snap) { progress = reverse ? 1f : 0f; }
        }

        /// <summary>
        /// Updates the Transform.
        /// </summary>
        void Update() {
            if(auto)MoveTowards(speed * Time.deltaTime);
        }

        /// <summary>
        /// Draws the Spline Actor gizmos.
        /// </summary>
        void OnDrawGizmos() {

            if(spline) {
                Vector3 p = spline.positions.Get(position);
                p.y += 1.0f;
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(p, 0.1f);

                p = transform.position;
                Gizmos.color = new Color(1.0f, 0.5f, 0.5f); Gizmos.DrawLine(p, p + transform.right * 0.5f);
                Gizmos.color = new Color(0.5f, 1.0f, 0.5f); Gizmos.DrawLine(p, p + transform.up * 0.5f);
                Gizmos.color = new Color(0.5f, 0.5f, 1.0f); Gizmos.DrawLine(p, p + transform.forward * 0.5f);
            }
        }

    }
}