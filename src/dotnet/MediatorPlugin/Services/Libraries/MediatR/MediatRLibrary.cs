using System.Collections.Generic;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.Services.Create;
using ReSharper.MediatorPlugin.Services.Libraries;

namespace ReSharper.MediatorPlugin.Services.MediatR;

internal sealed class MediatRLibrary : Library
{
    private const string MediatrModuleName = "MediatR";
    
    private const string MediatrBaseRequestFullyQualifiedName = $"{MediatrModuleName}.IBaseRequest";
    private const string MediatrNotificationFullyQualifiedName = $"{MediatrModuleName}.INotification";

    private static readonly List<string> MediatrRequestHandlerFullyQualifiedNames = [
        $"{MediatrModuleName}.IRequestHandler`1",
        $"{MediatrModuleName}.IRequestHandler`2"
    ];
    private static readonly List<string> MediatrNotificationHandlerFullyQualifiedNames = [
        $"{MediatrModuleName}.INotificationHandler`1",
    ];
    
    private readonly IHandlrCreator _handlrCreator;

    public MediatRLibrary() : base
    (
        MediatrModuleName,
        MediatrBaseRequestFullyQualifiedName,
        MediatrNotificationFullyQualifiedName,
        MediatrRequestHandlerFullyQualifiedNames,
        MediatrNotificationHandlerFullyQualifiedNames
    )
    {
        _handlrCreator = new HandlrCreator();
    }
    
    public override IClassLikeDeclaration CreateHandlrFor
    (
        IIdentifier identifier
    )
    {
        return _handlrCreator.CreateHandlrFor(identifier);
    } 
}