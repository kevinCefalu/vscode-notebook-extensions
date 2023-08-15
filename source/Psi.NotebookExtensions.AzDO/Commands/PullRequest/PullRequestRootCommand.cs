using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.DotNet.Interactive;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Psi.NotebookExtensions.AzDO.Services;


namespace Psi.NotebookExtensions.AzDO.Commands;

public static class PullRequestRootCommand
{
  private const string COMMAND_NAME = "azdo-pr";
  private const string COMMAND_DESCRIPTION = "Displays information about Azure DevOps pull request(s)";

  public static void Register(Kernel kernel, AzDOClientFactory clientFactory)
  {
    Command rootCommand = new($"#!{COMMAND_NAME}", COMMAND_DESCRIPTION);

    rootCommand.RegisterGetPullRequestCommand(clientFactory);
    rootCommand.RegisterListPullRequestCommand(clientFactory);

    kernel.AddDirective(rootCommand);
  }
}
