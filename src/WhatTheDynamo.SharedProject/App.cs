using Autodesk.Internal.InfoCenter;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI.Events;
using WhatTheDynamo.Classes;
using RibbonButton = Autodesk.Windows.RibbonButton;
using RibbonItem = Autodesk.Revit.UI.RibbonItem;


namespace WhatTheDynamo
{
    internal class App : IExternalApplication
    {
        private UIControlledApplication _uiApp;
        public Result OnStartup(UIControlledApplication a)
        {
            _uiApp = a;

            Global.DynamoVersionFound = FindDynamoVersions();
            if (Global.DynamoVersionFound)
            {
                //first show the notification
                ShowNotification();

                //then rename the button
                var button = Utilities.GetButton("Manage", "Visual Programming", "Dynamo");
                button.Text = $"Dynamo{Environment.NewLine}{Global.DynamoVersion.Major}.{Global.DynamoVersion.Minor}";

      
                //and set the contextual help to take you to the packages folder
  
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
                Category = "What the Dyn?!",
                IsNew = true,
                Timestamp = DateTime.Now,
                Uri = new Uri(Global.DefaultDynamoPackagePath)
            };

            ComponentManager.InfoCenterPaletteManager.ShowBalloon(result);
        }
      
    }
}
