using System.CommandLine;
using System.CommandLine.Binding;

namespace Psi.NotebookExtensions.AzDO.Commands;

internal class ListPullRequestOptions
{
  public string? Project { get; init; }
  public string? Repository { get; init; }

  public ListPullRequestOptions(string? project, string? repository) => (Project, Repository) = (project, repository);
}

internal class ListPullRequestOptionsBinder : BinderBase<ListPullRequestOptions>
{
  private readonly Option<string> _projectOption;
  private readonly Option<string> _repositoryOption;

  public ListPullRequestOptionsBinder(Option<string> projectOption, Option<string> repositoryOption)
  {
    _projectOption = projectOption;
    _repositoryOption = repositoryOption;
  }

  protected override ListPullRequestOptions GetBoundValue(BindingContext bindingContext) => new(
    project: bindingContext.ParseResult.GetValueForOption(_projectOption),
    repository: bindingContext.ParseResult.GetValueForOption(_repositoryOption)
  );
}
