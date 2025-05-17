using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class BuildScript
{
    public static void Build()
    {
        try
        {
            Debug.Log("Démarrage du build Android...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            // Vérifier et créer le dossier de build
            string buildPath = "Builds";
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }
            
            // Assurer que les paramètres Android sont correctement configurés
            SetupAndroidBuild.Configure();
            
            // Générer un nom de fichier avec timestamp
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string apkName = $"{PlayerSettings.productName}_{PlayerSettings.bundleVersion}_{timestamp}.apk";
            string apkPath = Path.Combine(buildPath, apkName);
            
            // Vérifier si les scènes sont bien configurées
            if (EditorBuildSettings.scenes.Length == 0)
            {
                throw new Exception("Aucune scène n'est configurée pour le build!");
            }
            
            // Options de build avec compression optimisée
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = GetEnabledScenePaths(),
                locationPathName = apkPath,
                target = BuildTarget.Android,
                options = BuildOptions.CompressWithLz4HC
            };
            
            // Activer le mode développeur si nécessaire (pour le débogage)
            bool developmentBuild = Environment.GetEnvironmentVariable("DEVELOPMENT_BUILD") == "true";
            if (developmentBuild)
            {
                buildPlayerOptions.options |= BuildOptions.Development;
                buildPlayerOptions.options |= BuildOptions.ConnectWithProfiler;
                Debug.Log("Mode développement activé pour ce build");
            }
            
            // Exécuter le build
            Debug.Log($"Début du build vers: {apkPath}");
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            
            // Analyser le résultat du build
            stopwatch.Stop();
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($"Build réussi en {stopwatch.Elapsed.TotalMinutes:F2} minutes!");
                Debug.Log($"Taille du build: {report.summary.totalSize / 1048576.0f:F2} MB");
                Debug.Log($"Warnings: {report.summary.totalWarnings}");
                
                // Créer un fichier manifeste avec les informations du build
                CreateBuildManifest(apkPath, report.summary, stopwatch.Elapsed);
            }
            else
            {
                throw new Exception($"Build échoué avec le statut: {report.summary.result}, erreurs: {report.summary.totalErrors}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Erreur de build: {e.Message}\n{e.StackTrace}");
            EditorApplication.Exit(1);
        }
    }
    
    // Génère le manifeste du build avec des informations détaillées
    private static void CreateBuildManifest(string apkPath, UnityEditor.Build.Reporting.BuildSummary summary, TimeSpan buildTime)
    {
        string manifestPath = Path.Combine("Builds", "build_manifest.json");
        string json = $@"{{
            ""buildDate"": ""{DateTime.Now}"",
            ""buildDuration"": ""{buildTime.TotalMinutes:F2} minutes"",
            ""productName"": ""{PlayerSettings.productName}"",
            ""bundleVersion"": ""{PlayerSettings.bundleVersion}"",
            ""bundleVersionCode"": {PlayerSettings.Android.bundleVersionCode},
            ""unityVersion"": ""{Application.unityVersion}"",
            ""buildSize"": ""{summary.totalSize / 1048576.0f:F2} MB"",
            ""warnings"": {summary.totalWarnings},
            ""errors"": {summary.totalErrors},
            ""outputPath"": ""{apkPath.Replace("\\", "\\\\")}"",
            ""sceneCount"": {EditorBuildSettings.scenes.Length}
        }}";
        
        File.WriteAllText(manifestPath, json);
        Debug.Log($"Manifeste du build créé à: {manifestPath}");
    }
    
    // Récupère les chemins des scènes activées
    private static string[] GetEnabledScenePaths()
    {
        var scenes = EditorBuildSettings.scenes;
        var enabledScenes = new System.Collections.Generic.List<string>();
        
        foreach (var scene in scenes)
        {
            if (scene.enabled)
            {
                enabledScenes.Add(scene.path);
            }
        }
        
        if (enabledScenes.Count == 0)
        {
            Debug.LogWarning("Aucune scène activée trouvée. Utilisation de toutes les scènes disponibles.");
            foreach (var scene in scenes)
            {
                enabledScenes.Add(scene.path);
            }
        }
        
        return enabledScenes.ToArray();
    }
}
