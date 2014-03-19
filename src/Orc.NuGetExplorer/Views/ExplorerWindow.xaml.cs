// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExplorerWindow.xaml.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using Catel;
    using Catel.Windows;
    using Orc.NuGetExplorer.ViewModels;

    /// <summary>
    /// Interaction logic for ExplorerView.xaml.
    /// </summary>
    public partial class ExplorerView : DataWindow
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ExplorerView"/> class.
        /// </summary>
        public ExplorerView()
            : base(DataWindowMode.Close)
        {
            InitializeComponent();
        }
        #endregion

        protected override void OnViewModelChanged()
        {
            if (ViewModel != null)
            {
                SelectCategory("online");
            }
        }

        private ExplorerViewModel GetViewModel()
        {
            return ViewModel as ExplorerViewModel;
        }

        private void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            var vm = GetViewModel();
            if (vm == null)
            {
                return;
            }

            foreach (TreeViewItem item in treeView.Items)
            {
                if (IsTopTreeViewItem(item))
                {
                    item.IsExpanded = false;
                }
            }

            var previousSelection = vm.SelectedGroup;

            var selectedItem = (TreeViewItem)e.Source;
            var topItem = GetTopTreeViewItem(selectedItem);

            topItem.IsExpanded = true;
            var newSelection = topItem.Header as string;

            if (!string.Equals(previousSelection, newSelection))
            {
                vm.SelectedGroup = newSelection;
            }
        }

        private TreeViewItem GetTopTreeViewItem(TreeViewItem item)
        {
            var parentTreeViewItem = item.Parent as TreeViewItem;
            if (parentTreeViewItem == null)
            {
                return item;
            }

            return GetTopTreeViewItem(parentTreeViewItem);
        }

        private bool IsTopTreeViewItem(TreeViewItem item)
        {
            var parentTreeViewItem = item.Parent as TreeViewItem;
            if (parentTreeViewItem == null)
            {
                return true;
            }

            return false;
        }

        private void SelectCategory(string categoryName)
        {
            Argument.IsNotNullOrWhitespace(() => categoryName);

            int indexToSelect = -1;

            switch (categoryName.ToLower())
            {
                case "installed":
                    indexToSelect = 0;
                    break;

                case "online":
                    indexToSelect = 1;
                    break;

                case "updates":
                    indexToSelect = 2;
                    break;
            }

            if (indexToSelect >= 0)
            {
                var treeViewItem = (TreeViewItem) treeView.Items[indexToSelect];
                treeViewItem.IsSelected = true;
            }
        }
    }
}