using UnityEditor;
using UnityEngine;
using System.IO;
using System;

public class SetupAndroidBuild
{
    public static void PrepareProject()
    {
        try
        {
            UnityEngine.Debug.Log("Configuration de l'environnement de build Android...");
            
            // Configuration des paramètres Android
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;
            
            // Configuration de l'application
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.yourcompany.angrybirdsremake");
            PlayerSettings.productName = "Angry Birds Remake";
            PlayerSettings.companyName = "Your Company";
            PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.Android.bundleVersionCode = 1;
            
            // Configuration de la qualité
            PlayerSettings.Android.disableDepthAndStencilBuffers = false;
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new UnityEngine.Rendering.GraphicsDeviceType[] { 
                UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3,
                UnityEngine.Rendering.GraphicsDeviceType.Vulkan
            });
            
            // Orientation
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeRight;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            
            // Icône de l'application
            // Note: Vous pouvez ajouter ici la configuration de l'icône si nécessaire
            
            // Configuration de la mémoire
            PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
            
            // Enregistrer les changements
            AssetDatabase.SaveAssets();
            
            UnityEngine.Debug.Log("Configuration du projet terminée avec succès.");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Erreur lors de la configuration du projet: " + e.Message);
            EditorApplication.Exit(1);
        }
    }
}
