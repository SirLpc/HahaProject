using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace thelab.core {

    /// <summary>
    /// Class that implements a simple flicker effect on UI
    /// </summary>
    public class FlickerComponent : MonoBehaviour {
    
        /// <summary>
        /// Oscilations per second.
        /// </summary>
        public int frequence = 30;

        /// <summary>
        /// Min alpha
        /// </summary>
        public float min = 0f;

        /// <summary>
        /// Max alpha
        /// </summary>
        public float max = 1f;

        /// <summary>
        /// Internals.
        /// </summary>        
        private float m_switch;
        private int m_frame;

        /// <summary>
        /// Internal reference to the targets
        /// </summary>
        private Image m_image;        
        private CanvasGroup m_group;
        private MeshRenderer m_renderer;
        private string m_renderer_color_attrib;

        /// <summary>
        /// CTOR.
        /// </summary>
        protected void Awake() {
            m_image = GetComponent<Image>();
            m_group = GetComponent<CanvasGroup>();    
            if(!m_image) { 
                m_renderer = GetComponent<MeshRenderer>();
                Material m = m_renderer.sharedMaterial;
                m_renderer_color_attrib="";
                if(m) {
                    if(m.HasProperty("_TintColor")) m_renderer_color_attrib = "_TintColor"; else
                    if(m.HasProperty("_Color")) m_renderer_color_attrib = "_Color"; else
                    if(m.HasProperty("_Tint")) m_renderer_color_attrib = "_Tint";                     
                }
            }
            m_switch = 0f;      
            m_frame=0;      
        }
	    
        /// <summary>
        /// Callback called sync to rendering.
        /// </summary>
        protected void OnRenderObject () {

            if(Camera.current!=Camera.main) return;
            
            m_frame++;

            int f     = m_frame;            
            float v   = 0f;            
            float opf = 60f / ((float)frequence);
            int cap   = Mathf.CeilToInt(opf);
            
            if((f % cap)==0) m_switch = m_switch<=0f ? 1f : 0f;

            v = Mathf.Lerp(min,max,m_switch);

	        if(m_image) {
                Color c = m_image.color;
                c.a = v;
                m_image.color = c;
            }

            if(m_group) {
                m_group.alpha = v;
            }

            

            if(m_renderer) {
                Material m = m_renderer.sharedMaterial;
                
                if(m) {
                    if(!string.IsNullOrEmpty(m_renderer_color_attrib)) {
                        Color c;
                        c = m.GetColor(m_renderer_color_attrib); c.a = v; m.SetColor(m_renderer_color_attrib,c);                     
                    }
                }
            }

        }
                
    }
}