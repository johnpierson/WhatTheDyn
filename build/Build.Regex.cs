using System.Text.RegularExpressions;

sealed partial class Build
{
    static readonly Regex YearRegex = YearRegexGenerator();
    readonly Regex ArgumentsRegex = ArgumentsRegexGenerator();

    [GeneratedRegex(@"\d{4}")]
    private static partial Regex YearRegexGenerator();

    /// <summary>
    ///     Extracts the Revit year from a publish folder, matching against the folder NAME only.
    ///     Matching the full path meant the first four consecutive digits anywhere in it won, so a
    ///     checkout under something like D:\Projects2024 would resolve every version to 2024 and
    ///     collapse all of them into a single bundle folder without failing.
    /// </summary>
    static string GetRevitVersion(string publishDirectory)
    {
        var directoryName = new DirectoryInfo(publishDirectory).Name;
        var match = YearRegex.Match(directoryName);

        Assert.True(match.Success, $"No Revit version could be read from the folder name: {directoryName}");
        Assert.True(int.TryParse(match.Value, out var year) && year is >= 2000 and <= 2099,
            $"'{match.Value}' in folder '{directoryName}' is not a plausible Revit version");

        return match.Value;
    }

    [GeneratedRegex("'(.+?)'", RegexOptions.Compiled)]
    private static partial Regex ArgumentsRegexGenerator();
}