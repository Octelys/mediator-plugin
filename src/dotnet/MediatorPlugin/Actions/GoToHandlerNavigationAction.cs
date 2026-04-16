using JetBrains.Application.DataContext;
using JetBrains.Application.Shortcuts.ShortcutManager;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.MediatorPlugin.Diagnostics;
using ReSharper.MediatorPlugin.Services.Find;
using ReSharper.MediatorPlugin.Services.Navigation;

namespace ReSharper.MediatorPlugin.Actions;

#pragma warning disable CS0612
[Action("GoToHandlerAction", "Go to Handler", ShortcutScope = ShortcutScope.TextEditor, DefaultShortcutText = "Alt+H")]
#pragma warning restore CS0612
public class GoToHandlerNavigationAction : IExecutableAction
{
    private readonly IHandlerSelector _handlerSelector = new HandlerSelector();

    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
        var selectedTreeNode = context.GetSelectedTreeNode<ITreeNode>();

        if (selectedTreeNode is not IIdentifier identifier)
            return nextUpdate.Invoke();

        return _handlerSelector.IsMediatorRequestSupported(identifier);
    }

    public void Execute(IDataContext context, DelegateExecute nextExecute)
    {
        Logger.Instance.Log(LoggingLevel.INFO, "GoToHandlerNavigationAction.Execute called");

        var solution = context.GetComponent<ISolution>();
        var selectedTreeNode = context.GetSelectedTreeNode<ITreeNode>();

        if (selectedTreeNode is not IIdentifier)
        {
            Logger.Instance.Log(LoggingLevel.VERBOSE, "Selected element is not an identifier");
            return;
        }

        _handlerSelector.NavigateToHandler
        (
            solution,
            selectedTreeNode,
            new DataContextNavigationOptionsFactory(context)
        );
    }
}
