using System;
using System.Collections.Generic;
using UnityEngine;

namespace thelab.core {

    /// <summary>
    /// Class extends the Basic AudioComponent and adds a generic typed indexer.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioComponent<T> : AudioComponent
    {
        /// <summary>
        /// Enumaration or other identifier of this audio.
        /// </summary>
        new public T type { 

            get {
                Array el = Enum.GetValues(typeof(T));
                if (base.type < 0) return el.Length <= 0 ? default(T) : ((T)(object)el.GetValue(0));
                return (T)(object)el.GetValue(base.type);
            }
            set { base.type = (int)(object)value; }
        }

        /// <summary>
        /// Searches children for an audio indexed by its 'type' variable.
        /// </summary>
        /// <param name="p_type"></param>
        /// <returns></returns>
        public AudioComponent Find(T p_type) {        
            return 
            audios.Find(delegate(AudioComponent it) {               
                if (it is AudioComponent<T>) { 
                    AudioComponent<T> cit = (AudioComponent<T>)it; 
                    if(!(cit.type is IComparable)) return false;
                    IComparable cit_type = (IComparable)cit.type;
                    return cit_type.CompareTo(p_type)==0; 
                }
                return false;
            });
        }

        #region Playback

        /// <summary>
        /// Searches a children by type and plays an audio based on its type.
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_time"></param>
        /// <param name="p_volume"></param>
        public void Play(T p_type, float p_time = -1f, float p_volume = -1f)  { AudioComponent a = Find(p_type); if(a)a.Play(p_time, p_volume); }

        /// <summary>
        /// Instantiate an audio, plays it and destroy it.
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_time"></param>
        /// <param name="p_volume"></param>
        public void PlayInstance(T p_type, float p_time = -1f, float p_volume = -1f)  { 
            AudioComponent a = Find(p_type);
            if(a) {        
                Transform p = a.transform;        
                string n = a.name;
                a = Instantiate<AudioComponent>(a);
                a.transform.SetParent(p);
                a.name = n;                
                a.source.loop=false;
                a.Play(p_time,p_volume);
                Destroy(a.gameObject,a.source.clip.length+0.5f);                
            }
        }

        /// <summary>
        /// Searches a children by type and stops the playback.
        /// </summary>
        /// <param name="p_type"></param>
        public void Stop(T p_type) { AudioComponent a = Find(p_type); if(a)a.Stop();  }

        /// <summary>
        /// Searches a children by type and pauses its playback.
        /// </summary>
        /// <param name="p_type"></param>
        public void Pause(T p_type) { AudioComponent a = Find(p_type); if(a) a.Pause(); }

        #endregion

        #region Fade

        /// <summary>
        /// Searches a children by its type and fades the volume.
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_volume"></param>
        /// <param name="p_time"></param>
        /// <param name="p_easing"></param>
        public void Fade(T p_type, float p_volume, float p_time,Easing p_easing=null) { AudioComponent a = Find(p_type); if(a)a.Fade(p_volume,p_time,p_easing);  }

        /// <summary>
        /// Searches a children by its type and fades the volume.
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_volume"></param>
        /// <param name="p_easing"></param>
        public void Fade(T p_type, float p_volume,Easing p_easing=null) { Fade(p_type,p_volume,0.8f,p_easing);  }

        #endregion

        #region FadePitch

        /// <summary>
        /// Fades the audio pitch.
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_pitch"></param>
        /// <param name="p_time"></param>
        public void FadePitch(T p_type, float p_pitch, float p_time,Easing p_easing=null) { AudioComponent a = Find(p_type); if(a)a.FadePitch(p_pitch,p_time,p_easing); }

        /// <summary>
        /// Fades the audio pitch.
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_pitch"></param>
        /// <param name="p_time"></param>
        public void FadePitch(T p_type, float p_pitch,Easing p_easing=null)  { FadePitch(p_type,p_pitch,0.8f,p_easing); }

        #endregion


    }

    /// <summary>
    /// Class that wraps the Audio features of Unity and adds utility functions to organize and facilitate audio playback and manipulation.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioComponent : MonoBehaviour {
        /// <summary>
        /// Generic Enumeration value of this audio for identification.
        /// </summary>
        public int type;

        /// <summary>
        /// Returns the audio source of this component.
        /// </summary>
        public AudioSource source { get { return GetComponent<AudioSource>(); } }

        /// <summary>
        /// List of children audios.
        /// </summary>
        public List<AudioComponent> audios {
            get { 
                if(m_audios!=null) return m_audios; 
                m_audios = new List<AudioComponent>(GetComponentsInChildren<AudioComponent>()); 
                m_audios.Remove(this);
                return m_audios;
            }
        }
        private List<AudioComponent> m_audios;

        #region Find

        /// <summary>
        /// Finds a children AudioComponent by name.
        /// </summary>
        /// <param name="p_name"></param>
        /// <returns></returns>
        public AudioComponent Find(string p_name) {
            Transform t = transform.Find(p_name);
            if (!t) return null;
            return t.GetComponent<AudioComponent>();
        }

        #endregion

        #region Play
        
        /// <summary>
        /// Searches a children by name and plays it.
        /// </summary>
        /// <param name="p_name"></param>
        /// <param name="p_time"></param>
        /// <param name="p_volume"></param>
        public void Play(string p_name, float p_time = -1f, float p_volume = -1f) { AudioComponent a = Find(p_name); if(a)a.Play(p_time, p_volume); }

        /// <summary>
        /// Plays the audio of this component.
        /// </summary>
        /// <param name="p_audio"></param>
        /// <param name="p_time"></param>
        /// <param name="p_volume"></param>
        public void Play(float p_time = -1f, float p_volume = -1f) {
            AudioSource src = source;
            if (src == null) {
                Debug.LogWarning("AudioComponent> Tried to play null sound!");
                return;
            }
            src.Play();
            if (p_time >= 0f)   src.time    = p_time;
            if (p_volume >= 0f) src.volume  = p_volume;
        }

        #endregion

        #region Stop

        /// <summary>
        /// Searches a children by name and stops the playback.
        /// </summary>
        /// <param name="p_type"></param>
        public void Stop(string p_name){ AudioComponent a = Find(p_name); if(a)a.Stop(); }
    
        /// <summary>
        /// Stops the playback.
        /// </summary>
        public void Stop() {if(source)source.Stop(); }

        #endregion

        #region Pause

        /// <summary>
        /// Searches a children by name and pauses its playback.
        /// </summary>
        /// <param name="p_type"></param>
        public void Pause(string p_name) { AudioComponent a = Find(p_name); if(a) a.Pause(); }

        /// <summary>
        /// Stops the audio
        /// </summary>
        /// <param name="p_audio"></param>
        public void Pause() { if (source) source.Pause(); }

        #endregion

        #region Fade

        /// <summary>
        /// Searches a children by name and fades the volume.
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_volume"></param>
        /// <param name="p_time"></param>
        public void Fade(string p_name, float p_volume, float p_time,Easing p_easing=null) { AudioComponent a = Find(p_name); if(a)a.Fade(p_volume,p_time,p_easing); }

        /// <summary>
        /// Searches a children by name and fades the volume.
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_volume"></param>
        /// <param name="p_time"></param>
        public void Fade(string p_name, float p_volume,Easing p_easing=null) { Fade(p_name,p_volume,0.8f,p_easing); }

        /// <summary>
        /// Fades the audio volume.
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_volume"></param>
        /// <param name="p_time"></param>
        public void Fade(float p_volume, float p_time,Easing p_easing=null) { if (!source) return; Tween.Add<float>(source, "volume", p_volume, p_time,p_easing==null ? null : p_easing); }

        /// <summary>
        /// Fades the audio volume.
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_volume"></param>
        /// <param name="p_time"></param>
        public void Fade(float p_volume, Easing p_easing=null) { Fade(p_volume,0.8f,p_easing); }

        #endregion

        #region FadePitch

        /// <summary>
        /// Fades the audio pitch.
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_pitch"></param>
        /// <param name="p_time"></param>
        public void FadePitch(string p_name, float p_pitch, float p_time,Easing p_easing=null) { AudioComponent a = Find(p_name); if(a)a.FadePitch(p_pitch,p_time,p_easing); }

        /// <summary>
        /// Fades the audio pitch.
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_pitch"></param>
        /// <param name="p_time"></param>
        public void FadePitch(string p_name, float p_pitch,Easing p_easing=null) {  FadePitch(p_name,p_pitch,0.8f,p_easing); }
        
        /// <summary>
        /// Fades the audio pitch.
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_pitch"></param>
        /// <param name="p_time"></param>
        public void FadePitch(float p_pitch,Easing p_easing=null) { Fade(p_pitch,0.8f,p_easing); }

        /// <summary>
        /// Fades the audio pitch.
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_pitch"></param>
        /// <param name="p_time"></param>
        public void FadePitch(float p_pitch, float p_time = 0.8f,Easing p_easing=null) {
            if (!source) return;
            if (p_time <= 0f) { source.pitch = p_pitch; }
            else              { Tween.Add<float>(source, "pitch", p_pitch, p_time, p_easing==null ? Cubic.Out : p_easing); }
        }


        #endregion

    }

}