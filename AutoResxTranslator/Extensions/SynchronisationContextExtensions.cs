using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutoResxTranslator.Extensions;
internal static class SynchronizationContextExtensions
{
  public static void Invoke(this SynchronizationContext context, Action action)
  {
    context.Send(delegate
    {
      action();
    }, null);
  }

  public static TResult Invoke<TResult>(this SynchronizationContext context, Func<TResult> func)
  {
    TResult result = default(TResult);
    context.Send(delegate
    {
      result = func();
    }, null);
    return result;
  }

  public static Task<TResult> InvokeAsync<TResult>(this SynchronizationContext context, Func<TResult> func)
  {
    TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
    context.Post(delegate
    {
      try
      {
        TResult result = func();
        tcs.SetResult(result);
      }
      catch (Exception exception)
      {
        tcs.SetException(exception);
      }
    }, null);
    return tcs.Task;
  }

  public static Task<TResult> InvokeAsync<TResult, TInput>(this SynchronizationContext context, Func<TInput, TResult> func, TInput input)
  {
    TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
    context.Post(delegate
    {
      try
      {
        TResult result = func(input);
        tcs.SetResult(result);
      }
      catch (Exception exception)
      {
        tcs.SetException(exception);
        throw;
      }
    }, null);
    return tcs.Task;
  }
}