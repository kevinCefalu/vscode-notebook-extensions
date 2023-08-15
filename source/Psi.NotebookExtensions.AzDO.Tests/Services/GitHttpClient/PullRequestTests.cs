using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Xunit.Abstractions;

namespace Psi.NotebookExtensions.AzDO.Services.Tests;

[Collection("AzDO Git Client")]
public class PullRequestTests
{
  private readonly ITestOutputHelper _output;
  private readonly GitHttpClientTestFixture _fixture;

  private GitHttpClient client => _fixture.client;

  public PullRequestTests(GitHttpClientTestFixture fixture, ITestOutputHelper output)
  {
    _output = output;
    _fixture = fixture;
  }

  [Theory]
  [InlineData(37063)]
  [InlineData(37068)]
  public async void PullRequest_CanGetById(int pullRequestId)
  {
    GitPullRequest pr = await client.GetPRAsync(pullRequestId);

    Assert.NotNull(pr);
    Assert.Equal(pullRequestId, pr.PullRequestId);
  }

  [Theory]
  [InlineData(int.MinValue)]
  [InlineData(-1)]
  [InlineData(0)]
  public async void PullRequest_ThrowsOnGetByInvalidId(int pullRequestId)
  {
    await Assert.ThrowsAsync<VssServiceException>(
      async () => await client.GetPRAsync(pullRequestId));
  }

  [Theory]
  [InlineData("Netchex")]
  [InlineData("Netchex DevOps")]
  [InlineData("Netchex Rewrite")]
  public async void PullRequest_CanListByProject(string project)
  {
    List<GitPullRequest> prs = await client.GetPRsAsync(project);

    _output.WriteLine($"Found {prs.Count} in {project}");

    Assert.NotNull(prs);
    Assert.NotEmpty(prs);
  }

  [Theory]
  [InlineData("Project Doesn't Exist")]
  [InlineData("😉")]
  [InlineData("""\ (0_o) /""")]
  [InlineData("°")]
  // [InlineData("")] // Apparently valid
  // [InlineData("Netchex ")] // Apparently valid
  // [InlineData(" Netchex")] // Apparently valid
  public async void PullRequest_ThrowsOnListByInvalidProject(string project)
  {
    await Assert.ThrowsAsync<ProjectDoesNotExistWithNameException>(
      async () => await client.GetPRsAsync(project));
  }

  [Theory]
  [InlineData("Netchex", PullRequestStatus.Active)]
  [InlineData("Netchex Rewrite", PullRequestStatus.Active)]
  public async void PullRequest_CanListByProjectAndStatus(string project, PullRequestStatus status)
  {
    List<GitPullRequest> prs = await client.GetPRsAsync(project, status: status);

    _output.WriteLine($"Found {prs.Count} {Enum.GetName(status)} in {project}");

    Assert.NotNull(prs);
    Assert.NotEmpty(prs);
  }

  [Theory]
  [InlineData("Netchex", "www")]
  [InlineData("Netchex", "24a98cb8-f986-4565-94c2-6d8362e70a4e")]
  [InlineData("Netchex Rewrite", "Netchex.Gateway.Api.V2")]
  [InlineData("Netchex Rewrite", "7fd77167-5adc-4722-b7ce-e2f1b580e67b")]
  [InlineData("Netchex Rewrite", "Netchex.ClearCompany")]
  [InlineData("Netchex Rewrite", "c8e10069-b92b-41e3-9e4f-b860d39228c5")]
  public async void PullRequest_CanListByProjectAndRepository(string project, string repository)
  {
    List<GitPullRequest> prs = await client.GetPRsAsync(project, repository);

    _output.WriteLine($"Found {prs.Count} in {project}/{repository}");

    Assert.NotNull(prs);
    Assert.NotEmpty(prs);
  }

  [Theory]
  [InlineData("Netchex", "www", PullRequestStatus.Active)]
  [InlineData("Netchex", "24a98cb8-f986-4565-94c2-6d8362e70a4e", PullRequestStatus.Active)]
  [InlineData("Netchex Rewrite", "Netchex.Gateway.Api.V2", PullRequestStatus.Active)]
  [InlineData("Netchex Rewrite", "7fd77167-5adc-4722-b7ce-e2f1b580e67b", PullRequestStatus.Active)]
  [InlineData("Netchex Rewrite", "Netchex.ClearCompany", PullRequestStatus.Active)]
  [InlineData("Netchex Rewrite", "c8e10069-b92b-41e3-9e4f-b860d39228c5", PullRequestStatus.Active)]
  public async void PullRequest_CanListByProjectRepositoryAndStatus(string project, string repository, PullRequestStatus status)
  {
    List<GitPullRequest> prs = await client.GetPRsAsync(project, repository, status: status);

    _output.WriteLine($"Found {prs.Count} {Enum.GetName(status)} in {project}/{repository}");

    Assert.NotNull(prs);
    Assert.NotEmpty(prs);
  }
}
