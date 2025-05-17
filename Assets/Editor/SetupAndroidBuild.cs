using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.IO;
using System;

public class SetupAndroidBuild
{
    // Méthode originale maintenue pour compatibilité
    public static void Configure()
    {
        Debug.Log("Configuration des paramètres Android...");
        ConfigureAndroidSettings();
        ConfigureScenes();
        AssetDatabase.SaveAssets();
        Debug.Log("Configuration Android terminée");
    }

    // Nouvelle méthode pour la préparation du projet - utilisée dans l'étape de pré-build
    public static void PrepareProject()
    {
        Debug.Log("Préparation du projet pour le build Android...");
        
        try
        {
            // Configuration des paramètres Android
            ConfigureAndroidSettings();
            
            // Configuration des scènes
            ConfigureScenes();
            
            // Pré-compilation des scripts sans faire de build complet
            AssetDatabase.Refresh();
            
            // Pré-génération des bibliothèques shader pour accélérer le build final
            PrecompileShaders();
            
            // Pré-chargement des assets majeurs
            PreloadMajorAssets();
            
            // Création d'un rapport de projet qui peut être consulté après le build
            GenerateProjectReport();
            
            AssetDatabase.SaveAssets();
            Debug.Log("Préparation du projet terminée avec succès");
        }
        catch (Exception e)
        {
            Debug.LogError("Erreur lors de la préparation du projet: " + e.Message);
            EditorApplication.Exit(1);
        }
    }
    
    // Extraction des paramètres Android pour réutilisation
    private static void ConfigureAndroidSettings()
    {
        // Changement de plateforme vers Android
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        
        // Paramètres du projet
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.bundleVersion = "1.0";
        PlayerSettings.companyName = "Demo";
        PlayerSettings.productName = "CloudBeesDemo";
        
        // Build system
        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        
        // API graphique
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
        GraphicsDeviceType[] devices = { GraphicsDeviceType.OpenGLES3 };
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, devices);
    }
    
    // Extraction de la configuration des scènes pour réutilisation
    private static void ConfigureScenes()
    {
        string menuScenePath = "Assets/Scenes/Menu.unity"; // Chemin à ajuster si nécessaire
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
        
        if (File.Exists(menuScenePath))
        {
            scenes.Add(new EditorBuildSettingsScene(menuScenePath, true));
        }
        else
        {
            Debug.LogWarning("Menu.unity introuvable à : " + menuScenePath);
        }
        
        foreach (var sceneGUID in AssetDatabase.FindAssets("t:scene"))
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
            if (scenePath != menuScenePath)
            {
                scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            }
        }
        
        EditorBuildSettings.scenes = scenes.ToArray();
    }
    
    // Pré-compilation des shaders pour accélérer le build final
    private static void PrecompileShaders()
    {
        Debug.Log("Pré-compilation des shaders...");
        
        // Trouve tous les shaders dans le projet
        string[] shaderGuids = AssetDatabase.FindAssets("t:shader");
        
        // Force la compilation des shaders
        foreach (string guid in shaderGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
        
        Debug.Log($"Pré-compilation de {shaderGuids.Length} shaders terminée");
    }
    
    // Pré-chargement des assets majeurs pour accélérer le build
    private static void PreloadMajorAssets()
    {
        Debug.Log("Pré-chargement des assets majeurs...");
        
        // Préchargement des préfabs
        string[] prefabGuids = AssetDatabase.FindAssets("t:prefab");
        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            EditorUtility.DisplayProgressBar("Pré-chargement des préfabs", path, 0.5f);
            AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }
        
        // Préchargement des textures importantes
        string[] textureGuids = AssetDatabase.FindAssets("t:texture");
        foreach (string guid in textureGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("UI") || path.Contains("Icon"))
            {
                AssetDatabase.LoadAssetAtPath<Texture>(path);
            }
        }
        
        EditorUtility.ClearProgressBar();
        Debug.Log("Pré-chargement des assets terminé");
    }
    
    // Génération d'un rapport de projet pour le débogage
    private static void GenerateProjectReport()
    {
        string reportPath = "ProjectReport.txt";
        using (StreamWriter writer = new StreamWriter(reportPath))
        {
            writer.WriteLine("=== RAPPORT DU PROJET UNITY ===");
            writer.WriteLine($"Date: {DateTime.Now}");
            writer.WriteLine($"Version Unity: {Application.unityVersion}");
            writer.WriteLine($"Plateforme cible: {EditorUserBuildSettings.activeBuildTarget}");
            writer.WriteLine($"Nombre de scènes: {EditorBuildSettings.scenes.Length}");
            
            writer.WriteLine("\n=== SCÈNES ===");
            foreach (var scene in EditorBuildSettings.scenes)
            {
                writer.WriteLine($"- {scene.path} (Activé: {scene.enabled})");
            }
            
            writer.WriteLine("\n=== PLUGINS ===");
            string[] pluginGuids = AssetDatabase.FindAssets("t:PluginImporter");
            foreach (string guid in pluginGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                PluginImporter plugin = AssetDatabase.LoadAssetAtPath<PluginImporter>(path);
                writer.WriteLine($"- {Path.GetFileName(path)} (Android: {plugin.GetCompatibleWithPlatform(BuildTarget.Android)})");
            }
        }
        
        Debug.Log("Rapport de projet généré: " + Path.GetFullPath(reportPath));
    }
}
