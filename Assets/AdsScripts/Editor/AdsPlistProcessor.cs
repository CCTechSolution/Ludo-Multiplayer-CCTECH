//#if ((UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX))
//#define APPLE_LOGIN
//#endif

#if UNITY_IOS || UNITY_IPHONE

using System.IO;
using UnityEditor.Callbacks;
using UnityEditor;
using System;
using UnityEngine;
using UnityEditor.iOS.Xcode;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// PostProcessor script to automatically fill all required dependencies
/// </summary>
public class AdsPlistProcessor
{

#if APPLE_LOGIN
    private const string EntitlementsArrayKey = "com.apple.developer.applesignin";
    private const string DefaultAccessLevel = "Default";
    private const string AuthenticationServicesFramework = "AuthenticationServices.framework";
    private const BindingFlags NonPublicInstanceBinding = BindingFlags.NonPublic | BindingFlags.Instance;
#endif


    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            AddPListValues(buildPath);

            PBXProject project = new PBXProject();
            string projectPath = PBXProject.GetPBXProjectPath(buildPath);
            project.ReadFromFile(projectPath);


            project.AddCapability(project.GetUnityMainTargetGuid(), PBXCapabilityType.PushNotifications);


            var projCapability = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", null, project.GetUnityMainTargetGuid());
            project.AddCapability(project.GetUnityMainTargetGuid(), PBXCapabilityType.BackgroundModes);
            projCapability.AddBackgroundModes(BackgroundModesOptions.BackgroundFetch);


            projCapability.WriteToFile();



#if APPLE_LOGIN
            project.AddCapability(project.GetUnityMainTargetGuid(), PBXCapabilityType.SignInWithApple);

            var manager = new ProjectCapabilityManager(projectPath, Application.productName.Replace(" ","")+ ".entitlements", null, project.GetUnityMainTargetGuid());
            var managerType = typeof(ProjectCapabilityManager);
            var getOrCreateEntitlementDocMethod = managerType.GetMethod("GetOrCreateEntitlementDoc", NonPublicInstanceBinding);


            var entitlementFilePathField = managerType.GetField("m_EntitlementFilePath", NonPublicInstanceBinding);
            var entitlementFilePath = entitlementFilePathField.GetValue(manager) as string;
            var entitlementDoc = getOrCreateEntitlementDocMethod.Invoke(manager, new object[] { }) as PlistDocument;
            if (entitlementDoc != null)
            {
                var plistArray = new PlistElementArray();
                plistArray.AddString(DefaultAccessLevel);
                entitlementDoc.root[EntitlementsArrayKey] = plistArray;
            }
            manager.WriteToFile();
#endif


            project.WriteToFile(projectPath);
        }

        //GoogleMobileAds.Editor.GoogleMobileAdsSettingsEditor.OpenInspector();
    }

    static void AddPListValues(string pathToXcode)
    {
        // Get Plist from Xcode project 
        string plistPath = pathToXcode + "/Info.plist";
        // Read in Plist 
        PlistDocument plistObj = new PlistDocument();
        plistObj.ReadFromString(File.ReadAllText(plistPath));

        // set values from the root obj
        PlistElementDict plistRoot = plistObj.root;

        // Set value in plist
        plistRoot.SetString("NSUserTrackingUsageDescription", "This identifier will be used to deliver personalized ads to you.");







        /***/

        // Get root
        PlistElementDict rootDict = plistRoot;

        PlistElementArray SKAdNetworkItems = null;
        if (rootDict.values.ContainsKey("SKAdNetworkItems"))
        {
            try
            {
                SKAdNetworkItems = rootDict.values["SKAdNetworkItems"] as PlistElementArray;
            }
            catch (Exception e)
            {
                Debug.LogWarning(string.Format("Could not obtain SKAdNetworkItems PlistElementArray: {0}", e.Message));
            }
        }
        // If not exists, create it
        if (SKAdNetworkItems == null)
        {
            SKAdNetworkItems = rootDict.CreateArray("SKAdNetworkItems");
        }

        string path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Assets/AdsScripts/Editor/UnityAdsSKAdNetworkItems.xml");




        if (File.Exists(path))
        {
            List<string> skAdNetworkIds = new List<string>();

            using (FileStream fs = File.OpenRead(path))
            {
                XmlDocument document = new XmlDocument();
                document.Load(fs);

                XmlNode root = document.LastChild;
                
                XmlNodeList nodes = root.SelectNodes("SKAdNetworkIdentifier");

                foreach (XmlNode node in nodes)
                {
                    skAdNetworkIds.Add(node.InnerText);
                }



                if (SKAdNetworkItems != null)
                {
                    foreach (string id in skAdNetworkIds)
                    {
                        if (!ContainsSKAdNetworkIdentifier(SKAdNetworkItems, id))
                        {
                            PlistElementDict added = SKAdNetworkItems.AddDict();
                            added.SetString("SKAdNetworkIdentifier", id);
                        }
                    }
                }
            }
        }

        /***/




        // save
        File.WriteAllText(plistPath, plistObj.WriteToString());



    }

    private static bool ContainsSKAdNetworkIdentifier(PlistElementArray skAdNetworkItemsArray, string id)
    {
        foreach (PlistElement elem in skAdNetworkItemsArray.values)
        {
            try
            {
                PlistElementDict elemInDict = elem.AsDict();
                PlistElement value;
                bool identifierExists = elemInDict.values.TryGetValue("SKAdNetworkIdentifier", out value);

                if (identifierExists && value.AsString().Equals(id))
                {
                    return true;
                }
            }
#pragma warning disable 0168
            catch (Exception e)
#pragma warning restore 0168
            {
                // Do nothing
            }
        }

        return false;
    }

}
#endif