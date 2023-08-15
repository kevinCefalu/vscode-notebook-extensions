using Xunit.Abstractions;

namespace Psi.NotebookExtensions.AzDO.Services.Tests;

public class AzDOClientFactoryTestFixture : IDisposable
{
  internal readonly string organization;
  internal readonly string personalAccessToken;
  internal readonly AzDOClientFactory clientFactory;

  public AzDOClientFactoryTestFixture()
  {
    organization = Environment.GetEnvironmentVariable("AzDO_Organization", EnvironmentVariableTarget.Process)
      ?? throw new Exception("Environment variable 'AzDO_Organization' is not set");
    personalAccessToken = Environment.GetEnvironmentVariable("AzDO_PAT", EnvironmentVariableTarget.Process)
      ?? throw new Exception("Environment variable 'AzDO_PAT' is not set");

    clientFactory = new AzDOClientFactory(organization, personalAccessToken);
  }

  #region IDisposable Implementation

  private bool disposedValue;

  protected virtual void Dispose(bool disposing)
  {
    if (!disposedValue)
    {
      if (disposing)
      {
        clientFactory.Dispose();
      }

      disposedValue = true;
    }
  }

  public void Dispose()
  {
    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }

  #endregion
}
