using System.Collections.Generic;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.Services.Libraries;

namespace ReSharper.MediatorPlugin.Services.MediatR;

internal sealed class MediatorLibrary : Library
{
    private const string MediatorModuleName = "Mediator";
    
    private const string MediatorBaseRequestFullyQualifiedName = $"{MediatorModuleName}.IBaseRequest";
    private const string MediatorNotificationFullyQualifiedName = $"{MediatorModuleName}.INotification";

    private static readonly List<string> MediatorRequestHandlerFullyQualifiedNames = [
        $"{MediatorModuleName}.IRequestHandler`1",
        $"{MediatorModuleName}.IRequestHandler`2"
    ];
    private static readonly List<string> MediatorNotificationHandlerFullyQualifiedNames = [
        $"{MediatorModuleName}.INotificationHandler`1"
    ];
    
    public MediatorLibrary() : base
    (
        MediatorModuleName,
        MediatorBaseRequestFullyQualifiedName,
        MediatorNotificationFullyQualifiedName,
        MediatorRequestHandlerFullyQualifiedNames,
        MediatorNotificationHandlerFullyQualifiedNames
    )
    {
    }

    public override IClassLikeDeclaration CreateHandlrFor
    (
        IIdentifier identifier
    )
    {
        throw new System.NotImplementedException();
    }
}