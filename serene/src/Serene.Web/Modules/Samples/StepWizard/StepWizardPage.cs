namespace Serene.Samples.Pages;

[Route("Samples/StepWizard")]
public class StepWizardPage : Controller
{
    [PageAuthorize]
    public ActionResult Index()
    {
        return View("~/Areas/Samples/StepWizard/Index.cshtml");
    }
}
