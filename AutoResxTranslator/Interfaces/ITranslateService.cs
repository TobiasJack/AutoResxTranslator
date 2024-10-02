using System.IO;
using System.Threading.Tasks;

namespace AutoResxTranslator.Interfaces;
public interface ITranslateService
{
  public abstract string ServiceTranslateURL { get; }
  public abstract string RequestUserAgent { get; }

  Task<(bool succeed, string result)> TranslateAsync(string text, string sourceLng, string destLng, string textTranslatorUrlKey);

  Task<(bool succeed, string output)> ReadTranslatedResultAsync(Stream rawdata);
}
