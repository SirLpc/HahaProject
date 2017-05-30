using UnityEngine;
using System.Collections;

namespace thelab.core {

    /// <summary>
    /// Component to help setup extra adjustments in individual elements inside a SplineLayout
    /// </summary>
    public class SplineLayoutElement : MonoBehaviour {

        /// <summary>
        /// Flag to tell the layout to ignore layouting this element.
        /// </summary>
        public bool ignoreLayout;

        /// <summary>
        /// Flag to tell the spline layout to not orient this element.
        /// </summary>
        public bool ignoreRotation;

        /// <summary>
        /// Flag that tells the global up vector will be used instead of the element's own up vector.
        /// </summary>
        public bool useGlobalUp = true;

        /// <summary>
        /// Flag that tells the forward vector must align to ground.
        /// </summary>
        public bool groundAlign = false;

        /// <summary>
        /// Flag that tells the element will snap to the closes node.
        /// </summary>
        public bool snap;

        /// <summary>
        /// Position offset to be added.
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// Rotation offset to be added.
        /// </summary>
        public Vector3 rotation;

        /// <summary>
        /// Length along the spline to be added.
        /// </summary>
        public float length;
        
        
    }

}