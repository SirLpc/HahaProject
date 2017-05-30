using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace thelab.core
{

    /// <summary>
    /// Component that implements a Dictionary instance.
    /// </summary>
    public class DictionaryBehaviour<K, V> : DictionaryBehaviour
    {
        /// <summary>
        /// Key list.
        /// </summary>
        public List<K> keys;

        /// <summary>
        /// Values list.
        /// </summary>
        public List<V> values;

        /// <summary>
        /// Access operator.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public V this[K k]
        {
            get { int p = keys.IndexOf(k); return p < 0 ? ((V)(object)null) : values[p];  }
            set 
            { 
                int p = keys.IndexOf(k);
                if(p >= 0)
                {
                    if(value == null)
                    {
                        RemoveAt(p);
                    }
                    else
                    {
                        values[p] = value;
                    }
                }
                else
                {
                    Add(k,value);
                }
            }
        }

        /// <summary>
        /// Returns this dictionary data as a list of key-value pairs.
        /// </summary>
        public List<KeyValuePair<K,V>> pairs
        {
            get
            {
                List<KeyValuePair<K,V>> res = new List<KeyValuePair<K,V>>();
                for(int i = 0; i < Count; i++) res.Add(new KeyValuePair<K,V>(keys[i],values[i]));                
                return res;
            }

            set
            {
                if(value==null) { Clear(); return; }
                for(int i=0;i<values.Count;i++) Add(value[i].Key,value[i].Value);
            }
        }

        /// <summary>
        /// Returns an instance of C# dictionary filled with this component data.
        /// </summary>
        public Dictionary<K,V> instance
        {
            get
            {
                Dictionary<K,V> res = new Dictionary<K,V>();
                for(int i = 0; i < Count; i++) res[keys[i]] = values[i];
                return res;
            }

            set
            {
                Dictionary<K,V> d = value;
                if(d == null) return;
                Clear();
                pairs = d.ToList();
            }
        }

        /// <summary>
        /// Returns the count of key-value pairs.
        /// </summary>
        override public int Count { get { return keys == null ? 0 : keys.Count; } }

        /// <summary>
        /// Returns a flag indicating a given key exists.
        /// </summary>
        /// <param name="p_key"></param>
        /// <returns></returns>
        public bool ContainsKey(K p_key)
        {
            return keys.IndexOf(p_key) >= 0;
        }

        /// <summary>
        /// Clears this dictionary.
        /// </summary>
        override public void Clear()
        {
            if(keys == null) keys = new List<K>(); else keys.Clear();
            if(values == null) values = new List<V>(); else values.Clear();
        }

        /// <summary>
        /// Adds a pair with default key and value.
        /// </summary>
        override public void Add(){ Add(default(K),default(V)); }

        /// <summary>
        /// Adds or updates a key-value pair.
        /// </summary>
        /// <param name="p_key"></param>
        /// <param name="p_value"></param>
        public void Add(K p_key,V p_value)
        {
            if(keys == null)   keys   = new List<K>();
            if(values == null) values = new List<V>();
            keys.Add(p_key);
            values.Add(p_value);
        }

        /// <summary>
        /// Removes a key-value pair based on the key index on the key list.
        /// </summary>
        /// <param name="p_index"></param>
        public override void RemoveAt(int p_index)
        {
            if(p_index<0) return;
            if(keys != null)   if(p_index < keys.Count)   keys.RemoveAt(p_index);
            if(values != null) if(p_index < values.Count) values.RemoveAt(p_index);
        }

        /// <summary>
        /// CTOR.
        /// </summary>
        void Awake()
        {
            if(keys==null)   keys = new List<K>();
            if(values==null) values = new List<V>();
        }
    }

    /// <summary>
    /// Base class for all DictionaryBehaviours
    /// </summary>
    public class DictionaryBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Returns the count of key-value pairs.
        /// </summary>
        virtual public int Count { get { return 0; } }

        /// <summary>
        /// Adds a pair with default key and value.
        /// </summary>
        virtual public void Add() {}

        /// <summary>
        /// Removes a key-value pair based on the key index on the key list.
        /// </summary>
        /// <param name="p_index"></param>
        virtual public void RemoveAt(int p_index) { }

        /// <summary>
        /// Clears this dictionary.
        /// </summary>
        virtual public void Clear() {  }
    }
        
}