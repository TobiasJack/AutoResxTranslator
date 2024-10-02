namespace AutoResxTranslator.Interfaces;

/// <summary>
/// Base Interface to declare a Dependency Injection Form
/// </summary>
public interface IDIForm
{
}

/// <summary>
/// Interface for a Dependency Injection Form with Parameters
/// </summary>
/// <typeparam name="TContainerType"></typeparam>
public interface IDIForm<TContainerType> : IDIForm where TContainerType : IFormParameterContainer
{
  void Show(TContainerType containerType);
}
