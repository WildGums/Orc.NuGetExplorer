// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryNavigator.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using Catel.Collections;

    internal class RepositoryNavigator //(cannot be inherited from ModelBase)
    {
        #region Constructors
        public RepositoryNavigator()
        {
            RepoCategories = new FastObservableCollection<RepositoryCategory>();
        }
        #endregion

        #region Properties
        public IList<RepositoryCategory> RepoCategories { get; private set; }
        public NamedRepository SelectedNamedRepository { get; set; }
        public RepositoryCategory SelectedRepositoryCategory { get; set; }
        #endregion
    }
}