﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutoResxTranslator.DIForms;
public class WindowsFormsLifetime : IHostLifetime, IDisposable
{
  private CancellationTokenRegistration _applicationStartedRegistration;

  private CancellationTokenRegistration _applicationStoppingRegistration;

  private readonly WindowsFormsLifetimeOptions _options;

  private readonly IHostEnvironment _environment;

  private readonly IHostApplicationLifetime _applicationLifetime;

  private readonly ILogger _logger;

  public WindowsFormsLifetime(IOptions<WindowsFormsLifetimeOptions> options, IHostEnvironment environment, IHostApplicationLifetime applicationLifetime, ILoggerFactory loggerFactory)
  {
    _options = options?.Value ?? throw new ArgumentNullException("options");
    _environment = environment ?? throw new ArgumentNullException("environment");
    _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException("applicationLifetime");
    _logger = loggerFactory?.CreateLogger("Microsoft.Hosting.Lifetime") ?? throw new ArgumentNullException("loggerFactory");
  }

  public Task WaitForStartAsync(CancellationToken cancellationToken)
  {
    if (!_options.SuppressStatusMessages)
    {
      _applicationStartedRegistration = _applicationLifetime.ApplicationStarted.Register(delegate (object? state)
      {
        ((WindowsFormsLifetime)state).OnApplicationStarted();
      }, this);
      _applicationStoppingRegistration = _applicationLifetime.ApplicationStopping.Register(delegate (object? state)
      {
        ((WindowsFormsLifetime)state).OnApplicationStopping();
      }, this);
    }

    if (_options.EnableConsoleShutdown)
    {
      Console.CancelKeyPress += OnCancelKeyPress;
    }

    return Task.CompletedTask;
  }

  private void OnApplicationStarted()
  {
    _logger.LogInformation("Application started. Close the startup Form" + (_options.EnableConsoleShutdown ? " or press Ctrl+C" : string.Empty) + " to shut down.");
    _logger.LogInformation("Hosting enviroment: {envName}", _environment.EnvironmentName);
    _logger.LogInformation("Content root path: {contentRoot}", _environment.ContentRootPath);
  }

  private void OnApplicationStopping()
  {
    _logger.LogInformation("Application is shutting down...");
  }

  private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
  {
    e.Cancel = true;
    _applicationLifetime.StopApplication();
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }

  public void Dispose()
  {
    _applicationStartedRegistration.Dispose();
    _applicationStoppingRegistration.Dispose();
    if (_options.EnableConsoleShutdown)
    {
      Console.CancelKeyPress -= OnCancelKeyPress;
    }
  }
}