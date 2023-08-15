using Microsoft.TeamFoundation.SourceControl.WebApi;
using Xunit.Abstractions;

namespace Psi.NotebookExtensions.AzDO.Services.Tests;

[Collection("AzDO Git Client")]
public class GitHttpClientTests
{
  private readonly ITestOutputHelper _output;
  private readonly GitHttpClientTestFixture _fixture;

  private GitHttpClient client => _fixture.client;

  public GitHttpClientTests(GitHttpClientTestFixture fixture, ITestOutputHelper output)
  {
    _output = output;
    _fixture = fixture;
  }

  [Fact]
  public void Client_NotNull()
  {
    _output.WriteLine($"Git Client Base Address: {client.BaseAddress}");

    Assert.NotNull(client);
  }

  [Fact]
  public void Client_BaseURLStartsWithClientFactoryBaseURL()
  {
    _output.WriteLine($"Git Client Base Address: {client.BaseAddress}");

    Assert.NotNull(client);
  }
}
