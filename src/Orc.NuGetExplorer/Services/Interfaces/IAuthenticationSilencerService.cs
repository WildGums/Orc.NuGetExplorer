namespace Orc.NuGetExplorer
{
    using System;

    public interface IAuthenticationSilencerService
    {
        IDisposable UseAuthentication(bool authenticateIfRequired = true);
    }
}