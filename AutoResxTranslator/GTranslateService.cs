using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

/* 
 * AutoResxTranslator
 * by Salar Khalilzadeh
 * 
 * https://github.com/salarcode/AutoResxTranslator/
 * Mozilla Public License v2
 */
namespace AutoResxTranslator;

public class GTranslateService : BaseTranslateService
{
  private const string RequestGoogleUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:91.0) Gecko/20100101 Firefox/91.0";
  private const string RequestGoogleTranslatorUrl = "https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&hl=en&dt=t&dt=bd&dj=1&source=icon&tk=467103.467103&q={2}";

  public GTranslateService(HttpClient client) : base(client)
  {
    _httpClient.Timeout = TimeSpan.FromMilliseconds(50 * 1000);
  }

  public override string ServiceTranslateURL => RequestGoogleTranslatorUrl;
  public override string RequestUserAgent => RequestGoogleUserAgent;

  /// <summary>
  ///  the main trick :)
  /// </summary>
  public override async Task<(bool succeed, string output)> ReadTranslatedResultAsync(Stream rawdata)
  {
    string text;
    string result;

    using (var reader = new StreamReader(rawdata, Encoding.UTF8))
    {
      text = await reader.ReadToEndAsync();
    }

    try
    {
      dynamic obj = SimpleJson.DeserializeObject(text);

      var final = "";

      // the number of lines
      int lines = obj[0].Count;
      for (int i = 0; i < lines; i++)
      {
        // the translated text.
        final += (obj[0][i][0]).ToString();
      }
      result = final;
      return (true, result);
    }
    catch (Exception ex)
    {
      result = ex.Message;
      return (false, result);
    }
  }
}
