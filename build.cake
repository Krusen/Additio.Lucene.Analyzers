var target = Argument("target", "Default");

var version = "1.0.0";
var buildOutput = "./.build";

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .IsDependentOn("Pack")
    ;

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildOutput);
});

Task("Build")
    .Does(() =>
{
    NuGetRestore("./src/Additio.Lucene.Analyzers.sln");

    MSBuild("./src/Additio.Lucene.Analyzers.sln", settings =>
        settings.SetVerbosity(Verbosity.Minimal)
                .SetConfiguration("Release")
                .WithTarget("Rebuild")
                .UseToolVersion(MSBuildToolVersion.VS2015)
                .SetMaxCpuCount(0)
                .ArgumentCustomization = args => args.Append("/nologo")
    );
});


Task("Pack")
    .IsDependentOn("Build")
    .Does(() =>
{
    var settings = new NuGetPackSettings
    {
        Version = version,
        Symbols = false,
        OutputDirectory = buildOutput
    };

    NuGetPack("./Additio.Lucene.Analyzers.Danish.nuspec", settings);
    NuGetPack("./Additio.Lucene.Analyzers.Danish.Sitecore.nuspec", settings);
});

RunTarget(target);