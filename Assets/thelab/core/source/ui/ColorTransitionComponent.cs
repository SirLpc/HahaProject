
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace thelab.core
{
    
    
    /// <summary>
    /// Class that implements color transition for a group of elements.
    /// </summary>
    public class ColorTransitionComponent : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        
        /// <summary>
        /// List of targets
        /// </summary>
        public Graphic[] targets;        
       
        /// <summary>
        /// List of original targets colors.
        /// </summary>
        private Color[] m_target_colors;

        /// <summary>
        /// Default color.
        /// </summary>
        public Color normalColor = Color.white;

        /// <summary>
        /// Mouse over color.
        /// </summary>
        public Color hilightColor = new Color(0.7f,0.7f,0.7f,1f);

        /// <summary>
        /// Mouse down color
        /// </summary>
        public Color pressedColor = new Color(0.5f,0.5f,0.5f,1f);

        /// <summary>
        /// Disabled color.
        /// </summary>
        public Color disabledColor = new Color(0.7f,0.7f,0.7f,0.5f);

        /// <summary>
        /// Transition time.
        /// </summary>
        public float duration = 0.1f;

        /// <summary>
        /// Multiply colors by starting color.
        /// </summary>
        public bool multiply = true;

        /// <summary>
        /// CTOR.
        /// </summary>
        protected void Awake() {
            m_target_colors = new Color[targets.Length];
            for(int i=0;i<targets.Length;i++) m_target_colors[i] = targets[i].color;
            ApplyColor(normalColor,0f);
        }

        /// <summary>
        /// Internal Handler.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)  { ApplyColor(pressedColor,duration);  }

        /// <summary>
        /// Internal Handler.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData) {  ApplyColor(hilightColor,duration); }

        /// <summary>
        /// Internal Handler.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData)    { ApplyColor(normalColor,duration);  }

        /// <summary>
        /// Internal Handler.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData) { ApplyColor(hilightColor,duration);        }

        /// <summary>
        /// Handles Pointer Exit events
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)  { ApplyColor(normalColor,duration);  }
        
        /// <summary>
        /// Handler for focus.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnSelect(BaseEventData eventData) { ApplyColor(hilightColor,duration); }

        /// <summary>
        /// Handler for unfocus.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDeselect(BaseEventData eventData) { ApplyColor(normalColor,duration); }

        /// <summary>
        /// Apply colors with or without transition.
        /// </summary>
        /// <param name="p_color"></param>
        /// <param name="p_transition"></param>
        private void ApplyColor(Color p_color,float p_duration) {
            for(int i = 0; i < targets.Length; i++) {
                Color c = multiply ? (m_target_colors[i] * p_color) : p_color;
                if(p_duration <= 0f) { targets[i].color = c; continue; }
                Tween.Add<Color>(targets[i],"color",c,0f,p_duration,Cubic.Out);
            }
        }
        
    }

}