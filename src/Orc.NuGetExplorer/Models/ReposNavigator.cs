// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReposNavigator.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using Catel.Collections;

    public class ReposNavigator //(cannot be inherited from ModelBase)
    {
        #region Constructors
        public ReposNavigator()
        {
            RepoCategories = new FastObservableCollection<RepoCategory>();
        }
        #endregion

        #region Properties
        public IList<RepoCategory> RepoCategories { get; private set; }
        public NamedRepository SelectedNamedRepository { get; set; }
        public RepoCategory SelectedRepositoryCategory { get; set; }
        #endregion
    }
}