namespace Sitecore.Support.Mvc.Controllers
{
  using Sitecore;
  using Sitecore.Configuration;
  using Sitecore.Mvc.Extensions;
  using Sitecore.Support.Mvc.Filters;
  using Sitecore.Mvc.Presentation;
  using Sitecore.StringExtensions;
  using Sitecore.Web;
  using System.Web.Mvc;
  using System.Web.Routing;

  [UsedImplicitly]
  public class SupportSitecoreController : Controller
  {
    [SupportRequireLogin]
    public virtual ActionResult Index()
    {
      if (this.Disabled())
      {
        return new EmptyResult();
      }
      return this.GetDefaultAction();
    }

    protected virtual bool Disabled()
    {
      return (base.RouteData.Values["scOutputGenerated"] as string).ToBool();
    }

    protected virtual ActionResult GetDefaultAction()
    {
      IView pageView = PageContext.Current.PageView;
      if (pageView == null)
      {
        return this.RedirectToNotFound();
      }
      return base.View(pageView);
    }

    protected virtual ActionResult RedirectToNotFound()
    {
      PageContext current = PageContext.Current;
      string text = current.RequestContext.HttpContext.Request.Url.LocalPath;
      string text2 = (current.RequestContext.RouteData.Route as Route).ValueOrDefault((Route route) => route.Url);
      if (text2 != null)
      {
        text += " (route: {0})".FormatWith(text2);
      }
      string userName = Context.GetUserName();
      string siteName = Context.GetSiteName();
      string url = WebUtil.AddQueryString(Settings.ItemNotFoundUrl, "item", text, "user", userName, "site", siteName);
      return this.Redirect(url);
    }
  }
}