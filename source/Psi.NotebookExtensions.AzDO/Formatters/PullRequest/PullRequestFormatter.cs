
using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive.Formatting;
using Microsoft.TeamFoundation.SourceControl.WebApi;

using static Microsoft.DotNet.Interactive.Formatting.PocketViewTags;

namespace Psi.NotebookExtensions.AzDO.Formatters;

public static class PullRequestFormatter
{
  public static void Register()
  {
    Formatter.Register<GitPullRequest>((pr, writer) =>
    {
      var content = div(
        GetRemoteCSS(),
        GetLocalCSS(),

        div[@class: "wrapper"](

          div[@class: "cards"](
            div[@class: "card is-collapsed"](
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
            )
          )
        ),

        GetRemoteJS(),
        GetLocalJS()
      );

      writer.Write(content);

    }, HtmlFormatter.MimeType);
  }

  public static PocketView GetRemoteCSS()
  {
    PocketView content = link[
      rel: "stylesheet",
      crossorigin: "anonymous",
      referrerpolicy: "no-referrer",
      href: "https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css",
      integrity: "sha512-9usAa10IRO0HhonpyAIVpjrylPvoDwiPUiKdWk5t3PyolY1cOd4DSE0Ga+ri4AuTroPR5aQvXU9xC6qOPnzFeg=="
    ]();

    return content;
  }

  public static object GetLocalCSS()
  {
    PocketView content = style[type: "text/css"](new HtmlString("""
      :root {
        --clr-dark: #333a45;
        --clr-light: #eceef1;
        --clr-card-border: rgba(65, 65, 65, 0.158);
        --clr-card-background: #cccccc;

        --box-shadow: 0.25em 0.25em rgba(0, 0, 0, 0.06);
        --card-gap: 15px;
        --card-background-color: #333a45;
        --text-color: #333a45;
        --wrapper-background-color: #fff;
        --column-count: 3;
      }

      *,
      *::before,
      *::after {
        box-sizing: border-box;
      }

      .wrapper {
        margin: 1em 1em;
      }

      .wrapper>.header {
        padding: 1em 0 0 2em;
        text-align: left;
      }

      .wrapper>.header>.header-title {
        margin: 0;
        font-size: 2.5em;
        font-weight: 500;
        line-height: 1.1;
      }

      .wrapper>.header>.header-subtitle {
        margin: 0;
        font-size: 1.5em;
        font-weight: 100;
        font-style: oblique;
        line-height: 1.1;
        opacity: 0.75;
      }

      .wrapper>.cards {
        padding: var(--card-gap);
        display: flex;
        flex-flow: row wrap;
        align-items: flex-start;
      }

      .card {
        margin: var(--card-gap);
        width: calc((100% / var(--column-count)) - calc(var(--card-gap) * 2));
        transition: all 0.2s ease-in-out;
      }

      .card:hover>.card-inner {
        filter: brightness(125%);
        transform: scale(1.025);
        box-shadow: var(--box-shadow);
      }

      .card:hover>.card-inner>.pr-id {
        font-weight: 100;
      }

      .card.is-collapsed>.card-inner:after {
        content: "";
        opacity: 0;
      }

      .card.is-collapsed>.card-expander {
        max-height: 0;
        min-height: 0;
        overflow: hidden;
        margin-top: 0;
        opacity: 0;
      }

      .card.is-expanded>.card-inner {
        filter: brightness(95%);
      }

      .card.is-expanded>.card-inner:after {
        content: "";
        opacity: 1;
        display: block;
        height: 0;
        width: 0;
        position: absolute;
        bottom: -30px;
        left: calc(50% - 15px);
        border-left: 15px solid transparent;
        border-right: 15px solid transparent;
        border-bottom: 15px solid var(--clr-dark);
      }

      .card.is-expanded>.card-expander {
        max-height: 1000px;
        min-height: 300px;
        overflow: visible;
        margin-top: 30px;
        opacity: 1;
      }

      .card.is-expanded:hover .card-inner {
        transform: scale(1);
      }

      .card.is-inactive .card-inner {
        pointer-events: none;
        opacity: 0.75;
      }

      .card.is-inactive:hover .card-inner {
        background-color: var(--card-background-color);
        transform: scale(1);
      }

      .card-inner {
        width: 100%;
        padding: var(--card-gap);
        position: relative;
        cursor: pointer;
        background-color: var(--card-background-color);
        color: var(--clr-light);
        font-size: 1.15em;
        transition: all 0.2s ease-in-out;
      }

      .card-inner:after {
        transition: all 0.3s ease-in-out;
      }

      .card-inner .fa {
        margin: 0 0.5em 0 0;
        font-size: 0.75em;
      }

      .card-inner h3 {
        margin: 0;
        padding: 0;
        font-size: 0.8em;
        font-weight: 100;
      }

      .card-inner h3 .pr-id {
        font-weight: bold;
      }

      .card-inner h3 .pr-id::before {
        content: "#";
      }

      .card-inner h3 .pr-title {
        font-weight: 100;
      }

      .card-inner .pr-status {
        position: absolute;
        bottom: -5px;
        right: -18px;
        font-size: 1.5em;
      }

      .card-inner .pr-status.fa-check-circle {
        color: rgb(46, 182, 117);
      }

      .card-inner .pr-status.fa-times-circle {
        color: rgb(182, 46, 46);
      }

      .card-expander {
        transition: all 0.25s ease-in-out;
        background-color: var(--clr-dark);
        filter: brightness(125%);
        width: 100%;
        position: relative;
        display: flex;
        justify-content: center;
        align-items: center;
        color: var(--clr-light);
        font-size: 1em;
      }

      .card-expander .fa {
        font-size: 0.75em;
        position: absolute;
        top: 10px;
        right: 10px;
        cursor: pointer;
      }

      .card-expander .fa:hover {
        opacity: 0.9;
      }

      @media screen and (max-width: 991px) {
        .card {
          width: calc((100% / 2) - 30px);
        }
      }

      @media screen and (max-width: 767px) {
        .card {
          width: 100%;
        }
      }

      @media screen and (min-width: 992px) {
        .card:nth-of-type(3n+2) .card-expander {
          margin-left: calc(-100% - 30px);
        }

        .card:nth-of-type(3n+3) .card-expander {
          margin-left: calc(-200% - 60px);
        }

        .card:nth-of-type(3n+4) {
          clear: left;
        }

        .card-expander {
          width: calc(300% + 60px);
        }
      }

      @media screen and (min-width: 768px) and (max-width: 991px) {
        .card:nth-of-type(2n+2) .card-expander {
          margin-left: calc(-100% - 30px);
        }

        .card:nth-of-type(2n+3) {
          clear: left;
        }

        .card-expander {
          width: calc(200% + 30px);
        }
      }
    """));

    return content;
  }

  public static object GetRemoteJS()
  {
    PocketView content = script[
      src: "https://cdnjs.cloudflare.com/ajax/libs/jquery/2.1.3/jquery.min.js",
      type: "text/javascript"
    ]();

    return content;
  }

  public static object GetLocalJS()
  {
    PocketView content = script[type: "text/javascript"](new HtmlString("""
      var $cell = $('.card');

      //open and close card when clicked on card
      $cell.find('.js-expander').click(function() {

        var $thisCell = $(this).closest('.card');

        if ($thisCell.hasClass('is-collapsed')) {
          $cell.not($thisCell).removeClass('is-expanded').addClass('is-collapsed').addClass('is-inactive');
          $thisCell.removeClass('is-collapsed').addClass('is-expanded');

          if ($cell.not($thisCell).hasClass('is-inactive')) {
            //do nothing
          } else {
            $cell.not($thisCell).addClass('is-inactive');
          }

        } else {
          $thisCell.removeClass('is-expanded').addClass('is-collapsed');
          $cell.not($thisCell).removeClass('is-inactive');
        }
      });

      //close card when click on cross
      $cell.find('.js-collapser').click(function() {

        var $thisCell = $(this).closest('.card');

        $thisCell.removeClass('is-expanded').addClass('is-collapsed');
        $cell.not($thisCell).removeClass('is-inactive');

      });
    """));

    return content;
  }
}
