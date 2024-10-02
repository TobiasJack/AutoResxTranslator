using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoResxTranslator.Interfaces;
public interface IFormProvider
{
  Task<T> GetFormAsync<T>() where T : Form;

  Task<Form> GetMainFormAsync();

  Task<Form> GetFormAsync(Type type);
}
