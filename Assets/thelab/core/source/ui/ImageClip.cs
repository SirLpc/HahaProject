using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace thelab.core {

    /// <summary>
    /// Class that extends MovieClip to implement Unity's Image UI clip player.
    /// </summary>    
    public class ImageClip : MovieClip<Sprite,Image> {

        /// <summary>
        /// CTOR.
        /// </summary>
        protected override void Awake() {
            if(!target) target = GetComponent<Image>();
            base.Awake();
        }

        /// <summary>
        /// Handler for frame changes.
        /// </summary>
        /// <param name="p_frame"></param>
        protected override void OnFrame(Sprite p_frame) {
            if(target) target.sprite = p_frame;
        }
    }

}