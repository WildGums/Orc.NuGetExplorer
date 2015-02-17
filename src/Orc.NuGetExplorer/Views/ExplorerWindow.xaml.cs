// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExplorerWindow.xaml.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Views
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Catel;
    using Catel.MVVM;
    using Catel.Windows;
    using Orc.NuGetExplorer.ViewModels;

    /// <summary>
    /// Interaction logic for ExplorerView.xaml.
    /// </summary>
    public partial class ExplorerView
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
            var vm = GetViewModel();
            if (vm != null)
            {
                InitializeSources(vm);

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

            var newTreeViewItem = (TreeViewItem)e.Source;
            var topItem = GetTopTreeViewItem(newTreeViewItem);

            foreach (TreeViewItem item in treeView.Items)
            {
                if (IsTopTreeViewItem(item) && !ReferenceEquals(newTreeViewItem, item))
                {
                    item.IsExpanded = false;

                    if (!ReferenceEquals(topItem, item))
                    {
                        foreach (TreeViewItem childItem in item.Items)
                        {
                            childItem.IsSelected = false;
                        }
                    }
                }
            }

            foreach (TreeViewItem childItem in topItem.Items)
            {
                if (!ReferenceEquals(childItem, newTreeViewItem))
                {
                    childItem.IsSelected = false;
                }
            }

            var previousSelection = vm.SelectedGroup;
            topItem.IsExpanded = true;
            topItem.IsSelected = false;

            var newSelection = topItem.Header as string;
            if (!string.Equals(previousSelection, newSelection))
            {
                vm.SelectedGroup = newSelection;
            }

            string newPackageSource = null;

            if (ReferenceEquals(newTreeViewItem, topItem))
            {
                // Select ALL
                var selectedPackageSourceItem = (TreeViewItem) newTreeViewItem.Items[0];
                selectedPackageSourceItem.IsSelected = true;
                newPackageSource = selectedPackageSourceItem.Header as string;
            }
            else
            {
                var selectedPackageSourceItem = newTreeViewItem;
                newPackageSource = selectedPackageSourceItem.Header as string;
            }

            if (!string.Equals(newPackageSource, vm.SelectedPackageSource))
            {
                vm.SelectedPackageSource = newPackageSource;
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

        private void InitializeSources(ExplorerViewModel vm)
        {
            var topItems = (from item in treeView.Items.Cast<TreeViewItem>()
                            where IsTopTreeViewItem(item)
                            select item);

            foreach (var topItem in topItems)
            {
                topItem.Items.Clear();

                var allItem = new TreeViewItem
                {
                    Header = "All"
                };

                allItem.Selected += OnTreeViewItemSelected;
                topItem.Items.Add(allItem);

                foreach (var packageSource in vm.AvailablePackageSources)
                {
                    var packageSourceItem = new TreeViewItem
                    {
                        Header = packageSource
                    };

                    packageSourceItem.Selected += OnTreeViewItemSelected;
                    topItem.Items.Add(packageSourceItem);
                }
            }
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