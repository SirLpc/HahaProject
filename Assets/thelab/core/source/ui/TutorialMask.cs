using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace thelab.core
{
    /// <summary>
    /// Class that handles a set of Image elements in order to block only a region of the screen.
    /// </summary>
    public class TutorialMask : MonoBehaviour
    {
        /// <summary>
        /// X position of the mask center.
        /// </summary>
        public float x { get { return m_x; } set { m_x = value; Refresh(); } }
        [SerializeField]
        private float m_x;

        /// <summary>
        /// Y position of the mask center.
        /// </summary>
        public float y { get { return m_y; } set { m_y = value; Refresh(); } }
        [SerializeField]
        private float m_y;

        /// <summary>
        /// Width of the mask center.
        /// </summary>
        public float width { get { return m_width; } set { m_width = value; Refresh(); } }
        [SerializeField]
        private float m_width;

        /// <summary>
        /// Height of the mask center.
        /// </summary>
        public float height { get { return m_height; } set { m_height = value; Refresh(); } }
        [SerializeField]
        private float m_height;

        /// <summary>
        /// List of pieces
        /// </summary>
        [SerializeField]
        private List<RectTransform> m_pieces;

        /// <summary>
        /// Center piece.
        /// </summary>
        private RectTransform m_center { get { return m_pieces[2]; } }

        [SerializeField]
        internal bool m_init;

        /// <summary>
        /// Internal init.
        /// </summary>
        internal void Init()
        {
            if (m_init) return;
            m_init = true;
            m_x = 0f;
            m_y = 0f;
            m_width  = 100f;
            m_height = 100f;
            m_pieces = new List<RectTransform>();
            for (int i = 0; i < transform.childCount; i++) m_pieces.Add(transform.GetChild(i).GetComponent<RectTransform>());
            Refresh();
        }


        /// <summary>
        /// Masks a target.
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_size_scale"></param>
        public void Mask(RectTransform p_target,float p_size_scale=1f)
        {
            if (!p_target) return;
            RectTransform t = p_target;

            Transform target_parent = t.transform.parent;
            int target_index = t.transform.GetSiblingIndex();

            t.SetParent(transform,true);

            Vector2 p = t.anchoredPosition;
            Vector2 s = t.sizeDelta;
            Vector2 pv = t.pivot;

            Vector2 ms = s; 
            ms.Scale(Vector2.one * p_size_scale);

            x = p.x - (s.x * pv.x) + (s.x * 0.5f);
            y = p.y - (s.y * pv.y) + (s.y * 0.5f);
            width  = ms.x;
            height = ms.y;

            Debug.Log(p);

            t.SetParent(target_parent);
            t.SetSiblingIndex(target_index);
        }

        /// <summary>
        /// Updates the mask.
        /// </summary>
        private void Refresh()
        {
            Rect parent_rect = GetComponentInParent<RectTransform>().rect;

            float hmw = m_width*0.5f;
            float hmh = m_height*0.5f;

            float px = (m_x)-hmw;
            float py = (m_y)+hmh;

            float w = Mathf.Max(0f, m_width);
            float h = Mathf.Max(0f, m_height);

            m_center.anchoredPosition = new Vector2(px, py);
            m_center.sizeDelta        = new Vector2(w,h);

            RectTransform p;

            //px = m_x;
            //py = m_y;

            float top_height    = Mathf.Max(0f,-py);
            float bottom_height = Mathf.Max(0f,parent_rect.height + py - h);

            p = m_pieces[0]; 
            p.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   top_height);

            p = m_pieces[1];
            p.anchoredPosition = new Vector2(0f, py);
            p.sizeDelta = new Vector2(Mathf.Max(0f,px), h);
            
            p = m_pieces[3];
            p.anchoredPosition = new Vector2(px+w, py);
            p.sizeDelta = new Vector2(Mathf.Max(0f,parent_rect.width - px - w), h);
            
            p = m_pieces[4]; 
            p.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   bottom_height);

        }
    }
}

