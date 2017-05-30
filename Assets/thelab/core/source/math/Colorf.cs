using UnityEngine;
using System.Collections;

namespace thelab.core {

    /// <summary>
    /// Color functions helper class.
    /// </summary>
    public class Colorf {

        /// <summary>
        /// One over 255
        /// </summary>
        const float InvByte = 0.003921568627451f;
        
        /// <summary>
        /// Converts a 32 bit ARGB color to Color class.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        static public Color ARGBToColor(uint v) {
            float ca = ((float)((v>>24)&0xff)) * InvByte;
            float cr = ((float)((v>>16)&0xff)) * InvByte;
            float cg = ((float)((v>>8 )&0xff)) * InvByte;
            float cb = ((float)((v    )&0xff)) * InvByte;
            return new Color(cr,cg,cb,ca);
        }

        /// <summary>
        /// Converts a 32 bit RGBA color to Color class.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        static public Color RGBAToColor(uint v) {
            float cr = ((float)((v>>24)&0xff)) * InvByte;
            float cg = ((float)((v>>16)&0xff)) * InvByte;
            float cb = ((float)((v>>8 )&0xff)) * InvByte;
            float ca = ((float)((v    )&0xff)) * InvByte;
            return new Color(cr,cg,cb,ca);
        }

        /// <summary>
        /// Converts a 24 bit RGB color to Color class.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        static public Color RGBToColor(uint v,float a=1f) {            
            float cr = ((float)((v>>16)&0xff)) * InvByte;
            float cg = ((float)((v>>8 )&0xff)) * InvByte;
            float cb = ((float)((v    )&0xff)) * InvByte;
            return new Color(cr,cg,cb,a);
        }

        /// <summary>
        /// Converts a Color class to 32 bit ARGB color.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        static public uint ColorToARGB(Color v) {
            byte ca = A(v);
            byte cr = R(v);
            byte cg = G(v);
            byte cb = B(v);
            return (uint)((ca<<24)|(cr<<16)|(cg<<8)|(cb));
        }

        /// <summary>
        /// Converts a Color class to 32 bit RGBA color.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        static public uint ColorToRGBA(Color v) {
            byte ca = A(v);
            byte cr = R(v);
            byte cg = G(v);
            byte cb = B(v);
            return (uint)((cr<<24)|(cg<<16)|(cb<<8)|(ca));
        }

        /// <summary>
        /// Converts a Color class to 24 bit RGB color.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        static public uint ColorToRGB(Color v) {            
            byte cr = R(v);
            byte cg = G(v);
            byte cb = B(v);
            return (uint)((cr<<16)|(cg<<8)|(cb));
        }

        /// <summary>
        /// Red channel as byte.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        static public byte R(Color c) { return (byte)(c.r*255f); }

        /// <summary>
        /// Green channel as byte.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        static public byte G(Color c) { return (byte)(c.g*255f); }

        /// <summary>
        /// Blue channel as byte.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        static public byte B(Color c) { return (byte)(c.b*255f); }

        /// <summary>
        /// Alpha channel as byte.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        static public byte A(Color c) { return (byte)(c.a*255f); }

        /// <summary>
        /// Returns RGB channels as bytes.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        static public byte[] RGB(Color c) { return new byte[] { Colorf.R(c), Colorf.G(c), Colorf.B(c) }; }

        /// <summary>
        /// Returns RGBA channels as bytes.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        static public byte[] RGBA(Color c) { return new byte[] { Colorf.R(c), Colorf.G(c), Colorf.B(c), Colorf.A(c) }; }

        /// <summary>
        /// Returns ARGB channels as bytes.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        static public byte[] ARGB(Color c) { return new byte[] { Colorf.A(c),Colorf.R(c), Colorf.G(c), Colorf.B(c) }; }

        /// <summary>
        /// Interpolates a lsit of colors
        /// </summary>
        /// <param name="r"></param>
        /// <param name="p_colors"></param>
        /// <returns></returns>
        static public Color Gradient(float r,params Color[] p_colors) {
            float len = (float)p_colors.Length;
            float pos = r*(len-1f);
            int i0 = Mathf.FloorToInt(pos);
            int i1 = Mathf.CeilToInt(pos);
            if(i1>=p_colors.Length)i1 = p_colors.Length-1;
            float blend = pos - ((float)i0);
            return Color.Lerp(p_colors[i0],p_colors[i1],blend);
        }

        /// <summary>
        /// Interpolates a lsit of colors
        /// </summary>
        /// <param name="r"></param>
        /// <param name="p_colors"></param>
        /// <returns></returns>
        static public Color Gradient(float r,params uint[] p_colors) {
            float len = (float)p_colors.Length;
            float pos = r*(len-1f);
            int i0 = Mathf.FloorToInt(pos);
            int i1 = Mathf.CeilToInt(pos);
            if(i1>=p_colors.Length)i1 = p_colors.Length-1;
            float blend = pos - ((float)i0);
            Color c0 = ARGBToColor(p_colors[i0]);
            Color c1 = ARGBToColor(p_colors[i1]);
            return Color.Lerp(c0,c1,blend);
        }

        /// <summary>
        /// Component wise add
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static public uint AddARGB(uint a,uint b) {
            int ca = (int)Mathf.Clamp(((a>>24)&0xff) + ((b>>24)&0xff),0,255);
            int cr = (int)Mathf.Clamp(((a>>16)&0xff) + ((b>>16)&0xff),0,255);
            int cg = (int)Mathf.Clamp(((a>> 8)&0xff) + ((b>> 8)&0xff),0,255);
            int cb = (int)Mathf.Clamp(((a    )&0xff) + ((b    )&0xff),0,255);
            return (uint)((ca<<24)|(cr<<16)|(cg<< 8)|(cb));
        }

        /// <summary>
        /// Component wise add
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static public uint AddRGBA(uint a,uint b) {
            int cr = (int)Mathf.Clamp(((a>>24)&0xff) + ((b>>24)&0xff),0,255);
            int cg = (int)Mathf.Clamp(((a>>16)&0xff) + ((b>>16)&0xff),0,255);
            int cb = (int)Mathf.Clamp(((a>> 8)&0xff) + ((b>> 8)&0xff),0,255);
            int ca = (int)Mathf.Clamp(((a    )&0xff) + ((b    )&0xff),0,255);
            return (uint)((cr<<24)|(cg<<16)|(cb<< 8)|(ca));
        }

        /// <summary>
        /// Component wise add
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static public uint AddRGB(uint a,uint b) {            
            int cr =(int)Mathf.Clamp(((a>>16)&0xff) + ((b>>16)&0xff),0,255);
            int cg =(int)Mathf.Clamp(((a>> 8)&0xff) + ((b>> 8)&0xff),0,255);
            int cb =(int)Mathf.Clamp(((a    )&0xff) + ((b    )&0xff),0,255);
            return (uint)((cr<<16)|(cg<<8)|(cb));
        }
        
    }

}