using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http;

namespace Serenity.Web.EsBuild;

internal class EsBuildMinifier : ICssMinifier, IScriptMinifier
{
    private EsBuildCLI cli;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger<EsBuildMinifier> logger;

    public EsBuildMinifier(IHttpClientFactory httpClientFactory, ILogger<EsBuildMinifier> logger = null)
    {
        this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        this.logger = logger;
    }

    private EsBuildCLI GetCLI()
    {
        if (cli != null)
            return cli;

        var httpClient = httpClientFactory.CreateClient(nameof(EsBuildDownloader));
        var downloader = new EsBuildDownloader(httpClient: httpClient);
        var targetDirectory = Path.Combine(Path.GetTempPath(), ".esbuild");
        var executablePath = downloader.Download(targetDirectory: targetDirectory);
        return (cli = new EsBuildCLI(httpClientFactory, executablePath));
    }

    public CssMinifyResult MinifyCss(string source, CssMinifyOptions options)
    {
        try
        {
            return new CssMinifyResult
            {
                Code = GetCLI().MinifyCss(source, options.LineBreakThreshold == 0 ?
                    int.MaxValue - 1000 : options.LineBreakThreshold)
            };
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error minifying CSS");
            return new CssMinifyResult { Code = source, HasErrors = true };
        }
    }

    public ScriptMinifyResult MinifyScript(string source, ScriptMinifyOptions options)
    {
        try
        {
            return new ScriptMinifyResult
            {
                Code = GetCLI().MinifyScript(source, options.LineBreakThreshold == 0 ?
                    int.MaxValue - 1000 : options.LineBreakThreshold)
            };
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error minifying script");
            return new ScriptMinifyResult { Code = source, HasErrors = true };
        }
    }
}