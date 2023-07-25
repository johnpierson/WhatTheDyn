using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    foreach (AW.RibbonPanel panel in tab.Panels)
                    {
                        if (panel.Source.Title == panelName)
                        {
                            return panel.Source.Items.First(b => b.AutomationName.Equals(itemName));
                        }
                    }
                }
            }
            return null;
        }
    }
}
