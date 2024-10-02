using System.Windows.Forms;

namespace AutoResxTranslator.Interfaces;
public interface IWindowsFormsSynchronizationContextProvider
{
  WindowsFormsSynchronizationContext SynchronizationContext { get; }
}