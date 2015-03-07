// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer.Example
{
    using Models;

    public class EchoService : IEchoService
    {
        private PackageManagementEcho _echo;
        public PackageManagementEcho GetPackageManagementEcho()
        {
            if (_echo == null)
            {
                _echo = new PackageManagementEcho();
            }

            return _echo;
        }
    }
}