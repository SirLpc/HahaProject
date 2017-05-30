using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

namespace thelab.core {

    /// <summary>
    /// Callback to handle WebImage load events.
    /// </summary>
    [System.Serializable]
    public class WebImageCallback : UnityEvent<WebImageEvent> { }

    /// <summary>
    /// Event data for WebImage feedbacks.
    /// </summary>
    [System.Serializable]
    public class WebImageEvent {

        /// <summary>
        /// Reference to the target image.
        /// </summary>
        public WebImage target;

        /// <summary>
        /// Load progress.
        /// </summary>
        public float progress;

        /// <summary>
        /// Error string if any.
        /// </summary>
        public string error;
    }

    /// <summary>
    /// Class that implements an Image component with support for web loaded images.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class WebImage : MonoBehaviour {
        
        /// <summary>
        /// Current url of a loaded image.
        /// </summary>
        public string url;

        /// <summary>
        /// Load on start.
        /// </summary>
        public bool loadOnStart = true;
            
        /// <summary>
        /// Event handler.
        /// </summary>
        public WebImageCallback OnEvent;

        private void Start()
        {
            if (!loadOnStart)
                return;

            Load();
        }


        public void Load()
        {
            var sp = Web.LoadSprite(url, (data, progress, loader) => { });
            GetComponent<Image>().sprite = sp;
        }
    }

}