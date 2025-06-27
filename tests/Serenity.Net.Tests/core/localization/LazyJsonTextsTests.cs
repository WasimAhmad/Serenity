using System.Text;
using Serenity.Localization;
using Serenity.TestUtils;

public class LazyJsonTextsTests
{
    private class TrackingFileSystem : MockFileSystem
    {
        public int ReadCount { get; private set; }

        public new string ReadAllText(string path, Encoding encoding = null)
        {
            ReadCount++;
            return base.ReadAllText(path, encoding);
        }
    }

    [Fact]
    public void JsonTexts_Loads_On_First_Access()
    {
        var fs = new TrackingFileSystem();
        fs.AddFile(@"/My/en.json", "{\"x\":\"1\"}");
        var registry = new LocalTextRegistry();

        JsonLocalTextRegistration.AddJsonTexts(registry, "/My", fs);

        Assert.Equal(0, fs.ReadCount);
        Assert.Equal("1", registry.TryGet("en", "x", pending: false));
        Assert.Equal(1, fs.ReadCount);
        Assert.Equal("1", registry.TryGet("en", "x", pending: false));
        Assert.Equal(1, fs.ReadCount);
    }

    [Fact]
    public void JsonTexts_Loads_Fallback_On_Demand()
    {
        var fs = new TrackingFileSystem();
        fs.AddFile(@"/My/en.json", "{\"x\":\"1\"}");
        fs.AddFile(@"/My/en-US.json", "{\"y\":\"2\"}");
        var registry = new LocalTextRegistry();

        JsonLocalTextRegistration.AddJsonTexts(registry, "/My", fs);

        Assert.Equal("1", registry.TryGet("en-US", "x", pending: false));
        Assert.Equal(2, fs.ReadCount);
        Assert.Equal("2", registry.TryGet("en-US", "y", pending: false));
        Assert.Equal(2, fs.ReadCount);
    }
}
