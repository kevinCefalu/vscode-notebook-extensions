var target = Argument("target", "Test");
var configuration = Argument("configuration", "Debug");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
  .WithCriteria(c => HasArgument("rebuild"))
  .Does(() =>
{
  var cleanSettings = new CleanDirectorySettings()
  {
    Force = true,
  };

  Information($"Cleaning './source/**/bin/{configuration}' directories");
  CleanDirectories($"./source/**/bin/{configuration}", cleanSettings);

  Information($"Cleaning './source/**/bin/nupkg' directories");
  CleanDirectories($"./source/**/bin/nupkg", cleanSettings);
});

Task("Build")
  .IsDependentOn("Clean")
  .Does(() =>
{
  DotNetBuild("./source/vscode-notebook-extensions.sln",
    new DotNetBuildSettings
    {
      Configuration = configuration,
      NoLogo = true,
    });
});

Task("Test")
  .IsDependentOn("Build")
  .Does(() =>
{
  DotNetTest("./source/vscode-notebook-extensions.sln",
    new DotNetTestSettings
    {
      Configuration = configuration,
      NoBuild = true,
      Loggers = new[] {
        "console;verbosity=detailed",
      }
    }
  );
});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
