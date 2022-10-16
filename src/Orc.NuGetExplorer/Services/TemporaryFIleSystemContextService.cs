namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using Catel.IoC;

    internal class TemporaryFIleSystemContextService : ITemporaryFIleSystemContextService
    {
        private readonly ITypeFactory _typeFactory;

        public TemporaryFIleSystemContextService(ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => typeFactory);

            _typeFactory = typeFactory;
        }

        public ITemporaryFileSystemContext? Context { get; private set; }

        public IDisposable UseTemporaryFIleSystemContext()
        {
            using (var context = _typeFactory.CreateRequiredInstance<TemporaryFileSystemContext>())
            {
                return new DisposableToken<ITemporaryFileSystemContext>(context, token => { }, token => { });
            }
        }
    }
}
