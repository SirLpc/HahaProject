using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

namespace thelab.core
{

    #region Quad
    /// <summary>
    /// Class that handles quadratic form equations.
    /// </summary>
    public class Quad
    {
        /// <summary>
        /// Easing equation: 'y = x*x'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float In(float p_r) { return p_r * p_r; }
        /// <summary>
        /// Easing equation: 'y = x*(-x+2)'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float Out(float p_r) { return p_r * (-p_r + 2f); }
        /// <summary>
        /// Easing equation: 'y = x*(-3*x + 4)'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float OutBack(float p_r) { return p_r * (-3f * p_r + 4f); }
        /// <summary>
        /// Easing equation: 'y = x*(3*x - 2)'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float BackIn(float p_r) { return p_r * (3f * p_r - 2f); }
    }
    #endregion

    #region Cubic
    /// <summary>
    /// Class that handles cubic form equations.
    /// </summary>
    public class Cubic
    {
        /// <summary>
        /// Easing equation: 'y = x*x*x'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float In(float p_r) { return p_r * p_r * p_r; }
        /// <summary>
        /// Easing equation: 'y = x*(x*(x-3)+3)'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float Out(float p_r) { return p_r * (p_r * (p_r - 3f) + 3f); }
        /// <summary>
        /// Easing equation: 'y = -2*x*(x*(x-1.5))'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float InOut(float p_r) { return -2f * p_r * (p_r * (p_r - 1.5f)); }
        /// <summary>
        /// Easing equation: 'y = x*(x*(4*x -6)+3)'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float OutIn(float p_r) { return p_r * (p_r * (4f * p_r - 6f) + 3f); }
        /// <summary>
        /// Easing equation: 'y = x*(x*(4*x-3))'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float BackIn(float p_r) { return p_r * (p_r * (4f * p_r - 3f)); }
        /// <summary>
        /// Easing equation: 'y = x*(x*(4*x -9) +6)'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float OutBack(float p_r) { return p_r * (p_r * (4f * p_r - 9f) + 6f); }
    }
    #endregion

    #region Quartic
    /// <summary>
    /// Class that handles quartic form equations.
    /// </summary>
    public class Quartic
    {
        /// <summary>
        /// Easing equation: 'y = x*x*x*x'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float In(float p_r) { return p_r * p_r * p_r * p_r; }
        /// <summary>
        /// Easing equation: 'y = x*(x*(x*(-x+4)-6)+4)'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float Out(float p_r) { return p_r * (p_r * (p_r * (-p_r + 4f) - 6f) + 4f); }
        /// <summary>
        /// Easing equation: 'y = x*(x*(x*(x+2)-4)+2)'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float OutIn(float p_r) { return p_r * (p_r * (p_r * (p_r + 2f) - 4f) + 2f); }
        /// <summary>
        /// Easing equation: 'y = x*(x*(x*(x+2)+1)-3)'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float BackIn(float p_r) { return p_r * (p_r * (p_r * (p_r + 2f) + 1f) - 3f); }
        /// <summary>
        /// Easing equation: 'y = x*(x*(x*(-2*x+10)-15)+8)'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float OutBack(float p_r) { return p_r * (p_r * (p_r * (-2f * p_r + 10f) - 15f) + 8f); }
    }
    #endregion

    #region Quintic
    /// <summary>
    /// Class that handles quintic form equations.
    /// </summary>
    public class Quintic
    {
        /// <summary>
        /// Easing equation: 'y = x*x*x*x*x'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float In(float p_r) { return p_r * p_r * p_r * p_r * p_r; }
        /// <summary>
        /// Easing equation: 'y = x*(x*(x*(x*(x-5)+10)-10)+5)'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float Out(float p_r) { return p_r * (p_r * (p_r * (p_r * (p_r - 5f) + 10f) - 10f) + 5f); }
    }
    #endregion

    #region Elastic
    /// <summary>
    /// Class that handles elastic effect interpolation.
    /// </summary>
    public class Elastic
    {
        /// <summary>
        /// Easing equation: 'y = x*(x*(x*(x*(56*x -175) + 200) -100) + 20)'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float OutBig(float p_r) { return p_r * (p_r * (p_r * (p_r * ((56f) * p_r + (-175f)) + (200f)) + (-100f)) + (20f)); }
        /// <summary>
        /// Easing equation: 'y = x*(x*(x*(x*(33*x -106) + 126) -67) + 15)'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float OutSmall(float p_r) { return p_r * (p_r * (p_r * (p_r * ((33f) * p_r + (-106f)) + (126f)) + (-67f)) + (15f)); }
        /// <summary>
        /// Easing equation: 'y = x*(x*(x*(x*(33*x -59)+32)-5))'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float InBig(float p_r) { return p_r * (p_r * (p_r * (p_r * ((33f) * p_r + (-59f)) + (32f)) + (-5f))); }
        /// <summary>
        /// Easing equation: 'y = x*(x*(x*(x*(56*x-105)+60)-10))'.
        /// </summary>
        /// <param name="p_r">Ratio input</param>
        /// <returns>Ratio output.</returns>
        static public float InSmall(float p_r) { return p_r * (p_r * (p_r * (p_r * ((56f) * p_r + (-105f)) + (60f)) + (-10f))); }
    }
    #endregion

    /// <summary>
    /// Class that implements Robert Penner's style of easings.
    /// </summary>
    class Penner {

    }

}