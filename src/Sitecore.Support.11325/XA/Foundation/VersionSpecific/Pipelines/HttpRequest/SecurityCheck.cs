using System;
using System.Web;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Publishing;
using Sitecore.Sites;
using Sitecore.Web;
using Sitecore.XA.Foundation.Multisite.Extensions;

namespace Sitecore.Support.XA.Foundation.VersionSpecific.Pipelines.HttpRequest
{
  public class SecurityCheck : HttpRequestProcessor
  {
    public override void Process(HttpRequestArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));
      if (Context.Site.IsSxaSite() && Context.Item != null && !HasAccess())
      {
        args.AbortPipeline();
        string loginPage = GetLoginPage(Context.Site);
        if (loginPage.Length > 0)
        {
          Tracer.Info("Redirecting to login page \"" + loginPage + "\".");
          var returnUrl = HttpUtility.UrlEncode(HttpContext.Current.Request.Url.PathAndQuery);
          var urlString = String.Format("{0}?returnUrl={1}", loginPage, returnUrl);
          WebUtil.Redirect(urlString, false);
        }
        else
        {
          Tracer.Info("Redirecting to error page as no login page was found.");
          WebUtil.RedirectToErrorPage("Login is required, but no valid login page has been specified for the site (" + Context.Site.Name + ").", false);
        }
      }
    }

    protected virtual string GetLoginPage(SiteContext site)
    {
      if (site == null)
      {
        return string.Empty;
      }
      if (site.DisplayMode == DisplayMode.Normal)
      {
        return site.LoginPage;
      }
      SiteContext shellSite = SiteContext.GetSite("shell");
      if (shellSite != null)
      {
        return shellSite.LoginPage;
      }
      return string.Empty;
    }

    protected virtual bool HasAccess()
    {
      Tracer.Info("Checking security for current user \"" + Context.User.Name + "\".");
      SiteContext site = Context.Site;
      if (site != null && site.RequireLogin && (!Context.User.IsAuthenticated && !IsLoginPageRequest()))
      {
        Tracer.Warning("Site \"" + site.Name + "\" requires login and no user is logged in.");
        return false;
      }
      if (site != null && site.DisplayMode != DisplayMode.Normal && (!Context.User.IsAuthenticated && string.IsNullOrEmpty(PreviewManager.GetShellUser())) && !IsLoginPageRequest())
      {
        Tracer.Warning("Current display mode is \"" + site.DisplayMode + "\" and no user is logged in.");
        return false;
      }
      if (Context.Item == null)
      {
        Tracer.Info("Access is granted as there is no current item.");
        return true;
      }
      if (Context.Item.Access.CanRead())
      {
        Tracer.Info("Access granted as the current user \"" + Context.GetUserName() + "\" has read access to current item.");
        return true;
      }
      Tracer.Warning("The current user \"" + Context.GetUserName() + "\" does not have read access to the current item \"" + Context.Item.Paths.Path + "\".");
      return false;
    }

    protected virtual bool IsLoginPageRequest()
    {
      string loginPage = GetLoginPage(Context.Site);
      if (string.IsNullOrEmpty(loginPage))
      {
        return false;
      }
      return HttpContext.Current.Request.RawUrl.StartsWith(loginPage, StringComparison.InvariantCultureIgnoreCase);
    }
  }
}