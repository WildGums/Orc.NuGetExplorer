namespace Orc.NuGetExplorer.Behaviors
{
    using Catel.MVVM;
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public class LoadItemsOnDropDownBehavior : Catel.Windows.Interactivity.BehaviorBase<ComboBox>
    {
        protected override void OnAssociatedObjectLoaded()
        {
            AssociatedObject.DropDownOpened += OnAssociatedObjectDropDownOpened;
        }

        private void OnAssociatedObjectDropDownOpened(object sender, EventArgs e)
        {
            ExecuteItemSourceInitializationCommand();
        }

        public Command Command
        {
            get { return (Command)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(Command), typeof(LoadItemsOnDropDownBehavior), new PropertyMetadata(null));

        protected void ExecuteItemSourceInitializationCommand()
        {
            using (SynchronizationContextScopeManager.OutOfContext())
            {
                Command?.Execute();
            }
        }

        private void ExecuteCommand()
        {
            Command?.Execute();
        }
    }
}
