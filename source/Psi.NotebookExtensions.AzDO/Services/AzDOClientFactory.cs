using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Psi.NotebookExtensions.AzDO.Services;

public class AzDOClientFactory : IDisposable
{
  private const string DEFAULT_HOST_NAME = "dev.azure.com";

  private readonly string _organization;
  private readonly string _personalAccessToken;

  private readonly Uri _baseUri;
  private readonly VssCredentials _credentials;
  private readonly VssConnection _connection;

  public string OrganizationUrl => _connection.Uri.ToString();

  public AzDOClientFactory(string organization, string personalAccessToken, string hostName = DEFAULT_HOST_NAME)
  {
    _organization = organization;
    _personalAccessToken = personalAccessToken;
    _baseUri = new Uri($"https://{hostName}/{_organization}");

    _credentials = new VssBasicCredential(string.Empty, _personalAccessToken);
    _connection = new VssConnection(_baseUri, _credentials);
  }

  public T GetClient<T>() where T : VssHttpClientBase
  {
    return _connection.GetClient<T>();
  }

  #region IDisposable Implementation

  private bool _isDisposed;

  protected virtual void Dispose(bool disposing)
  {
    if (!_isDisposed)
    {
      if (disposing)
      {
        _connection?.Dispose();
      }

      _isDisposed = true;
    }
  }

  public void Dispose()
  { // !: Do not change; use `Dispose(bool disposing)`
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }

  #endregion
}
