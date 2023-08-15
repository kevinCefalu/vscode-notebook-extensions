using Microsoft.DotNet.Interactive;
using Psi.NotebookExtensions.AzDO.Commands;
using Psi.NotebookExtensions.AzDO.Formatters;
using Psi.NotebookExtensions.AzDO.Services;

namespace Psi.NotebookExtensions.AzDO;

public class PullRequestExtension : IKernelExtension
{
  private const string DEFAULT_HOST_NAME = "dev.azure.com";
  private readonly AzDOClientFactory _clientFactory;

  public PullRequestExtension()
  {
    var organization = Environment.GetEnvironmentVariable("AzDO_Organization", EnvironmentVariableTarget.Process)
      ?? throw new Exception("Environment variable 'AzDO_Organization' is not set");
    var personalAccessToken = Environment.GetEnvironmentVariable("AzDO_PAT", EnvironmentVariableTarget.Process)
      ?? throw new Exception("Environment variable 'AzDO_PAT' is not set");

    _clientFactory = new AzDOClientFactory(organization, personalAccessToken, DEFAULT_HOST_NAME);
  }

  public PullRequestExtension(string organization, string personalAccessToken, string hostName = DEFAULT_HOST_NAME)
  {
    _clientFactory = new AzDOClientFactory(organization, personalAccessToken, hostName);
  }

  public Task OnLoadAsync(Kernel kernel)
  {
    $"Connected to Azure DevOps Server: {_clientFactory.OrganizationUrl}".Display();

    RegisterFormatters();
    RegisterCommands(kernel);

    return Task.CompletedTask;
  }

  private void RegisterFormatters()
  {
    PullRequestListFormatter.Register();
    PullRequestFormatter.Register();
  }

  private void RegisterCommands(Kernel kernel)
  {
    PullRequestRootCommand.Register(kernel, _clientFactory);
  }
}
