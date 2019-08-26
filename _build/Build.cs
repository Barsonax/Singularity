using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.DotCover;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Tools.SonarScanner;
using Nuke.Common.Utilities;
using Nuke.DocFX;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.DotCover.DotCoverTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;
using static Nuke.Common.Tools.SonarScanner.SonarScannerTasks;
using static Nuke.DocFX.DocFXTasks;
using System;
using System.Threading.Tasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter] readonly string ApiKey;
    [Parameter] readonly bool CoberturaReport;

    [Solution("src/Singularity.sln")] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath BaseBuildOutput => RootDirectory / "BuildOutput";
    AbsolutePath BuildOutput => BaseBuildOutput / Configuration;

    [Parameter]
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "artifacts";

    [Parameter]
    readonly string SonarCloudLogin;

    AbsolutePath CoverageDirectory => RootDirectory / "coverage";

    AbsolutePath SrcPath => RootDirectory / "src";
    AbsolutePath DocFXPath => SrcPath / "Docs";
    AbsolutePath DocFxOutput => DocFXPath / "_site";
    AbsolutePath DocFXJson => DocFXPath / "docfx.json";
    AbsolutePath DocsRepository => DocFXPath / "repository";
    AbsolutePath DocsRepositoryFolder => DocsRepository / "docs";

    [PathExecutable]
    private static Tool Git;

    [PathExecutable]
    private static Tool Dotnet;

    [PackageExecutable("dotnet-sonarscanner", @"tools\netcoreapp2.1\any\SonarScanner.MSBuild.dll")]
    private static Tool SonarScanner;

    private Dictionary<string, object> NoWarns = new Dictionary<string, object> { { "NoWarn", "NU1701" }, };

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            EnsureCleanDirectory(BuildOutput);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution)
                .SetProperties(NoWarns));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetProperties(NoWarns)
                .SetProperty("BaseOutputPath", BaseBuildOutput + "/")
                .SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
                .SetFileVersion(GitVersion.GetNormalizedFileVersion())
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
            .SetOutput(BuildOutput / "netcoreapp2.0")
            .SetWorkingDirectory(SrcPath)
            .SetFramework("netcoreapp2.0")
            .SetConfiguration(Configuration)
            .SetProperties(NoWarns)
            .EnableNoBuild());
        });

    Target Coverage => _ => _
        .DependsOn(Compile)
        .Executes(() =>
    {
        string testdlls = GlobFiles(BuildOutput / "netcoreapp2.0", "*.Test.dll").Join(" ");
        string targetArgs = $"vstest {testdlls} /logger:trx;LogFileName=testresults.trx";
        string dotnetPath = ToolPathResolver.GetPathExecutable("dotnet");
        AbsolutePath coverageSnapshot = CoverageDirectory / "coverage.dcvr";
        AbsolutePath coverageXml = CoverageDirectory / "coverage.xml";
        AbsolutePath coverageReport = CoverageDirectory / "CoverageReport";
        AbsolutePath coberturaReport = CoverageDirectory / "cobertura_coverage.xml";

        DotCoverCover(c => c
             .SetTargetExecutable(dotnetPath)
             .SetTargetWorkingDirectory(RootDirectory)
             .SetTargetArguments(targetArgs)
             .SetFilters("+:Singularity*;-:Class=Singularity.FastExpressionCompiler*;-:*Test*")
             .SetOutputFile(coverageSnapshot));

        DotCoverReport(c => c
            .SetSource(coverageSnapshot)
            .SetOutputFile(coverageXml)
            .SetReportType(DotCoverReportType.DetailedXml));

        if (CoberturaReport)
        {
            ReportGenerator(c => c
                .SetReports(coverageXml)
                .SetTargetDirectory(CoverageDirectory)
                .SetReportTypes(Nuke.Common.Tools.ReportGenerator.ReportTypes.Cobertura));
        }
        else
        {
            ReportGenerator(c => c
                .SetReports(coverageXml)
                .SetTargetDirectory(coverageReport));
        }
    });

    Target RunSonarScanner => _ => _
        .Requires(() => !string.IsNullOrEmpty(SonarCloudLogin))
        .Before(Compile)
        .Triggers(SonarEnd)
        .Executes(() =>
        {
            var server = "https://sonarcloud.io";
            var projectKey = "Barsonax_Singularity";
            var organisation = "barsonax-github";
            var exclusions = "src/Singularity/FastExpressionCompiler/*,src/Tests/Singularity.TestClasses/**/*";
            var branch = GitVersion.BranchName;
            var version = GitVersion.GetNormalizedAssemblyVersion();
            SonarScanner($"begin /k:{projectKey} /o:{organisation} /v:{version} /d:sonar.login={SonarCloudLogin} /d:sonar.host.url={server} /d:sonar.exclusions={exclusions} /d:sonar.branch.name={branch}");
        });

    Target SonarEnd => _ => _
    .After(RunSonarScanner, Test, Coverage, Compile)
        .Executes(() =>
        {
            SonarScannerEnd(s => s
            .SetLogin(SonarCloudLogin));
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetPack(s => s
            .SetProject(Solution)
            .SetOutputDirectory(ArtifactsDirectory)
            .SetConfiguration(Configuration)
            .SetVersion(GitVersion.NuGetVersion)
            .SetProperty("BaseOutputPath", BaseBuildOutput + "/")
            .EnableIncludeSymbols()
            .EnableNoBuild());
        });


    Target Push => _ => _
        .Requires(() => !string.IsNullOrEmpty(ApiKey))
        .After(Test)
        .After(Coverage)
        .OnlyWhenDynamic(() => GitRepository.Branch == "master")
        .After(Pack)
        .Executes(() =>
        {
            Parallel.ForEach(ArtifactsDirectory.GlobFiles("*.nupkg", "*.snupkg").NotEmpty(), (nupkgFile) =>
            {
                var errorIsWarning = false;
                try
                {
                    Dotnet.Invoke($"nuget push {nupkgFile} --source https://www.nuget.org --api-key {ApiKey}", customLogger: (type, output) =>
                    {
                        if (output.StartsWith("error: Response status code does not indicate success: 409"))
                        {
                            errorIsWarning = true;
                            Nuke.Common.Logger.Log(LogLevel.Warning, $"Ignoring {output}");
                        }
                        else
                        {
                            ProcessTasks.DefaultLogger(type, output);
                        }
                    });
                }
                catch (System.Exception e)
                {
                    if (errorIsWarning)
                    {
                        Logger.Warn(e.Message);
                    }
                    else
                    {
                        throw e;
                    }
                }
            });
        });

    Target BuildDocs => _ => _
    .Executes(() =>
    {
        Environment.SetEnvironmentVariable("MSBUILD_EXE_PATH", @"C:\Program Files\dotnet\sdk\1.1.13\MSBuild.dll");
        DocFXMetadata(s => s.SetProjects(DocFXJson));
        DocFXBuild(s => s.SetConfigFile(DocFXJson));
    });

    Target PushDocs => _ => _
        .DependsOn(BuildDocs)
        .OnlyWhenDynamic(() => GitRepository.Branch == "master")
        .Executes(() =>
    {
        EnsureCleanDirectory(DocsRepository);
        Git($"clone https://github.com/Barsonax/Singularity.Docs {DocsRepository}");
        EnsureCleanDirectory(DocsRepositoryFolder);
        CopyDirectoryRecursively(DocFxOutput, DocsRepositoryFolder, DirectoryExistsPolicy.Merge);
        System.IO.Directory.SetCurrentDirectory(DocsRepository);
        try
        {
            Git("add docs");
            IgnoreError(() => Git("commit -m nuke_build_generated_commit"));
            Git("push");
        }
        finally
        {
            System.IO.Directory.SetCurrentDirectory(RootDirectory);
        }
    });

    private void IgnoreError(Action action)
    {
        try
        {
            action.Invoke();
        }
        catch (System.Exception e)
        {
            Nuke.Common.Logger.Log(LogLevel.Warning, $"Ignored error {e.Message}");
        }
    }
}