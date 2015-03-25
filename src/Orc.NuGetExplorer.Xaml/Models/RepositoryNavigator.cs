// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryNavigator.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using Catel.Collections;
    using Catel.Data;

    internal class RepositoryNavigator : ModelBase
    {
        #region Constructors
        public RepositoryNavigator()
        {
            RepoCategories = new FastObservableCollection<RepositoryCategory>();
        }
        #endregion

        #region Properties
        public IList<RepositoryCategory> RepoCategories { get; private set; }
        public IRepository SelectedRepository { get; set; }
        public RepositoryCategory SelectedRepositoryCategory { get; set; }
        #endregion
    }
}