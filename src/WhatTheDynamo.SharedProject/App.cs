using Autodesk.Internal.InfoCenter;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using WhatTheDynamo.Classes;


namespace WhatTheDynamo
{
    internal class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            Global.DynamoVersionFound = FindDynamoVersions();
            if (Global.DynamoVersionFound)
            {
                ShowNotification();
            }
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
        internal bool FindDynamoVersions()
        {
            var dynamoRevit = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.Contains("DynamoRevitVersionSelector"));

            if (dynamoRevit is null) return false;


            Global.DynamoVersion = dynamoRevit.GetName().Version;
            //now set the package path location
            Global.DefaultDynamoPackagePath =
                $"{Global.UserRoaming}\\Dynamo\\Dynamo Revit\\{Global.DynamoVersion.Major}.{Global.DynamoVersion.Minor}\\packages";

            return true;
        }
        internal static void ShowNotification()
        {
            ResultItem result = new ResultItem
            {
                Title = $"{Properties.Resources.NotificationBubble} {Global.DynamoVersion}",
                Category = "whatTheDynamo",
                IsNew = true,
                Timestamp = DateTime.Now
            };

            ComponentManager.InfoCenterPaletteManager.ShowBalloon(result);
        }
    }
}
