using TasksPages = Serene.Tasks.Pages;

[assembly: NavigationMenu(6000, "Tasks", icon: "fa-tasks")]
[assembly: NavigationLink(6100, "Tasks/Task Items", typeof(TasksPages.TaskItemPage), icon: "fa-tasks")]
