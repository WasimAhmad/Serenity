using Microsoft.AspNetCore.Mvc;
using Serenity.Web;

namespace Serene.Workflow.WorkflowDesigner
{
    [PageAuthorize]
    [Route("Workflow/WorkflowDesigner/[action]")]
    public class WorkflowDesignerPageController : Controller
    {
        [Route("~/Workflow/WorkflowDesigner")]
        public ActionResult Index()
        {
            return View("~/Modules/Workflow/WorkflowDesigner/WorkflowDesignerPage.cshtml");
        }
    }
}
