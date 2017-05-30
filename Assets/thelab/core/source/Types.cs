using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace thelab.core
{

    /// <summary>
    /// Enumeration to define the type format used in Math
    /// </summary>
    public enum MathType
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// Float
        /// </summary>
        Float,
        /// <summary>
        /// Int
        /// </summary>
        Int,
        /// <summary>
        /// Vector2
        /// </summary>
        Vector2,
        /// <summary>
        /// Vector3
        /// </summary>
        Vector3,
        /// <summary>
        /// Vector4
        /// </summary>
        Vector4,
        /// <summary>
        /// Color
        /// </summary>
        Color,
        /// <summary>
        /// Quaternion
        /// </summary>
        Quaternion,
        /// <summary>
        /// Transform
        /// </summary>
        Transform,
        /// <summary>
        /// Rect
        /// </summary>
        Rect,
    }

}