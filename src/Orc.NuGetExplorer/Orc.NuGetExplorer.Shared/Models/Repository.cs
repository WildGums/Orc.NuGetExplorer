// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Repository.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{

    public class Repository : IRepository
    {
        #region Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public PackageOperationType OperationType { get; set; }
        #endregion
    }
}