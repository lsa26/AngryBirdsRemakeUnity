using UnityEditor;
          using UnityEngine;
          using System.Collections.Generic;
          using System.IO;
          using System;

          public class BuildScript
          {
              static string[] GetScenePaths()
              {
                  // Trouver toutes les scènes dans Assets/Scenes
                  List<string> scenes = new List<string>();

                  // Ajouter explicitement les scènes que nous avons identifié
                  scenes.Add("Assets/Scenes/MainMenu.unity");
                  scenes.Add("Assets/Scenes/Level1.unity");
                  scenes.Add("Assets/Scenes/Level2.unity");

                  // Afficher les scènes trouvées dans le log
                  Debug.Log("Scènes incluses dans le build:");
                  foreach (string scene in scenes)
                  {
                      Debug.Log("- " + scene);
                  }

                  return scenes.ToArray();
              }

              [MenuItem("Build/Build Android")]
              public static void BuildAndroid()
              {
                  Debug.Log("Démarrage du build Android...");
                  
                  // Configurer les paramètres Android
                  PlayerSettings.Android.keystorePass = "";
                  PlayerSettings.Android.keyaliasPass = "";
                  
                  // Définir un bundle ID valide
                  PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.yourgame.angrybirdsremake");
                  
                  // Configurer la version minimale
                  PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
                  
                  // Définir le chemin de sortie
                  string buildPath = Path.Combine(Application.dataPath, "../Builds/Android/AngryBirdsRemake.apk");
                  
                  // Créer le répertoire s'il n'existe pas
                  Directory.CreateDirectory(Path.GetDirectoryName(buildPath));
                  
                  // Options de build
                  BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
                  {
                      scenes = GetScenePaths(),
                      locationPathName = buildPath,
                      target = BuildTarget.Android,
                      options = BuildOptions.None
                  };
                  
                  // Afficher la configuration
                  Debug.Log("Chemin de build: " + buildPath);
                  Debug.Log("Nombre de scènes: " + buildPlayerOptions.scenes.Length);
                  
                  // Lancer le build
                  BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
                  BuildSummary summary = report.summary;
                  
                  // Afficher les résultats
                  Debug.Log("Résultat du build: " + summary.result);
                  Debug.Log("Taille du build: " + summary.totalSize + " bytes");
                  Debug.Log("Temps total: " + summary.totalTime.TotalSeconds + " secondes");
                  
                  // Vérifier si le build a réussi
                  if (summary.result == BuildResult.Succeeded)
                  {
                      Debug.Log("Build réussi: " + buildPath);
                      EditorApplication.Exit(0);
                  }
                  else
                  {
                      Debug.LogError("Build échoué!");
                      EditorApplication.Exit(1);
                  }
              }
          }
