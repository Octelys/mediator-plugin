using System.Collections.Generic;
using JetBrains.Application.UI.PopupLayout;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.Diagnostics;
using ReSharper.MediatorPlugin.Services.Libraries;
using ReSharper.MediatorPlugin.Services.Navigation;

namespace ReSharper.MediatorPlugin.Services.Find;

internal sealed class HandlerSelector : IHandlerSelector
{
    private readonly ILibraryAdaptor _libraryAdaptor;

    public HandlerSelector()
    {
        _libraryAdaptor = new LibraryAdaptor();
    }
    
    public bool IsMediatorRequestSupported
    (
        IIdentifier identifier
    )
    {
        return _libraryAdaptor.IsSupported(identifier);
    }
    
    public void NavigateToHandler
    (
        ISolution solution,
        ITreeNode selectedTreeNode,
        INavigationOptionsFactory navigationOptionsFactory
    )
    {
        if (selectedTreeNode is not IIdentifier selectedIdentifier)
        {
            Logger.Instance.Log(LoggingLevel.VERBOSE, $"Selected element is not an instance {nameof(IIdentifier)}");
            return;
        }

        var handlers = new List<IDeclaredElement>(_libraryAdaptor.FindHandlers(selectedIdentifier));

        if (handlers.Count == 0)
            return;

        //  Get DeclaredElementNavigationService
        var navigationService = solution.GetComponent<DeclaredElementNavigationService>();

        //  Create popup context (this may need to be adapted to your UI context)
        PopupWindowContextSource popupContext = navigationOptionsFactory
            .Get("Which handler do you want to navigate to?")
            .PopupWindowContextSource;

        // Show popup to select handler
        navigationService.ExecuteCandidates
        (
            handlers,
            popupContext,
            true
        );
    }
}