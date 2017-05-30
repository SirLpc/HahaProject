
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace thelab.core
{
    
    /// <summary>
    /// Delegate that describes the event method.
    /// </summary>
    /// <param name="p_event"></param>
    public delegate void EventComponentDelegate(UIEvent p_event);

    /// <summary>
    /// Class that extends UnityEvent for editor purposes.
    /// </summary>
    [Serializable]
    public class EventComponentCallback : UnityEvent<UIEvent> { }

    #region enum UIEventType

    /// <summary>
    /// Event Type enumeration.
    /// </summary>
    public enum UIEventType
    {
        /// <summary>
        /// None
        /// </summary>
        None = -1,
        /// <summary>
        /// Down
        /// </summary>
        Down,
        /// <summary>
        /// Up
        /// </summary>
        Up,
        /// <summary>
        /// Hold
        /// </summary>
        Hold,
        /// <summary>
        /// Click
        /// </summary>
        Click,
        /// <summary>
        /// Enter
        /// </summary>
        Enter,
        /// <summary>
        /// Stay
        /// </summary>
        Stay,
        /// <summary>
        /// Move
        /// </summary>
        Move,
        /// <summary>
        /// Exit
        /// </summary>
        Exit,        
        /// <summary>
        /// DragStart
        /// </summary>
        DragStart,
        /// <summary>
        /// DragOver
        /// </summary>
        DragOver,
        /// <summary>
        /// DragUpdate
        /// </summary>
        DragUpdate,
        /// <summary>
        /// DragEnd
        /// </summary>
        DragEnd,
        /// <summary>
        /// Drop
        /// </summary>
        Drop,
        /// <summary>
        /// Scroll
        /// </summary>
        Scroll,
    }

    #endregion

    #region class Event

    /// <summary>
    /// Class that implements UIEvent data.
    /// </summary>
    [Serializable]
    public class UIEvent
    {
        /// <summary>
        /// Event type.
        /// </summary>
        public UIEventType type;

        /// <summary>
        /// Target.
        /// </summary>
        public EventComponent target;

    }

    #endregion

    /// <summary>
    /// Class that describes a simple component that handles UI events.
    /// </summary>
    public class EventComponent : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IMoveHandler, IDropHandler, IEndDragHandler, IScrollHandler
    {

        /// <summary>
        /// Allowed events.
        /// </summary>
        public UIEventType[] allowed = new UIEventType[] { UIEventType.Click };

        /// <summary>
        /// Flag that indicates the mouse is down.
        /// </summary>
        public bool down;

        /// <summary>
        /// Flag that indicates the mouse is over.
        /// </summary>
        public bool over;

        /// <summary>
        /// Flag that indicates that dragging is occuring.
        /// </summary>
        public bool drag;

        /// <summary>
        /// Time in seconds the mouse button is being held.
        /// </summary>
        public float hold;

        /// <summary>
        /// Time in seconds the mouse is over.
        /// </summary>
        public float stay;

        /// <summary>
        /// Data from move events.
        /// </summary>
        public AxisEventData axis;

        /// <summary>
        /// Event data.
        /// </summary>
        public PointerEventData data;

        /// <summary>
        /// Target object used in some events.
        /// </summary>
        public GameObject element;

        /// <summary>
        /// Callback.
        /// </summary>
        public EventComponentCallback OnEvent;
        
        /// <summary>
        /// Returns a flag indicating the event will be emmited.
        /// </summary>
        /// <param name="p_type"></param>
        /// <returns></returns>
        public bool WillDispatch(UIEventType p_type) { return enabled ? (Array.IndexOf(allowed, p_type) >= 0) : false; }
        
        /// <summary>
        /// Emmits an event.
        /// </summary>
        /// <param name="p_type"></param>
        public void Dispatch(UIEventType p_type)
        {
            if(OnEvent == null) return;
            if(WillDispatch(p_type)) {   
                UIEvent ev = new UIEvent();
                ev.target = this;
                ev.type = p_type;
                OnEvent.Invoke(ev);
            }
        }

        /// <summary>
        /// Internal Handler.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)  { data = eventData; down = true; hold = 0f;  Dispatch(UIEventType.Down); Activity.Run(OnHoldUpdate); }

        /// <summary>
        /// Internal Handler.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData) { data = eventData; down = false; Dispatch(UIEventType.Click); }

        /// <summary>
        /// Internal Handler.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData)    { data = eventData; down = false; Dispatch(UIEventType.Up);   }

        /// <summary>
        /// Internal Handler.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData) {
            data = eventData;
            over = true; 
            stay = 0f; 
            Dispatch(UIEventType.Enter);
            if(eventData.pointerDrag) { 
                element = eventData.pointerDrag; 
                Dispatch(UIEventType.DragOver);
                EventComponent t = element.GetComponent<EventComponent>();
                if(t) { t.element = gameObject; t.Dispatch(UIEventType.DragOver); }
            }
            Activity.Run(OnStayUpdate); 
        }

        /// <summary>
        /// Handles Pointer Exit events
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)  { data = eventData; over = false; Dispatch(UIEventType.Exit);  }

        /// <summary>
        /// Handles Move events
        /// </summary>
        /// <param name="eventData"></param>
        public void OnMove(AxisEventData eventData) { axis = eventData; Dispatch(UIEventType.Move); }

        /// <summary>
        /// Handles Drag events
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData) { data = eventData; DragStart(); Dispatch(UIEventType.DragUpdate); }

        /// <summary>
        /// Handles Drop events
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrop(PointerEventData eventData) {            
            data = eventData;
            DragEnd(); 
            element = eventData.pointerDrag;            
            Dispatch(UIEventType.Drop);
            EventComponent t = element.GetComponent<EventComponent>();
            if(t){ t.element = gameObject; t.Dispatch(UIEventType.Drop); }
        }   
        
        /// <summary>
        /// Handles End Drag events
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData) { data = eventData; DragEnd(); }
        
        private void DragStart() { if(!drag) Dispatch(UIEventType.DragStart); drag = true; }        
        private void DragEnd()   { if(drag)  Dispatch(UIEventType.DragEnd); drag = false; }        
        private bool OnHoldUpdate(float t) { if(down) { Dispatch(UIEventType.Hold); hold += Time.deltaTime; } return down; }
        private bool OnStayUpdate(float t) { if(over) { Dispatch(UIEventType.Stay); stay += Time.deltaTime; } return over; }

        /// <summary>
        /// Handles Scroll events
        /// </summary>
        /// <param name="eventData"></param>
        public void OnScroll(PointerEventData eventData) { data = eventData; Dispatch(UIEventType.Scroll); }
    }

}