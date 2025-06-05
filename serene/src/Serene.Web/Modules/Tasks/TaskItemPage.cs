namespace Serene.Tasks.Pages;

[PageAuthorize(typeof(TaskItemRow))]
public class TaskItemPage : Controller
{
    [Route("Tasks/TaskItem")]
    public ActionResult Index()
    {
        return this.GridPage("@/Tasks/TaskItemPage", TaskItemRow.Fields.PageTitle());
    }
}
