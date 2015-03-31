// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryCategory.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Catel.Data;

    internal class RepositoryCategory : ModelBase
    {
        #region Constructors
        public RepositoryCategory()
        {
            Repositories = new ObservableCollection<IRepository>();
        }
        #endregion

        #region Properties
        public bool IsSelected { get; set; }
        public string Name { get; set; }
        public IList<IRepository> Repositories { get; private set; }
        #endregion
    }
}