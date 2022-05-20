using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class MultiPlatformBuilder : MonoBehaviour {

    [MenuItem("Build/Build all")]
    public static void BuildAll() {
        BuildWebGL();
        BuildAndroid();
        BuildLinux();
        BuildWindows();
        BuildMac();
    }

    [MenuItem("Build/Build WebGL")] public static void BuildWebGL() { BuildPlatform(BuildTarget.WebGL, "webgl"); }
    [MenuItem("Build/Build Android")] public static void BuildAndroid() { BuildPlatform(BuildTarget.Android, "android.apk"); }
    [MenuItem("Build/Build Linux")] public static void BuildLinux() { BuildPlatform(BuildTarget.StandaloneLinux64, "linux/lolsuXP.x86_64"); }
    [MenuItem("Build/Build Windows")] public static void BuildWindows() { BuildPlatform(BuildTarget.StandaloneWindows64, "windows/lolsuXP.exe"); }
    [MenuItem("Build/Build MacOS")] public static void BuildMac() { BuildPlatform(BuildTarget.StandaloneOSX, "osx.app"); }

    public static void BuildPlatform(BuildTarget target, string destination) {
        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();
        options.locationPathName = $"Builds/{Application.version} {destination}";
        options.target = target;
        options.options = BuildOptions.None;
        BuildReport report = BuildPipeline.BuildPlayer(options);

        switch (report.summary.result) {
            case BuildResult.Succeeded: Debug.Log("Successfully built: " + target.ToString()); break;
            case BuildResult.Failed: Debug.Log("Build failed for: " + target.ToString()); break;
            case BuildResult.Cancelled: Debug.Log("Build cancelled for: " + target.ToString()); break;
            case BuildResult.Unknown: Debug.Log("Build status unknown for: " + target.ToString()); break;
        }
    }

}
