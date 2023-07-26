using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Autodesk.Revit.DB.Events;
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
