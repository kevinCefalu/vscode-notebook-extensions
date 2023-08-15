
using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive.Formatting;
using Microsoft.TeamFoundation.SourceControl.WebApi;

using static Microsoft.DotNet.Interactive.Formatting.PocketViewTags;

namespace Psi.NotebookExtensions.AzDO.Formatters;

public static class PullRequestListFormatter
{
  public static void Register()
  {
    Formatter.Register<List<GitPullRequest>>((prs, writer) =>
    {
      var content = div(
        PullRequestFormatter.GetRemoteCSS(),
        PullRequestFormatter.GetLocalCSS(),

        div[@class: "wrapper"](
          div[@class: "header"](
            h1[@class: "header-title"]("Active Pull Requests"),
            h2[@class: "header-subtitle"]("Azure DevOps Server")
          ),

          div[@class: "cards"](
            prs.Select(pr => div[@class: "card is-collapsed"](
              div[@class: "card-inner js-expander"](
                h3[@class: "card-heading"](
                  // i[@class: "fa fa-code-pull-request fa-1x"](),
                  span[@class: "pr-id"](pr.PullRequestId.ToDisplayString()), " - ",
                  span[@class: "pr-title"](pr.Title.ToDisplayString())
                ),
                i[@class: $"pr-status fa {(
                  pr.Status == PullRequestStatus.Completed ? "fa-check-circle" :
                  pr.Status == PullRequestStatus.Abandoned ? "fa-times-circle" :
                  "fa-spinner fa-spin"
                )}"]()
              ),
              div[@class: "card-expander"](
                i[@class: "fa fa-close js-collapser"](),
                p(pr.Description)
              )
            ))
          )
        ),

        PullRequestFormatter.GetRemoteJS(),
        PullRequestFormatter.GetLocalJS()
      );

      writer.Write(content);

    }, HtmlFormatter.MimeType);
  }
}
