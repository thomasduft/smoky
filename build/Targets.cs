using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using static Bullseye.Targets;
using static SimpleExec.Command;

namespace targets;

internal static class Program
{
  // Add name of the solution here:
  private const string solution = "smoky.sln";

  // Fill list of packaple projects here:
  private static IList<string> packableProjects = new List<string>{
    "smoky"
  };

  // Adjust your nuget source here:
  private const string nugetSource = "https://api.nuget.org/v3/index.json";

  static async Task Main(string[] args)
  {
    var version = string.Empty;
    var key = string.Empty;

    if (args[0].Contains("--"))
    {
      var firstArg = args[0].Split("--")[0].Trim();
      var customArgs = args[0].Split("--")[1].Trim().Split("&");

      if (customArgs.Any(x => x.Contains("version")))
      {
        var versionArgs = customArgs.First(x => x.Contains("version"));
        version = versionArgs.Split('=')[1].Trim();
      }

      if (customArgs.Any(x => x.Contains("key")))
      {
        var keyArgs = customArgs.First(x => x.Contains("key"));
        key = keyArgs.Split('=')[1].Trim();
      }

      args[0] = firstArg;
    }

    if (!string.IsNullOrWhiteSpace(version))
    {
      Console.WriteLine($"Using version: '{version}'" + Environment.NewLine);
    }

    #region Tooling targets
    const string RestoreTools = "restore-tools";
    const string AddChangelog = "add-changelog";
    const string LatestGitTag = "latest-git-tag";

    Target(RestoreTools, () =>
    {
      Run("dotnet", "tool restore");
    });

    Target(AddChangelog, () =>
    {
      Run("dotnet", "tool run releasy add-changelog");
    });

    Target(LatestGitTag, () =>
    {
      // goal is to echo the result of: git tag | sort -r --version-sort | head -n1
      System.Console.WriteLine("git tag | sort -r --version-sort | head -n1");
      // Run("git", "tag");
    });
    #endregion

    #region Build targets
    const string Clean = "clean";
    const string Build = "build";
    const string Test = "test";
    const string Release = "release";

    Target(Clean, () =>
    {
      Run("dotnet", $"clean {solution} -c Release -v m --nologo");
    });

    Target(Build, DependsOn(Clean), () =>
    {
      Run("dotnet", $"build {solution} -c Release --nologo");
    });

    Target(Test, DependsOn(Build), () =>
    {
      Run("dotnet", $"test {solution} -c Release --no-build --nologo");
    });

    Target(Release, DependsOn(RestoreTools, Test), () =>
    {
      if (string.IsNullOrWhiteSpace(version))
      {
        throw new Bullseye.TargetFailedException("Version for updating changelog is missing!");
      }

      // updating the changelog
      Run("dotnet", $"tool run releasy update-changelog -v {version} -p https://github.com/thomasduft/microwf/issues/");

      // committing the changelog changes
      Run("git", $"commit -am \"Committing changelog changes for v'{version}'\"");

      // applying the tag
      Run("git", $"tag -a v{version} -m \"version '{version}'\"");

      // pushing
      Run("git", $"push origin v{version}");
    });
    #endregion

    #region Deployment targets
    const string artifactsDirectory = "./artifacts";
    const string CleanArtifacts = "clean-artifacts";
    const string Pack = "pack";
    const string Deploy = "deploy";

    Target(CleanArtifacts, () =>
    {
      if (Directory.Exists(artifactsDirectory))
      {
        Directory.Delete(artifactsDirectory, true);
      }
    });

    Target(Pack, DependsOn(Build, CleanArtifacts), () =>
    {
      if (string.IsNullOrWhiteSpace(version))
      {
        throw new Bullseye.TargetFailedException("Version for packaging is missing!");
      }

      // pack packages
      var directory = Directory.CreateDirectory(artifactsDirectory).FullName;
      var projects = GetFiles("src", $"*.csproj");
      foreach (var project in projects)
      {
        if (project.Contains(".Tests"))
          continue;

        if (packableProjects.Any(m => project.Contains(m)))
        {
          Run("dotnet", $"pack {project} -c Release -p:PackageVersion={version} -p:Version={version} -o {directory} --no-build --nologo");
        }
      }
    });

    Target(Deploy, DependsOn(RestoreTools, Test, Pack), () =>
    {
      if (string.IsNullOrWhiteSpace(key))
      {
        throw new Bullseye.TargetFailedException("Key for deploying to NuGet is missing!");
      }

      // push packages
      var directory = Directory.CreateDirectory(artifactsDirectory).FullName;
      var packages = GetFiles(directory, $"*.nupkg");
      foreach (var package in packages)
      {
        Run("dotnet", $"nuget push {package} -s {nugetSource} -k {key}");
      }
    });
    #endregion

    await RunTargetsAndExitAsync(args);
  }

  #region Helpers
  private static IEnumerable<string> GetFiles(
      string directoryToScan,
      string filter
    )
  {
    List<string> files = new List<string>();

    files.AddRange(Directory.GetFiles(
      directoryToScan,
      filter,
      SearchOption.AllDirectories
    ));

    return files;
  }

  static void CopyDirectory(string sourceDir, string destinationDir, bool recursive = false)
  {
    var dir = new DirectoryInfo(sourceDir);
    if (!dir.Exists)
      throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

    foreach (FileInfo file in dir.GetFiles())
    {
      Directory.CreateDirectory(destinationDir);
      string targetFilePath = Path.Combine(destinationDir, file.Name);
      file.CopyTo(targetFilePath, true);
    }

    if (recursive)
    {
      foreach (DirectoryInfo subDir in dir.GetDirectories())
      {
        string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
        CopyDirectory(subDir.FullName, newDestinationDir, true);
      }
    }
  }
  #endregion
}