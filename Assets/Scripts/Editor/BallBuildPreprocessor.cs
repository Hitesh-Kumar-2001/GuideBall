using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Copies Assets/Balls/Prefab/ → Assets/Resources/Balls/Prefab/ before build,
// then removes the temporary Resources copy after build.
public class BallBuildPreprocessor : IPreprocessBuildWithReport, IPostprocessBuild
{
    const string SourceFolder    = "Assets/Balls/Prefab";
    const string ResourcesFolder = "Assets/Resources/Balls/Prefab";

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (!Directory.Exists(SourceFolder))
        {
            Debug.LogWarning($"[BallBuildPreprocessor] Source folder not found: {SourceFolder}");
            return;
        }

        if (!Directory.Exists(ResourcesFolder))
            Directory.CreateDirectory(ResourcesFolder);

        foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { SourceFolder }))
        {
            var src  = AssetDatabase.GUIDToAssetPath(guid);
            var dest = $"{ResourcesFolder}/{Path.GetFileName(src)}";
            AssetDatabase.CopyAsset(src, dest);
        }

        AssetDatabase.Refresh();
        Debug.Log($"[BallBuildPreprocessor] Prefabs copied to {ResourcesFolder}");
    }

    public void OnPostprocessBuild(BuildTarget target, string path)
    {
        if (!Directory.Exists(ResourcesFolder)) return;

        FileUtil.DeleteFileOrDirectory(ResourcesFolder);
        FileUtil.DeleteFileOrDirectory(ResourcesFolder + ".meta");

        var parent = "Assets/Resources/Balls";
        if (Directory.Exists(parent) && Directory.GetFileSystemEntries(parent).Length == 0)
        {
            FileUtil.DeleteFileOrDirectory(parent);
            FileUtil.DeleteFileOrDirectory(parent + ".meta");
        }

        AssetDatabase.Refresh();
        Debug.Log("[BallBuildPreprocessor] Temporary Resources copy cleaned up.");
    }
}
