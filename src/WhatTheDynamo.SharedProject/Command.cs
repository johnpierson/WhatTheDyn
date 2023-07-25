#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WhatTheDynamo.Classes;

#endregion

namespace WhatTheDynamo
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (Global.DynamoVersionFound)
            {
                App.ShowNotification();
                return Result.Succeeded;
            }

            return Result.Failed;
        }
    }
}
