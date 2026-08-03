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
            //nothing this add-in does is worth failing Revit's startup over, so no exception escapes here.
            //returning anything but Succeeded shows the user an error dialog on every launch
            try
            {
                //store the version if it is found, and the package path if it is found
                Global.DynamoVersionFound = FindDynamoVersions();

                //no Dynamo is a normal environment, not a failure: stay quiet
                if (!Global.DynamoVersionFound) return Result.Succeeded;

                FindDynamoPackagePath();

                //if we got here, we found Dynamo, show the notification
                ShowNotification();

                //then rename the button, if it is where we expect it to be
                var button = Utilities.GetButton("Manage", "visualprogramming_shr", "Dynamo");
                if (button is not null)
                {
                    button.Text = $"Dynamo{Environment.NewLine}{Global.DynamoVersion.Major}.{Global.DynamoVersion.Minor}";
                }
            }
            catch (Exception)
            {
                //a cosmetic add-in must never break Revit startup
            }

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

        internal void FindDynamoPackagePath()
        {
            if (!Global.DynamoVersionFound) return;

            //find the DynamoSettings.xml
            string probableXmlPath =
                Path.Combine(Global.UserRoaming, "Dynamo", "Dynamo Revit", Global.TruncatedDynamoVersion);

            //fall back to the conventional location unless the settings file names a user folder
            Global.DefaultDynamoPackagePath = DefaultPackagePath();

            if (!Directory.Exists(probableXmlPath)) return;

            Global.DynamoSettingsXml = Path.Combine(probableXmlPath, "DynamoSettings.xml");

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(File.ReadAllText(Global.DynamoSettingsXml));

                //package folders line from XML
                var xnList = xml.SelectSingleNode("/PreferenceSettings/CustomPackageFolders");
                if (xnList is null) return;

                //skip the built in and the program data locations, and find the first one that a user uses.
                foreach (XmlNode xn in xnList.ChildNodes)
                {
                    string packagePath = xn.InnerText;

                    if (IsBuiltInPackagePath(packagePath)) continue;

                    Global.DefaultDynamoPackagePath = packagePath;
                    return;
                }
            }
            catch (Exception)
            {
                //can't read settings file, keep the default set above
            }
        }

        //ordinal comparisons only: under Turkish and Azerbaijani casing rules the 'I' in %BuiltInPackages%
        //does not lower-case to an ASCII 'i', which let the literal token through as if it were a real folder
        static bool IsBuiltInPackagePath(string packagePath)
        {
            if (string.IsNullOrWhiteSpace(packagePath)) return true;

            if (packagePath.IndexOf("builtinpackages", StringComparison.OrdinalIgnoreCase) >= 0) return true;

            //ProgramData is not necessarily on C:
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            return !string.IsNullOrEmpty(programData) &&
                   packagePath.StartsWith(programData, StringComparison.OrdinalIgnoreCase);
        }

        static string DefaultPackagePath()
        {
            return Path.Combine(Global.UserRoaming, "Dynamo", "Dynamo Revit", Global.TruncatedDynamoVersion, "packages");
        }

        internal static void ShowNotification()
        {
            //build our notification bubble
            ResultItem result = new ResultItem
            {
                Title = $"Current loaded Dynamo Version: {Global.DynamoVersion}",
                Category = "What the Dyn?!",
                IsNew = true,
                Timestamp = DateTime.Now
            };

            //the dynamo package path was found, make the bubble open it.
            //settings files can hold tokens rather than real paths, so only link something Uri accepts
            if (!string.IsNullOrWhiteSpace(Global.DefaultDynamoPackagePath) &&
                Uri.TryCreate(Global.DefaultDynamoPackagePath, UriKind.Absolute, out var packageUri))
            {
                result.Uri = packageUri;
            }

            //show the result
            ComponentManager.InfoCenterPaletteManager.ShowBalloon(result);
        }

    }
}
