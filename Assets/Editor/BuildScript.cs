using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using UnityEditor.Build.Reporting;
using System.Collections.Generic;
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
        // Configuration pour Android
        EditorUserBuildSettings.development = false;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.allowDebugging = false;
        
        // Démarrer le chronomètre pour mesurer le temps de build
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // Chemin où le build sera créé
        string buildPath = Path.Combine(Directory.GetCurrentDirectory(), "Builds");
        Directory.CreateDirectory(buildPath);
        
        // Nom du fichier APK
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string apkName = $"AngryBirdsRemake_{timestamp}.apk";
        string apkPath = Path.Combine(buildPath, apkName);

        // Options de build
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetScenePaths(),
            locationPathName = apkPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        // Exécuter le build
        UnityEngine.Debug.Log($"Démarrage du build vers {apkPath}...");
        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        
        // Traiter les résultats du build
        CreateBuildManifest(apkPath, report.summary);
        
        // Mesurer le temps écoulé
        stopwatch.Stop();
        TimeSpan ts = stopwatch.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        
        // Afficher le résultat
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
        // Créer un fichier manifeste avec les informations du build
        string manifestPath = Path.Combine(Path.GetDirectoryName(apkPath), "build_manifest.json");
        
        StringBuilder jsonBuilder = new StringBuilder();
        jsonBuilder.AppendLine("{");
        jsonBuilder.AppendLine($"  \"buildDate\": \"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\"");
        jsonBuilder.AppendLine($", \"buildResult\": \"{summary.result}\"");
        jsonBuilder.AppendLine($", \"buildPath\": \"{apkPath}\"");
        jsonBuilder.AppendLine($", \"totalErrors\": {summary.totalErrors}");
        jsonBuilder.AppendLine($", \"totalWarnings\": {summary.totalWarnings}");
        jsonBuilder.AppendLine($", \"unityVersion\": \"{Application.unityVersion}\"");
        
        // Ajouter PlayerSettings
        jsonBuilder.AppendLine($", \"productName\": \"{PlayerSettings.productName}\"");
        jsonBuilder.AppendLine($", \"bundleIdentifier\": \"{PlayerSettings.applicationIdentifier}\"");
        jsonBuilder.AppendLine($", \"version\": \"{PlayerSettings.bundleVersion}\"");
        jsonBuilder.AppendLine($", \"buildNumber\": \"{PlayerSettings.Android.bundleVersionCode}\"");
        
        jsonBuilder.AppendLine("}");
        
        File.WriteAllText(manifestPath, jsonBuilder.ToString());
        UnityEngine.Debug.Log($"Manifeste de build créé à {manifestPath}");
    }
}
