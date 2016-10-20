using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class LocalPlayerPrefabs
{

#if (UNITY_EDITOR || UNITY_ANDROID)
    private const string UIDKEY = "UIDKEY";
    private const string UPWDKEY = "UPWDKEY";
    private const string UNAMEKEY = "UNAMEKEY";

    public static string GetId()
    {
        string id = PlayerPrefs.GetString(UIDKEY);
        if (string.IsNullOrEmpty(id))
        {
            id = string.Format("UID{0}", Random.Range(0, 10000).ToString("0000"));
            PlayerPrefs.SetString(UIDKEY, id);
        }
        return id;
    }

    public static string GetPwd()
    {
        string pwd = PlayerPrefs.GetString(UPWDKEY);
        if (string.IsNullOrEmpty(pwd))
        {
            pwd = string.Format("PWD{0}", Random.Range(0, 10000).ToString("0000"));
            PlayerPrefs.SetString(UPWDKEY, pwd);
        }
        return pwd;
    }

    public static void SetName(string name)
    {
        PlayerPrefs.SetString(UNAMEKEY, name);
    }

    public static string GetName()
    {
        return PlayerPrefs.GetString(UNAMEKEY);
    }

#else
    private static string UIDKEY = string.Empty;
    private static string UPWDKEY = string.Empty;
    private static string UNAMEKEY = string.Empty;

    public static string GetId()
    {
        string id = UIDKEY;
        if (string.IsNullOrEmpty(id))
        {
            id = string.Format("UID{0}", Random.Range(0, 10000).ToString("0000"));
            UIDKEY = id;
        }
        return id;
    }

    public static string GetPwd()
    {
        string pwd = UPWDKEY;
        if (string.IsNullOrEmpty(pwd))
        {
            pwd = string.Format("PWD{0}", Random.Range(0, 10000).ToString("0000"));
            UPWDKEY = pwd;
        }
        return pwd;
    }

    public static void SetName(string name)
    {
        UPWDKEY = name;
    }

    public static string GetName()
    {
        return UNAMEKEY;
    }
#endif



}

