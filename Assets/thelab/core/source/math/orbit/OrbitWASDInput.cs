using UnityEngine;
using System.Collections;

namespace thelab.core {

    /// <summary>
    /// Class that implements the classic WASD + Mouse input for moving Cameras.
    /// </summary>    
    public class OrbitWASDInput : MonoBehaviour {
    
        /// <summary>
        /// Reference to the Orbit component.
        /// </summary>
        public OrbitTransform orbit { get { return m_orbit ? m_orbit : (m_orbit = GetComponent<OrbitTransform>()); } }
        private OrbitTransform m_orbit;

        /// <summary>
        /// Mouse sensitivity
        /// </summary>
        public float sensitivity = 1f;

        /// <summary>
        /// Speed of flyby.
        /// </summary>
        public float moveSpeed = 3f;

        /// <summary>
        /// Speed aceleration.
        /// </summary>
        [Tooltip("Seconds until Max speed")]
        public float moveAccel = 1f;

        /// <summary>
        /// Multiplier for Shift turbo.
        /// </summary>
        public float moveMultiplier=3f;

        /// <summary>
        /// Speed of scroll distance.
        /// </summary>
        public float scrollSpeed = 0.25f;
        
        /// <summary>
        /// Last sampled mouse position.
        /// </summary>
        private Vector2 m_last_mouse;

        /// <summary>
        /// Last sampled angle.
        /// </summary>
        private Vector2 m_last_angle;

        /// <summary>
        /// Current speed.
        /// </summary>
        private float m_current_speed;

        /// <summary>
        /// CTOR.
        /// </summary>
        protected void Awake() {        
            m_current_speed  = 0f;
            m_last_mouse     = Input.mousePosition;
        }

        /// <summary>
        /// Updates the input.
        /// </summary>
        protected void Update() {
            
            if(!enabled) return;

            bool has_accel = false;

            float mult = 1f;

            if(Input.GetKey(KeyCode.LeftShift)) {
                mult = moveMultiplier;
            }

            if(Input.GetKey(KeyCode.W)) { orbit.anchor +=  orbit.transform.forward *  m_current_speed * Time.deltaTime; has_accel=true;}
            if(Input.GetKey(KeyCode.S)) { orbit.anchor +=  orbit.transform.forward * -m_current_speed * Time.deltaTime; has_accel=true;}
            if(Input.GetKey(KeyCode.A)) { orbit.anchor +=  orbit.transform.right   * -m_current_speed * Time.deltaTime; has_accel=true;}
            if(Input.GetKey(KeyCode.D)) { orbit.anchor +=  orbit.transform.right   *  m_current_speed * Time.deltaTime; has_accel=true;}

			if(Input.GetKey(KeyCode.E)) { orbit.anchor += orbit.transform.up *  m_current_speed * Time.deltaTime;  has_accel = true; } 
			if(Input.GetKey(KeyCode.Q)) { orbit.anchor += orbit.transform.up * -m_current_speed * Time.deltaTime; has_accel = true;  }
            
            if(has_accel) {
                float iaccel = moveAccel<=0f ? 0f : (1f/moveAccel);
                m_current_speed = Mathf.Lerp(m_current_speed,moveSpeed*mult,Time.deltaTime * iaccel);
            }
            else {
                m_current_speed=0f;
            }
            
            if(Input.GetKeyDown(KeyCode.Mouse0)) {
                m_last_mouse = Input.mousePosition;
                m_last_angle = orbit.angle;
            }

            float dx = Input.mousePosition.x - m_last_mouse.x;
            float dy = Input.mousePosition.y - m_last_mouse.y;

            if(Input.GetKey(KeyCode.Mouse0)) {
                Vector2 a = orbit.angle;
                a.x = m_last_angle.x + ( dx * sensitivity);
                a.y = m_last_angle.y + (-dy * sensitivity);                
                orbit.angle = a;
            }	

            if(Mathf.Abs(Input.mouseScrollDelta.y)>0f) {
                orbit.distance += Input.mouseScrollDelta.y<0f ? scrollSpeed : -scrollSpeed;                
            }
            
        }
        
    }

}