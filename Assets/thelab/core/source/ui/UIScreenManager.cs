using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace thelab.core {

    /// <summary>
    /// Class that handles all overlay screens
    /// </summary>
    [RequireComponent(typeof(Container))]
    public class UIScreenManager : MonoBehaviour {

        /// <summary>
        /// Currently active screen.
        /// </summary>
        public UIScreen current;

        /// <summary>
        /// Stack of open screens.
        /// </summary>
        public List<UIScreen> stack;

        /// <summary>
        /// This overlay container.
        /// </summary>
        public Container container { get { return m_container ? m_container : (m_container = GetComponent<Container>()); } }
        private Container m_container;

        /// <summary>
        /// Reference to the focus group if any.
        /// </summary>
        public FocusGroup focus { get { return m_focus ? m_focus : (m_focus = GetComponent<FocusGroup>());} }
        private FocusGroup m_focus;

        /// <summary>
        /// List of screen templates.
        /// </summary>
        public UIScreen[] screens;

        #region Find

        /// <summary>
        /// Returns a screen template of given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T FindTemplate<T>() where T : UIScreen {
            for(int i=0;i<screens.Length;i++) {
               if(screens[i].GetType() == typeof(T)) {                
                    return (T)screens[i];            
               }
            } 
            return default(T);
        }

        /// <summary>
        /// Searches for a given screen template based on typed tagging.
        /// </summary>
        /// <param name="p_type"></param>
        /// <returns></returns>
        public UIScreen FindTemplateByTags<T>(params T[] p_tags) {
            for(int i=0;i<screens.Length;i++) {
                UIScreen it = screens[i];
                Tag<T> tag = it.GetComponent<Tag<T>>();
                if(!tag) continue;
                if(tag.Match(p_tags)) { return it; }
            }
            return null;
        }

        /// <summary>
        /// Searches an opened screen by its type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Find<T>() where T : UIScreen {
            for(int i=0;i<stack.Count;i++) { if(stack[i].GetType()==typeof(T)) return (T)stack[i]; }
            return default(T);
        }

        /// <summary>
        /// Searches an opened screen by its tags if any.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public UIScreen FindByTags<T>(params T[] p_tags) {
            for(int i=0;i<stack.Count;i++) { 
                Tag<T> tag = stack[i].GetComponent<Tag<T>>();
                if(!tag) continue;
                if(tag.Match(p_tags)) return stack[i];
            }
            return null;
        }

        #endregion

        #region Create

        /// <summary>
        /// Searches a template and creates it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_type"></param>
        /// <returns></returns>
        public T Create<T>() where T : UIScreen {
            return (T)Create(FindTemplate<T>());
        }

        /// <summary>
        /// Searches a template and creates it.
        /// </summary>
        /// <param name="p_type"></param>
        /// <returns></returns>
        public UIScreen CreateByTags<T>(params T[] p_tags) {            
            return Create(FindTemplateByTags<T>(p_tags));
        }

        /// <summary>
        /// Creates a new screen based on a template.
        /// </summary>
        /// <param name="p_template"></param>
        /// <returns></returns>
        public UIScreen Create(UIScreen p_template) {
            UIScreen it = p_template;            
            if(!it) return null;
            it = Instantiate<UIScreen>(it);
            it.name = it.name.Replace("(Clone)","");
            it.transform.SetParent(transform,false);
            it.transform.SetAsLastSibling();
            it.alpha = -0.1f;
            return it;
        }

        #endregion

        #region Open

        /// <summary>
        /// Opens a screen and register it on the stack system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public UIScreen Open(UIScreen p_screen,bool p_show=true,float p_duration=0.6f,float p_delay=0f) {
            if(stack.IndexOf(p_screen)>=0) return p_screen;
            if(!p_screen) { Debug.LogWarning("UIScreenManager> Warning Screen not found!"); return null; }
            if(p_show)p_screen.Show(p_duration,p_delay);
            p_screen.transform.SetAsLastSibling();
            stack.Add(p_screen);            
            container.mouseEnabled = stack.Count > 0;
            if(focus) {
                focus.Refresh();
                focus.Focus(p_screen);
            }
            return p_screen;
        }

        /// <summary>
        /// Creates and open a screen at the top of the stack.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T Open<T>(bool p_show=true) where T : UIScreen {
            return (T)Open(Create<T>(),p_show);
        }

        /// <summary>
        /// Creates and open a screen at the top of the stack.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public UIScreen OpenByTags<T>(bool p_show,params T[] p_tags) {
            return Open(CreateByTags<T>(p_tags),p_show);
        }

        /// <summary>
        /// Creates and open a screen at the top of the stack.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_tags"></param>
        /// <returns></returns>
        public UIScreen OpenByTags<T>(params T[] p_tags) {
            return OpenByTags<T>(true,p_tags);
        }

        /// <summary>
        /// Creates and open a screen at the top of the stack.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_tags"></param>
        /// <returns></returns>
        public UIScreen OpenByTags<T>(bool p_show,float p_duration,float p_delay,params T[] p_tags) {
            UIScreen scr = CreateByTags<T>(p_tags);
            Open(scr,p_show,p_duration,p_delay);
            return scr;
        }

        #endregion

        #region Close

        /// <summary>
        /// Searches an opened screen by its type and closes it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public void Close<T>(float p_duration=0.4f,float p_delay=0f) where T : UIScreen {
            T scr = Find<T>();
            if(scr) { Close(scr,p_duration,p_delay); }            
        }

        /// <summary>
        /// Closes the target screen.
        /// </summary>
        /// <param name="p_screen"></param>
        public void Close(UIScreen p_screen,float p_duration=0.4f,float p_delay=0f) {
            if(stack.IndexOf(p_screen) < 0) {
                Debug.LogWarning("ScreenManager> screen["+p_screen+"] not found on stack.");                
            }
            if(stack.Count<=1) if(focus) focus.Unfocus();
            if(!p_screen) return;
            p_screen.Hide(p_duration,p_delay);
            p_screen.OnEvent.AddListener(delegate(ScreenEvent ev) {
                if(ev.type == ScreenEventType.Hide) DestroyScreen(p_screen,0f,0f);
            });
            
        }

        /// <summary>
        /// Closes a window matching the tags.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_tags"></param>
        public void CloseByTags<T>(params T[] p_tags) {
            Close(FindByTags<T>(p_tags),0.4f,0f);
        }

        /// <summary>
        /// Close the topmost screen. If 'all' is true, cleans the stack.
        /// </summary>
        /// <param name="p_all"></param>
        public void Close(bool p_all=false) {
            if(stack.Count<=0) return;
            if(p_all) { while(stack.Count>0) Close(); return; }
            UIScreen it = stack[stack.Count-1];            
            Close(it,0.4f,0f);
        }

        /// <summary>
        /// Destroys a screen based on its duration and delay parameters.
        /// </summary>
        /// <param name="p_duration"></param>
        /// <param name="p_delay"></param>
        public void DestroyScreen(UIScreen p_screen,float p_duration=0.4f,float p_delay=0f) {

            if(stack.IndexOf(p_screen)>=0)stack.Remove(p_screen);
            Destroy(p_screen.gameObject,p_duration+p_delay+0.5f);
            container.mouseEnabled = stack.Count > 0;
        }

        #endregion

    }
}
