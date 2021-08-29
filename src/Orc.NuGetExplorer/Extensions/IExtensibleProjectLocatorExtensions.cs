namespace Orc.NuGetExplorer
{
    using System.Linq;
    using Catel;
    using Orc.NuGetExplorer.Management;

    public static class IExtensibleProjectLocatorExtensions
    {
        public static IExtensibleProject GetDefaultProject(this IExtensibleProjectLocator extensibleProjectLocator)
        {
            Argument.IsNotNull(() => extensibleProjectLocator);
            return extensibleProjectLocator.GetAllExtensibleProjects().FirstOrDefault();
        }
    }
}
