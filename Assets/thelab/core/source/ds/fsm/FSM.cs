using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace thelab.core
{

    /// <summary>
    /// Delegate that describes the event method.
    /// </summary>
    /// <param name="p_event"></param>
    public delegate void FSMDelegate(FSM p_target);

    [Serializable]
    public class FSMCallback : UnityEvent<FSM> { 
    
        /// <summary>
        /// Wrapper for event invoke.
        /// </summary>
        /// <param name="p_target"></param>
        public void Call(FSM p_target) {
            int count = GetPersistentEventCount();
            for(int i=0;i<count;i++) {
                UnityEngine.Object it = GetPersistentTarget(i);
                string method = GetPersistentMethodName(i);
                Reflection.Invoke(it,method,p_target);
            }
        }

    }
    

    /// <summary>
    /// Class that implements a Finite State Machine component.
    /// </summary>
    public class FSM<T> : FSM
    {

        #region class Transition

        /// <summary>
        /// Class that implements a transition between 2 states in the FSM.
        /// </summary>
        [System.Serializable]
        public class Transition
        {
            public T from;
            public T to;
            public FSMCallback OnEvent;
        }

        #endregion

        #region Log

        /// <summary>
        /// Class that describes a state log.
        /// </summary>
        public class Log
        {
            public T state;
            public float time;
        }

        #endregion

        /// <summary>
        /// Curre state.
        /// </summary>
        public T state
        {
            get { return m_state; }

            set {                
                if(EqualityComparer<T>.Default.Equals(m_state,value)) return;
                T prev  = m_state;                
                m_state = value;
                if(Application.isPlaying)
                {
                    Log l = new Log();
                    l.state = prev;
                    l.time  = time;
                    log.Add(l);
                    OnChange(prev,m_state);
                    if(OnChangeEvent != null) {
                        OnChangeEvent.Call(this);
                    }
                }
            }
        }
        [SerializeField]
        private T m_state;

        /// <summary>
        /// Previous assigned state.
        /// </summary>
        public T previous
        {
            get { return log.Count<= 0 ? default(T) : log[log.Count-1].state; }
        }
        
        /// <summary>
        /// State log.
        /// </summary>
        public List<Log> log { get { return m_log==null ? (m_log = new List<Log>()) : m_log; } }
        private List<Log> m_log;

        /// <summary>
        /// Custom transitions list.
        /// </summary>
        public List<Transition> transitions { get { return m_transitions==null ? (m_transitions = new List<Transition>()) : m_transitions; } }
        private List<Transition> m_transitions;
                
        /// <summary>
        /// Handler for currently active state.
        /// </summary>
        public FSMCallback OnStateEvent;

        /// <summary>
        /// Handler for state change events.
        /// </summary>
        public FSMCallback OnChangeEvent;
        
        /// <summary>
        /// CTOR.
        /// </summary>
        override protected void Awake()
        {            
            if(m_state==null) m_state = default(T);            
            time = 0f;
            Activity.Add(this);
        }

        /// <summary>
        /// Sets this FSM current state.
        /// </summary>
        /// <param name="p_state"></param>
        public void Set(T p_state)
        {
            state = p_state;
        }

        /// <summary>
        /// Changes the state after a given time in seconds.
        /// </summary>
        /// <param name="p_state"></param>
        /// <param name="p_time"></param>
        public void Delay(T p_state,float p_time) {
            Timer.RunOnce(delegate() {
                state = p_state;
            },p_time);
        }

        /// <summary>
        /// Resets this FSM internal state.
        /// </summary>
        override public void Clear() {
            state = default(T);
            time  = 0f;
            log.Clear();
        }

        /// <summary>
        /// Returns the class Type of the state flag.
        /// </summary>
        /// <returns></returns>
        public override Type GetStateType()
        {
            return typeof(T);
        }

        /// <summary>
        /// Callback called 
        /// </summary>
        /// <param name="p_previous"></param>
        /// <param name="p_next"></param>
        virtual protected void OnChange(T p_previous,T p_next) {  }

        /// <summary>
        /// Callback called while this FSM is active.
        /// </summary>
        /// <param name="p_state"></param>
        virtual protected void OnState(T p_state) { }

        /// <summary>
        /// Updates the current state.
        /// </summary>
        override public void OnUpdate() 
        {
            time += Time.unscaledDeltaTime;
            if(state!=null)
            {
                OnState(state);
                if(OnStateEvent!=null) OnStateEvent.Invoke(this);
            }
        }
    }

    /// <summary>
    /// Base class for the Finite State Machine component.
    /// </summary>
    public class FSM : MonoBehaviour, IUpdateable
    {
    
        /// <summary>
        /// Execution time of this FSM.
        /// </summary>
        public float time;

        /// <summary>
        /// CTOR.
        /// </summary>
        virtual protected void Awake() { }

        /// <summary>
        /// Clear this FSM internal state.
        /// </summary>
        virtual public void Clear() {}

        /// <summary>
        /// Returns the state flag type.
        /// </summary>
        /// <returns></returns>
        virtual public Type GetStateType() { return null; }

        /// <summary>
        /// Update method.
        /// </summary>
        virtual public void OnUpdate() { }

    }

}