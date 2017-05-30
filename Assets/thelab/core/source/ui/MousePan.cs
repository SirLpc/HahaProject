using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace thelab.core {

    /// <summary>
    /// Component that allows a RectTransform to be panned by click and drag.
    /// </summary>
    [RequireComponent(typeof(EventComponent))]
    public class MousePan : MonoBehaviour {
        
        /// <summary>
        /// Handler for events.
        /// </summary>
        public EventComponent events;
        
        /// <summary>
        /// Target to be moved.
        /// </summary>
        public RectTransform target;

        /// <summary>
        /// Mouse down position.
        /// </summary>
        protected Vector2 m_mouse_down;
        /// <summary>
        /// Target's down position.
        /// </summary>
        protected Vector2 m_target_down;

        /// <summary>
        /// Flag that tells the mouse is down.
        /// </summary>
        protected bool m_isdown;

        /// <summary>
        /// Reference to the canvas scaler.
        /// </summary>
        protected CanvasScaler m_canvas;
        
        /// <summary>
        /// CTOR.
        /// </summary>
        protected void Awake() {
         events = GetComponent<EventComponent>();
         events.allowed = new UIEventType[] { UIEventType.Down };
         events.OnEvent.AddListener(OnEvent);
         Transform it = transform;
            while(it) {
                m_canvas = it.GetComponent<CanvasScaler>();
                if(m_canvas) break;
                it = it.parent;
            }
        }

        /// <summary>
        /// Gets the adjusted mouse position.
        /// </summary>
        /// <returns></returns>
        protected Vector2 GetMousePosition() {
            Vector2 ss  = m_canvas ? m_canvas.referenceResolution : new Vector2(Screen.width,Screen.height);
            Vector2 nmp = Input.mousePosition;
            nmp.x /= Screen.width;
            nmp.y /= Screen.height;
            return new Vector2(nmp.x*ss.x,nmp.y*ss.y);
        }

        /// <summary>
        /// Handler for events.
        /// </summary>
        /// <param name="p_type"></param>
        protected void OnEvent(UIEvent p_event) {
            if(!target) return;                       
            switch(p_event.type) {
                case UIEventType.Down: {
                     m_mouse_down  = GetMousePosition();        
                     m_target_down = target.anchoredPosition;
                     m_isdown      = true;
                }
                break;                
            }
        }

        /// <summary>
        /// Movement update
        /// </summary>
        protected void Update() {
            if(m_isdown) {
                if(Input.GetKeyUp(KeyCode.Mouse0)) { m_isdown = false; return; }
                Vector2 mp = GetMousePosition();
                Vector2 delta = mp - m_mouse_down;                      
                target.anchoredPosition = m_target_down + delta;
            }
        }

    }

}