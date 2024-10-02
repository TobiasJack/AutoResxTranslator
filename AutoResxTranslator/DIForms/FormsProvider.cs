using AutoResxTranslator.Extensions;
using AutoResxTranslator.Interfaces;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoResxTranslator.DIForms;
public class FormProvider : IFormProvider
{
  private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

  private readonly IServiceProvider _serviceProvider;

  private readonly IWindowsFormsSynchronizationContextProvider _syncContextManager;

  public FormProvider(IServiceProvider serviceProvider, IWindowsFormsSynchronizationContextProvider syncContextManager)
  {
    _serviceProvider = serviceProvider;
    _syncContextManager = syncContextManager;
  }

  public async Task<T> GetFormAsync<T>() where T : Form
  {
    await _semaphore.WaitAsync();
    T form = await _syncContextManager.SynchronizationContext.InvokeAsync(() => _serviceProvider.GetService<T>());
    _semaphore.Release();
    return form;
  }

  public async Task<Form> GetFormAsync(Type type)
  {
    await _semaphore.WaitAsync();
    object form = await _syncContextManager.SynchronizationContext.InvokeAsync(() => _serviceProvider.GetService(type));
    _semaphore.Release();
    return form as Form;
  }

  public Task<Form> GetMainFormAsync()
  {
    ApplicationContext service = _serviceProvider.GetService<ApplicationContext>();
    return Task.FromResult(service.MainForm);
  }
}