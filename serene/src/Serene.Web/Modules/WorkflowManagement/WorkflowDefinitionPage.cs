namespace Serene.WorkflowManagement.Pages;

[PageAuthorize(typeof(Serenity.Workflow.Entities.WorkflowDefinitionRow))]
public class WorkflowDefinitionPage : Controller
{
    [Route("Workflow/Definitions")]
    public ActionResult Index()
    {
        return this.GridPage("@/WorkflowManagement/WorkflowDefinitionPage", Serenity.Workflow.Entities.WorkflowDefinitionRow.Fields.PageTitle());
    }
}
