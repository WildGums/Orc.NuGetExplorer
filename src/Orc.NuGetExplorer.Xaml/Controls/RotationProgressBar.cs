namespace Orc.NuGetExplorer.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Shapes;
    using Catel.Logging;

    public class RotationProgressBar : ProgressBar
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public RotationProgressBar()
        {
            ValueChanged += OnRotationProgressBarValueChanged;
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SetCurrentValue(SuccessProperty, !(ShowWarning || ShowError));
        }

        private void OnRotationProgressBarValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            IsInProgress = e.NewValue > Minimum && e.NewValue < Maximum;
        }

        public double Speed
        {
            get { return (double)GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Speed"/> dependency property. Represents basic rotation speed
        /// </summary>
        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register(nameof(Speed), typeof(double), typeof(RotationProgressBar), new PropertyMetadata(1d));


        public Path IconData
        {
            get { return (Path)GetValue(IconDataProperty); }
            set { SetValue(IconDataProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IconData"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconDataProperty =
            DependencyProperty.Register(nameof(IconData), typeof(Path), typeof(RotationProgressBar), new PropertyMetadata());


        public bool IsInProgress
        {
            get { return (bool)GetValue(IsInProgressProperty); }
            protected set { SetValue(IsInProgressPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsInProgressPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(IsInProgress), typeof(bool), typeof(RotationProgressBar), new PropertyMetadata(false, (s, e) => (s as RotationProgressBar).OnIsInProgressChanged(e)));

        private void OnIsInProgressChanged(DependencyPropertyChangedEventArgs e)
        {
            Log.Debug($"Progress status changed: { ((bool)e.NewValue ? "activated" : "ended") }");
            SetCurrentValue(SuccessProperty, !(ShowWarning || ShowError));
        }

        /// <summary>
        /// Identifies the <see cref="IsInProgress"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInProgressProperty = IsInProgressPropertyKey.DependencyProperty;

        public bool Success
        {
            get { return (bool)GetValue(SuccessProperty); }
            set { SetValue(SuccessProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Success"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SuccessProperty = DependencyProperty.Register(nameof(Success), typeof(bool), typeof(RotationProgressBar), new PropertyMetadata(false));


        public bool ShowWarning
        {
            get { return (bool)GetValue(ShowWarningProperty); }
            set { SetValue(ShowWarningProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ShowWarning"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowWarningProperty =
            DependencyProperty.Register(nameof(ShowWarning), typeof(bool), typeof(RotationProgressBar), new PropertyMetadata(false, (s, e) => (s as RotationProgressBar).OnShowWarningChanged(e)));

        private void OnShowWarningChanged(DependencyPropertyChangedEventArgs e)
        {
            SetCurrentValue(SuccessProperty, !(ShowWarning || ShowError));
            Log.Debug($"Set RotationProgressBar status to {Success}");
        }


        public bool ShowError
        {
            get { return (bool)GetValue(ShowErrorProperty); }
            set { SetValue(ShowErrorProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ShowError"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowErrorProperty =
            DependencyProperty.Register(nameof(ShowError), typeof(bool), typeof(RotationProgressBar), new PropertyMetadata(false, (s, e) => (s as RotationProgressBar).OnShowErrorChanged(e)));

#pragma warning disable S4144 // Methods should not have identical implementations
        private void OnShowErrorChanged(DependencyPropertyChangedEventArgs e)
#pragma warning restore S4144 // Methods should not have identical implementations
        {
            SetCurrentValue(SuccessProperty, !(ShowWarning || ShowError));
            Log.Debug($"Set RotationProgressBar status to {Success}");
        }
    }
}
