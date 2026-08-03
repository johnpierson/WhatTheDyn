using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;

sealed partial class Build
{
    /// <summary>
    ///     Asserts that every published .addin manifest actually describes the files next to it.
    ///     Revit resolves &lt;Assembly&gt; relative to the manifest and looks up &lt;FullClassName&gt; inside it;
    ///     when either is wrong the add-in silently fails to load at startup, which no compile step catches.
    /// </summary>
    Target VerifyAddinLayout => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var checkedManifests = 0;

            foreach (var project in Bundles)
            foreach (var publishDirectory in GetPublishDirectories(project))
            foreach (var manifest in Directory.GetFiles(publishDirectory, "*.addin"))
            {
                Log.Information("Manifest: {Manifest}", manifest);
                VerifyManifest(manifest);
                checkedManifests++;
            }

            Assert.True(checkedManifests > 0, "No .addin manifests were found to verify");
            Log.Information("Verified {Count} add-in manifests", checkedManifests);
        });

    static void VerifyManifest(string manifestPath)
    {
        var manifestDirectory = Path.GetDirectoryName(manifestPath)!;
        var document = XDocument.Load(manifestPath);
        var entries = document.Root?.Elements().ToList();
        Assert.True(entries is {Count: > 0}, $"Manifest declares no add-in entries: {manifestPath}");

        foreach (var entry in entries!)
        {
            var assemblyPath = entry.Element("Assembly")?.Value;
            var fullClassName = entry.Element("FullClassName")?.Value;
            Assert.True(!string.IsNullOrWhiteSpace(assemblyPath), $"Add-in entry has no <Assembly>: {manifestPath}");
            Assert.True(!string.IsNullOrWhiteSpace(fullClassName), $"Add-in entry has no <FullClassName>: {manifestPath}");

            var resolvedAssembly = Path.Combine(manifestDirectory, assemblyPath!);
            Assert.True(File.Exists(resolvedAssembly),
                $"Manifest points at an assembly that was not built: '{assemblyPath}' (resolved to '{resolvedAssembly}'). " +
                "Revit resolves this path relative to the .addin file and will fail to load the add-in.");

            Assert.True(ContainsType(resolvedAssembly, fullClassName!),
                $"Assembly '{assemblyPath}' does not contain the type '{fullClassName}' declared in {manifestPath}");

            Log.Information("  {ClassName} -> {Assembly}", fullClassName, assemblyPath);
        }
    }

    static bool ContainsType(string assemblyPath, string fullClassName)
    {
        using var stream = File.OpenRead(assemblyPath);
        using var peReader = new PEReader(stream);

        var metadata = peReader.GetMetadataReader();
        foreach (var handle in metadata.TypeDefinitions)
        {
            var type = metadata.GetTypeDefinition(handle);
            var @namespace = metadata.GetString(type.Namespace);
            var name = metadata.GetString(type.Name);
            var candidate = string.IsNullOrEmpty(@namespace) ? name : $"{@namespace}.{name}";

            if (candidate == fullClassName) return true;
        }

        return false;
    }
}
