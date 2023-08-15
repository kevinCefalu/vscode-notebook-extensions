using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Psi.NotebookExtensions.AzDO.Services.Tests;

public class GitHttpClientTestFixture : AzDOClientFactoryTestFixture
{
  internal readonly GitHttpClient client;

  public GitHttpClientTestFixture() : base()
  {
    client = clientFactory.GetClient<GitHttpClient>();
  }
}
