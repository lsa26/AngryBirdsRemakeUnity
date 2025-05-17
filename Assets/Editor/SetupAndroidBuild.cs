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

            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;

            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.yourcompany.angrybirdsremake");
            PlayerSettings.productName = "Angry Birds Remake";
            PlayerSettings.companyName = "Your Company";
            PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.Android.bundleVersionCode = 1;

            PlayerSettings.Android.disableDepthAndStencilBuffers = false;
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new UnityEngine.Rendering.GraphicsDeviceType[] {
                UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3,
                UnityEngine.Rendering.GraphicsDeviceType.Vulkan
            });

            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeRight;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;

            PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;

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
