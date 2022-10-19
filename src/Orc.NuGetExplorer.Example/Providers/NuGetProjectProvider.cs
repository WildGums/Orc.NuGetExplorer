namespace Orc.NuGetExplorer.Example.Providers
{
    using System;
    using Catel.IoC;
    using NuGet.Frameworks;
    using Orc.NuGetExplorer.Management;

    public class NuGetProjectProvider : IDefaultExtensibleProjectProvider
    {
        private readonly IExtensibleProjectLocator _extensibleProjectLocator;

        private readonly IExtensibleProject _defaultProject;

        public NuGetProjectProvider(IExtensibleProjectLocator extensibleProjectLocator, ITypeFactory typeFactory)
        {
            ArgumentNullException.ThrowIfNull(extensibleProjectLocator);
            ArgumentNullException.ThrowIfNull(typeFactory);

            _extensibleProjectLocator = extensibleProjectLocator;

            _defaultProject = typeFactory.CreateInstanceWithParametersAndAutoCompletion<ExampleProject>();

            _extensibleProjectLocator.Register(_defaultProject);
            _extensibleProjectLocator.Enable(_defaultProject);
        }

        public IExtensibleProject GetDefaultProject()
        {
            return _defaultProject;
        }
    }
}
