namespace Orc.NuGetExplorer.Windows
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Windows;

    public partial class DialogHost : Catel.Windows.DataWindow
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public DialogHost(IViewModel vm)
            : base(vm, DataWindowMode.Custom)
        {
            var dialogOptions = (ViewModel as IDialogViewModel)?.Dialog;

            if (dialogOptions.IsCloseButtonAvaialble)
            {
                var button = DataWindowButton.FromSync("Close", OnCloseExecute, OnCloseCanExecute);
                AddCustomButton(button);
            }


            foreach (var option in dialogOptions.Options)
            {
                CreateDataWindowButtonForDialog(option);
            }

            InitializeComponent();
        }

        protected override void Initialize()
        {
            SetCurrentValue(DialogCommandProperty, (ViewModel as IDialogViewModel)?.RunOption);

            base.Initialize();
        }

        private void CreateDataWindowButtonForDialog(IDialogOption option)
        {
            DataWindowButton btn = null;

            if (option.IsApplyBehavior)
            {
                btn = DataWindowButton.FromAsync(option.Caption, async () => await OnApplyExecuteAsync(), OnApplyCanExecute);

                btn.IsDefault = option.IsDefault;
            }

            if (option.IsOkBehavior)
            {
                btn = DataWindowButton.FromAsync(option.Caption, async () => await OnOkExecuteAsync(), OnOkCanExecute);

                btn.IsDefault = option.IsDefault;
            }

            if (option.IsCancelBehavior)
            {
                btn = DataWindowButton.FromAsync(option.Caption, async () => await OnCancelExecuteAsync(), OnCancelCanExecute);
                btn.IsCancel = true;
            }

            AddCustomButton(btn);
        }

        private async Task WrapDialogOptionAsync(Func<Task> commonButtonTask, IDialogOption dialogOption)
        {
            await OnExecuteWithDialogOptionActionAsync(dialogOption);
            await commonButtonTask();
        }

        private async Task OnExecuteWithDialogOptionActionAsync(IDialogOption dialogOption)
        {
            if (DialogCommand != null)
            {
                Log.Info("User pressed dialog option, executing command");

                // Not everyone is using the ICatelCommand, make sure to check if execution is allowed
                if (DialogCommand.CanExecute(dialogOption))
                {
                    DialogCommand.Execute(dialogOption);
                }
            }
        }


        public ICommand DialogCommand
        {
            get { return (ICommand)GetValue(DialogCommandProperty); }
            set { SetValue(DialogCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DialogCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DialogCommandProperty =
            DependencyProperty.Register("DialogCommand", typeof(ICommand), typeof(DialogHost), new PropertyMetadata(null));

    }
}
