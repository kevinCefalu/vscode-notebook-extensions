using Xunit.Abstractions;

namespace Psi.NotebookExtensions.AzDO.Services.Tests;

[Collection("AzDO Client Factory")]
public class AzDOClientFactoryTests
{
  private readonly ITestOutputHelper _output;
  private readonly AzDOClientFactoryTestFixture _fixture;

  private AzDOClientFactory clientFactory => _fixture.clientFactory;
  private string organization => _fixture.organization;

  public AzDOClientFactoryTests(AzDOClientFactoryTestFixture fixture, ITestOutputHelper output)
  {
    _output = output;
    _fixture = fixture;
  }

  [Fact]
  public void ClientFactory_NotNull()
  {
    _output.WriteLine($"Connected to {clientFactory.OrganizationUrl}");

    Assert.NotNull(clientFactory);
  }

  [Fact]
  public void ClientFactory_CorrectOrganizationURL()
  {
    _output.WriteLine($"Connected to {clientFactory.OrganizationUrl}");

    Assert.StartsWith($"https://dev.azure.com/{organization}", clientFactory.OrganizationUrl);
  }
}
