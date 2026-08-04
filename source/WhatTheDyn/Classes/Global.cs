using System;
using Autodesk.Windows;

namespace WhatTheDynamo.Classes
{
    internal class Global
    {
        public static string UserRoaming => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        internal static Version DynamoVersion { get; set; }
        internal static string TruncatedDynamoVersion => $"{DynamoVersion.Major}.{DynamoVersion.Minor}";
        internal static bool DynamoVersionFound { get; set; } = false;

        //dynamo settings file
        internal static string DynamoSettingsXml { get; set; }
        //default dynamo package path
        internal static string DefaultDynamoPackagePath { get; set; }

        internal static RibbonItem DynamoButton { get; set; }
        internal static RibbonPanel VisualProgrammingPanel { get; set; }
        internal static RibbonTab ManageTab { get; set; }
    }
}
