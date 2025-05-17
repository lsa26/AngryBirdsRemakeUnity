using UnityEditor;
using UnityEngine;
using System;

public class SetupAndroidBuild
{
    public static void PrepareProject()
    {
        try
        {
            Debug.Log("🔧 Configuration de l'environnement de build Android...");

            // Niveau API minimum : Android 6.0 (Marshmallow)
            PlayerSettings.Android.minSdkVersion    = AndroidSdkVersions.AndroidApiLevel23;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;

            // Définir l'identifiant de l'application pour Android
            PlayerSettings.applicationIdentifier = "com.yourcompany.angrybirdsremake";
            PlayerSettings.productName           = "Angry Birds Remake";
            PlayerSettings.companyName           = "Your Company";
            PlayerSettings.bundleVersion         = "1.0.0";
            PlayerSettings.Android.bundleVersionCode = 1;

            // Configurer les APIs graphiques (OpenGLES3 + Vulkan)
            PlayerSettings.SetGraphicsAPIs(
                BuildTarget.Android,
                new UnityEngine.Rendering.GraphicsDeviceType[] {
                    UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3,
                    UnityEngine.Rendering.GraphicsDeviceType.Vulkan
                }
            );

            // Orientation : paysage uniquement
            PlayerSettings.defaultInterfaceOrientation       = UIOrientation.LandscapeRight;
            PlayerSettings.allowedAutorotateToPortrait       = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft  = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;

            // Préférer l'installation automatique sur l'appareil
            PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;

            // Sauvegarder toutes les modifications
            AssetDatabase.SaveAssets();
            Debug.Log("✅ Configuration terminée avec succès.");
        }
        catch (Exception e)
        {
            Debug.LogError("❌ Erreur lors de la configuration Android : " + e.Message);
            EditorApplication.Exit(1);
        }
    }
}
