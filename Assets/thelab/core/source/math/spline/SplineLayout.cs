using UnityEngine;
using System.Collections;

namespace thelab.core {

    /// <summary>
    /// Component to help layout elements along a spline.
    /// </summary>
    public class SplineLayout : MonoBehaviour {

        /// <summary>
        /// Target spline.
        /// </summary>
        public SplineComponent spline;

        /// <summary>
        /// Padding at the start of the spline.
        /// </summary>
        public float paddingStart;

        /// <summary>
        /// Padding at the end of the spline.
        /// </summary>
        public float paddingEnd;

        /// <summary>
        /// Refreshes the elements distribution.
        /// </summary>
        public void Refresh() {

            if(!spline) return;
            
            float length = spline.positions.length;

            int count = transform.childCount;

            float l0 = Mathf.Min(paddingStart,length);
            float l1 = Mathf.Max(length - paddingEnd,0f);

            float r = 0f;

            for(int i=0;i<count;i++) {

                Transform t           = transform.GetChild(i);
                SplineLayoutElement s = t.GetComponent<SplineLayoutElement>();
                if(s) if(s.ignoreLayout) continue;

                Vector3 off_pos = s ? s.position : Vector3.zero;
                Vector3 off_rot = s ? s.rotation : Vector3.zero;
                float off_l     = s ? s.length   : 0f;

                r = ((float)i) / ((float)(count-1));

                float l = Mathf.Lerp(l0,l1,r) + off_l;

                Vector3 pos  = spline.positions.Get(l);
                
                int closest_id = -1;

                if(s)
                if(s.snap) {
                    pos = spline.positions.GetClosestNode(l,out closest_id);
                }

                t.position = pos + off_pos;

                if(s.ignoreRotation) {
                    continue;
                }

                bool use_point_orient = true;

                if(s) if(s.snap) use_point_orient = false;

                Vector3 dir = Vector3.forward;

                if(use_point_orient) {
                    Vector3 p0 = spline.positions.Get(l-0.1f);
                    Vector3 p1 = spline.positions.Get(l+0.1f);
                    dir = (p1-p0).normalized;
                }
                else {
                    closest_id = Mathf.Clamp(closest_id,0,t.childCount-1);
                    Transform closest_t = t.GetChild(closest_id);
                    dir = closest_t.forward;
                }

                if(s) if(s.groundAlign) { dir.y = 0f; dir.Normalize(); }

                bool global_up = s ? s.useGlobalUp : true;
                
                t.localRotation = Quaternion.LookRotation(dir,global_up ? Vector3.up : t.up);
                t.localEulerAngles += off_rot;
                           
            }
        }
        
    }

}