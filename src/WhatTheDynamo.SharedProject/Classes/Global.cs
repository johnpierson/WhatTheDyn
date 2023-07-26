using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using Autodesk.Windows;

namespace WhatTheDynamo.Classes
{
    internal class Global
    {
        internal static string ExecutingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string UserRoaming => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        internal static Version DynamoVersion { get; set; }
        internal static bool DynamoVersionFound { get; set; } = false;
        internal static string DefaultDynamoPackagePath { get; set; }

        internal static RibbonItem DynamoButton { get; set; }
        internal static RibbonPanel VisualProgrammingPanel { get; set; }
        internal static RibbonTab ManageTab { get; set; }
        internal static RibbonTab AddinsTab { get; set; }
    }
}
