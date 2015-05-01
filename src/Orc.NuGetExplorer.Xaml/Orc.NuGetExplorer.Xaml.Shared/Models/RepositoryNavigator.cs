// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryNavigator.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.ComponentModel;
    using Catel.Data;

    internal class RepositoryNavigator : ModelBase
    {
        #region Constructors
        public RepositoryNavigator()
        {
            // used BindingList because it implements IRaiseItemChangedEvents
            RepositoryCategories = new BindingList<RepositoryCategory>();
            
        }
        #endregion

        public bool Initialized { get; private set; }

        public void Initialize()
        {
            RepositoryCategories.ListChanged += OnRepositoryCategoriesChanged;

            Initialized = true;
        }

        #region Properties
        public BindingList<RepositoryCategory> RepositoryCategories { get; private set; }
        public IRepository SelectedRepository { get; set; }
        public RepositoryCategory SelectedRepositoryCategory { get; set; }
        #endregion

        #region Methods
        private void OnRepositoryCategoriesChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType != ListChangedType.ItemChanged || e.PropertyDescriptor.Name != "IsSelected")
            {
                return;
            }

            var repositoryCategory = RepositoryCategories[e.NewIndex];
            if (repositoryCategory.IsSelected)
            {
                SelectedRepositoryCategory = repositoryCategory;
            }
        }
        #endregion
    }
}