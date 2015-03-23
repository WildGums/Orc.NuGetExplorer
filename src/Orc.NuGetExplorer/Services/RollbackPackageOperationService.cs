// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RollbackPackageOperationService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;

    internal class RollbackPackageOperationService : IRollbackPackageOperationService
    {
        private readonly Stack<Action> _rollbackActions = new Stack<Action>();

        public void PushRollbackAction(Action rollbackAction)
        {
            _rollbackActions.Push(rollbackAction);
        }
    }
}