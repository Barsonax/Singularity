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

    [Parameter] readonly AbsolutePath BaseBuildOutput = RootDirectory / "BuildOutput";
    AbsolutePath BuildOutput => BaseBuildOutput / Configuration;

    [Parameter]
    readonly string SonarCloudLogin;

    AbsolutePath CoverageDirectory => RootDirectory / "coverage";
    AbsolutePath CoverageXml => CoverageDirectory / "coverage.xml";
    AbsolutePath CoverageHtml => CoverageDirectory / "coverage.html";
    AbsolutePath CoberturaReportPath => CoverageDirectory / "Cobertura.xml";

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
                .SetProperty("GeneratePackageOnBuild", true)
                .SetVersion(GitVersion.NuGetVersion)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
            .SetOutput(BuildOutput / "netcoreapp3.0")
            .SetWorkingDirectory(SrcPath)
            .SetFramework("netcoreapp3.0")
            .SetConfiguration(Configuration)
            .SetProperties(NoWarns)
            .EnableNoBuild());
        });

    Target Coverage => _ => _
        .DependsOn(Compile)
        .Executes(() =>
    {
        string testdlls = GlobFiles(BuildOutput / "netcoreapp3.0", "*.Test.dll").Join(" ");
        string targetArgs = $"vstest {testdlls} /logger:trx;LogFileName=testresults.trx";
        string dotnetPath = ToolPathResolver.GetPathExecutable("dotnet");
        AbsolutePath coverageSnapshot = CoverageDirectory / "coverage.dcvr";
        AbsolutePath coverageReport = CoverageDirectory / "CoverageReport";

        DotCoverCover(c => c
             .SetTargetExecutable(dotnetPath)
             .SetTargetWorkingDirectory(RootDirectory)
             .SetTargetArguments(targetArgs)
             .SetFilters("+:Singularity*;-:Class=Singularity.FastExpressionCompiler*;-:*Test*;-:*Example*;-:*Benchmark*")
             .SetOutputFile(coverageSnapshot));

        DotCoverReport(c => c
            .SetSource(coverageSnapshot)
            .SetOutputFile(CoverageXml)
            .SetReportType(DotCoverReportType.DetailedXml));

        DotCoverReport(c => c
            .SetSource(coverageSnapshot)
            .SetOutputFile(CoverageHtml)
            .SetReportType(DotCoverReportType.Html));

        if (CoberturaReport)
        {
            ReportGenerator(c => c
                .SetReports(CoverageXml)
                .SetTargetDirectory(CoverageDirectory)
                .SetReportTypes(Nuke.Common.Tools.ReportGenerator.ReportTypes.Cobertura));
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
            var exclusions = "src/Singularity/FastExpressionCompiler/*,src/Tests/Singularity.TestClasses/**/*,src/Examples/**/*";
            var branch = GitVersion.BranchName;
            var version = GitVersion.AssemblySemVer;
            SonarScanner($"begin /k:{projectKey} /o:{organisation} /v:{version} /d:sonar.login={SonarCloudLogin} /d:sonar.host.url={server} /d:sonar.exclusions={exclusions} /d:sonar.cs.dotcover.reportsPaths={CoverageHtml} /d:sonar.branch.name={branch}");
        });

    Target SonarEnd => _ => _
    .After(RunSonarScanner, Test, Coverage, Compile)
        .Executes(() =>
        {
            SonarScannerEnd(s => s
            .SetLogin(SonarCloudLogin));
        });


    Target Push => _ => _
        .Requires(() => !string.IsNullOrEmpty(ApiKey))
        .After(Test, Coverage, Compile)
        .Executes(() =>
        {
            var source = "https://api.nuget.org/v3/index.json";
            Parallel.ForEach(BuildOutput.GlobFiles("*.nupkg").NotEmpty(), (nupkgFile) =>
            {
                var errorIsWarning = false;
                try
                {
                    Dotnet.Invoke($"nuget push {nupkgFile} --source {source} --api-key {ApiKey}", customLogger: (type, output) =>
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
        DocFXMetadata(s => s.SetProjects(DocFXJson).SetMSBuildProperty("SolutionDir", $"{Solution.Directory}/"));
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