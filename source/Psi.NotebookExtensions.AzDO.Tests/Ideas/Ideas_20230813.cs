using Microsoft.TeamFoundation.SourceControl.WebApi;
using Psi.NotebookExtensions.AzDO.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Psi.NotebookExtensions.AzDO.Tests.Ideas
{
  public class Ideas_20230813 : IDisposable
  {
    private readonly ITestOutputHelper _testOutput;
    internal readonly string organization;
    internal readonly string personalAccessToken;
    internal readonly AzDOClientFactory clientFactory;

    public Ideas_20230813(ITestOutputHelper testOutput)
    {
      _testOutput = testOutput;

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

    [Theory(Skip = "For TDD only; maunally run")]
    [InlineData("Netchex", PullRequestStatus.All)]
    [InlineData("Netchex", PullRequestStatus.Abandoned)]
    [InlineData("Netchex", PullRequestStatus.Active)]
    [InlineData("Netchex", PullRequestStatus.Completed)]
    [InlineData("Netchex", PullRequestStatus.NotSet)]
    [InlineData("Netchex Rewrite", PullRequestStatus.All)]
    [InlineData("Netchex Rewrite", PullRequestStatus.Abandoned)]
    [InlineData("Netchex Rewrite", PullRequestStatus.Active)]
    [InlineData("Netchex Rewrite", PullRequestStatus.Completed)]
    [InlineData("Netchex Rewrite", PullRequestStatus.NotSet)]
    public async void CanGetPullRequestsByProjectAndStatus(string project, PullRequestStatus status, CancellationToken ct = default)
    {
      var (skip, top, count) = (0, 1000, 0);
      var searchCriteria = new GitPullRequestSearchCriteria() { IncludeLinks = true, Status = status };
      using var client = clientFactory.GetClient<GitHttpClient>();

      List<GitPullRequest> prCollection = new();

      do
      {
        var pullRequests = await client.GetPullRequestsByProjectAsync(
          project, searchCriteria, skip: skip, top: top, cancellationToken: ct);

        prCollection.AddRange(pullRequests);
        (count, skip) = (pullRequests.Count, skip + top);
      } while (count == top);

      _testOutput.WriteLine($"Found {prCollection.Count} pull requests in project '{project}' with status '{status}'");

      Assert.NotNull(prCollection);
    }

    [Theory(Skip = "For TDD only; maunally run")]
    // [InlineData("Netchex", PullRequestStatus.All)] // hits concurrency limit
    [InlineData("Netchex", PullRequestStatus.Abandoned)]
    [InlineData("Netchex", PullRequestStatus.Active)]
    // [InlineData("Netchex", PullRequestStatus.Completed)] // hits concurrency limit
    [InlineData("Netchex", PullRequestStatus.NotSet)]
    // [InlineData("Netchex Rewrite", PullRequestStatus.All)] // hits concurrency limit
    [InlineData("Netchex Rewrite", PullRequestStatus.Abandoned)]
    [InlineData("Netchex Rewrite", PullRequestStatus.Active)]
    // [InlineData("Netchex Rewrite", PullRequestStatus.Completed)] // hits concurrency limit
    [InlineData("Netchex Rewrite", PullRequestStatus.NotSet)]
    public async void CanGetPullRequestsByProjectAndStatus_Deep(string project, PullRequestStatus status, CancellationToken ct = default)
    {
      List<GitPullRequest> pullRequestCollection = new();
      var (skip, top, count, lockObj) = (0, 1000, 0, new object());
      var searchCriteria = new GitPullRequestSearchCriteria() { IncludeLinks = true, Status = status };

      using (var client = clientFactory.GetClient<GitHttpClient>())
      {
        do
        {
          // TODO: Try converting this to getting a list of repos, and then calling getpullrequestsasync on each repo
          var pullRequests = await client.GetPullRequestsByProjectAsync(
            project, searchCriteria, skip: skip, top: top, cancellationToken: ct);

          var prs = pullRequests.AsParallel().WithDegreeOfParallelism(8).Select(
            async pr => {
              return (await client.GetPullRequestAsync(
                pr.Repository.ProjectReference.Id,
                pr.Repository.Id, pr.PullRequestId,
                cancellationToken: ct));

              // return fullPR;
              // prCollection.Add(pr);
              // lock (lockObj) prCollection.Add(fullPR);
            }
          ).ToList();

          pullRequestCollection.AddRange(await Task.WhenAll(prs));

          (count, skip) = (pullRequests.Count, skip + top);
        } while (count == top);
      }

      _testOutput.WriteLine($"Found {pullRequestCollection.Count} pull requests in project '{project}' with status '{status}'");

      Assert.NotNull(pullRequestCollection);
    }
  }
}
