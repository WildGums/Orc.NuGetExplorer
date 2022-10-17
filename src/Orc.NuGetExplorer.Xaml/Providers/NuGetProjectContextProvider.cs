﻿namespace Orc.NuGetExplorer.Providers
{
    using Catel.IoC;
    using NuGet.ProjectManagement;
    using Orc.NuGetExplorer.Management;


    public class NuGetProjectContextProvider : INuGetProjectContextProvider
    {
        private readonly ITypeFactory _typeFactory;

        public NuGetProjectContextProvider(ITypeFactory typeFactory)
        {
            _typeFactory = typeFactory;
        }

        public INuGetProjectContext GetProjectContext(FileConflictAction fileConflictAction)
        {
            var projectContext = _typeFactory.CreateRequiredInstanceWithParametersAndAutoCompletion<NuGetProjectContext>(fileConflictAction);

            return projectContext;
        }
    }
}
