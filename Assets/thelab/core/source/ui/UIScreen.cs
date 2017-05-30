using UnityEngine;
using System.Collections;
using thelab.core;
using UnityEngine.Events;

#pragma warning disable 0108
#pragma warning disable 0109


namespace thelab.core {

    #region enum ScreenEventType

    /// <summary>
    /// Enumeration that describes possible events for the screen.
    /// </summary>
    public enum ScreenEventType {

        /// <summary>
        /// Transition finished.
        /// </summary>
        Transition,

        /// <summary>
        /// Show Transition finished.
        /// </summary>
        Show,

        /// <summary>
        /// Hide Transition finished.
        /// </summary>
        Hide,

        /// <summary>
        /// Show Fade will start
        /// </summary>
        ShowStart,

        /// <summary>
        /// Hide Fade will start.
        /// </summary>
        HideStart,

    }

    #endregion

    #region class ScreenEvent

    /// <summary>
    /// Class that describes a Screen event.
    /// </summary>
    public class ScreenEvent {

        /// <summary>
        /// Type fo the event.
        /// </summary>
        public ScreenEventType type;

        /// <summary>
        /// Reference to the screen.
        /// </summary>
        public UIScreen target;
        
        /// <summary>
        /// CTOR.
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_target"></param>
        public ScreenEvent(ScreenEventType p_type,UIScreen p_target) {
            type = p_type;
            target = p_target;
        }
    }

    #endregion

    /// <summary>
    /// Unity callback to handle screen events from Editor.
    /// </summary>
    [System.Serializable]
    public class ScreenEventCallback : UnityEvent<ScreenEvent> { }

    /// <summary>
    /// Delegate that describes the event method.
    /// </summary>
    /// <param name="p_event"></param>
    public delegate void ScreenEventDelegate(ScreenEvent p_event);
    
    /// <summary>
    /// Base Class for screens.
    /// </summary>
    public class UIScreen : Container {

        #region enum Mode 

        /// <summary>
        /// Transition Mode
        /// </summary>
        public enum Mode {
            /// <summary>
            /// Screen will just fade in/out using alpha
            /// </summary>
            Alpha=0,
            /// <summary>
            /// Screen will transition using the GameObject's Animation
            /// </summary>
            Animation,
            /// <summary>
            /// The 'OnTransition' method must be extended to support custom Transitions
            /// </summary>
            Custom
        }

        #endregion

        /// <summary>
        /// Transition ratio.
        /// </summary>
        public float transition {
            get { return m_transition;  }
            set { m_transition = value; OnTransition(m_transition); }
        }
        [SerializeField]
        [HideInInspector]
        private float m_transition;

        /// <summary>
        /// Transition mode for this screen.
        /// </summary>
        [HideInInspector]
        public Mode mode;
        
        /// <summary>
        /// Returns the reference to the animation component.
        /// </summary>
        public Animation animation { get { return m_animation ? m_animation : (m_animation = GetComponent<Animation>()); } }
        private Animation m_animation;

        /// <summary>
        /// Clip to be used as transition animation.
        /// </summary>
        [HideInInspector]
        public AnimationClip clip;

        /// <summary>
        /// Callback.
        /// </summary>
        [HideInInspector]
        public ScreenEventCallback OnEvent;

        /// <summary>
        /// Changes the screen alpha.
        /// </summary>
        /// <param name="p_transition"></param>
        /// <param name="p_duration"></param>
        /// <param name="p_delay"></param>
        /// <param name="p_easing"></param>
        /// <param name="p_callback"></param>
        virtual public void Fade(float p_transition, float p_duration=0.3f,float p_delay=0f,Easing p_easing=null,System.Action<UIScreen> p_callback=null) {
        
            ScreenEventType t = p_transition<=0f ? ScreenEventType.HideStart : ScreenEventType.ShowStart;
            
            Dispatch(t,p_callback);

            Tween.Kill(this,"transition");
            if(p_duration<=0f) { transition = p_transition; Dispatch(p_transition,p_callback); return; }            
            Tween.Add<float>(this,"transition",p_transition,p_duration,p_delay,p_easing==null ? Cubic.Out : p_easing).onComplete =
            delegate(Tween tw) {
                Dispatch(p_transition,p_callback);
            };
        }

        /// <summary>
        /// Makes the screen appear.
        /// </summary>
        /// <param name="p_duration"></param>
        /// <param name="p_delay"></param>
        /// <param name="p_easing"></param>
        /// <param name="p_callback"></param>
        public void Show(float p_duration=0.3f,float p_delay=0f,Easing p_easing=null,System.Action<UIScreen> p_callback=null) { Fade(1f,p_duration,p_delay,p_easing,p_callback); }

        /// <summary>
        /// Makes the screen disappear.
        /// </summary>
        /// <param name="p_duration"></param>
        /// <param name="p_delay"></param>
        /// <param name="p_easing"></param>
        /// <param name="p_callback"></param>
        public void Hide(float p_duration=0.3f,float p_delay=0f,Easing p_easing=null,System.Action<UIScreen> p_callback=null) { Fade(-0.1f,p_duration,p_delay,p_easing,p_callback); }
        
        /// <summary>
        /// Callback called when this screen is suffering a transition. Override it to customize this screen transition behaviour.
        /// </summary>
        /// <param name="p_value"></param>
        virtual protected void OnTransition(float p_value) {
			//Debug.Log(gameObject.name + " " + p_value);
            switch(mode) {
                
                case Mode.Alpha:        alpha = p_value; return;

                case Mode.Animation:
                
                 if(!animation) return;
                 string clip_name = clip ? clip.name : "fade";
                 if(!animation.GetClip(clip_name)) return;
                 AnimationState ast = animation[clip_name];                 
                 bool e  = ast.enabled;
                 float w = ast.weight;                 
                 ast.enabled = true;
                 ast.normalizedTime = p_value; 
                 ast.weight         = 1f; 
                 animation.Sample();
                 ast.enabled = e;                        
                 ast.weight  = w;

                 return; 
                  
                case Mode.Custom: return;
            }
            
        }

        /// <summary>
        /// Helper method to dispatch this screen events.
        /// </summary>
        /// <param name="p_transition"></param>
        /// <param name="p_callback"></param>
        private void Dispatch(float p_transition,System.Action<UIScreen> p_callback) {
            if(p_callback!=null) p_callback(this);
            if(OnEvent!=null) {                                                           
                OnEvent.Invoke(new ScreenEvent(ScreenEventType.Transition,this));
                ScreenEventType t = p_transition <= 0f ? ScreenEventType.Hide : (p_transition >= 1f ? ScreenEventType.Show : ScreenEventType.Transition);
                if(t!= ScreenEventType.Transition) OnEvent.Invoke(new ScreenEvent(t,this));
            }
        }

        /// <summary>
        /// Helper method to dispatch events.
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_callback"></param>
        private void Dispatch(ScreenEventType p_type,System.Action<UIScreen> p_callback) {
            if(p_callback!=null) p_callback(this);
            if(OnEvent!=null) {                                                           
                OnEvent.Invoke(new ScreenEvent(p_type,this));                                
            }
        }
    
    }
}