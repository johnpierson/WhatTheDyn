using System;
using System.Linq;
using WhatTheDynamo.Classes;
using AW = Autodesk.Windows;

namespace WhatTheDynamo
{
    internal class Utilities
    {
        public static AW.RibbonItem GetButton(string tabId, string panelId, string itemName)
        {
            AW.RibbonControl ribbon = AW.ComponentManager.Ribbon;
            if (ribbon is null) return null;

            foreach (AW.RibbonTab tab in ribbon.Tabs)
            {
                if (tab.Id != tabId) continue;

                Global.ManageTab = tab;

                foreach (AW.RibbonPanel panel in tab.Panels)
                {
                    //the visual programming panel data is as follows
                    //ID: "visualprogramming_shr"
                    //AutomationName: "Visual Programming"
                    if (panel.Source?.Id != panelId) continue;

                    Global.VisualProgrammingPanel = panel;

                    //AutomationName is the display text, so an item without one (a separator, for example)
                    //must not throw, and a missing Dynamo button must return null rather than blow up startup
                    Global.DynamoButton = panel.Source.Items
                        .FirstOrDefault(b => string.Equals(b?.AutomationName, itemName, StringComparison.Ordinal));

                    return Global.DynamoButton;
                }
            }
            return null;
        }

    }
}
