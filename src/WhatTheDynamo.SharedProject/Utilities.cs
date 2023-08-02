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

            foreach (AW.RibbonTab tab in ribbon.Tabs)
            {
                if (tab.Id == tabId)
                {
                    Global.ManageTab = tab;

                    foreach (AW.RibbonPanel panel in tab.Panels)
                    {
                        //the visual programming panel data is as follows
                        //ID: "visualprogramming_shr"
                        //AutomationName: "Visual Programming"
                        if (panel.Source.Id == panelId)
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
