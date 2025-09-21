using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.ReSharper.Psi.Tree;

namespace ReSharper.MediatorPlugin.Services.Libraries;

internal abstract class Library : ILibrary
{
    private static readonly List<ITypeElement> EmptyTypeElements = new List<ITypeElement>();
    
    private readonly string _moduleName;
    
    private readonly string _requestClrName;
    private readonly string _notificationClrName;

    private readonly List<string> _requestHandlerClrNames;
    private readonly List<string> _notificationHandlerClrNames;

    private List<ITypeElement> _requestTypeElements = [];
    private List<ITypeElement> _notificationTypeElements = [];

    protected Library
    (
        string moduleName,
        string requestClrName,
        string notificationClrName,
        List<string> requestHandlerClrNames,
        List<string> notificationHandlerClrNames
    )
    {
        _moduleName = moduleName;

        _requestClrName = requestClrName;
        _notificationClrName = notificationClrName;
        
        _requestHandlerClrNames = requestHandlerClrNames;
        _notificationHandlerClrNames = notificationHandlerClrNames;
    }

    public IEnumerable<ITypeElement> FindHandlers
    (
        IIdentifier identifier
    )
    {
        InitializeTypeElementsIfNeeded(identifier);
        
        if (IsRequest(identifier))
        {
            return identifier.FindHandlers
            (
                _requestTypeElements
            );
        }
        
        if (IsNotification(identifier))
        {
            return identifier.FindHandlers
            (
                _notificationTypeElements
            );
        }

        return EmptyTypeElements;
    }
    
    public bool IsSupported
    (
        IIdentifier identifier
    )
    {
        return IsRequest(identifier) || IsNotification(identifier);
    }

    public abstract IClassLikeDeclaration CreateHandlrFor
    (
        IIdentifier identifier
    );
    
    private void InitializeTypeElementsIfNeeded
    (
        IIdentifier identifier
    )
    {
        if (_requestTypeElements.Count > 0 || _notificationTypeElements.Count > 0)
            return;
        
        //  Need to get the PSI 
        IPsiServices psiServices = identifier.GetPsiServices();
        
        var psiModules = psiServices.GetComponent<IPsiModules>();
        IAssemblyPsiModule? mediatrPsiModule = psiModules.GetAssemblyModules().FirstOrDefault(psiModule => psiModule.Name == _moduleName);

        if (mediatrPsiModule is null)
            return;
        
        ISymbolScope symbolScope = psiServices.Symbols.GetSymbolScope(mediatrPsiModule, true, false);

        LoadTypeElementsFromSymbolScope
        (
            symbolScope,
            _requestHandlerClrNames,
            _requestTypeElements
        );
        
        LoadTypeElementsFromSymbolScope
        (
            symbolScope,
            _notificationHandlerClrNames,
            _notificationTypeElements
        );
    }
    
    private static void LoadTypeElementsFromSymbolScope
    (
        ISymbolScope symbolScope,
        IReadOnlyCollection<string> typeClrNames,
        List<ITypeElement> typeElements
    )
    {
        foreach (var notificationHandlerClrName in typeClrNames)
        {
            ITypeElement? typeElement = symbolScope.GetTypeElementByCLRName(notificationHandlerClrName);
            
            if(typeElement is null)
                continue;
            
            typeElements.Add(typeElement);
        }
    }
    
    private bool IsRequest
    (
        IIdentifier identifier
    )
    {
        return identifier.IsRequestTypeSupported
        (
            _moduleName,
            _requestClrName
        );
    }

    private bool IsNotification
    (
        IIdentifier identifier
    )
    {
        return identifier.IsRequestTypeSupported
        (
            _moduleName,
            _notificationClrName
        );
    }
}