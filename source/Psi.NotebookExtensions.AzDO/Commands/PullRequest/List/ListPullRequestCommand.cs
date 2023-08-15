using System.Collections.Concurrent;
using System.CommandLine;
using System.Text.Json;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Psi.NotebookExtensions.AzDO.Services;

namespace Psi.NotebookExtensions.AzDO.Commands;

public static class ListPullRequestCommand
{
  private const string COMMAND_NAME = "list";
  private const string COMMAND_DESCRIPTION = "Displays information about a list of Azure DevOps pull requests by project and repository name";

  public static void RegisterListPullRequestCommand(this Command rootCommand, AzDOClientFactory clientFactory)
  {
    Command command = new(COMMAND_NAME, COMMAND_DESCRIPTION);
    command.BuildCommandHandler(clientFactory);
    rootCommand.AddCommand(command);
  }

  private static void BuildCommandOptions(out Option<string> projectOption, out Option<string> repositoryOption)
  {
    projectOption = new(new[] { "--project", "-p" }, "Name of the Azure DevOps project");
    repositoryOption = new(new[] { "--repository", "-r" }, "Name of the Azure DevOps repository");
  }

  private static void BuildCommandHandler(this Command command, AzDOClientFactory clientFactory)
  {
    BuildCommandOptions(out Option<string> projectOption, out Option<string> repositoryOption);

    command.AddOption(projectOption);
    command.AddOption(repositoryOption);

    command.SetHandler(async (options) =>
    {
      var client = clientFactory.GetClient<GitHttpClient>();
      var prs = await client.GetPRsAsync(options.Project!, options.Repository);

      JsonSerializerOptions jsonOptions = new() { WriteIndented = true };
      File.WriteAllText(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        "desktop", $"pr.{DateTime.Now:yyyyMMdd.HHmmss}.json"), JsonSerializer.Serialize(prs, jsonOptions));

      prs.Display();

    }, new ListPullRequestOptionsBinder(projectOption, repositoryOption));
  }
}
