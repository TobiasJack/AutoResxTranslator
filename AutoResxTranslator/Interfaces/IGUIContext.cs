using System;
using System.Threading.Tasks;

namespace AutoResxTranslator.Interfaces;
public interface IGUIContext
{
  void Invoke(Action action);

  TResult Invoke<TResult>(Func<TResult> action);

  Task<TResult> InvokeAsync<TResult>(Func<TResult> action);

  Task<TResult> InvokeAsync<TResult, TInput>(Func<TInput, TResult> action, TInput input);
}
