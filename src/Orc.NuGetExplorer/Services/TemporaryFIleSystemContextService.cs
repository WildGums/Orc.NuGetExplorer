namespace Orc.NuGetExplorer;

using System;
using Catel;
using Microsoft.Extensions.DependencyInjection;

internal class TemporaryFIleSystemContextService : ITemporaryFIleSystemContextService
{
    private readonly IServiceProvider _serviceProvider;

    public TemporaryFIleSystemContextService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ITemporaryFileSystemContext? Context { get; private set; }

    public IDisposable UseTemporaryFIleSystemContext()
    {
        using (var context = ActivatorUtilities.CreateInstance<TemporaryFileSystemContext>(_serviceProvider))
        {
            return new DisposableToken<ITemporaryFileSystemContext>(context, token => { }, token => { });
        }
    }
}
