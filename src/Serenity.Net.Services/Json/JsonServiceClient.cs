using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Serenity.Services;

/// <summary>
/// A JSON service client implementation
/// </summary>
/// <remarks>
/// Creates an instance of JsonServiceClient for the passed baseUrl
/// </remarks>
/// <param name="baseUrl">The base url</param>
public class JsonServiceClient
{
    /// <summary>
    /// Cookie container
    /// </summary>
    protected readonly CookieContainer cookies = new();

    private readonly HttpClient httpClient;

    public JsonServiceClient(string baseUrl)
    {
        BaseUrl = baseUrl;

        var handler = new HttpClientHandler
        {
            CookieContainer = cookies
        };

        httpClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromMinutes(10)
        };
    }

    /// <summary>
    /// Base url for the client
    /// </summary>
    protected string BaseUrl { get; set; }

    /// <summary>
    /// Post to JSON service
    /// </summary>
    /// <typeparam name="TResponse">The type of response expected</typeparam>
    /// <param name="relativeUrl">Relative url</param>
    /// <param name="request">Request object</param>
    /// <returns></returns>
    public virtual TResponse Post<TResponse>(string relativeUrl, object request)
        where TResponse : new()
    {
        return PostAsync<TResponse>(relativeUrl, request).GetAwaiter().GetResult();
    }

    public virtual async Task<TResponse> PostAsync<TResponse>(string relativeUrl, object request)
        where TResponse : new()
    {
        return await InternalPostAsync<TResponse>(relativeUrl, request);
    }

    /// <summary>
    /// Posts to a JSON service, internal version
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="relativeUrl">Relative url</param>
    /// <param name="request">The request object</param>
    /// <returns>The response</returns>
    /// <exception cref="ValidationError">Throws a validation error exception
    /// if the returned response contains a service error.</exception>
    protected TResponse InternalPost<TResponse>(string relativeUrl, object request)
        where TResponse : new()
    {
        return InternalPostAsync<TResponse>(relativeUrl, request).GetAwaiter().GetResult();
    }

    protected async Task<TResponse> InternalPostAsync<TResponse>(string relativeUrl, object request)
        where TResponse : new()
    {
        var url = UriHelper.Combine(BaseUrl, relativeUrl);
        var r = JSON.Stringify(request, writeNulls: true);
        using var content = new StringContent(r, Encoding.UTF8, "application/json");

        using var response = await httpClient.PostAsync(url, content);
        using var stream = await response.Content.ReadAsStreamAsync();
        using var sr = new StreamReader(stream);
        var rt = await sr.ReadToEndAsync();
        var resp = JSON.ParseTolerant<TResponse>(rt);

        if (resp is ServiceResponse serviceResponse &&
            serviceResponse.Error != null)
        {
            throw new ValidationError(
                serviceResponse.Error.Code,
                serviceResponse.Error.Arguments,
                serviceResponse.Error.Message);
        }

        return resp;
    }
}