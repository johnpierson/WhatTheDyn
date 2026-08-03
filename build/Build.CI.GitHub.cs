using System.Text;
using Nuke.Common.Git;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.GitHub;
using Octokit;

sealed partial class Build
{
    Target PublishGitHub => _ => _
        .DependsOn(CreateInstaller, CreateBundle)
        .Requires(() => GitHubToken)
        .Requires(() => GitRepository)
        .OnlyWhenStatic(() => IsServerBuild)
        .Executes(async () =>
        {
            GitHubTasks.GitHubClient = new GitHubClient(new ProductHeaderValue(Solution.Name))
            {
                Credentials = new Credentials(GitHubToken)
            };

            var gitHubName = GitRepository.GetGitHubName();
            var gitHubOwner = GitRepository.GetGitHubOwner();

            ValidateTag();
            await ValidateReleaseDoesNotExistAsync(gitHubOwner, gitHubName);

            var artifacts = Directory.GetFiles(ArtifactsDirectory, "*");
            var changelog = CreateGithubChangelog();
            Assert.NotEmpty(artifacts, "No artifacts were found to create the Release");

            var newRelease = new NewRelease(ReleaseTag)
            {
                Name = Version,
                Body = changelog,
                TargetCommitish = GitRepository.Commit
            };

            var release = await GitHubTasks.GitHubClient.Repository.Release.Create(gitHubOwner, gitHubName, newRelease);
            await UploadArtifactsAsync(release, artifacts);
        });

    /// <summary>The tag this release publishes under, e.g. "v2.0.0" for Version "2.0.0".</summary>
    static string ReleaseTag => $"v{Version}";

    /// <summary>
    ///     Releases are triggered by pushing a tag, so the tag must agree with the compiled version.
    ///     The previous guard shelled out to "git describe --tags --always" and compared the result to
    ///     the current commit; on a CI clone without tags that comparison always matched and the guard
    ///     returned before checking anything, letting a duplicate release attempt fail later against the API.
    /// </summary>
    void ValidateTag()
    {
        var pushedTag = Environment.GetEnvironmentVariable("GITHUB_REF_NAME");
        Assert.True(!string.IsNullOrWhiteSpace(pushedTag), "No tag was found: releases must be triggered by pushing a tag");
        Assert.True(pushedTag == ReleaseTag,
            $"The pushed tag '{pushedTag}' does not match the version being built ('{ReleaseTag}'). " +
            $"Update Version in Build.Configuration.cs or push the matching tag.");

        Log.Information("Releasing {Tag}", ReleaseTag);
    }

    async Task ValidateReleaseDoesNotExistAsync(string gitHubOwner, string gitHubName)
    {
        try
        {
            await GitHubTasks.GitHubClient.Repository.Release.Get(gitHubOwner, gitHubName, ReleaseTag);
        }
        catch (NotFoundException)
        {
            return;
        }

        Assert.Fail($"A release already exists for tag {ReleaseTag}. Bump Version in Build.Configuration.cs.");
    }

    static async Task UploadArtifactsAsync(Release release, IEnumerable<string> artifacts)
    {
        foreach (var file in artifacts)
        {
            await using var stream = File.OpenRead(file);
            var releaseAssetUpload = new ReleaseAssetUpload
            {
                ContentType = "application/x-binary",
                FileName = Path.GetFileName(file),
                RawData = stream
            };

            await GitHubTasks.GitHubClient.Repository.Release.UploadAsset(release, releaseAssetUpload);
            Log.Information("Artifact: {Path}", file);
        }
    }

    /// <summary>
    ///     Release notes are the changelog entry for this version. A missing or empty entry used to
    ///     log a warning and publish a release with an empty body; it now fails the build instead.
    /// </summary>
    string CreateGithubChangelog()
    {
        Assert.FileExists(ChangeLogPath);
        Log.Information("Changelog: {Path}", ChangeLogPath);

        var changelog = BuildChangelog();
        Assert.True(changelog.Length > 0,
            $"No '# {Version}' entry exists in {ChangeLogPath}. Add one before releasing.");

        WriteCompareUrl(changelog);
        return changelog.ToString();
    }

    void WriteCompareUrl(StringBuilder changelog)
    {
        var previousTag = GetPreviousReleaseTag();
        if (previousTag is null) return;

        changelog.AppendLine();
        changelog.AppendLine();
        changelog.Append("Full changelog: ");
        changelog.Append(GitRepository.GetGitHubCompareTagsUrl(ReleaseTag, previousTag));
    }

    /// <summary>Most recent release tag before the one being published, or null when this is the first.</summary>
    static string GetPreviousReleaseTag()
    {
        var tags = GitTasks.Git($"tag --list --sort=-v:refname", logInvocation: false, logOutput: false)
            .Select(output => output.Text.Trim())
            .Where(tag => tag.Length > 0 && tag != ReleaseTag)
            .ToList();

        return tags.FirstOrDefault();
    }

    StringBuilder BuildChangelog()
    {
        //headings are matched exactly: a Contains check would let "# 1.1.10" satisfy version "1.1.1"
        var heading = $"# {Version}";
        const string separator = "# ";

        var hasEntry = false;
        var changelog = new StringBuilder();
        foreach (var line in File.ReadLines(ChangeLogPath))
        {
            if (hasEntry)
            {
                if (line.StartsWith(separator)) break;

                changelog.AppendLine(line);
                continue;
            }

            if (line.Trim() == heading)
            {
                hasEntry = true;
            }
        }

        TrimEmptyLines(changelog);
        return changelog;
    }

    static void TrimEmptyLines(StringBuilder builder)
    {
        //length is rechecked each iteration: an entry of nothing but blank lines would otherwise
        //empty the builder and then index into it
        while (builder.Length > 0 && (builder[^1] == '\r' || builder[^1] == '\n'))
        {
            builder.Remove(builder.Length - 1, 1);
        }

        while (builder.Length > 0 && (builder[0] == '\r' || builder[0] == '\n'))
        {
            builder.Remove(0, 1);
        }
    }
}