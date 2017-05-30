using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace thelab.core {

    /// <summary>
    /// Default extension of the list component.
    /// </summary>
    public class ListComponent : ListComponent<Component> { }

    /// <summary>
    /// Class that handles the instantiation, caching and cleanup of GameObject lists.
    /// </summary>
    public class ListComponent<T> : MonoBehaviour where T : Component {

        /// <summary>
        /// List of templates.
        /// </summary>
        public T template;

        /// <summary>
        /// List of allocated elements.
        /// </summary>
        public List<T> list;

        /// <summary>
        /// List of filtered objects at the moment.
        /// </summary>
        public List<T> filter;

        /// <summary>
        /// Shortcut operator to access elements.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public T this[int i] { get {return list[i]; } }

        /// <summary>
        /// Element count.
        /// </summary>
        public int Count { get { return list.Count; }  }
        
        /// <summary>
        /// Reference to the last filter query
        /// </summary>
        protected Predicate<T> m_last_filter;
        
        /// <summary>
        /// CTOR.
        /// </summary>
        protected void Awake() {            
        }

        #region CRUD

        /// <summary>
        /// Instantiates a new element and adds it at an index.
        /// </summary>
        /// <returns></returns>
        public T Add(int p_index) {
            int idx = Mathf.Clamp(p_index,0,list.Count);
            T res = GetInstance();
            list.Insert(idx,res);
            res.transform.SetSiblingIndex(idx+1);
            Rename();            
            //Applies the filter in the element in case there is one already set
            if(m_last_filter!=null) ApplyFilter(res,m_last_filter(res));
            return res;
        }

        /// <summary>
        /// Adds a new item at the end of the list.
        /// </summary>
        /// <returns></returns>
        public T Add() { return Add(list.Count); }

        /// <summary>
        /// Adds an item at start.
        /// </summary>
        /// <returns></returns>
        public T Unshift() { return Add(0); }

        /// <summary>
        /// Removes an item at the index
        /// </summary>
        /// <param name="p_index"></param>
        public void Remove(int p_index,bool p_destroy=false) {
            if(p_index<0)           return;
            if(p_index>=list.Count) return;
            T c = list[p_index];
            Remove(c,p_destroy);
        }

        /// <summary>
        /// Removes an item.
        /// </summary>
        /// <param name="p_item"></param>
        /// <param name="p_destroy"></param>
        public void Remove(T p_item,bool p_destroy=false) {
            T c = p_item;
            if(p_destroy) {
                Destroy(c.gameObject);
            }
            else {
                c.gameObject.SetActive(false);
            }
            list.Remove(c);
            Rename();
            if(filter.IndexOf(c)>=0) filter.Remove(c);
        }

        /// <summary>
        /// Removes an item at the end.
        /// </summary>
        public void Push(bool p_destroy=false) { Remove(list.Count-1,p_destroy); }

        /// <summary>
        /// Removes an item at start.
        /// </summary>
        public void Shift(bool p_destroy=false) { Remove(0,p_destroy); }

        /// <summary>
        /// Removes the first occurrence.
        /// </summary>
        /// <param name="p_callback"></param>
        /// <param name="p_destroy"></param>
        public void Remove(Predicate<T> p_callback,bool p_destroy=false) {
            for(int i=0;i<list.Count;i++) {
                if(p_callback(list[i])) {
                    Remove(list[i],p_destroy);
                    break;
                }
            }
        }

        /// <summary>
        /// Removes all occurrences.
        /// </summary>
        /// <param name="p_callback"></param>
        /// <param name="p_destroy"></param>
        public void RemoveAll(Predicate<T> p_callback,bool p_destroy=false) {
            for(int i=0;i<list.Count;i++) {
                if(p_callback(list[i])) {
                    Remove(list[i],p_destroy);                    
                }
            }
        }

        /// <summary>
        /// Clears the list.
        /// </summary>
        /// <param name="p_destroy"></param>
        public void Clear(bool p_destroy=false) {
            for(int i=0;i<list.Count;i++) {
                T c = list[i];
                if(p_destroy) {
                    Destroy(c.gameObject);
                }
                else {
                    c.gameObject.SetActive(false);
                }
            }
            list.Clear();            
        }
        
        #endregion

        #region Filter

        /// <summary>
        /// Applies a filter in all elements hiding the ones falling out of it
        /// </summary>
        /// <param name="p_query"></param>
        public void Filter(Predicate<T> p_query) {
            ClearFilter();
            m_last_filter = p_query;
            if(m_last_filter==null) return;
            for(int i=0;i<list.Count;i++) {
                T c = list[i];
                bool res = p_query(c);
                ApplyFilter(c,res);
            }
        }

        /// <summary>
        /// Clears the filter applied.
        /// </summary>
        public void ClearFilter() {
            filter.Clear();
            for(int i=0;i<list.Count;i++) {
                if(!list[i]) continue;
                list[i].gameObject.SetActive(true);
            }
            m_last_filter = null;
        }
        
        /// <summary>
        /// Callback to handle true/false results on filtering.
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_result"></param>
        virtual protected void ApplyFilter(T p_target,bool p_result) {
            p_target.gameObject.SetActive(p_result);
        }

        /// <summary>
        /// Searches the list and return the result if the predicate returns true.
        /// </summary>
        /// <param name="p_callback"></param>
        /// <returns></returns>
        public T Find(Predicate<T> p_callback) {            
            for(int i=0;i<list.Count;i++) if(p_callback(list[i])) return list[i];
            return null;
        }

        /// <summary>
        /// Searches the list and return the list of results if the predicate returns true.
        /// </summary>
        /// <param name="p_callback"></param>
        /// <returns></returns>
        public T[] FindAll(Predicate<T> p_callback) {            
            List<T> res = new List<T>();
            for(int i=0;i<list.Count;i++) if(p_callback(list[i])) res.Add(list[i]);
            return res.ToArray();
        }

        #endregion

        #region Sort

        /// <summary>
        /// Sorts the list and set its hierarchy position based on the result.
        /// </summary>
        /// <param name="p_callback"></param>
        public void Sort(Comparison<T> p_callback) {
            if(p_callback==null) return;
            list.Sort(p_callback);
            for(int i=0;i<list.Count;i++) {
                T c = list[i];
                if(!c) continue;
                c.transform.SetSiblingIndex(i+1);
            }
            Rename();
        }

        #endregion

        /// <summary>
        /// Renames the items to keep the list organized in the hierarchy panel.
        /// </summary>
        private void Rename() {
            int len = list.Count;
            for(int i=0;i<list.Count;i++) list[i].name = i.ToString(len<100 ? (len<10 ? "0" : "00"):"000");
        }

        /// <summary>
        /// Either instantiate or re-use a new instance.
        /// </summary>
        /// <returns></returns>
        protected T GetInstance() {
            T res = null;
            int len = transform.childCount;
            for(int i=0;i<len;i++) {
                Transform t = transform.GetChild(i);                
                T c = t.GetComponent<T>();
                if(c==template)                    continue;    //ignore the template.
                if(list.IndexOf(c)>=0)             continue;    //ignore the ones part of the list
                //if(filter.IndexOf(c)>=0)         continue;    //ignore the ones in the filter list.
                //found one "free"
                res = c;
                break;
            }
            if(!res) {
                res = Instantiate<T>(template);
                res.transform.SetParent(transform,false);                
            }
            res.gameObject.SetActive(true);
            res.transform.SetSiblingIndex(len);
            return res;
        }

    }

}