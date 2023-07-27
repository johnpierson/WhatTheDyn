using Autodesk.Internal.InfoCenter;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using WhatTheDynamo.Classes;


namespace WhatTheDynamo
{
    internal class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            //store the version if it is found, and the package path if it is found
            Global.DynamoVersionFound = FindDynamoVersions();
            FindDynamoPackagePath();

            //if we didn't find the Dynamo installation, return a fail result
            if (!Global.DynamoVersionFound) return Result.Failed;

            //if we got here, we found Dynamo, show the notification
            ShowNotification();

            //then rename the button
            var button = Utilities.GetButton("Manage", "Visual Programming", "Dynamo");
            button.Text = $"Dynamo{Environment.NewLine}{Global.DynamoVersion.Major}.{Global.DynamoVersion.Minor}";

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

        internal bool FindDynamoVersions()
        {
            //find the DynamoRevit dll
            var dynamoRevit = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.Contains("DynamoRevitVersionSelector"));

            //if DynamoRevit isn't loaded return false
            if (dynamoRevit is null) return false;

            //get the version from that DynamoRevit dll
            Global.DynamoVersion = dynamoRevit.GetName().Version;

            return true;
        }

        internal bool FindDynamoPackagePath()
        {
            if(!Global.DynamoVersionFound) return false;

            //find the DynamoSettings.xml
            string probableXmlPath =
                Path.Combine(Global.UserRoaming, "Dynamo", "Dynamo Revit", Global.TruncatedDynamoVersion);

            //if the directory is found, try to read the XML
            if (Directory.Exists(probableXmlPath))
            {
                Global.DynamoSettingsXml = Path.Combine(probableXmlPath, "DynamoSettings.xml");

                try
                {
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(File.ReadAllText(Global.DynamoSettingsXml));

                    //package folders line from XML
                    var xnList = xml.SelectNodes("/PreferenceSettings/CustomPackageFolders")[0];
                    var packagePaths = xnList.ChildNodes;

                    //skip the built in and the program data locations, and find the first one that a user uses.
                    foreach (XmlNode xn in packagePaths)
                    {
                        string packagePath = xn.InnerText;

                        if (!packagePath.ToLower().Contains(@"builtinpackages") && !packagePath.ToLower().StartsWith("c:\\programdata\\"))
                        {
                            Global.DefaultDynamoPackagePath = packagePath;
                            return true;
                        }
                    }
                }
                catch (Exception)
                {
                    //can't read settings file, set it to default
                    Global.DefaultDynamoPackagePath =
                        $"{Global.UserRoaming}\\Dynamo\\Dynamo Revit\\{Global.DynamoVersion.Major}.{Global.DynamoVersion.Minor}\\packages";
                    return true;
                }

            }
            else
            {
                //can't find settings file, set it to default
                Global.DefaultDynamoPackagePath =
                    $"{Global.UserRoaming}\\Dynamo\\Dynamo Revit\\{Global.DynamoVersion.Major}.{Global.DynamoVersion.Minor}\\packages";
                return true;
            }

            //we reached this endpoint and probably couldn't find it, return false
            return false;
        }
        internal static void ShowNotification()
        {
            //build our notification bubble
            ResultItem result = new ResultItem
            {
                Title = $"{Properties.Resources.NotificationBubble} {Global.DynamoVersion}",
                Category = "What the Dyn?!",
                IsNew = true,
                Timestamp = DateTime.Now
            };

            //the dynamo package path was found, return it
            if (!string.IsNullOrWhiteSpace(Global.DefaultDynamoPackagePath))
            {
                result.Uri = new Uri(Global.DefaultDynamoPackagePath);
            }

            //show the result
            ComponentManager.InfoCenterPaletteManager.ShowBalloon(result);
        }

    }
}
