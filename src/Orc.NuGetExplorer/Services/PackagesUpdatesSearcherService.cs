// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagesUpdatesSearcherService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Repositories;

    internal class PackagesUpdatesSearcherService : IPackagesUpdatesSearcherService
    {
        #region Fields
        private readonly IAuthenticationSilencerService _authenticationSilencerService;
        private readonly IPackageQueryService _packageQueryService;
        private readonly IPackageRepositoryService _packageRepositoryService;
        #endregion

        #region Constructors
        public PackagesUpdatesSearcherService(IPackageRepositoryService packageRepositoryService, IAuthenticationSilencerService authenticationSilencerService,
            IPackageQueryService packageQueryService)
        {
            Argument.IsNotNull(() => packageRepositoryService);
            Argument.IsNotNull(() => authenticationSilencerService);
            Argument.IsNotNull(() => packageQueryService);

            _packageRepositoryService = packageRepositoryService;
            _authenticationSilencerService = authenticationSilencerService;
            _packageQueryService = packageQueryService;
        }
        #endregion

        #region Methods
        public IEnumerable<IPackageDetails> SearchForUpdates(bool allowPrerelease = false, bool authenticateIfRequired = false)
        {
            using (_authenticationSilencerService.UseAuthentication(authenticateIfRequired))
            {
                var packageRepository = _packageRepositoryService.GetAggeregateUpdateRepository() as UpdateRepository;
                if (packageRepository == null)
                {
                    return Enumerable.Empty<IPackageDetails>();
                }
                packageRepository.AllowPrerelease = allowPrerelease;

                return _packageQueryService.GetPackages(packageRepository, allowPrerelease);
            }
        }
        #endregion
    }
}