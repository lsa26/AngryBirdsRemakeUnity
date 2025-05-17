using UnityEditor;
using UnityEngine;
using System;

public class SetupAndroidBuild
{
    public static void PrepareProject()
    {
        try
        {
            UnityEngine.Debug.Log("Configuration Android...");

            // API 6.0 Marshmallow minimum (API level 23)
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
            // Cible Android 13 (API level 33) ou plus haut
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;

            // Utiliser la nouvelle surcharge avec NamedBuildTarget
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.yourcompany.angrybirdsremake");
            PlayerSettings.productName = "Angry Birds Remake";
            PlayerSettings.companyName = "Your Company";
            PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.Android.bundleVersionCode = 1;

            // Graphiques : OpenGLES3 + Vulkan
            PlayerSettings.SetGraphicsAPIs(
                BuildTarget.Android, 
                new UnityEngine.Rendering.GraphicsDeviceType[] {
                    UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3,
                    UnityEngine.Rendering.GraphicsDeviceType.Vulkan
                });

            // Orientation paysage uniquement
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeRight;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;

            // Location d’installation automatique
            PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;

            AssetDatabase.SaveAssets();
            UnityEngine.Debug.Log("Configuration terminée avec succès.");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Erreur de configuration Android: " + e.Message);
            EditorApplication.Exit(1);
        }
    }
}
