using Serenity.Services;

namespace Serenity.Workflow
{
    public class GetPermittedActionsResponse : ServiceResponse
    {
        public List<string> Actions { get; set; } = new();
    }
}
