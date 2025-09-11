#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Version  upgrader handles performing and upgrade tasks.
/// </summary>
[InitializeOnLoad]
    public class VersionUpgrader
{
    /// <summary>
    /// Initializes static members of the <see cref="VersionUpgrader"/> class.
    /// </summary>
    static VersionUpgrader()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            Debug.Log("VersionUpgrader start");

        string[] result = AssetDatabase.FindAssets("Config", new string[] { "Assets/Config/" });

        string path = AssetDatabase.GUIDToAssetPath(result[0]);
        var config = (Config)AssetDatabase.LoadAssetAtPath(path, typeof(Config));

        
        config.BuildCode = Application.platform == RuntimePlatform.IPhonePlayer
            ? PlayerSettings.iOS.buildNumber
            : PlayerSettings.Android.bundleVersionCode.ToString();


        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("VersionUpgrader done");
        }
    }

#endif