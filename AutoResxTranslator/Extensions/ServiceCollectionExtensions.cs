using AutoResxTranslator.DIForms;
using AutoResxTranslator.Interfaces;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using System;
using System.Threading.Tasks;

namespace AutoResxTranslator.Extensions;
public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddWindowsFormsLifetime(this IServiceCollection services, Action<WindowsFormsLifetimeOptions> configure, Action<IServiceProvider> preApplicationRunAction = null, Func<Task<bool>> canRunApplication = null)
  {
    services.AddSingleton<IHostLifetime, WindowsFormsLifetime>();
    services.AddHostedService(delegate (IServiceProvider sp)
    {
      IOptions<WindowsFormsLifetimeOptions> requiredService = sp.GetRequiredService<IOptions<WindowsFormsLifetimeOptions>>();
      IHostApplicationLifetime requiredService2 = sp.GetRequiredService<IHostApplicationLifetime>();
      WindowsFormsSynchronizationContextProvider requiredService3 = sp.GetRequiredService<WindowsFormsSynchronizationContextProvider>();
      return new WindowsFormsHostedService(requiredService, requiredService2, sp, requiredService3, preApplicationRunAction, canRunApplication);
    });
    services.Configure(configure ?? ((Action<WindowsFormsLifetimeOptions>)delegate
    {
      new WindowsFormsLifetimeOptions();
    }));
    services.AddSingleton<IFormProvider, FormProvider>();
    services.AddSingleton<WindowsFormsSynchronizationContextProvider>();
    services.AddSingleton((Func<IServiceProvider, IWindowsFormsSynchronizationContextProvider>)((IServiceProvider sp) => sp.GetRequiredService<WindowsFormsSynchronizationContextProvider>()));
    services.AddSingleton((Func<IServiceProvider, IGUIContext>)((IServiceProvider sp) => sp.GetRequiredService<WindowsFormsSynchronizationContextProvider>()));
    return services;
  }
}