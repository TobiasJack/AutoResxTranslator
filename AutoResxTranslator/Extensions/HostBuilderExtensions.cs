using AutoResxTranslator.DIForms;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Windows.Forms;

namespace AutoResxTranslator.Extensions;
public static class HostBuilderExtensions
{
  public static IHostBuilder UseWindowsFormsLifetime<TStartForm>(this IHostBuilder hostBuilder, Action<WindowsFormsLifetimeOptions> configure = null, Action<IServiceProvider> preApplicationRunAction = null) where TStartForm : Form
  {
    return hostBuilder.ConfigureServices(delegate (HostBuilderContext hostContext, IServiceCollection services)
    {
      services.AddSingleton<TStartForm>().AddSingleton((IServiceProvider provider) => new ApplicationContext(provider.GetRequiredService<TStartForm>())).AddWindowsFormsLifetime(configure, preApplicationRunAction);
    });
  }

  public static IHostBuilder UseWindowsFormsLifetime<TAppContext>(this IHostBuilder hostBuilder, Func<TAppContext> applicationContextFactory = null, Action<WindowsFormsLifetimeOptions> configure = null, Action<IServiceProvider> preApplicationRunAction = null) where TAppContext : ApplicationContext
  {
    return hostBuilder.ConfigureServices(delegate (HostBuilderContext hostContext, IServiceCollection services)
    {
      if (applicationContextFactory != null)
      {
        services.AddSingleton((IServiceProvider provider) => applicationContextFactory());
      }
      else
      {
        services.AddSingleton<TAppContext>();
      }

      services.AddSingleton((Func<IServiceProvider, ApplicationContext>)((IServiceProvider provider) => provider.GetRequiredService<TAppContext>()));
      services.AddWindowsFormsLifetime(configure, preApplicationRunAction);
    });
  }

  public static IHostBuilder UseWindowsFormsLifetime<TAppContext, TStartForm>(this IHostBuilder hostBuilder, Func<TStartForm, TAppContext> applicationContextFactory, Action<WindowsFormsLifetimeOptions> configure = null, Action<IServiceProvider> preApplicationRunAction = null) where TAppContext : ApplicationContext where TStartForm : Form
  {
    return hostBuilder.ConfigureServices(delegate (HostBuilderContext hostContext, IServiceCollection services)
    {
      services.AddSingleton<TStartForm>();
      services.AddSingleton(delegate (IServiceProvider provider)
      {
        TStartForm requiredService = provider.GetRequiredService<TStartForm>();
        return applicationContextFactory(requiredService);
      });
      services.AddSingleton((Func<IServiceProvider, ApplicationContext>)((IServiceProvider provider) => provider.GetRequiredService<TAppContext>()));
      services.AddWindowsFormsLifetime(configure, preApplicationRunAction);
    });
  }
}