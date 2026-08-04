using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WixSharp;

namespace Installer;

public static class Generator
{
    public static WixEntity[] GenerateWixEntities(IEnumerable<string> args)
    {
        //four digits specifically: \d+ would happily read "24" out of a folder named "R24 addin"
        //and produce an Addins\24 directory that Revit never scans
        var versionRegex = new Regex(@"\d{4}");
        var versionStorages = new Dictionary<string, List<WixEntity>>();

        foreach (var directory in args)
        {
            var directoryInfo = new DirectoryInfo(directory);
            var versionMatch = versionRegex.Match(directoryInfo.Name);
            if (!versionMatch.Success)
                throw new InvalidOperationException($"No Revit version could be read from the folder name: {directoryInfo.Name}");

            var fileVersion = versionMatch.Value;
            var feature = new Feature
            {
                Name = $"Revit {fileVersion}",
                Description = $"Install add-in for Revit {fileVersion}",
                ConfigurableDir = $"INSTALL{fileVersion}"
            };

            var files = new Files(feature, $@"{directory}\*.*");
            if (versionStorages.TryGetValue(fileVersion, out var storage))
                storage.Add(files);
            else
                versionStorages.Add(fileVersion, [files]);

            var assemblies = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
            Console.WriteLine($"Installer files for version '{fileVersion}':");
            foreach (var assembly in assemblies) Console.WriteLine($"'{assembly}'");
        }

        return versionStorages
            .Select(storage => new Dir(new Id($"INSTALL{storage.Key}"), storage.Key, storage.Value.ToArray()))
            .Cast<WixEntity>()
            .ToArray();
    }
}