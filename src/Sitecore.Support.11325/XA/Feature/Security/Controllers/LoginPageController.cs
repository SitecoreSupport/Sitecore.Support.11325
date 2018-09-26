namespace Sitecore.Support.XA.Feature.Security.Controllers
{
  using Sitecore;
  using Sitecore.Mvc.Controllers;
  using Sitecore.Mvc.Extensions;
  using System;
  using System.Web;
  using System.Web.Mvc;

  [UsedImplicitly]
  public class LoginPageController : SitecoreController
  {
    public virtual ActionResult IndexForLogin()
    {
      if (base.HttpContext.Request.IsAuthenticated)
      {
        Uri url = base.HttpContext.Request.Url;
        if (url != (Uri)null)
        {
          string returnUrl = GetReturnUrl(url);
          if (!returnUrl.IsWhiteSpaceOrNull())
          {
            return Redirect(returnUrl);
          }
        }
      }
      if (Disabled())
      {
        return new EmptyResult();
      }
      return GetDefaultAction();
    }

    protected virtual string GetReturnUrl(Uri requestUrl)
    {
      return HttpUtility.ParseQueryString(requestUrl.Query)["returnUrl"];
    }
  }
}