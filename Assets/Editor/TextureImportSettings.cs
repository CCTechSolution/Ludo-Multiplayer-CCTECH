using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureImportSettings : AssetPostprocessor
{

    /*
    [MenuItem("Tools/Apply iOS Texture Import Settings")]
    static void ApplyTextureImportSettings()
    {
        string[] texturePaths = AssetDatabase.FindAssets("t:Texture2D", new string[] { "Assets" });

        foreach (string texturePath in texturePaths)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(texturePath);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer != null)
            {
                importer.textureType = TextureImporterType.Default;
                importer.textureCompression = TextureImporterCompression.Compressed;
                //importer.mipmapEnabled = false; // Assuming you don't want mipmaps
                importer.isReadable = false; // Assuming you don't need to read pixels
                importer.alphaIsTransparency = true; // Assuming you don't need alpha
                importer.alphaSource = TextureImporterAlphaSource.FromInput;
                importer.sRGBTexture = true; // Assuming textures are in sRGB color space
                importer.wrapMode = TextureWrapMode.Clamp; // Assuming you want to clamp textures
                importer.filterMode = FilterMode.Trilinear; // Assuming you want trilinear filtering

                TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings();
                platformSettings.name = "iOS";
                platformSettings.overridden = true;
                platformSettings.maxTextureSize = 1024;
                platformSettings.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;

                importer.SetPlatformTextureSettings(platformSettings);

                importer.SaveAndReimport();
            }
        }

        Debug.Log("iOS Texture Import Settings Applied Successfully!");
    }*/

    void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        AssetDatabase.StartAssetEditing();

        foreach (string path in importedAssets)
        {
            if (Path.GetExtension(path).ToLower() == ".png" || Path.GetExtension(path).ToLower() == ".jpg" || Path.GetExtension(path).ToLower() == ".tga" || Path.GetExtension(path).ToLower() == ".tif" || Path.GetExtension(path).ToLower() == ".psd")
            {
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (texture != null)
                {
                    OnPostprocessTexture(texture);
                }
            }
        }

        AssetDatabase.StopAssetEditing();

        foreach (string path in importedAssets)
        {
            TextureImporter importer = assetImporter as TextureImporter;
            AssetDatabase.ImportAsset(importer.assetPath);
        }

    }

    void OnPostprocessTexture(Texture2D texture)
    {
        // Check if the texture is square
        bool isPOT = IsPowerOfTwo(texture.width) && IsPowerOfTwo(texture.height);

        // Get the import settings
        TextureImporter importer = assetImporter as TextureImporter;
        TextureImporterPlatformSettings platformSettings = importer.GetPlatformTextureSettings("Android");
        platformSettings.maxTextureSize = 1024;
        platformSettings.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
        platformSettings.overridden = true; // Set override to true

        if (isPOT)
        {
            platformSettings.textureCompression = TextureImporterCompression.Compressed;
            platformSettings.format = TextureImporterFormat.ETC2_RGBA8Crunched;
            platformSettings.compressionQuality = 100;
        }
        else
        {
            platformSettings.textureCompression = TextureImporterCompression.Compressed;
            platformSettings.format = TextureImporterFormat.ASTC_6x6;
            platformSettings.compressionQuality = (int)TextureCompressionQuality.Best;
        }

        importer.SetPlatformTextureSettings(platformSettings);
        

        // Apply the import settings
        //AssetDatabase.ImportAsset(importer.assetPath);
    }

    bool IsPowerOfTwo(int number)
    {
        // A power of two has only one bit set in its binary representation.
        // For example: 1, 2, 4, 8, 16, 32, ...

        // Check if the number is positive
        if (number <= 0)
            return false;

        // Use bitwise AND to check if only one bit is set
        return (number & (number - 1)) == 0;
    }



}
