namespace Orc.NuGetExplorer
{
    using System;

    public interface IRollbackPackageOperationService
    {
        void PushRollbackAction(Action rollbackAction, IPackageOperationContext? context);
        void Rollback(IPackageOperationContext context);
        void ClearRollbackActions(IPackageOperationContext context);
    }
}
