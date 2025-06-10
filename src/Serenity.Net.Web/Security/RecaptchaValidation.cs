using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Serenity.Web;

/// <summary>
/// Recaptcha validation extensions. This was written for a very old version
/// of Google Recaptcha and might not be working properly.
/// </summary>
public static class RecaptchaValidation
{
    /// <summary>
    /// Validates a recaptcha token
    /// </summary>
    /// <param name="secretKey">Secret key</param>
    /// <param name="token">Token</param>
    /// <param name="localizer">Text localizer</param>
    /// <remarks>Inspired from https://github.com/tanveery/recaptcha-net/blob/master/src/Recaptcha.Web/RecaptchaVerificationHelper.cs</remarks>
    public static void Validate(string secretKey, string token, ITextLocalizer localizer)
    {
        ValidateAsync(secretKey, token, localizer).GetAwaiter().GetResult();
    }

    public static async Task ValidateAsync(string secretKey, string token, ITextLocalizer localizer)
    {
        if (string.IsNullOrEmpty(token))
            throw new ValidationError("Recaptcha", localizer.Get("Validation.Recaptcha"));

        var values = new Dictionary<string, string>
        {
            ["secret"] = secretKey,
            ["response"] = token
        };

        var content = new FormUrlEncodedContent(values);
        using var httpClient = new HttpClient();
        using var response = await httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
        var responseJson = await response.Content.ReadAsStringAsync();

        var recaptchaResponse = JSON.ParseTolerant<RecaptchaResponse>(responseJson);
        if (recaptchaResponse == null ||
            !recaptchaResponse.Success)
        {
            throw new ValidationError("Recaptcha", localizer.Get("Validation.Recaptcha"));
        }
    }

    private class RecaptchaResponse
    {
        public bool Success { get; set; }
        [JsonPropertyName("error-codes")]
        [Newtonsoft.Json.JsonProperty("error-codes")]
        public string[] ErrorCodes { get; set; }
    }
}