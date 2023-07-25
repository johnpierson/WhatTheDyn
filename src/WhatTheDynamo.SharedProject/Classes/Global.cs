using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace WhatTheDynamo.Classes
{
    internal class Global
    {
        internal static string ExecutingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string UserRoaming => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        internal static Version DynamoVersion { get; set; }
        internal static bool DynamoVersionFound { get; set; } = false;
        internal static string DefaultDynamoPackagePath { get; set; }
    }
}
