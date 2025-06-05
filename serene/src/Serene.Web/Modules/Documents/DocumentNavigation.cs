using DocumentsPages = Serene.Documents.Pages;

[assembly: NavigationMenu(6200, "Documents", icon: "fa-file")]
[assembly: NavigationLink(6210, "Documents/Document", typeof(DocumentsPages.DocumentPage), icon: "fa-file")]
