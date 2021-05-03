using System;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

[Serializable]
public sealed class AssetLink : Asset
{
    #region Variables

    const string RESOURCE_FOLDER = "Resources/";
	public string name;
	public string assetGUID;

    string fullPath = null;
    string resourcePath = null;
    Object loadedAsset;

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
    
    public override bool IsLoaded => loadedAsset;

    #endregion

	
    #region Unity lifecycles

	public AssetLink(Object asset)
	{
        this.SetAsset(asset);
	}
	

    [Obsolete("For RESOURCES use AssetResource, for STREAMING ASSETS use AssetStreaming")]
    public AssetLink(string path)
    {
        name = Path.GetFileNameWithoutExtension(path);
        fullPath = path;
    }

    #endregion


    #region Public methods

    public override void Load()
    {
        #if UNITY_EDITOR
        loadedAsset = AssetDatabase.LoadAssetAtPath(FullPath, typeof(Object));
        #else
        if (!IsLoaded)
        {
            if (IsResource)
            {
                loadedAsset = Resources.Load(ResourcePath, typeof(Object));
            }
        }
        #endif
    }
    

    public override void Unload()
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
    public Object GetAsset()
    {
        Object result = null;
        if (IsLoaded)
        {
            result = loadedAsset;
        }
        else
        {
            Load();
            if (IsLoaded)
            {
                result = loadedAsset;
            }
        }
        return result;
    }
    

    // Use IDisposable interface or Unload for free resources
    public Object GetInstance()
    {
        Object result = null;

        var asset = GetAsset();
        if (asset)
        {
            result = Object.Instantiate(loadedAsset);
        }
        
        return result;
    }

    #endregion


    #region Private methods

    private void SetAsset(Object asset)
    {
        #if UNITY_EDITOR
        if (asset != null)
        {
            string newAssetPath = AssetDatabase.GetAssetPath(asset);
            assetGUID = AssetDatabase.AssetPathToGUID(newAssetPath);
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
