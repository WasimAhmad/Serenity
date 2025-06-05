namespace Serene.Tasks.Pages;

[PageAuthorize(typeof(TaskItemRow))]
public class TaskItemPage : Controller
{
    [Route("Tasks/TaskItem")]
    public ActionResult Index()
    {
        return this.GridPage("@/Tasks/TaskItem/TaskItemPage", TaskItemRow.Fields.PageTitle());
    }
}
