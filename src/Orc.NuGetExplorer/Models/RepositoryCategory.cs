// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryCategory.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Catel.Data;

    internal class RepositoryCategory : ModelBase
    {
        #region Fields
        private int _selectedIndex;
        #endregion

        #region Constructors
        public RepositoryCategory(RepositoryCategoryType category)
        {
            Name = Enum.GetName(typeof (RepositoryCategoryType), category);
            Repos = new ObservableCollection<NamedRepository>();
        }
        #endregion

        #region Properties
        public bool IsSelected { get; set; }
        public string Name { get; set; }
        public IList<NamedRepository> Repos { get; private set; }
        #endregion
    }
}