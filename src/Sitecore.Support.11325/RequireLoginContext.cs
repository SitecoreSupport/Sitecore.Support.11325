namespace Sitecore.Mvc.Filters
{
  using Sitecore;

  internal class RequireLoginContext
  {
    internal virtual bool IsPageModeNormal
    {
      get
      {
        return Context.PageMode.IsNormal;
      }
    }

    internal virtual bool RequireLogin
    {
      get
      {
        return Context.Site.RequireLogin;
      }
    }

    internal virtual string SiteLoginPage
    {
      get
      {
        return Context.Site.LoginPage;
      }
    }

    internal virtual string SiteName
    {
      get
      {
        return Context.Site.Name;
      }
    }
  }
}
