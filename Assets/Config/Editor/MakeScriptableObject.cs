using UnityEngine;
using System.Collections;
using UnityEditor;

public class MakeScriptableObject
{
    [MenuItem("Assets/Create/Scriptable Object")]
    public static void CreateMyAsset()
    {
        ScriptableObject asset = ScriptableObject.CreateInstance<UnityEngine.ScriptableObject>();

        string name = UnityEditor.AssetDatabase.GenerateUniqueAssetPath("Assets/Config/Config.asset");
        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}