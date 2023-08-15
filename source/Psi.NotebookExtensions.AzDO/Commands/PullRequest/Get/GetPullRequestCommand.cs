using System.CommandLine;
using System.Text.Json;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Psi.NotebookExtensions.AzDO.Services;

namespace Psi.NotebookExtensions.AzDO.Commands;

public static class GetPullRequestCommand
{
  private const string COMMAND_NAME = "get";
  private const string COMMAND_DESCRIPTION = "Displays information about an Azure DevOps pull request by ID";

  public static void RegisterGetPullRequestCommand(this Command rootCommand, AzDOClientFactory clientFactory)
  {
    Command command = new(COMMAND_NAME, COMMAND_DESCRIPTION);
    command.BuildCommandHandler(clientFactory);
    rootCommand.AddCommand(command);
  }

  private static void BuildCommandOptions(out Option<int> idOption)
  {
    idOption = new(new[] { "--id", "-i" }, "ID number of the pull request");
  }

  private static void BuildCommandHandler(this Command command, AzDOClientFactory clientFactory)
  {
    BuildCommandOptions(out Option<int> idOption);

    command.AddOption(idOption);

    command.SetHandler(async (options) =>
    {
      var client = clientFactory.GetClient<GitHttpClient>();
      GitPullRequest pr = await client.GetPullRequestByIdAsync(options.Id);

      pr = await client.GetPullRequestAsync(pr.Repository.ProjectReference.Id,
        pr.Repository.Id, pr.PullRequestId, null, null, 100, true, true);

      pr.Display();

    }, new GetPullRequestOptionsBinder(idOption));
  }
}
