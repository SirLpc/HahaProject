using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace thelab.core {

    /// <summary>
    /// Callback that handles WWW load/upload progress, completion and errors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="p_data"></param>
    /// <param name="p_progress"></param>
    /// <param name="p_loader"></param>
    public delegate void WebCallback<T>(T p_data,float p_progress,WWW p_loader=null);

    /// <summary>
    /// Class that handles the upload/download of assets and data.
    /// </summary>
    public class Web {

        /// <summary>
        /// Load an asset from web, with or without data to send, and extracts the correct result from the loader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_url"></param>
        /// <param name="p_callback"></param>
        /// <param name="p_data"></param>
        /// <param name="p_headers"></param>
        /// <param name="p_timeout"></param>
        /// <param name="p_editor"></param>
        /// <returns></returns>
        static protected WWW Load<T>(string p_url,WebCallback<T> p_callback,object p_data,Dictionary<string,string> p_headers = null,int p_timeout=-1,bool p_editor=false,bool p_use_cache=false,int p_version=1) {

            WWW ld;
            Dictionary<string,string> d = p_headers==null ? (new Dictionary<string,string>()) : p_headers;
            if(p_data==null) {
                ld = p_use_cache ? WWW.LoadFromCacheOrDownload(p_url,p_version) : new WWW(p_url,null,d);                
            }
            else {
                if(p_data is WWWForm) 
                    ld = p_use_cache ? WWW.LoadFromCacheOrDownload(p_url,p_version) : new WWW(p_url,(WWWForm)p_data); 
                else
                    ld = p_use_cache ? WWW.LoadFromCacheOrDownload(p_url,p_version) : new WWW(p_url,(byte[])p_data,d);
            }
            if(p_editor) {
                #if UNITY_EDITOR
                Activity.RunEditor(GenerateCallback<T>(ld,p_callback,p_timeout),0f);
                #endif
            }
            else {
                Activity.Run(GenerateCallback<T>(ld,p_callback,p_timeout),0f);
            }            
            return ld;
        }

        /// <summary>
        /// Generates the WWW handling callback.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ld"></param>
        /// <param name="p_callback"></param>
        /// <param name="p_timeout"></param>
        /// <returns></returns>
        static private System.Action<Activity> GenerateCallback<T>(WWW ld,WebCallback<T> p_callback,int p_timeout) {
            return delegate(Activity a) {            
            
                //Check for timeout.
                if(p_timeout>=0) { if(a.elapsed>=p_timeout) { a.Stop(); ld.Dispose(); if(p_callback!=null)p_callback(default(T),1f,null); return; } }
                
                //Error detected
                if(ld.error != null) {
                    a.Stop(); 
                    if(p_callback!=null)p_callback(default(T),1f,ld); 
                    return;
                }

                float p=0f;
                
                //Upload step
                if(ld.progress<=0f)
                if(ld.uploadProgress<1f) {
                    p = ld.uploadProgress;
                    if(p_callback!=null)p_callback(default(T),p-1f,ld);  
                }
                //Download step
                if(ld.progress>0f) { 
                    p = ld.progress;
                    if(p_callback!=null)p_callback(default(T),p*0.999f,ld);  
                }

                //Check if done
                if(ld.isDone) {
                    a.Stop();

                    if(typeof(T)==typeof(string))       { if(p_callback!=null)p_callback((T)(object)ld.text,1f,ld);                   } else                        
                    if(typeof(T)==typeof(Texture2D))    { if(p_callback!=null)p_callback((T)(object)ld.texture,1f,ld);                } else
                    if(typeof(T)==typeof(Texture))      { if(p_callback!=null)p_callback((T)(object)ld.textureNonReadable,1f,ld);     } else
                    if(typeof(T)==typeof(byte[]))       { if(p_callback!=null)p_callback((T)(object)ld.bytes,1f,ld);                  } else
                    if(typeof(T)==typeof(AudioClip))    { if(p_callback!=null)p_callback((T)(object)ld.GetAudioClip(),1f,ld);              } else
                    if(typeof(T)==typeof(AssetBundle))  { if(p_callback!=null)p_callback((T)(object)ld.assetBundle,1f,ld);            } else
#if UNITY_EDITOR
                    if(typeof(T)==typeof(MovieTexture)) { if(p_callback!=null)p_callback((T)(object)ld.GetMovieTexture(),1f,ld);                  } else
#endif
                    if(typeof(T)==typeof(CSVFile))      { if(p_callback!=null)p_callback((T)(object)new CSVFile(ld.text),1f,ld);      } else {
                        //Try as Json
                        try {
                            T res = Serialize.FromJson<T>(ld.text);
                            if(p_callback!=null)p_callback(res,1f,ld);
                        }
                        catch(Exception err) {
                            Debug.Log("Web> Error Parsing Json.\n"+err.Message);
                            if(p_callback!=null)p_callback(default(T),1f,ld);
                        }
                    }
                }

            };
        }

        /// <summary>
        /// Load an asset from web, manage its progress and extracts the correct result from the loader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_url"></param>
        /// <param name="p_callback"></param>
        /// <param name="p_timeout"></param>
        /// <returns></returns>
        static public WWW Load<T>(string p_url,WebCallback<T> p_callback,int p_timeout=-1) { return Load<T>(p_url,p_callback,null,null,p_timeout,false); }

        /// <summary>
        /// Load an asset from web after sending data by POST, manage its progress and extracts the correct result from the loader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_url"></param>
        /// <param name="p_callback"></param>
        /// <param name="p_data"></param>
        /// <param name="p_timeout"></param>
        /// <returns></returns>
        static public WWW Load<T>(string p_url,WebCallback<T> p_callback,WWWForm p_data,int p_timeout=-1) { return Load<T>(p_url,p_callback,p_data,null,p_timeout); }

        /// <summary>
        /// Load an asset from web after sending data by POST, manage its progress and extracts the correct result from the loader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_url"></param>
        /// <param name="p_callback"></param>
        /// <param name="p_data"></param>
        /// <param name="p_timeout"></param>
        /// <returns></returns>
        static public WWW Load<T>(string p_url,WebCallback<T> p_callback,byte[] p_data,int p_timeout=-1) { return Load<T>(p_url,p_callback,p_data,null,p_timeout); }

        /// <summary>
        /// Load an asset from web after sending data by POST, manage its progress and extracts the correct result from the loader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_url"></param>
        /// <param name="p_callback"></param>
        /// <param name="p_data"></param>
        /// <param name="p_headers"></param>
        /// <param name="p_timeout"></param>
        /// <returns></returns>
        static public WWW Load<T>(string p_url,WebCallback<T> p_callback,WWWForm p_data,Dictionary<string,string> p_headers,int p_timeout=-1) { return Load<T>(p_url,p_callback,p_data,p_headers,p_timeout); }

        /// <summary>
        /// Load an asset from web after sending data by POST, manage its progress and extracts the correct result from the loader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_url"></param>
        /// <param name="p_callback"></param>
        /// <param name="p_data"></param>
        /// <param name="p_headers"></param>
        /// <param name="p_timeout"></param>
        /// <returns></returns>
        static public WWW Load<T>(string p_url,WebCallback<T> p_callback,byte[] p_data,Dictionary<string,string> p_headers,int p_timeout=-1) { return Load<T>(p_url,p_callback,p_data,p_headers,p_timeout); }

        /// <summary>
        /// Specilization of Load[AssetBundle](...) version. Allows the user to choose to use or not cache and version.
        /// </summary>
        /// <param name="p_url"></param>
        /// <param name="p_callback"></param>
        /// <param name="p_use_cache"></param>
        /// <param name="p_version"></param>
        /// <param name="p_timeout"></param>
        /// <returns></returns>
        static public WWW LoadBundle(string p_url,WebCallback<AssetBundle> p_callback,bool p_use_cache=true,int p_version=0,int p_timeout=-1) {
            WWW ld = p_use_cache ? WWW.LoadFromCacheOrDownload(p_url,p_version) : new WWW(p_url);
            Activity.Run(GenerateCallback<AssetBundle>(ld,p_callback,p_timeout),0f);
            return ld;            
        }

        /// <summary>
        /// Loads an image bytes and uses a default image as preview.
        /// </summary>
        /// <param name="p_url"></param>
        /// <param name="p_callback"></param>
        /// <param name="p_default"></param>
        /// <returns></returns>
        static public Texture2D LoadTexture(string p_url,WebCallback<byte[]> p_callback,Texture2D p_preview=null) {
            Texture2D pv = new Texture2D(1,1,TextureFormat.ARGB32,true);
            if(!p_preview) pv.SetPixel(0,0,Color.black); else pv.LoadImage(p_preview.EncodeToPNG());            
            Load<byte[]>(p_url,delegate(byte[] d,float p,WWW req) {                
                if(p>=1f) {
                    if(d!=null) {
                        if(pv)pv.LoadImage(d);
                    }                    
                    //Debug.Log("Web> LoadTexture - url["+p_url+"] length["+((d==null ? 0 : d.Length)/1024)+"kb]");
                }
                if(p_callback!=null) p_callback(d,p,req);
            });            
            return pv;
        }

        /// <summary>
        /// Loads a Texture and returns the soon-to-be Sprite.
        /// </summary>
        /// <param name="p_url"></param>
        /// <param name="p_callback"></param>
        /// <param name="p_preview"></param>
        /// <returns></returns>
        static public Sprite LoadSprite(string p_url,WebCallback<byte[]> p_callback,Texture2D p_preview = null) {
            Texture2D tex = LoadTexture(p_url,p_callback,p_preview);
            return Sprite.Create(tex,new Rect(0f,0f,tex.width,tex.height),Vector2.zero);
        }

        #region editor

        #if UNITY_EDITOR

        /// <summary>
        /// Load an asset from web, manage its progress and extracts the correct result from the loader. Runs on Editor escope.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_url"></param>
        /// <param name="p_callback"></param>
        /// <param name="p_timeout"></param>
        /// <returns></returns>
        static public WWW LoadEditor<T>(string p_url,WebCallback<T> p_callback,int p_timeout=-1) { return Load<T>(p_url,p_callback,null,null,p_timeout,true); }

        /// <summary>
        /// Load an asset from web after sending data by POST, manage its progress and extracts the correct result from the loader. Runs on Editor escope.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_url"></param>
        /// <param name="p_callback"></param>
        /// <param name="p_data"></param>
        /// <param name="p_timeout"></param>
        /// <returns></returns>
        static public WWW LoadEditor<T>(string p_url,WebCallback<T> p_callback,WWWForm p_data,int p_timeout=-1) { return Load<T>(p_url,p_callback,p_data,null,p_timeout,true); }

        /// <summary>
        /// Load an asset from web after sending data by POST, manage its progress and extracts the correct result from the loader. Runs on Editor escope.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_url"></param>
        /// <param name="p_callback"></param>
        /// <param name="p_data"></param>
        /// <param name="p_timeout"></param>
        /// <returns></returns>
        static public WWW LoadEditor<T>(string p_url,WebCallback<T> p_callback,byte[] p_data,int p_timeout=-1) { return Load<T>(p_url,p_callback,p_data,null,p_timeout,true); }

        /// <summary>
        /// Load an asset from web after sending data by POST, manage its progress and extracts the correct result from the loader. Runs on Editor escope.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_url"></param>
        /// <param name="p_callback"></param>
        /// <param name="p_data"></param>
        /// <param name="p_headers"></param>
        /// <param name="p_timeout"></param>
        /// <returns></returns>
        static public WWW LoadEditor<T>(string p_url,WebCallback<T> p_callback,WWWForm p_data,Dictionary<string,string> p_headers,int p_timeout=-1) { return Load<T>(p_url,p_callback,p_data,p_headers,p_timeout,true); }

        /// <summary>
        /// Load an asset from web after sending data by POST, manage its progress and extracts the correct result from the loader. Runs on Editor escope.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_url"></param>
        /// <param name="p_callback"></param>
        /// <param name="p_data"></param>
        /// <param name="p_headers"></param>
        /// <param name="p_timeout"></param>
        /// <returns></returns>
        static public WWW LoadEditor<T>(string p_url,WebCallback<T> p_callback,byte[] p_data,Dictionary<string,string> p_headers,int p_timeout=-1) { return Load<T>(p_url,p_callback,p_data,p_headers,p_timeout,true); }

        /// <summary>
        /// Specilization of Load[AssetBundle](...) version. Allows the user to choose to use or not cache and version. Runs on Editor escope.
        /// </summary>
        /// <param name="p_url"></param>
        /// <param name="p_callback"></param>
        /// <param name="p_use_cache"></param>
        /// <param name="p_version"></param>
        /// <param name="p_timeout"></param>
        /// <returns></returns>
        static public WWW LoadBundleEditor(string p_url,WebCallback<AssetBundle> p_callback,bool p_use_cache=true,int p_version=0,int p_timeout=-1) {
            WWW ld = p_use_cache ? WWW.LoadFromCacheOrDownload(p_url,p_version) : new WWW(p_url);
            Activity.RunEditor(GenerateCallback<AssetBundle>(ld,p_callback,p_timeout),0f);
            return ld;            
        }

        #endif

        #endregion

    }

}
