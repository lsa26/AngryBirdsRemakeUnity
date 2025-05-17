using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using UnityEditor.Build.Reporting;
using System.Diagnostics;

public class BuildScript
{
    private static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        return scenes;
    }

    public static void Build()
    {
        EditorUserBuildSettings.development = false;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.allowDebugging = false;

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        string buildPath = Path.Combine(Directory.GetCurrentDirectory(), "Builds");
        Directory.CreateDirectory(buildPath);

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string apkName = $"AngryBirdsRemake_{timestamp}.apk";
        string apkPath = Path.Combine(buildPath, apkName);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetScenePaths(),
            locationPathName = apkPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        UnityEngine.Debug.Log($"Démarrage du build vers {apkPath}...");
        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        CreateBuildManifest(apkPath, report.summary);

        stopwatch.Stop();
        TimeSpan ts = stopwatch.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

        if (report.summary.result == BuildResult.Succeeded)
        {
            FileInfo fileInfo = new FileInfo(apkPath);
            float fileSizeMB = fileInfo.Length / (1024f * 1024f);

            UnityEngine.Debug.Log($"Build réussi en {elapsedTime}");
            UnityEngine.Debug.Log($"Taille du build: {fileSizeMB:F2} MB");
            UnityEngine.Debug.Log($"Chemin du build: {apkPath}");
        }
        else
        {
            UnityEngine.Debug.LogError($"Build échoué après {elapsedTime}: {report.summary.result}");
        }
    }

    private static void CreateBuildManifest(string apkPath, UnityEditor.Build.Reporting.BuildSummary summary)
    {
        string manifestPath = Path.Combine(Path.GetDirectoryName(apkPath), "build_manifest.json");

        StringBuilder jsonBuilder = new StringBuilder();
        jsonBuilder.AppendLine("{");
        jsonBuilder.AppendLine($"  \"buildDate\": \"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\"");
        jsonBuilder.AppendLine($", \"buildResult\": \"{summary.result}\"");
        jsonBuilder.AppendLine($", \"buildPath\": \"{apkPath}\"");
        jsonBuilder.AppendLine($", \"totalErrors\": {summary.totalErrors}");
        jsonBuilder.AppendLine($", \"totalWarnings\": {summary.totalWarnings}");
        jsonBuilder.AppendLine($", \"unityVersion\": \"{Application.unityVersion}\"");
        jsonBuilder.AppendLine($", \"productName\": \"{PlayerSettings.productName}\"");
        jsonBuilder.AppendLine($", \"bundleIdentifier\": \"{PlayerSettings.applicationIdentifier}\"");
        jsonBuilder.AppendLine($", \"version\": \"{PlayerSettings.bundleVersion}\"");
        jsonBuilder.AppendLine($", \"buildNumber\": \"{PlayerSettings.Android.bundleVersionCode}\"");
        jsonBuilder.AppendLine("}");

        File.WriteAllText(manifestPath, jsonBuilder.ToString());
        UnityEngine.Debug.Log($"Manifeste de build créé à {manifestPath}");
    }
}
