using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Xunit.Abstractions;

namespace Psi.NotebookExtensions.AzDO.Services.Tests;

[Collection("AzDO Git Client")]
public class RepositoryTests
{
  private readonly ITestOutputHelper _output;
  private readonly GitHttpClientTestFixture _fixture;

  private GitHttpClient client => _fixture.client;

  public RepositoryTests(GitHttpClientTestFixture fixture, ITestOutputHelper output)
  {
    _output = output;
    _fixture = fixture;
  }

  [Theory]
  [InlineData("Netchex", "24a98cb8-f986-4565-94c2-6d8362e70a4e")]
  public async void Repository_CanGetByProjectNameAndRepositoryId(string project, Guid repository)
  {
    GitRepository repo = await client.GetRepoAsync(project, repository.ToString());

    Assert.NotNull(repo);
    Assert.Equal(project, repo.ProjectReference.Name);
    Assert.Equal(repository, repo.Id);
  }

  [Theory]
  [InlineData("Netchex", "24a98cb8-f986-4565-94c2-6d8362e70a4f")]
  public async void Repository_ThrowsOnGetByProjectNameAndInvalidRepositoryId(string project, Guid repository)
  {
    await Assert.ThrowsAsync<VssServiceException>(
      async () => await client.GetRepoAsync(project, repository.ToString()));
  }

  [Theory]
  [InlineData("Netchex", "www")]
  public async void Repository_CanGetByProjectAndRepositoryName(string project, string repository)
  {
    GitRepository repo = await client.GetRepoAsync(project, repository);

    Assert.NotNull(repo);
    Assert.Equal(project, repo.ProjectReference.Name);
    Assert.Equal(repository, repo.Name);
  }

  [Theory]
  [InlineData("Netchex", "Repository.Doesnt.Exist")]
  [InlineData("Netchex", "😉")]
  [InlineData("Netchex", """\ (0_o) /""")]
  [InlineData("Netchex", "°")]
  [InlineData("Netchex", " www")]
  // [InlineData("Netchex", "")] // Apparently valid
  // [InlineData("Netchex", "www ")] // Apparently valid
  public async void Repository_ThrowsOnGetByProjectNameAndInvalidRepositoryName(string project, string repository)
  {
    await Assert.ThrowsAsync<VssServiceException>(
      async () => await client.GetRepoAsync(project, repository));
  }

  [Theory]
  [InlineData("Netchex", "www")]
  [InlineData("Netchex", "24a98cb8-f986-4565-94c2-6d8362e70a4e")]
  public async void Repository_CanGetByProjectNameAndRepositoryIdOrName(string project, string repository)
  {
    GitRepository repo = await client.GetRepoAsync(project, repository);

    Assert.NotNull(repo);
    Assert.Equal(project, repo.ProjectReference.Name);

    if (Guid.TryParse(repository, out Guid repositoryId))
    {
      _output.WriteLine($"Asserting on repository id: {repository}");

      Assert.Equal(repositoryId, repo.Id);
    }
    else
    {
      _output.WriteLine($"Asserting on repository name: {repository}");

      Assert.Equal(repository, repo.Name);
    }
  }

  [Theory]
  [InlineData("Netchex")]
  [InlineData("Netchex DevOps")]
  [InlineData("Netchex Rewrite")]
  public async void Repository_CanListByProjectName(string project)
  {
    var repositories = await client.GetRepositoriesAsync(project);

    _output.WriteLine($"Found {repositories.Count} repositories in {project}");

    Assert.NotNull(repositories);
    Assert.NotEmpty(repositories);
  }

  [Theory]
  [InlineData("Project Doesn't Exist")]
  [InlineData("😉")]
  [InlineData("""\ (0_o) /""")]
  [InlineData("°")]
  // [InlineData("")] // Apparently valid
  // [InlineData("Netchex ")] // Apparently Valid
  // [InlineData(" Netchex")] // Apparently Valid
  public async void Repository_ThrowsOnListByInvalidProjectName(string project)
  {
    await Assert.ThrowsAsync<ProjectDoesNotExistWithNameException>(
      async () => await client.GetRepositoriesAsync(project));
  }
}
