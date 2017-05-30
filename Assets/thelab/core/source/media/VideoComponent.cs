using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace thelab.core {

    #region enum VideoEventType

    /// <summary>
    /// Enumeration for video events.
    /// </summary>
    public enum VideoEventType {
        Start,
        Update,
        Stop,
        Complete,
        Skip
    }   

    #endregion

    /// <summary>
    /// Class that extends UnityEvent for editor purposes.
    /// </summary>
    [System.Serializable]
    public class VideoComponentCallback : UnityEvent<VideoEventType> { }

    /// <summary>
    /// Class that implements the Video features.
    /// </summary>  
    [RequireComponent(typeof(AudioSource))]
    public class VideoComponent : MonoBehaviour
    {

#if UNITY_EDITOR

        /// <summary>
        /// Reference to the video.
        /// </summary>
        public MovieTexture video;

        /// <summary>
        /// Flag that indicates the video will start playing after the game starts.
        /// </summary>
        public bool playOnAwake;

        /// <summary>
        /// Flag that allow the video to stop after any input.
        /// </summary>
        public bool allowSkip;

        /// <summary>
        /// Handler for video related event.
        /// </summary>
        public VideoComponentCallback OnEvent;

        /// <summary>
        /// Flag that tells if the video is playing.
        /// </summary>
        private bool m_is_playing;

        /// <summary>
        /// Reference to the AudioSource.
        /// </summary>
        private AudioSource m_audio;

        /// <summary>
        /// Plays the video.
        /// </summary>
        public void Play() {
            if(video)video.Play(); 
            m_audio.Play();
        }

        /// <summary>
        /// Stops the video.
        /// </summary>
        public void Stop() {

            if(!m_is_playing) return;
            if(video)video.Stop();
            m_audio.Stop();
            m_is_playing=false;
            OnEvent.Invoke(VideoEventType.Stop);
        }

        /// <summary>
        /// CTOR.
        /// </summary>
        protected void Awake() {
            m_is_playing = false;
            m_audio      = GetComponent<AudioSource>();
            if(video) m_audio.clip = video.audioClip;
            if(playOnAwake) if(video) {  Play(); }
        }

        /// <summary>
        /// Updates the internal detection.
        /// </summary>
        protected void Update() {

            if(!video) {
                if(m_is_playing) { Stop(); }
                return;
            }

            bool has_input = Input.anyKeyDown;

            if(has_input) { OnEvent.Invoke(VideoEventType.Skip); Stop(); return; }

            if(video.isPlaying) {
            
                if(!m_is_playing) { 
                    m_is_playing = true;                     
                    OnEvent.Invoke(VideoEventType.Start);
                }
                else {
                    OnEvent.Invoke(VideoEventType.Update);
                }
            }
            else {
                if(m_is_playing) {
                    Stop();
                    OnEvent.Invoke(VideoEventType.Complete);                    
                }
            }
        }

#endif

    }
}