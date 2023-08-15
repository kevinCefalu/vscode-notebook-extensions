using System.Collections.Concurrent;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Psi.NotebookExtensions.AzDO.Services;

public static class GitHttpClientExtensions
{
  private static async Task<GitPullRequest> GetFullPRAsync(
      this GitHttpClient client,
      Guid projectId,
      Guid repositoryId,
      int pullRequestId,
      CancellationToken token = default)
  {
    GitPullRequest pr = await client.GetPullRequestAsync(projectId, repositoryId, pullRequestId,
      includeCommits: true, includeWorkItemRefs: true, cancellationToken: token);

    return pr;
  }

  public static async Task<GitPullRequest> GetPRAsync(
      this GitHttpClient client,
      int id,
      CancellationToken token = default)
  {
    GitPullRequest pr = await client.GetPullRequestByIdAsync(id, cancellationToken: token);

    pr = await client.GetFullPRAsync(pr.Repository.ProjectReference.Id,
      pr.Repository.Id, pr.PullRequestId, token);

    return pr;
  }

  public static async Task<List<GitPullRequest>> GetPRsAsync(
      this GitHttpClient client,
      string project,
      string? repository = null,
      PullRequestStatus status = PullRequestStatus.All,
      CancellationToken token = default)
  {
    GitPullRequestSearchCriteria searchCriteria = new()
    { // CreatorId, ReviewerId, SourceRefName, TargetRefName
      IncludeLinks = true,
      Status = status
    };

    if (!string.IsNullOrWhiteSpace(repository))
    {
      searchCriteria.RepositoryId = Guid.TryParse(repository, out Guid repoId) ?
        repoId : (await client.GetRepoAsync(project, repository, token)).Id;
    }

    var prs = await client.GetAllPRsInProjectBySearchCriteria(project, searchCriteria, token);

    return prs;
  }

  public static async Task<List<GitPullRequest>> GetAllPRsInProjectBySearchCriteria(
      this GitHttpClient client,
      string project,
      GitPullRequestSearchCriteria searchCriteria,
      CancellationToken token)
  {
    var (skip, top, count) = (0, 1000, 0);
    var prs = new List<GitPullRequest>();

    do
    {
      var page = await client.GetPullRequestsByProjectAsync(project,
        searchCriteria, skip: skip, top: top, cancellationToken: token);

      prs.AddRange(page);

      (count, skip) = (page.Count, skip + top);
    } while (count == top);
    return prs;
  }

  public static async Task<GitRepository> GetRepoAsync(
      this GitHttpClient client,
      string project,
      string repository,
      CancellationToken token = default)
  {
    GitRepository repo = await client.GetRepositoryAsync(project, repository, cancellationToken: token);

    return repo;
  }
}
