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

        //Basic rotation speed
        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register("Speed", typeof(double), typeof(RotationProgressBar), new PropertyMetadata(1d));

        public Path IconData
        {
            get { return (Path)GetValue(IconDataProperty); }
            set { SetValue(IconDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconDataProperty =
            DependencyProperty.Register("IconData", typeof(Path), typeof(RotationProgressBar), new PropertyMetadata());

        public bool IsInProgress
        {
            get { return (bool)GetValue(IsInProgressProperty); }
            protected set { SetValue(IsInProgressPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsInProgressPropertyKey =
            DependencyProperty.RegisterReadOnly("IsInProgress", typeof(bool), typeof(RotationProgressBar), new PropertyMetadata(false, OnIsInProgressChanged));

        private static void OnIsInProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Log.Debug($"Progress status changed: { ((bool)e.NewValue ? "activated" : "ended") }");
        }

        // Using a DependencyProperty as the backing store for IsInProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsInProgressProperty = IsInProgressPropertyKey.DependencyProperty;

        public bool Success
        {
            get { return (bool)GetValue(SuccessProperty); }
            set { SetValue(SuccessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Success.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SuccessProperty = DependencyProperty.Register("Success", typeof(bool), typeof(RotationProgressBar), new PropertyMetadata(false));
    }
}
