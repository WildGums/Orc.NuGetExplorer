// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RollbackPackageOperationService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class RollbackPackageOperationService : IRollbackPackageOperationService
    {
        #region Fields
        private readonly IDictionary<PackageOperationContext, Stack<Action>> _rollbackActions = new Dictionary<PackageOperationContext, Stack<Action>>();
        #endregion

        #region Methods
        public void PushRollbackAction(Action rollbackAction, PackageOperationContext context)
        {
            _rollbackActions[context].Push(rollbackAction);
        }

        public void Rollback(PackageOperationContext context)
        {
            Stack<Action> satck;
            if (_rollbackActions.TryGetValue(context, out satck))
            {
                while (satck.Any())
                {
                    var action = satck.Pop();
                    action();
                }
            }
        }

        public void ClearRollbackActions(PackageOperationContext context)
        {
            Stack<Action> satck;
            if (_rollbackActions.TryGetValue(context, out satck))
            {
                satck.Clear();
                _rollbackActions.Remove(context);
            }
        }
        #endregion
    }
}