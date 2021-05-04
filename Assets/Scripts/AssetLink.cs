using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public sealed class AssetLink : IDisposable
{
    #region Variables

    private const string RESOURCE_FOLDER = "Resources/";
	public string name;
	public string assetGUID;

	private string fullPath = null;
	private string resourcePath = null;
	private Object loadedAsset;

    #endregion


    #region Properties

	public string FullPath
	{
		get
		{
            if (string.IsNullOrEmpty(fullPath))
			{
				fullPath = GUIDMapper.GUIDToAssetPath(assetGUID);
			}
			return fullPath;
		}
	}


	public string ResourcePath
	{
		get
		{
            if (string.IsNullOrEmpty(resourcePath))
			{
				int startIndex = FullPath.IndexOf(RESOURCE_FOLDER, StringComparison.Ordinal);
				if(startIndex > -1)
				{
					startIndex += RESOURCE_FOLDER.Length;
                    resourcePath = FullPath.Substring(startIndex);
                    resourcePath = resourcePath.RemovePathExtention();
				}
				else
				{
                    resourcePath = string.Empty;
				}
			}

			return resourcePath;
		}
	}

    public bool IsResource => !string.IsNullOrEmpty(ResourcePath);
    
    public bool IsLoaded => loadedAsset;

    #endregion

	
    #region Unity lifecycles

	public AssetLink(Object asset)
	{
        this.SetAsset(asset);
	}
	

    public AssetLink(string path)
    {
        name = Path.GetFileNameWithoutExtension(path);
        fullPath = path;
    }

    #endregion


    #region Public methods

    public void Load<T>()
    {
        #if UNITY_EDITOR
        loadedAsset = UnityEditor.AssetDatabase.LoadAssetAtPath(FullPath, typeof(T));
        #else
        if (!IsLoaded)
        {
            if (IsResource)
            {
                loadedAsset = Resources.Load(ResourcePath, typeof(T));
            }
        }
        #endif
    }
    

    public void Unload()
    {
        #if UNITY_EDITOR
        loadedAsset = null;
        #else
        if (IsLoaded)
        {
            if (IsResource)
            {
                Resources.UnloadAsset(loadedAsset);
                loadedAsset = null;
            }
        }
        #endif
    }


    // Use IDisposable interface or Unload for free resources
    public T GetAsset<T>() where T : Object
    {
        T result = default;
        if (IsLoaded)
        {
            result = loadedAsset as T;
        }
        else
        {
            Load<T>();
            if (IsLoaded)
            {
                result = loadedAsset as T;
            }
        }
        return result;
    }
    

    // Use IDisposable interface or Unload for free resources
    public T GetInstance<T>() where T : Object
    {
	    T result = default;

        var asset = GetAsset<T>();
        if (asset)
        {
            result = Object.Instantiate(loadedAsset) as T;
        }
        
        return result;
    }
    
    
    public void Dispose()
    {
	    Unload();
    }

    #endregion


    #region Private methods

    private void SetAsset(Object asset)
    {
        #if UNITY_EDITOR
        if (asset != null)
        {
            string newAssetPath = UnityEditor.AssetDatabase.GetAssetPath(asset);
            assetGUID = UnityEditor.AssetDatabase.AssetPathToGUID(newAssetPath);
            name = asset.name;
        }
        else
        {
            assetGUID = string.Empty;
            name = string.Empty;
        }

        fullPath = string.Empty;
        resourcePath = string.Empty;
        #endif
    }
        
    #endregion
}
