using Serenity.Services;

namespace Serene.Web.Workflow.Core
{
    public class GetPermittedActionsResponse : ServiceResponse
    {
        public List<string> Actions { get; set; } = new();
    }
}
