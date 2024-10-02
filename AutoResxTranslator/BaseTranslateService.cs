using AutoResxTranslator.Interfaces;

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace AutoResxTranslator;
public class BaseTranslateService : ITranslateService
{
  internal readonly HttpClient _httpClient;

  public BaseTranslateService(HttpClient client)
  {
    _httpClient = client;
  }

  public virtual string ServiceTranslateURL { get => throw new NotImplementedException(); }
  public virtual string RequestUserAgent { get => throw new NotImplementedException(); }

  public virtual Task<(bool succeed, string output)> ReadTranslatedResultAsync(Stream rawdata) => throw new NotImplementedException();

  public async Task<(bool succeed, string result)> TranslateAsync(string text, string sourceLng, string destLng, string textTranslatorUrlKey)
  {
    return await createWebRequestAsync(text, sourceLng, destLng, textTranslatorUrlKey);
  }

  async Task<(bool succeed, string result)> createWebRequestAsync(string text, string lngSourceCode, string lngDestinationCode, string textTranslatorUrlKey)
  {
    try
    {
      text = HttpUtility.UrlEncode(text);

      var url = string.Format(ServiceTranslateURL, lngSourceCode, lngDestinationCode, text);

      var request = new HttpRequestMessage(HttpMethod.Get, url);
      request.Headers.Add("User-Agent", RequestUserAgent);

      var response = await _httpClient.SendAsync(request);

      if (!response.IsSuccessStatusCode)
      {
        return (false, "Response is failed with code: " + response.StatusCode);
      }

      using var streaam = await response.Content.ReadAsStreamAsync();

      (var succeed, var output) = await ReadTranslatedResultAsync(streaam);

      return (succeed, output);
    }
    catch (Exception ex)
    {
      return (false, "Request failed.\r\n " + ex.Message);
    }
  }
}
