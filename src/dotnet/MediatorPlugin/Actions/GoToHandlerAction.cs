using System;
using JetBrains.Application.Progress;
using JetBrains.Application.UI.PopupLayout;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using ReSharper.MediatorPlugin.Diagnostics;
using ReSharper.MediatorPlugin.Services.Find;
using ReSharper.MediatorPlugin.Services.Navigation;

namespace ReSharper.MediatorPlugin.Actions;

[ContextAction
(
    Name = "Go to Handler",
    Description = "Navigate to the Mediator handler for the selected request",
    GroupType = typeof(CSharpContextActions),
    Disabled = false,
    Priority = 1
)]
public sealed class GoToHandlerAction : ContextActionBase
{
    private readonly IHandlerSelector _handlerSelector;
    private readonly IIdentifier? _mediatrRequestIdentifier;
    private PopupWindowContextSource? _windowContext;

    public override string Text => "Go to Handler";

    public GoToHandlerAction(LanguageIndependentContextActionDataProvider dataProvider)
    {
        Guard.ThrowIfIsNull(dataProvider, nameof(dataProvider));
        
        Logger.Instance.Log(LoggingLevel.VERBOSE, "GoToHandlerAction instance has been created");

        _handlerSelector = new HandlerSelector();
        _mediatrRequestIdentifier = GetSelectedMediatrRequest(dataProvider);
    }

    public override bool IsAvailable(IUserDataHolder cache)
    {
        return _mediatrRequestIdentifier != null && 
               _handlerSelector.IsMediatorRequestSupported(_mediatrRequestIdentifier);
    }

    public override void Execute(ISolution solution, ITextControl textControl)
    {
        _windowContext = textControl.PopupWindowContextFactory.ForCaret();
        base.Execute(solution, textControl);
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        if (_windowContext == null)
        {
            Logger.Instance.Log(LoggingLevel.WARN, "Unable to retrieve the popup window context: navigation not possible");
            return DefaultActions.Empty;
        }

        if (_mediatrRequestIdentifier == null)
        {
            Logger.Instance.Log(LoggingLevel.VERBOSE, "Selected element is not a supported Mediator request");
            return DefaultActions.Empty;
        }

        Logger.Instance.Log(LoggingLevel.INFO, "Navigating to one of the found handlers");

        _handlerSelector.NavigateToHandler
        (
            solution,
            _mediatrRequestIdentifier,
            new PopupWindowContextSourceNavigationOptionsFactory(_windowContext)
        );

        return DefaultActions.Empty;
    }

    private static IIdentifier? GetSelectedMediatrRequest(LanguageIndependentContextActionDataProvider dataProvider)
    {
        var selectedTreeNode = dataProvider.GetSelectedTreeNode<ITreeNode>();
        
        if (selectedTreeNode is IIdentifier identifier)
        {
            Logger.Instance.Log(LoggingLevel.VERBOSE, $"Selected tree node is an identifier: {identifier.Name}");
            return identifier;
        }

        Logger.Instance.Log(LoggingLevel.VERBOSE, "Selected tree node is not an identifier");
        return null;
    }
}