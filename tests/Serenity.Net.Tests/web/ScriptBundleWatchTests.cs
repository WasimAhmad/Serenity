namespace Serenity.Web;

public class ScriptBundleWatchTests
{
    [Fact]
    public void When_Script_File_Changes_It_Reloads_Bundle()
    {
        var env = new MockHostEnvironment();
        var testFile = env.Path.Combine(env.WebRootPath, "test.js");
        env.AddWebFile(testFile, "before");
        var fileWatcherFactory = new MockFileWatcherFactory(env.FileSystem);
        var container = new ServiceCollection();
        container.AddSingleton<IWebHostEnvironment>(env);
        container.AddSingleton<IPermissionService, MockPermissions>();
        container.AddSingleton<IFileWatcherFactory>(fileWatcherFactory);
        container.AddScriptBundling(options =>
        {
            options.Enabled = true;
            options.Bundles["Test"] =
            [
                "~/" + env.Path.GetFileName(testFile)
            ];
        });
        var services = container.BuildServiceProvider();
        services.UseScriptWatching(env.Path.GetDirectoryName(testFile));
        var scriptManager = services.GetRequiredService<IDynamicScriptManager>();
        var bundleManager = services.GetRequiredService<IScriptBundleManager>();

        Assert.False(scriptManager.IsRegistered("Bundle.Test"));
        bundleManager.GetScriptBundle("~/" + env.Path.GetFileName(testFile));
        var before = scriptManager.GetScriptText("Bundle.Test");
        Assert.Equal("before", before?.Replace(";", "").Trim());

        env.File.WriteAllText(testFile, "after");
        fileWatcherFactory.Watchers.Single().RaiseChanged("test.js");

        bundleManager.GetScriptBundle("~/" + env.Path.GetFileName(testFile));
        var after = scriptManager.GetScriptText("Bundle.Test");
        Assert.Equal("after", after?.Replace(";", "").Trim());
    }

    [Fact]
    public void Bundle_Is_Registered_Lazily_On_Demand()
    {
        var env = new MockHostEnvironment();
        var testFile = env.Path.Combine(env.WebRootPath, "test2.js");
        env.AddWebFile(testFile, "lazy");
        var container = new ServiceCollection();
        container.AddSingleton<IWebHostEnvironment>(env);
        container.AddSingleton<IPermissionService, MockPermissions>();
        container.AddScriptBundling(options =>
        {
            options.Enabled = true;
            options.Bundles["Lazy"] = [ "~/" + env.Path.GetFileName(testFile) ];
        });
        var services = container.BuildServiceProvider();
        var scriptManager = services.GetRequiredService<IDynamicScriptManager>();
        var bundleManager = services.GetRequiredService<IScriptBundleManager>();

        Assert.False(scriptManager.IsRegistered("Bundle.Lazy"));
        var url = bundleManager.GetScriptBundle("~/" + env.Path.GetFileName(testFile));
        Assert.True(scriptManager.IsRegistered("Bundle.Lazy"));
        Assert.Contains("Bundle.Lazy.js?v=", url);
    }
}
