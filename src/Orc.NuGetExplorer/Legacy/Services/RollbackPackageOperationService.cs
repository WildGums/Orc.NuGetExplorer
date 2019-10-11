// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RollbackPackageOperationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class RollbackPackageOperationService : IRollbackPackageOperationService
    {
        #region Fields
        private readonly IDictionary<IPackageOperationContext, Stack<Action>> _rollbackActions = new Dictionary<IPackageOperationContext, Stack<Action>>();
        #endregion

        #region Methods
        public void PushRollbackAction(Action rollbackAction, IPackageOperationContext context)
        {
            Stack<Action> stack;
            if (!_rollbackActions.TryGetValue(context, out stack))
            {
                stack = new Stack<Action>();
                _rollbackActions.Add(context, stack);
            }

            stack.Push(rollbackAction);
        }

        public void Rollback(IPackageOperationContext context)
        {
            Stack<Action> stack;
            if (_rollbackActions.TryGetValue(context, out stack))
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
            Stack<Action> stack;
            if (_rollbackActions.TryGetValue(context, out stack))
            {
                stack.Clear();
                _rollbackActions.Remove(context);
            }
        }
        #endregion
    }
}