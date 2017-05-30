
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace thelab.core {

    /// <summary>
    /// Class that describes a simple component that handles UI events.
    /// Usage: if "move" on, move me anywhere; otherwise, put me on an object which with "DropComponent"!!
    /// </summary>    
    public class DragComponent : EventComponent {

        /// <summary>
        /// Alpha factor of the original.
        /// </summary>
        public float alphaOriginal = 1f;

        /// <summary>
        /// Alpha factor of the copy.
        /// </summary>
        public float alphaCopy = 1f;

        /// <summary>
        /// Drag position.
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// Flag that indicates the dragged object should be moved to the mouse position.
        /// </summary>
        public bool move;
        
        /// <summary>
        /// Reference to the drop element.
        /// </summary>
        [HideInInspector]
        public RectTransform drop;
        
        /// <summary>
        /// Internals.
        /// </summary>        
        //private bool m_is_drag;
        private Canvas m_canvas;
        private RectTransform m_rt;
        private RectTransform m_copy;

        private CanvasGroup m_original;
        private float m_original_alpha;
        private bool m_original_interactable;
        private bool m_original_blockraycast;

        /// <summary>
        /// CTOR.
        /// </summary>
        protected void Awake() {            

            List<UIEventType> ael = new List<UIEventType>();
            if(!WillDispatch(UIEventType.DragStart))   ael.Add(UIEventType.DragStart);
            if(!WillDispatch(UIEventType.DragEnd))     ael.Add(UIEventType.DragEnd);
            if(!WillDispatch(UIEventType.DragUpdate))  ael.Add(UIEventType.DragUpdate);
            if(!WillDispatch(UIEventType.Drop))        ael.Add(UIEventType.Drop);
            allowed = ael.ToArray();
            if(OnEvent!=null) OnEvent.AddListener(OnUIEvent);

            Transform t = transform;
            while(t) { m_canvas = t.GetComponentInParent<Canvas>(); if(m_canvas) break; t = transform.parent; }

            m_rt = GetComponent<RectTransform>();
        }

        /// <summary>
        /// Event handler.
        /// </summary>
        /// <param name="p_event"></param>
        private void OnUIEvent(UIEvent p_event) {

            switch(p_event.type) {
                case UIEventType.DragStart:    { OnDragStart(); } break;
                case UIEventType.DragEnd:      { OnDragEnd();   } break;
                case UIEventType.DragUpdate:   { OnDragUpdate(p_event); } break;
                case UIEventType.Drop: {
                    drop = p_event.target.element.GetComponent<RectTransform>();
                    if (drop.GetComponent<DropComponent>())
                        DropInDropComponent();
                }
                break;
            }
        }
        
        /// <summary>
        /// Handles the dragging start.
        /// </summary>
        private void OnDragStart() {

            Object o;
            CanvasGroup g;
            GameObject c;
            
            c           = Instantiate<GameObject>(gameObject);
            c.name      = "$drag";
            c.hideFlags = HideFlags.HideInHierarchy;

            o = c.GetComponent<DragComponent>(); if(o) Destroy(o);
            o = c.GetComponent<EventComponent>();    if(o) Destroy(o);

            g       = c.GetComponent<CanvasGroup>();
            m_copy  = c.GetComponent<RectTransform>();

            if(!g) {
                g = c.AddComponent<CanvasGroup>();
            }
            
            g.alpha = alphaCopy;
            g.blocksRaycasts = false;
            g.interactable = false;

            g = GetComponent<CanvasGroup>();

            if(g) {
                m_original = g;
                m_original_alpha = g.alpha;
                m_original_interactable = g.interactable;
                m_original_blockraycast = g.blocksRaycasts;
            }
            else {
                g = gameObject.AddComponent<CanvasGroup>();                
            }

            g.alpha = alphaOriginal;
            g.blocksRaycasts = false;
            g.interactable = false;

            m_copy.SetParent(m_canvas.transform,true);
            m_copy.localScale = Vector3.one;
            m_copy.position = gameObject.transform.position;
            
        }


        /// <summary>
        /// Handles the dragdrop clone update.
        /// </summary>
        /// <param name="p_event"></param>
        private void OnDragUpdate(UIEvent p_event) {

            PointerEventData d = p_event.target.data;
            Vector3 mp = Input.mousePosition;
            Vector2 pos = Vector2.zero;
            RectTransform crt = m_canvas.GetComponent<RectTransform>();
            RectTransform rt = m_rt;
            Vector2 down_pos = Vector2.zero;
            Camera cam = d.pressEventCamera;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt,d.pressPosition,cam,out down_pos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(crt,mp,cam,out pos);
            m_copy.transform.localPosition = (pos - down_pos);

            if(move) {
                m_copy.SetParent(transform.parent,true);
                position = m_copy.transform.localPosition;
                m_copy.SetParent(m_canvas.transform,true);
            }
        }

        /// <summary>
        /// Handles the dragging end.
        /// </summary>
        private void OnDragEnd() {            

            if(m_copy) Destroy(m_copy.gameObject);

            if(move) {
                transform.localPosition = position;
            }

            if(m_original) {                                     
               m_original.alpha           = m_original_alpha;                        
               m_original.blocksRaycasts  = m_original_blockraycast;
               m_original.interactable    = m_original_interactable;
            }
            else {
                m_original = GetComponent<CanvasGroup>();
                if(m_original) Destroy(m_original);
            }
            m_original = null;
        }

        /// <summary>
        /// Drop in DroptContainer if been drop on DropComponent
        /// </summary>
        private void DropInDropComponent()
        {
            if (move) return;

            transform.SetParent(drop);
            transform.localPosition = Vector3.zero;
        }
    }

    


}