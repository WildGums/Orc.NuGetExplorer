namespace Orc.NuGetExplorer.Example.Providers
{
    using Catel;
    using Catel.IoC;
    using Orc.NuGetExplorer.Management;

    public class NuGetProjectProvider : IDefaultExtensibleProjectProvider
    {
        private readonly IExtensibleProjectLocator _extensibleProjectLocator;

        private IExtensibleProject _defaultProject;

        public NuGetProjectProvider(IExtensibleProjectLocator extensibleProjectLocator, ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => extensibleProjectLocator);
            Argument.IsNotNull(() => typeFactory);

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
