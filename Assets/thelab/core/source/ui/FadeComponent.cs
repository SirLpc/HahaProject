using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace thelab.core {

    /// <summary>
    /// Class that handles fade in/out of UI elements
    /// </summary>    
    public class FadeComponent : MonoBehaviour {
    
        /// <summary>
        /// Alpha of the fade.
        /// </summary>
        public float alpha {
            get { return m_alpha; }
            set {                 
                if(group) { group.alpha = m_alpha = value; group.blocksRaycasts = (group.interactable = m_alpha>=0f); } else 
                if(image) { Color c = image.color; c.a = m_alpha = value; image.color = c; }
            }
        }
        [SerializeField]
        [HideInInspector]
        private float m_alpha;
        
        /// <summary>
        /// Reference to the image target.
        /// </summary>
        public Image image { get { return m_image ? m_image : (m_image=GetComponent<Image>());  } }
        private Image m_image;

        /// <summary>
        /// Reference to the group target.
        /// </summary>
        public CanvasGroup group { get { return m_group ? m_group : (m_group=GetComponent<CanvasGroup>());  } }
        private CanvasGroup m_group;

        /// <summary>
        /// CTOR.
        /// </summary>
        protected void Awake() {
            if(group) m_alpha = group.alpha; else
            if(image) m_alpha = image.color.a;
        }

        /// <summary>
        /// Animate the alpha of the target component.
        /// </summary>
        /// <param name="p_alpha"></param>
        /// <param name="p_duration"></param>
        /// <param name="p_delay"></param>
        /// <param name="p_easing"></param>
        public void Fade(float p_alpha,float p_duration=0.4f,float p_delay=0f,Easing p_easing=null) {
            Tween.Add<float>(this,"alpha",p_alpha,p_duration,p_delay,p_easing);
        }

        /// <summary>
        /// Animates the alpha to 100%
        /// </summary>
        public void FadeIn(float p_duration=0.4f,float p_delay=0f,Easing p_easing=null) {
            Fade(1f,p_duration,p_delay,p_easing);
        }

        /// <summary>
        /// Animates the alpha to 0%
        /// </summary>
        public void FadeOut(float p_duration=0.4f,float p_delay=0f,Easing p_easing=null) {
            Fade(-0.1f,p_duration,p_delay,p_easing);
        }
       
    }
}
