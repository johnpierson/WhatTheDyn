using System;
using Installer;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;
using Assembly = System.Reflection.Assembly;

const string outputName = "WhatTheDyn";
const string projectName = "WhatTheDyn";
const string manufacturer = "Design Tech Unraveled";

// Per-user and per-machine are separate product lines to Windows Installer: it never upgrades
// across installation contexts. Sharing one UpgradeCode (and, since WixSharp derives ProductId
// from GUID + Version, one ProductCode) made the two MSIs the same product, so switching scopes
// either hit maintenance mode or left both installed side by side.
var perUserUpgradeCode = new Guid("33489382-045A-45DA-8933-B46AE3D3A166");
var perMachineUpgradeCode = new Guid("6F2B4E51-9C3A-4D7E-8B1F-2A5C9D4E7B03");

var project = new Project
{
    OutDir = "output",
    Name = projectName,
    Platform = Platform.x64,
    UI = WUI.WixUI_FeatureTree,
    MajorUpgrade = MajorUpgrade.Default,
    GUID = perUserUpgradeCode,
    BannerImage = @"install\Resources\Icons\BannerImage.png",
    BackgroundImage = @"install\Resources\Icons\BackgroundImage.png",
    Version = Assembly.GetExecutingAssembly().GetName().Version.ClearRevision(),
    ControlPanelInfo =
    {
        Manufacturer = manufacturer,
        ProductIcon = @"install\Resources\Icons\ShellIcon.ico"
    }
};

var wixEntities = Generator.GenerateWixEntities(args);
project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.CustomizeDlg);

BuildSingleUserMsi();
BuildMultiUserUserMsi();

void BuildSingleUserMsi()
{
    project.InstallScope = InstallScope.perUser;
    project.OutFileName = $"{outputName}-{project.Version}-SingleUser";
    project.UpgradeCode = perUserUpgradeCode;
    project.ProductId = Project.CalculateProductId(perUserUpgradeCode, project.Version);
    project.Dirs =
    [
        new InstallDir(@"%AppDataFolder%\Autodesk\Revit\Addins\", wixEntities)
    ];
    project.BuildMsi();
}

void BuildMultiUserUserMsi()
{
    project.InstallScope = InstallScope.perMachine;
    project.OutFileName = $"{outputName}-{project.Version}-MultiUser";
    project.UpgradeCode = perMachineUpgradeCode;
    project.ProductId = Project.CalculateProductId(perMachineUpgradeCode, project.Version);
    project.Dirs =
    [
        new InstallDir(@"%CommonAppDataFolder%\Autodesk\Revit\Addins\", wixEntities)
    ];
    project.BuildMsi();
}