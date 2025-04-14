namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    public class DefaultPackageValidatorProvider : IPackageValidatorProvider
    {
        private readonly IDefaultNuGetFramework _defaultNuGetFramework;

        public DefaultPackageValidatorProvider(IDefaultNuGetFramework defaultNuGetFramework)
        {
            _defaultNuGetFramework = defaultNuGetFramework;
        }

        public virtual IReadOnlyList<IPackageValidator> GetValidators()
        {
            return new[]
            {
                new PackageTargetFrameworkValidator(_defaultNuGetFramework)
            };
        }
    }
}
