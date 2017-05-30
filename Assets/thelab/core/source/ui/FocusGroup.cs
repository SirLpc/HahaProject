using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace thelab.core {

    /// <summary>
    /// Class that implements a group of element whose focus can be switched by keys.
    /// Usage: if "capture" off, set targets yourself; otherwise, put selectable objects under me
    /// </summary>
    public class FocusGroup : MonoBehaviour {

        /// <summary>
        /// Active group.
        /// </summary>
        static public FocusGroup active;

        /// <summary>
        /// Capture elements automatically.
        /// </summary>
        public bool capture;

        /// <summary>
        /// Current target.
        /// </summary>
        public Selectable current;

        /// <summary>
        /// List of targets.
        /// </summary>
        public List<Selectable> targets;
        
        /// <summary>
        /// Ignore list.
        /// </summary>
        public List<GameObject> ignore;

        /// <summary>
        /// Reference to the last selected object.
        /// </summary>
        public GameObject lastSelection;

        /// <summary>
        /// Reference to the event system.
        /// </summary>
        protected EventSystem m_event_system;
        
        /// <summary>
        /// CTOR.
        /// </summary>
        protected void Awake() {            
            m_event_system = GameObject.FindObjectOfType<EventSystem>();            
            Refresh();
        }

        /// <summary>
        /// Sets this group as active for focus cycling.
        /// </summary>
        public void Focus(Component p_target=null) {
            active=this;
            if(targets.Count>0) {
                if(!p_target) {
                    current = targets[0];
                    if(current) if(current.IsInteractable()) current.Select();
                }
                else {
                    Selectable[] list = p_target.GetComponentsInChildren<Selectable>();
                    foreach(Selectable it in list) {
                        if(targets.IndexOf(it)>=0) {
                            current = it;
                            if(current) if(current.IsInteractable()) current.Select();
                            break;
                        }
                    }
                }             
            }
        }

        /// <summary>
        /// Unset this group as active one if it was active.
        /// </summary>
        public void Unfocus() {
            if(active==this) active=null;
        }

        /// <summary>
        /// Refreshes the target list.
        /// </summary>
        public void Refresh() {
            if(!active) active = this;
            targets.Clear();
            if(capture) {
                TraverseStep(transform,delegate(Transform it) {
                    if(it==transform) return true;
                    if(ignore.IndexOf(it.gameObject)>=0) return false;
                    Selectable[] sl = it.GetComponents<Selectable>();
                    for(int i=0;i<sl.Length;i++) {
                        Selectable s = sl[i];
                        if(targets.IndexOf(s)<0) targets.Add(s);
                    }                    
                    return true;
                });
            }            
        }
        
        /// <summary>
        /// Logic loop.
        /// </summary>
        protected void Update() {
            
            if(m_event_system) {
                GameObject g = m_event_system.currentSelectedGameObject;
                if(g != lastSelection) {
                    lastSelection = g;
                    if(lastSelection) {
                        Selectable s = lastSelection.GetComponent<Selectable>();
                        if(targets.IndexOf(s)>=0) {
                            current = s;
                            active = this;
                        }
                    }                    
                }
            }

            if(active) if(active!=this) return;

            if(Input.GetKeyDown(KeyCode.Tab)) {
                bool is_reverse = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                if(targets.Count<=0) return;

                int k=0;

                while(k<targets.Count) {
                    int idx = targets.IndexOf(current);
                    if(idx < 0) idx = 0;
                    else {
                        idx = is_reverse ? (idx-1) : ((idx + 1) % targets.Count);
                        if(idx<0) idx=targets.Count-1;
                    }                
                    Selectable s = targets[idx];
                    if(s) {
                        current = s;
                        if(s.gameObject.activeInHierarchy) {
                            if(s.IsInteractable()) {
                                s.Select();
                                active = this;
                                break;
                            }                        
                        }                    
                    }
                    
                    k++;
                }                
            }

            bool is_click = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space);

            if(is_click) {                
                if(current) {
                    IPointerClickHandler[] clicks = current.GetComponents<IPointerClickHandler>();
                    PointerEventData d = new PointerEventData(m_event_system);
                    foreach(IPointerClickHandler it in clicks) {
                        if(it==null)     continue;
                        if(it is Button) continue;
                        if(it is InputField) continue;
                        it.OnPointerClick(d);
                    }
                }                
            }
        }

        /// <summary>
        /// Traversal steps of the hierarchy.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="cb"></param>
        protected void TraverseStep(Transform t,System.Predicate<Transform> cb) {
            if(cb!=null) if(!cb(t))return;
            for(int i=0;i<t.childCount;i++) TraverseStep(t.GetChild(i),cb);
        }
        
    }

}