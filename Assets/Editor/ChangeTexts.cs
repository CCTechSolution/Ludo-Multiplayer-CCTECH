using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeTexts : EditorWindow
{
    //LilitaOne-Regular
    [MenuItem("Custom/Util/FixMissingFont")]

    public static void AssignFont()
    {
        // Find the font asset by name
        string[] guids = AssetDatabase.FindAssets("t:Font LilitaOne-Regular");

        if (guids.Length == 0)
        {
            Debug.LogError("LilitaOne-Regular font not found in the project.");
            return;
        }

        string fontPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        Font lilitaOneFont = AssetDatabase.LoadAssetAtPath<Font>(fontPath);

        if (lilitaOneFont == null)
        {
            Debug.LogError("Failed to load the LilitaOne-Regular font asset.");
            return;
        }

        // Assign font to all Text components in all scenes
        AssignFontInScenes(lilitaOneFont);

        // Assign font to all Text components in all prefabs
        AssignFontInPrefabs(lilitaOneFont);

        Debug.Log("Font assignment completed.");
    }

    private static void AssignFontInScenes(Font lilitaOneFont)
    {
        var scenes = EditorBuildSettings.scenes;

        List<string> scenePaths = new List<string>();
        foreach (var scene in scenes)
        {
            if (scene.enabled)
            {
                scenePaths.Add(scene.path);
            }
        }

        foreach (var scenePath in scenePaths)
        {
            // Load the scene
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // Find all Text components in the scene
            Text[] textComponents = GameObject.FindObjectsOfType<Text>();

            foreach (var text in textComponents)
            {
                if (text.font == null)
                {
                    text.font = lilitaOneFont;
                    EditorUtility.SetDirty(text);
                    Debug.Log($"Assigned LilitaOne-Regular font to Text component on GameObject {text.gameObject.name} in scene {scene.name}");
                }
            }

            // Save the scene
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }

    private static void AssignFontInPrefabs(Font lilitaOneFont)
    {
        // Find all prefabs in the project
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

        foreach (var prefabGuid in prefabGuids)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogWarning($"Could not load prefab at path {prefabPath}, skipping.");
                continue;
            }

            // Find all Text components in the prefab
            Text[] textComponents = prefab.GetComponentsInChildren<Text>(true);

            bool modified = false;
            foreach (var text in textComponents)
            {
                if (text.font == null)
                {
                    text.font = lilitaOneFont;
                    EditorUtility.SetDirty(text);
                    modified = true;
                    Debug.Log($"Assigned LilitaOne-Regular font to Text component on GameObject {text.gameObject.name} in prefab {prefab.name}");
                }
            }

            if (modified)
            {
                try
                {
                    PrefabUtility.SavePrefabAsset(prefab);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Could not save prefab {prefab.name} at path {prefabPath}: {e.Message}");
                }
            }
        }
    }


    [MenuItem("Custom/Util/ConvertText")]

    static void DoubleGrassDetailsSize()
    {

      /*  var scene = SceneManager.GetActiveScene();
        var objectsRoot = scene.GetRootGameObjects();

       List<TextMeshProUGUI> tmps = new List<TextMeshProUGUI>();
        foreach (var item in objectsRoot)
        {
           tmps.AddRange(item.GetComponentsInChildren<TextMeshProUGUI>(true));
        }

        foreach (var item in tmps)
        {
            ReplaceToText(item);

        }
      */


    }

    /* private static void ReplaceToText(TMP_Text item)
     {
         var oldText = item.text;
         var oldColor = item.color;
         var oldAlignment = item.alignment;

         var go = item.gameObject;
         DestroyImmediate(item);

         var textComp = go.AddComponent<UnityEngine.UI.Text>();
         textComp.text = oldText;
         textComp.color = oldColor;
         textComp.resizeTextForBestFit = true;

         textComp.alignment = MapTextAlignmentOptionsToTextAnchor(oldAlignment);




     }*/
    /*
        public static TextAnchor MapTextAlignmentOptionsToTextAnchor(TextAlignmentOptions alignment)
        {
            switch (alignment)
            {
                case TextAlignmentOptions.TopLeft:
                    return TextAnchor.UpperLeft;
                case TextAlignmentOptions.Top:
                case TextAlignmentOptions.TopJustified:
                case TextAlignmentOptions.TopFlush:
                case TextAlignmentOptions.TopGeoAligned:
                    return TextAnchor.UpperCenter;
                case TextAlignmentOptions.TopRight:
                    return TextAnchor.UpperRight;
                case TextAlignmentOptions.Left:
                    return TextAnchor.MiddleLeft;
                case TextAlignmentOptions.Center:
                case TextAlignmentOptions.Justified:
                case TextAlignmentOptions.Flush:
                case TextAlignmentOptions.CenterGeoAligned:
                    return TextAnchor.MiddleCenter;
                case TextAlignmentOptions.Right:
                    return TextAnchor.MiddleRight;
                case TextAlignmentOptions.BottomLeft:
                    return TextAnchor.LowerLeft;
                case TextAlignmentOptions.Bottom:
                case TextAlignmentOptions.BottomJustified:
                case TextAlignmentOptions.BottomFlush:
                case TextAlignmentOptions.BottomGeoAligned:
                    return TextAnchor.LowerCenter;
                case TextAlignmentOptions.BottomRight:
                    return TextAnchor.LowerRight;
                default:
                    return TextAnchor.UpperLeft; // Default case if alignment value is not recognized
            }
        }*/

}
