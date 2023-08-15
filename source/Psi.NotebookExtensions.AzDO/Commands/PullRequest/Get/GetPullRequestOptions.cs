using System.CommandLine;
using System.CommandLine.Binding;

namespace Psi.NotebookExtensions.AzDO.Commands;

internal class GetPullRequestOptions
{
  public int Id { get; init; }

  public GetPullRequestOptions(int id) => Id = id;
}

internal class GetPullRequestOptionsBinder : BinderBase<GetPullRequestOptions>
{
  private readonly Option<int> _idOption;

  public GetPullRequestOptionsBinder(Option<int> idOption)
  {
    _idOption = idOption;
  }

  protected override GetPullRequestOptions GetBoundValue(BindingContext bindingContext) => new(
    id: bindingContext.ParseResult.GetValueForOption(_idOption)
  );
}
