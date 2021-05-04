using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;


public class GUIDWritter : IPreprocessBuildWithReport
{
    private const string FILE_NAME = "guids.bytes";

    
    public int callbackOrder => 1;
    
    public void OnPreprocessBuild(BuildReport report)
    {
        CollectData();
    }

    
    [MenuItem("Assets/CollectGUIDs", false, 10001)]
    private static void CollectData()
    {
        WriteGUIDs("Assets/Resources/");
    }
    
    
    private static void WriteGUIDs(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (Directory.Exists(directory))
        {
            string filePath = Path.Combine(directory, FILE_NAME);
            string[] paths = AssetDatabase.GetAllAssetPaths();

            StringBuilder sb = new StringBuilder();
            for (int i = 0, pathsLength = paths.Length; i < pathsLength; i++)
            {
                var path = paths[i];
                if (
                    path.StartsWith("Assets/", StringComparison.Ordinal) &&
                    !path.Contains("Editor") &&
                    !path.Contains(".cs") &&
                    (
                        path.Contains("Resources") ||
                        path.Contains("StreamingAssets")
                    ))
                {
                    sb.Append(AssetDatabase.AssetPathToGUID(path));
                    sb.AppendLine(path);
                }
            }

            var file = new FileInfo(filePath);
            file.Directory.Create();
            File.WriteAllText(file.FullName, sb.ToString());
        }   
    }
}