using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

namespace thelab.core
{
    /// <summary>
    /// Class that handles a single activity running in the main pool or a thread.
    /// </summary>
    [System.Serializable]
    public class Timer : Activity
    {
        
        /// <summary>
        /// Creates a new Timer and calls 'callback' for each step.
        /// </summary>
        /// <param name="p_callback"></param>
        /// <param name="p_duration"></param>
        /// <param name="p_delay"></param>
        /// <param name="p_steps"></param>
        /// <param name="p_threaded"></param>
        /// <returns></returns>
        static public Timer Run(Action<int> p_callback,float p_duration,float p_delay=0f,int p_steps=0,bool p_threaded=false) {
            Timer n = new Timer(p_duration, p_delay, p_steps, p_threaded);
            n.OnTimerStep = delegate(Timer t) { if (p_callback != null) p_callback(t.step); };
            n.Start();
            return n;
        }

        
        /// <summary>
        /// Creates a Timer and after execution executes the method 'method' in the 'target' object.
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_method"></param>
        /// <param name="p_delay"></param>
        /// <param name="p_args"></param>
        /// <returns></returns>
        static public Timer Invoke(object p_target,string p_method,float p_delay,params object[] p_args) {
            Timer n = new Timer(p_delay, 0f, 0, false);
            n.OnTimerComplete = delegate(Timer t) { if (p_target != null) Reflection.Invoke(p_target, p_method, p_args); };
            n.Start();
            return n;
        }

        /// <summary>
        /// Creates a Timer and after execution executes the method 'method' in the 'target' object inside a thread context.
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_method"></param>
        /// <param name="p_delay"></param>
        /// <param name="p_args"></param>
        /// <returns></returns>
        static public Timer InvokeThread(object p_target, string p_method, float p_delay, params object[] p_args)
        {
            Timer n = new Timer(p_delay, 0f, 0, true);
            n.OnTimerComplete = delegate(Timer t) { if (p_target != null) Reflection.Invoke(p_target, p_method, p_args); };
            n.Start();
            return n;
        }

        /// <summary>
        /// Creates a Timer and after execution sets a 'target's 'property' with 'value'.
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_delay"></param>
        /// <param name="p_value"></param>
        /// <returns></returns>
        static public Timer Set(object p_target, string p_property, float p_delay, object p_value)
        {
            Timer n = new Timer(p_delay, 0f, 0, false);
            n.OnTimerComplete = delegate(Timer t) { if (p_target != null) Reflection.Set(p_target, p_property, p_value); };
            n.Start();
            return n;
        }

        /// <summary>
        /// Creates a Timer and after its execution samples the 'target' 'property' passing the result to 'callback'.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_target"></param>
        /// <param name="p_property"></param>
        /// <param name="p_delay"></param>
        /// <param name="p_callback"></param>
        /// <returns></returns>
        static public Timer Get<T>(object p_target, string p_property, float p_delay, Action<T> p_callback)
        {
            Timer n = new Timer(p_delay, 0f, 0, false);
            n.OnTimerComplete = delegate(Timer t) { if (p_target != null) if(p_callback!=null) p_callback(Reflection.Get<T>(p_target, p_property)); };
            n.Start();
            return n;
        }

        /// <summary>
        /// Duration of this Timer.
        /// </summary>
        public float duration { get; set; }

        /// <summary>
        /// Current cycle step.
        /// </summary>
        public int step { get; set; }

        /// <summary>
        /// Timer cycles count.
        /// </summary>
        public int count { get; set; }

        /// <summary>
        /// Returns the ratio of execution of this Timer.
        /// </summary>
        public float progress { get { float r = (duration <= 0f) ? (running ? (elapsed >= 0f ? 1f : 0f) : 0f) : (elapsed / duration); return (r<0f ? 0f : (r>1f ? 1f : r)); } }

        /// <summary>
        /// Callbacks called when the timer finished an execution.
        /// </summary>
        public Action<Timer> OnTimerStep;
        
        /// <summary>
        /// Callback called on Timer finishes its task.
        /// </summary>
        public Action<Timer> OnTimerComplete;
        
        /// <summary>
        /// Creates a new Activity.
        /// </summary>
        /// <param name="p_duration"></param>
        /// <param name="p_delay"></param>
        /// <param name="p_steps"></param>
        /// <param name="p_threaded"></param>
        public Timer(float p_duration,float p_delay=0f,int p_steps=0,bool p_threaded=false) : base(p_delay,p_threaded) {
            duration = p_duration;
            step = 0;
            count = p_steps;
        }

        /// <summary>
        /// Handles the Timer execution.
        /// </summary>
        protected override void OnExecute()
        {               
            if(elapsed >= duration)
            {
                elapsed = duration;
                if (OnTimerStep != null) OnTimerStep(this);
                step++;
                if (step >= count)
                {
                    OnComplete();
                    if (OnTimerComplete != null) OnTimerComplete(this);
                    Stop();
                    return;
                }
                else
                {
                    elapsed = 0f;
                    OnStep();
                }
            }
        }

        /// <summary>
        /// Callback called when the Timer completed one step.
        /// </summary>
        virtual protected void OnStep() { }

        /// <summary>
        /// Callback called when the Timer finished all its steps.
        /// </summary>
        virtual protected void OnComplete() { }
        
    }
}