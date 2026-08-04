<h1 align="center">
  <br>
  <img src="/_documentation/whatTheDyn-Logo.png" alt="whatTheDyn" width="400">
  <br>
</h1>

<h3 align="center">A plugin for Revit to tell you which version of Dynamo is frickin loaded.</h3>

[![Maintenance](https://img.shields.io/badge/Maintained%3F-yes-green.svg)](https://github.com/johnpierson/WhatTheDyn/graphs/commit-activity)
[![GitHub license](https://img.shields.io/github/license/johnpierson/WhatTheDyn)](https://github.com/johnpierson/WhatTheDyn/blob/main/LICENSE)

_If you feel so inclined, here is a method to donate to this project_

<a href="https://www.buymeacoffee.com/j0hnp" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png" alt="Buy Me A Coffee" style="height: 41px !important;width: 174px !important;box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;-webkit-box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;" ></a>

## What it does

On Revit startup, What the Dyn?! detects the loaded Dynamo version and:

- renames the ribbon's **Dynamo** button to include that version, and
- shows a notification bubble naming the version, which you can click to open your Dynamo packages folder.

There is also a **Show Dynamo version.** command under Add-Ins if you want the bubble again later.

## Supported versions

Revit **2025, 2026 and 2027**.

Revit 2020–2024 were supported through version 1.1.1 and dropped in 2.0.0. Note that the 1.1.1 installers shipped a manifest that pointed at the wrong assembly name, so that release never actually loaded; if you have it installed, uninstall it.

## Installation

Download an installer from the [latest release](https://github.com/johnpierson/WhatTheDyn/releases/latest):

- **SingleUser** installs for the current user only (`%AppData%\Autodesk\Revit\Addins`).
- **MultiUser** installs for everyone on the machine (`%ProgramData%\Autodesk\Revit\Addins`) and needs admin rights.

Pick one and stay with it. Windows Installer does not upgrade across installation scopes, so if you want to switch, uninstall the other one first.

`WhatTheDyn.zip` on the same release page is an Autodesk application bundle, if you prefer deploying that way.

## Building

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download) (`global.json` requires it; Revit 2027 targets .NET 10, and the 2025/2026 configurations target .NET 8, which the .NET 10 SDK also builds).

Open `WhatTheDyn.sln` in Visual Studio or JetBrains Rider and pick a configuration — the `R25`/`R26`/`R27` suffix selects the Revit version, so `Debug R27` builds against Revit 2027. Debug configurations deploy the add-in into your local Revit Addins folder so you can F5 straight into Revit; Release configurations deliberately do not touch your Revit installation.

To build everything the way CI does, use [NUKE](https://github.com/nuke-build/nuke):

```powershell
dotnet tool install Nuke.GlobalTool --global   # once per machine
```

```powershell
nuke                    # compile every Release configuration
nuke VerifyAddinLayout  # compile, then check each published manifest matches the assemblies beside it
nuke CreateInstaller    # build the two MSIs
nuke CreateBundle       # build the .bundle zip
```

Artifacts land in `output/`.

## Releasing

Releases are published by CI when a version tag is pushed. Pushing to `main` only builds and verifies.

1. Bump `Version` in [build/Build.Configuration.cs](build/Build.Configuration.cs).
2. Add a matching `# <version>` section to [Changelog.md](Changelog.md) — this becomes the release notes, and the release fails if it is missing.
3. Commit and push to `main`.
4. Tag it `v<version>` (for example `v2.0.0`) and push the tag.

The release job builds all configurations, verifies the add-in layout, creates the installers and bundle, and attaches them to a new GitHub release.

## Repository structure

| Folder           | Description                                                     |
|------------------|-----------------------------------------------------------------|
| `source/WhatTheDyn` | The add-in itself                                            |
| `build`          | NUKE build system                                               |
| `install`        | WixSharp installer, invoked by the build                        |
| `output`         | Generated installers and bundles (not committed)                |

## License

This code is licensed under [BSD 3-Clause](LICENSE).

## Contributors

This tool is primarily managed by the author of http://designtechunraveled.com and by [People Like You™](https://github.com/johnpierson/WhatTheDyn/graphs/contributors).

## Help improve What the Dyn?!

If you're interested in contributing, just submit a [pull request](https://github.com/johnpierson/WhatTheDyn/pulls) or a [feature request](https://github.com/johnpierson/WhatTheDyn/issues).
