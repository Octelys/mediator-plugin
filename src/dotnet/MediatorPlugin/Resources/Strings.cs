namespace ReSharper.MediatorPlugin.Resources;

/// <summary>
/// Texts referenced by name from attributes such as <c>[Action]</c>; ReSharper resolves them through
/// reflection, so each one must stay a public static property.
/// </summary>
public static class Strings
{
    public static string GoToHandlerActionText => "Go to Handler";
}
