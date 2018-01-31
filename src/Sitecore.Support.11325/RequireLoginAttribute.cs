namespace Sitecore.Support.Mvc.Filters
{
  using Sitecore.Diagnostics;
  using Sitecore.Mvc.Filters;
  using Sitecore.Web;
  using System;
  using System.Web.Mvc;
  using System.Web.Mvc.Filters;

  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public sealed class RequireLoginAttribute : FilterAttribute, IAuthenticationFilter
  {
    private readonly RequireLoginContext _requireLoginContext;

    public RequireLoginAttribute()
        : this(new RequireLoginContext())
    {
    }

    internal RequireLoginAttribute(RequireLoginContext requireLoginContext)
    {
      Assert.ArgumentNotNull(requireLoginContext, "requireLoginContext");
      this._requireLoginContext = requireLoginContext;
    }

    public void OnAuthentication(AuthenticationContext filterContext)
    {
      if (this._requireLoginContext.IsPageModeNormal && this._requireLoginContext.RequireLogin && !filterContext.HttpContext.Request.IsAuthenticated)
      {
        filterContext.Result = new HttpUnauthorizedResult();
      }
    }

    public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
    {
      if (filterContext.Result is HttpUnauthorizedResult)
      {
        string siteLoginPage = this._requireLoginContext.SiteLoginPage;
        if (string.IsNullOrEmpty(siteLoginPage))
        {
          Error.Raise("No login page specified for current site: " + (this._requireLoginContext.SiteName ?? "(unknown)"));
        }
        else
        {
          string text = WebUtil.ExtractFilePath(siteLoginPage);
          if (text[0] != '/')
          {
            text = "/" + text;
          }
          if (!string.Equals(filterContext.RequestContext.HttpContext.Request.Url.AbsolutePath, text, StringComparison.InvariantCultureIgnoreCase))
          {
            siteLoginPage = WebUtil.AddQueryString(siteLoginPage, "returnUrl", filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery);
            filterContext.Result = new RedirectResult(siteLoginPage);
          }
        }
      }
    }
  }
}