using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using WhatTheDynamo.Classes;


namespace WhatTheDynamo
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Result.Failed with no message shows the user nothing at all, so say what happened instead
            if (!Global.DynamoVersionFound)
            {
                TaskDialog.Show("What the Dyn?!", "No loaded Dynamo version was detected in this session.");
                return Result.Cancelled;
            }

            App.ShowNotification();
            return Result.Succeeded;
        }
    }
}
