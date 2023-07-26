using System.Linq;
using WhatTheDynamo.Classes;
using AW = Autodesk.Windows;

namespace WhatTheDynamo
{
    internal class Utilities
    {
        public static AW.RibbonItem GetButton(string tabName, string panelName, string itemName)
        {
            AW.RibbonControl ribbon = AW.ComponentManager.Ribbon;

            foreach (AW.RibbonTab tab in ribbon.Tabs)
            {
                if (tab.AutomationName == tabName)
                {
                    Global.ManageTab = tab;

                    foreach (AW.RibbonPanel panel in tab.Panels)
                    {
                        if (panel.Source.Title == panelName)
                        {
                            Global.VisualProgrammingPanel = panel;
                            Global.DynamoButton = panel.Source.Items.First(b => b.AutomationName.Equals(itemName));
                            return Global.DynamoButton;
                        }
                    }
                }
            }
            return null;
        }
        
    }
}
