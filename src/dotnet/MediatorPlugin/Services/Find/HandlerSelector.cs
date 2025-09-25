using JetBrains.Application.Threading;
using JetBrains.Application.UI.PopupLayout;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using ReSharper.MediatorPlugin.Diagnostics;
using ReSharper.MediatorPlugin.Services.Libraries;
using ReSharper.MediatorPlugin.Services.Navigation;
using System.Collections.Generic;
using JetBrains.TextControl.CodeWithMe;

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

        var handlers = new List<IDeclaredElement>
        (
            _libraryAdaptor.FindHandlers(selectedIdentifier)
        );

        if (handlers.Count == 0)
            return;

        var navigationService = solution.GetComponent<DeclaredElementNavigationService>();
        var locks = solution.GetComponent<IShellLocks>();
        var textControlManager = solution.GetComponent<ITextControlManager>();

        // Always marshal to UI thread for popup creation in ReSharper
        locks.ExecuteOrQueue
        (
            "ShowMediatorHandlersPopup",
            () =>
            {
                // Try to anchor popup to current text control (caret). This is important in VS.
                ITextControl? focusedTextControl = textControlManager.FocusedTextControlPerClient.ForCurrentClient();

                PopupWindowContextSource popupSource;

                if (focusedTextControl is not null)
                {
                    popupSource = focusedTextControl.PopupWindowContextFactory.ForCaret();
                }
                else
                {
                    // Fallback (less ideal, but ensures a context); you can craft another source if needed.
                    popupSource = navigationOptionsFactory.Get("Select handler").PopupWindowContextSource;
                }

                navigationService.ExecuteCandidates
                (
                    handlers,
                    popupSource,
                    false
                );
            }
        );
    }
}