#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public static class BuildScript
{
    private static readonly string[] scenes = FindEnabledEditorScenes();
    private const string GameName = "Katana";

    [MenuItem("File/Build")]
    public static void Build()
    {
        var buildsDir = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", "Builds"));

        BuildForLinux(buildsDir);
        BuildForWindows(buildsDir);
    }

    public static void BuildForLinux(string buildsDir)
    {
        string path = Path.Combine(buildsDir, "Linux");
        EnsureDirectoryExists(path);

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            target = BuildTarget.StandaloneLinux64,
            locationPathName = Path.Combine(path, $"{GameName}.x86_64"),
            options = BuildOptions.None
        };

        var result = BuildPipeline.BuildPlayer(options);
        Debug.Log("Linux build result: " + result.summary.result);
    }

    public static void BuildForWindows(string buildsDir)
    {
        string path = Path.Combine(buildsDir, "Windows");
        EnsureDirectoryExists(path);

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            target = BuildTarget.StandaloneWindows64,
            locationPathName = Path.Combine(path, $"{GameName}.exe"),
            options = BuildOptions.None
        };

        var result = BuildPipeline.BuildPlayer(options);
        Debug.Log("Windows build result: " + result.summary.result);
    }

    private static string[] FindEnabledEditorScenes() =>
        EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

    private static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
}
#endif