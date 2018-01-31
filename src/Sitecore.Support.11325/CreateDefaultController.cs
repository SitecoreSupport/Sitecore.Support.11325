namespace Sitecore.Support.Mvc.Pipelines.Request.CreateController
{
  using Sitecore.Support.Mvc.Controllers;
  using System.Web.Mvc;

  public class CreateDefaultController : Sitecore.Mvc.Pipelines.Request.CreateController.CreateDefaultController
  {
    protected override IController CreateController()
    {
      return new SupportSitecoreController();
    }
  }
}