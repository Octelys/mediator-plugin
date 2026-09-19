using System.Linq;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReSharper.MediatorPlugin.Diagnostics;

namespace ReSharper.MediatorPlugin.ReSharper.Psi.Tree;

internal static class MediatorCallSite
{
    public static IIdentifier? ResolveRequestIdentifier(ITreeNode? node)
    {
        if (node is null)
            return null;

        return ResolveFromEnclosingCall(node) ?? node as IIdentifier;
    }

    private static IIdentifier? ResolveFromEnclosingCall(ITreeNode node)
    {
        IInvocationExpression? invocation = node.GetContainingNode<IInvocationExpression>(returnThis: true);

        if (invocation is null)
        {
            Logger.Instance.Log(LoggingLevel.VERBOSE, "> Caret is not inside an invocation expression");
            return null;
        }

        foreach (ICSharpArgument argument in invocation.Arguments)
        {
            IIdentifier? identifier = ArgumentTypeDeclarationIdentifier(argument.Value);

            if (identifier is not null)
                return identifier;
        }

        Logger.Instance.Log(LoggingLevel.VERBOSE, $"> No invocation argument resolved to a navigable type declaration (method '{InvokedMethodName(invocation)}')");
        return null;
    }

    private static IIdentifier? ArgumentTypeDeclarationIdentifier(ICSharpExpression? argumentValue)
    {
        ITypeElement? typeElement = ResolveTypeElement(argumentValue);

        if (typeElement is null)
            return null;

        if (typeElement.GetDeclarations().FirstOrDefault() is not IClassLikeDeclaration declaration)
        {
            Logger.Instance.Log(LoggingLevel.VERBOSE, $"> Argument type '{typeElement.GetClrName().FullName}' has no source declaration to navigate from");
            return null;
        }

        Logger.Instance.Log(LoggingLevel.VERBOSE, $"> Resolved invocation argument to type '{declaration.DeclaredName}'");
        return declaration.NameIdentifier;
    }

    private static ITypeElement? ResolveTypeElement(ICSharpExpression? expression)
    {
        switch (expression)
        {
            case null:
                return null;

            case IObjectCreationExpression objectCreation
                when objectCreation.TypeReference?.Resolve().DeclaredElement is ITypeElement created:
                return created;

            default:
                return (expression.GetExpressionType().ToIType() as IDeclaredType)?.GetTypeElement();
        }
    }

    private static string InvokedMethodName(IInvocationExpression invocation)
    {
        return (invocation.InvokedExpression as IReferenceExpression)?.Reference.GetName() ?? "<unknown>";
    }
}
