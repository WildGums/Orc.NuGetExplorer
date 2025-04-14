namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    public interface IPackageValidatorProvider
    {
        IReadOnlyList<IPackageValidator> GetValidators();
    }
}
