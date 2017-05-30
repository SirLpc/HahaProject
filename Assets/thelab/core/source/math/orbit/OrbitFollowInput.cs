using UnityEngine;
using System.Collections;

namespace thelab.core {

    /// <summary>
    /// Class that implements following features for the orbit transform.
    /// </summary>    
    public class OrbitFollowInput  : MonoBehaviour {
    
        /// <summary>
        /// Reference to the Orbit component.
        /// </summary>
        public OrbitTransform orbit { get { return m_orbit ? m_orbit : (m_orbit = GetComponent<OrbitTransform>()); } }
        private OrbitTransform m_orbit;

        /// <summary>
        /// Flag that tells the follow target attribs.
        /// </summary>
        public enum Flag {
            None        = 0,
            PositionX   = 1,
            PositionY   = 2,
            PositionZ   = 4,
            PositionXY  = 3,            
            PositionXZ  = 5,
            PositionYZ  = 6,
            PositionXYZ = 7,
            RotationX   = 8,
            RotationY   = 16,
            RotationZ   = 32,
            RotationXY  = 24,            
            RotationXZ  = 40,
            RotationYZ  = 48,
            RotationXYZ = 56,
            All=63
        }        

        /// <summary>
        /// Target to follow.
        /// </summary>
        public Transform target;

        /// <summary>
        /// Attribs to follow.
        /// </summary>
        public Flag flags; 
                
        /// <summary>
        /// Sets this orbit into the target's position/rotation.
        /// </summary>
        public void Snap() {
            if(!target) return;
            orbit.anchor         = target.position;
            orbit.anchorRotation = target.rotation;            
        }

        /// <summary>
        /// Enable/Disable different follow flags.
        /// </summary>
        /// <param name="p_px"></param>
        /// <param name="p_y"></param>
        /// <param name="p_z"></param>
        /// <param name="p_rx"></param>
        /// <param name="p_ry"></param>
        /// <param name="p_z"></param>
        public void Follow(bool p_x,bool p_y,bool p_z,bool p_rx,bool p_ry,bool p_rz) {
            flags = Flag.None;
            flags |= p_x  ? Flag.PositionX : 0;
            flags |= p_y  ? Flag.PositionY : 0;
            flags |= p_z  ? Flag.PositionZ : 0;
            flags |= p_rx ? Flag.RotationX : 0;
            flags |= p_ry ? Flag.RotationY : 0;
            flags |= p_rz ? Flag.RotationZ : 0;
        }

        /// <summary>
        /// Updates the input.
        /// </summary>
        protected void LateUpdate() {        
            if(!target) return;
            if(!enabled) return;
    
            Vector3 pos     = target.position;
            Vector3 rot     = target.eulerAngles;

            if((flags & Flag.PositionX)==0) pos.x = orbit.anchor.x;
            if((flags & Flag.PositionY)==0) pos.y = orbit.anchor.y;
            if((flags & Flag.PositionZ)==0) pos.z = orbit.anchor.z;

            Vector3 oea = orbit.anchorEulerAngles;

            if((flags & Flag.RotationX)==0) rot.x = oea.x;
            if((flags & Flag.RotationY)==0) rot.y = oea.y;
            if((flags & Flag.RotationZ)==0) rot.z = oea.z;
            
            if((flags & Flag.PositionXYZ)!=0)  orbit.anchor          = pos;
            if((flags & Flag.RotationXYZ)!=0)  orbit.anchorRotation  = Quaternion.Euler(rot);
                
        }
        
    }

}