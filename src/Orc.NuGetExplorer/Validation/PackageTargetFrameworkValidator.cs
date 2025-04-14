namespace Orc.NuGetExplorer
{
    using System.Linq;
    using Catel;
    using Catel.Data;
    using NuGet.Frameworks;
    using NuGet.Protocol.Core.Types;

    public class PackageTargetFrameworkValidator : IPackageValidator
    {
        private readonly IDefaultNuGetFramework _defaultNuGetFramework;

        public PackageTargetFrameworkValidator(IDefaultNuGetFramework defaultNuGetFramework)
        {
            _defaultNuGetFramework = defaultNuGetFramework;

            // Exclude full fx by default
            ExcludeFullFramework = true;
        }

        public bool ExcludeFullFramework { get; set; }

        public IValidationContext Validate(IPackageSearchMetadata packageSearchMetadata)
        {
            var highestFramework = GetHighestNuGetFramework();

            var validationContext = new ValidationContext();

            // Check target framework, only install supported packages
            var dependencySets = packageSearchMetadata.DependencySets.ToArray();
            if (dependencySets.Length > 0)
            {
                if (!dependencySets.Any(x => x.TargetFramework.Framework.EqualsIgnoreCase(highestFramework.Framework) &&
                                             x.TargetFramework.Version <= highestFramework.Version))
                {
                    validationContext.Add(BusinessRuleValidationResult.CreateError($"Package is not compatible with the current target framework ({highestFramework})"));
                }
            }

            return validationContext;
        }

        protected virtual NuGetFramework GetHighestNuGetFramework()
        {
            // Fetch highest version, but we need to make sure that we only install supported
            // target frameworks (e.g. not .net 20 when we only have .net 10). We also need to
            // filter the full .NET Framework (e.g. 4.7.2)
            var source = _defaultNuGetFramework.GetHighest()
                .Where(x => true);

            if (ExcludeFullFramework)
            {
                source = source.Where(x => x.Framework != ".NETFramework");
            }

            var highestFramework = source.First();
            return highestFramework;
        }
    }
}
