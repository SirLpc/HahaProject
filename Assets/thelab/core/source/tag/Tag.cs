using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace thelab.core
{

    #region class Tag<T>

    /// <summary>
    /// Extension of the tag class to handle the different types of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Tag<T> : Tag {

        /// <summary>
        /// Returns the type of the values stored in this component.
        /// </summary>
        public System.Type type { get { return typeof(T); } }

        /// <summary>
        /// List of values.
        /// </summary>
        public List<T> tags;

        /// <summary>
        /// Array index operator.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public T this[int p] {
            get { return tags[p];  }                
            set { tags[p] = value; }
        }

        /// <summary>
        /// Values count.
        /// </summary>
        public int Count { get { return tags.Count; } }

        /// <summary>
        /// Returns a flag indicating if the informed tag is contained in the set.
        /// </summary>
        /// <param name="p_tag"></param>
        /// <returns></returns>
        public bool Contains(T p_tag) { return tags.IndexOf(p_tag) >= 0; }

        /// <summary>
        /// Returns a flag indicating if the informed tags match this component's set.
        /// </summary>
        /// <param name="p_tags"></param>
        /// <returns></returns>
        public bool Match(params T[] p_tags) { return Match(false, p_tags); }

        
        /// <summary>
        /// Returns a flag indicating if the informed tags match this component's set.
        /// If 'precise' is set, all tags must match in number and comparison.
        /// </summary>
        /// <param name="p_precise"></param>
        /// <param name="p_tags"></param>
        /// <returns></returns>
        public bool Match(bool p_precise,params T[] p_tags) {
            if (p_precise) if (p_tags.Length != tags.Count) return false;
            for(int i=0;i<p_tags.Length;i++) {
                bool found = false;
                for (int j = 0; j < tags.Count; j++) {
                    if(EqualityComparer<T>.Default.Equals(tags[j],p_tags[i])) found = true;
                }
                if (!found) return false;
            }
            return true;
        }

    }

    #endregion

    #region class Tag

    /// <summary>
    /// Simple component to mark objects with one or more tagging values.
    /// </summary>
    public class Tag : MonoBehaviour {

        #region static

        /// <summary>
        /// Globally searches for tags and returns the ones matching the desired label.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_label"></param>
        /// <returns></returns>
        static public List<T> FindAll<T>(string p_label) where T : Tag
        {
            List<T> res = new List<T>();
            T[] query = GameObject.FindObjectsOfType<T>();
            for (int i = 0; i < query.Length; i++) if (query[i].label == p_label) res.Add(query[i]);
           return res;
        }

        /// <summary>
        /// Globally searches for tags and returns the ones matching the desired label.
        /// </summary>
        /// <param name="p_label"></param>
        /// <returns></returns>
        static public List<Tag> FindAll(string p_label) { return FindAll<Tag>(p_label); }

        #endregion

        /// <summary>
        /// Tag label.
        /// </summary>
        public string label;
    }

    #endregion


}