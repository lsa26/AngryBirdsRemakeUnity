using UnityEditor;
using System.IO;

public class BuildScript
{
    public static void Build()
    {
        string outputPath = "Builds/Android/mygame.apk";
        if (!Directory.Exists("Builds/Android")) {
            Directory.CreateDirectory("Builds/Android");
        }

        BuildPlayerOptions buildOptions = new BuildPlayerOptions();
        buildOptions.scenes = new[] { "Assets/Scenes/Main.unity" };
        buildOptions.locationPathName = outputPath;
        buildOptions.target = BuildTarget.Android;
        buildOptions.options = BuildOptions.None;

        BuildPipeline.BuildPlayer(buildOptions);
    }
}
