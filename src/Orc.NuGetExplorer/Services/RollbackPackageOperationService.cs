namespace Orc.NuGetExplorer;

using System;
using System.Collections.Generic;
using System.Linq;
using Catel.Logging;
using Microsoft.Extensions.Logging;

internal class RollbackPackageOperationService : IRollbackPackageOperationService
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(RollbackPackageOperationService));

    private readonly IDictionary<IPackageOperationContext, Stack<Action>> _rollbackActions = new Dictionary<IPackageOperationContext, Stack<Action>>();

    public void PushRollbackAction(Action rollbackAction, IPackageOperationContext? context)
    {
        if (context is null)
        {
            Logger.LogWarning("Current package operation context doesn't exist. Ignore rollback actions");
            return;
        }

        if (!_rollbackActions.TryGetValue(context, out var stack))
        {
            stack = new Stack<Action>();
            _rollbackActions.Add(context, stack);
        }

        stack.Push(rollbackAction);
    }

    public void Rollback(IPackageOperationContext context)
    {
        if (_rollbackActions.TryGetValue(context, out var stack))
        {
            while (stack.Any())
            {
                var action = stack.Pop();
                action();
            }
        }
    }

    public void ClearRollbackActions(IPackageOperationContext context)
    {
        if (context is null)
        {
            Logger.LogWarning("Current package operation context doesn't exist. Ignore rollback actions");
            return;
        }

        if (_rollbackActions.TryGetValue(context, out var stack))
        {
            stack.Clear();
            _rollbackActions.Remove(context);
        }
    }
}
