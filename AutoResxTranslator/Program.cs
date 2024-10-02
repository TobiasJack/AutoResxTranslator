using AutoResxTranslator.Extensions;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

/* 
 * AutoResxTranslator
 * by Salar Khalilzadeh
 * 
 * https://github.com/salarcode/AutoResxTranslator/
 * Mozilla Public License v2
 */
namespace AutoResxTranslator
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static async Task Main(string[] args)
    {
      var builder = Host.CreateDefaultBuilder(args);

      builder.UseWindowsFormsLifetime<frmMain>(config =>
      {
        config.EnableConsoleShutdown = true;
      }, ConfigurePreApplicationRunActions)
      .ConfigureServices((context, services) =>
      {
        services.AddTransient<GTranslateService>();
        services.AddTransient<MsTranslateService>();
        services.AddHttpClient();
      });

      var app = builder.Build();
      await app.RunAsync();


      //Application.EnableVisualStyles();
      //Application.SetCompatibleTextRenderingDefault(false);
      //Application.Run(new frmMain());
    }

    static void ConfigurePreApplicationRunActions(IServiceProvider serviceProvider) 
    {

    }
  }
}
