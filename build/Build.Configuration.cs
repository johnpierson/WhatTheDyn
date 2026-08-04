using Nuke.Common.ProjectModel;

sealed partial class Build
{
    const string Version = "2.0.0";
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "output";
    readonly AbsolutePath ChangeLogPath = RootDirectory / "Changelog.md";

    protected override void OnBuildInitialized()
    {
        Configurations =
        [
            "Release*",
            "Installer*"
        ];

        Bundles =
        [
            Solution.WhatTheDyn
        ];

        InstallersMap = new()
        {
            {Solution.Installer, Solution.WhatTheDyn}
        };
    }

    /// <summary>
    ///     Locates the per-Revit-version publish folders produced by Nice3point.Revit.Build.Tasks,
    ///     which names them "Revit &lt;year&gt; &lt;Configuration&gt; addin" - the space-delimited " Release "
    ///     is what distinguishes them from the plain "Release R25" output folders.
    ///     Renaming the solution configurations or upgrading that package can break this contract.
    /// </summary>
    static string[] GetPublishDirectories(Project project)
    {
        var directories = Directory.GetDirectories(project.Directory, "* Release *", SearchOption.AllDirectories);
        Assert.NotEmpty(directories, $"No published add-in folders were found for the project: {project.Name}");
        return directories;
    }
}