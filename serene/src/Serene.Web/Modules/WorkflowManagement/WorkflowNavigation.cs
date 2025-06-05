using WorkflowPages = Serene.WorkflowManagement.Pages;

[assembly: NavigationMenu(12000, "Workflow", icon: "fa-random")]
[assembly: NavigationLink(12100, "Workflow/Definitions", typeof(WorkflowPages.WorkflowDefinitionPage), icon: "fa-random")]
