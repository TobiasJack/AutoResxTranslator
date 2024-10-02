using System.Windows.Forms;

namespace AutoResxTranslator.DIForms;
public class WindowsFormsLifetimeOptions
{
  public HighDpiMode HighDpiMode { get; set; } = HighDpiMode.SystemAware;


  public bool EnableVisualStyles { get; set; } = true;


  public bool CompatibleTextRenderingDefault { get; set; }

  public bool SuppressStatusMessages { get; set; }

  public bool EnableConsoleShutdown { get; set; }
}
