using System.Collections.Generic;
using UnityEngine;


namespace thelab.core
{

    /// <summary>
    /// Class that describes a simple component that handles UI events.
    /// </summary>    
    public class DropComponent : EventComponent
    {   
        /// <summary>
        /// Reference to the dragged element.
        /// </summary>
        [HideInInspector]
        public RectTransform target;
        
        /// <summary>
        /// Internals.
        /// </summary>        
      
        /// <summary>
        /// CTOR.
        /// </summary>
        protected void Awake() {            
            List<UIEventType> ael = new List<UIEventType>();            
            if(!WillDispatch(UIEventType.Drop))        ael.Add(UIEventType.Drop);
            allowed = ael.ToArray();
            if(OnEvent!=null) OnEvent.AddListener(OnUIEvent);            
        }

        /// <summary>
        /// Event handler.
        /// </summary>
        /// <param name="p_event"></param>
        private void OnUIEvent(UIEvent p_event) {
            switch(p_event.type) {
                case UIEventType.Drop: {
                    target = p_event.target.element.GetComponent<RectTransform>();
                }
                break;
            }
        }
        

    }

    


}