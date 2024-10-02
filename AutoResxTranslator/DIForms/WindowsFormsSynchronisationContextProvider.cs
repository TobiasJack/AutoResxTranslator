using AutoResxTranslator.Extensions;
using AutoResxTranslator.Interfaces;

using Microsoft.AspNetCore.SignalR;

using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoResxTranslator.DIForms;
public class WindowsFormsSynchronizationContextProvider : IWindowsFormsSynchronizationContextProvider, IGUIContext
{
  public WindowsFormsSynchronizationContext SynchronizationContext { get; internal set; }

  public void Invoke(Action action)
  {
    SynchronizationContext.Invoke(action);
  }

  public TResult Invoke<TResult>(Func<TResult> func)
  {
    return SynchronizationContext.Invoke(func);
  }

  public async Task<TResult> InvokeAsync<TResult>(Func<TResult> func)
  {
    return await SynchronizationContext.InvokeAsync(func);
  }

  public async Task<TResult> InvokeAsync<TResult, TInput>(Func<TInput, TResult> func, TInput input)
  {
    return await SynchronizationContext.InvokeAsync(func, input);
  }
}