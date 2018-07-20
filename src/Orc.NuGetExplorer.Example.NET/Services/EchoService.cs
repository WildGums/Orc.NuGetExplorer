// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Example
{
    using Models;

    public class EchoService : IEchoService
    {
        #region Fields
        private PackageManagementEcho _echo;
        #endregion

        #region Methods
        public PackageManagementEcho GetPackageManagementEcho()
        {
            if (_echo == null)
            {
                _echo = new PackageManagementEcho();
            }

            return _echo;
        }
        #endregion
    }
}