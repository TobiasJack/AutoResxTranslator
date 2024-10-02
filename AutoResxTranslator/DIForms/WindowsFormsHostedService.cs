using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoResxTranslator.DIForms;
public class WindowsFormsHostedService : IHostedService, IDisposable
{
  private CancellationTokenRegistration _applicationStoppingRegistration;

  private readonly WindowsFormsLifetimeOptions _options;

  private readonly IHostApplicationLifetime _hostApplicationLifetime;

  private readonly IServiceProvider _serviceProvider;

  private readonly WindowsFormsSynchronizationContextProvider _syncContextManager;

  private const int SW_HIDE = 0;

  private const int SW_SHOW = 5;

  public Action<IServiceProvider> PreApplicationRunAction { get; private set; }

  public Func<Task<bool>> CanRunApplication { get; private set; }

  [DllImport("kernel32.dll")]
  private static extern nint GetConsoleWindow();

  [DllImport("user32.dll")]
  private static extern bool ShowWindow(nint hWnd, int nCmdShow);

  public WindowsFormsHostedService(IOptions<WindowsFormsLifetimeOptions> options, IHostApplicationLifetime hostApplicationLifetime, IServiceProvider serviceProvider, WindowsFormsSynchronizationContextProvider syncContextManager, Action<IServiceProvider> preApplicationRunAction, Func<Task<bool>> canRunApplication)
  {
    _options = options.Value;
    _hostApplicationLifetime = hostApplicationLifetime;
    _serviceProvider = serviceProvider;
    _syncContextManager = syncContextManager;
    PreApplicationRunAction = preApplicationRunAction;
    CanRunApplication = canRunApplication;
  }

  public Task StartAsync(CancellationToken cancellationToken)
  {
    _applicationStoppingRegistration = _hostApplicationLifetime.ApplicationStopping.Register(delegate (object? state)
    {
      ((WindowsFormsHostedService)state).OnApplicationStopping();
    }, this);
    Thread thread = new Thread(startUiThreadAsync);
    thread.Name = "WindowsFormsLifetime UI Thread";
    thread.SetApartmentState(ApartmentState.STA);
    thread.Start();
    return Task.CompletedTask;
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }

  private async void startUiThreadAsync()
  {
    Application.SetHighDpiMode(_options.HighDpiMode);
    if (_options.EnableVisualStyles)
    {
      Application.EnableVisualStyles();
    }

    Application.SetCompatibleTextRenderingDefault(_options.CompatibleTextRenderingDefault);
    Application.ApplicationExit += OnApplicationExit;
    WindowsFormsSynchronizationContext.AutoInstall = false;
    _syncContextManager.SynchronizationContext = new WindowsFormsSynchronizationContext();
    SynchronizationContext.SetSynchronizationContext(_syncContextManager.SynchronizationContext);
    ApplicationContext applicationContext = _serviceProvider.GetService<ApplicationContext>();
    if (await ((CanRunApplication != null) ? CanRunApplication() : Task.FromResult(result: true)))
    {
      PreApplicationRunAction?.Invoke(_serviceProvider);
      Application.Run(applicationContext);
    }
  }

  private void OnApplicationStopping()
  {
    ApplicationContext applicationContext = _serviceProvider?.GetService<ApplicationContext>();
    Form form = applicationContext.MainForm;
    if (form != null && form.IsHandleCreated)
    {
      form.Invoke(delegate
      {
        form.Close();
        form.Dispose();
      });
    }
  }

  private void OnApplicationExit(object sender, EventArgs e)
  {
    _hostApplicationLifetime.StopApplication();
  }

  public void Dispose()
  {
    Application.ApplicationExit -= OnApplicationExit;
    _applicationStoppingRegistration.Dispose();
  }
}