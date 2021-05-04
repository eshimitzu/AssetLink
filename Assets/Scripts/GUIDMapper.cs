using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class GUIDMapper
{
    private const int GUID_LENGTH = 32;
    private const string FILE_NAME = "guids";

    private static Dictionary<string, string> guidMap = new Dictionary<string, string>();
    private static bool initialized = false;


    public static string GUIDToAssetPath(string guid)
    {
        string path = string.Empty;
#if UNITY_EDITOR
        path = AssetDatabase.GUIDToAssetPath(guid);
#else
        Init();
        if (!guidMap.TryGetValue(guid, out path))
        {
            Debug.Log("GUIDMapper.Log can't find path for Guid : " + guid);
        }
#endif
        return path;
    }


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RuntimeInitialize()
    {
#if !UNITY_EDITOR
        Init();
#endif
    }

    private static void Init()
    {
        if(initialized)
            return;
        
        var asset = Resources.Load<TextAsset>(FILE_NAME);
        
        if (asset)
        {
            Parse(asset.text);
        }
        else
        {
            Debug.LogError("GUIDMapper.Error can't find file : " + FILE_NAME);
        }
            
        initialized = true;
    }


    private static void Parse(string result)
    {
        float time = Time.realtimeSinceStartup;

        string[] paths = result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0, pathsLength = paths.Length; i < pathsLength; i++) 
        {
            var line = paths[i];
            string guid = line.Substring(0, GUID_LENGTH);
            string path = line.Substring(GUID_LENGTH, line.Length - GUID_LENGTH);
            guidMap.Add(guid, path);
        }

        time = (Time.realtimeSinceStartup - time) * 1000;
        Debug.Log("[GUIDMapper] Parsed by : " + time.ToString("f10") + " miliseconds for " + guidMap.Keys.Count);
    }
}
