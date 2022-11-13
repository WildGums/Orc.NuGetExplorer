namespace Orc.NuGetExplorer.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Catel.Logging;

    /// <summary>
    /// This is subtype of button
    /// which can be binded to tabcontrol
    /// and act as one of his tabitems
    /// </summary>
    public class TabControllerButton : RadioButton
    {
        private LinkedList<TabControllerButton> _group = new();

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        static TabControllerButton()
        {
            //custom metadata for checked property
            var pmeta = new FrameworkPropertyMetadata(false, (s, e) => ((TabControllerButton)s).OnIsCheckedChanged(e));

            IsCheckedProperty.OverrideMetadata(typeof(TabControllerButton), pmeta);
        }

        public TabControllerButton()
        {

        }

        private void OnIsCheckedChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && _group is not null)
            {
                SelectTab();
            }
        }

        public TabControl? TabSource
        {
            get { return (TabControl?)GetValue(TabSourceProperty); }
            set { SetValue(TabSourceProperty, value); }
        }


        /// <summary>
        /// Identifies the <see cref="TabSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabSourceProperty =
            DependencyProperty.Register(nameof(TabSource), typeof(TabControl), typeof(TabControllerButton), new PropertyMetadata(null, OnTabSourceChanged));

        public TabControllerButton? Next
        {
            get { return (TabControllerButton?)GetValue(NextProperty); }
            set { SetValue(NextProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Next"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NextProperty =
            DependencyProperty.Register(nameof(Next), typeof(TabControllerButton), typeof(TabControllerButton),
                new PropertyMetadata(null, (s, e) => ((TabControllerButton)s).OnNextChanged()));

        public bool IsFirst
        {
            get { return (bool)GetValue(IsFirstProperty); }
            set { SetValue(IsFirstProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsFirst"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFirstProperty =
            DependencyProperty.Register(nameof(IsFirst), typeof(bool), typeof(TabControllerButton), new PropertyMetadata(false));

        private static void OnTabSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabControllerButton tabBtn)
            {
                foreach (var t in tabBtn._group)
                {
                    t.SetCurrentValue(TabControllerButton.TabSourceProperty, tabBtn.TabSource);
                    Log.Debug($"Tab source property was set for button {t.Name}, original sender is {tabBtn.Name}");
                }
            }
        }

        private void OnTabControllerButtonClicked(object sender, RoutedEventArgs e)
        {
            if (_group is not null)
            {
                SelectTab();
            }
        }

        private void OnNextChanged()
        {
            try
            {
                var nextButton = Next;
                if (nextButton is not null)
                {
                    nextButton.SetCurrentValue(TabSourceProperty, TabSource);
                    nextButton._group = _group;   //keep reference on sibling memeber's group

                    var current = _group.Find(this);


                    if (_group.Count == 0 && current is null)
                    {
                        _group.AddFirst(this);
                        current = _group.Find(this);
                    }

                    if (current is not null)
                    {
                        _group.AddAfter(current, nextButton);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private int MyIndex()
        {
            return _group.TakeWhile(node => node != this).Count();
        }

        private void SelectTab()
        {
            if (TabSource is not null)
            {
                var index = MyIndex();

                var i = 0;

                if (TabSource.ItemsSource is null)
                {
                    return;
                }

                foreach (var item in TabSource.ItemsSource)
                {
                    //try to get container from source
                    var tab = TabSource.ItemContainerGenerator.ContainerFromItem(item);

                    if (tab is not null)
                    {
                        tab.SetCurrentValue(TabItem.IsSelectedProperty, i == index);
                    }

                    i++;
                }
            }
        }
    }
}
