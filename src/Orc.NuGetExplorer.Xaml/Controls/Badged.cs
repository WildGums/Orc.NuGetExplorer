namespace Orc.NuGetExplorer.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    [TemplatePart(Name = BadgeContentPartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = BadgePartName, Type = typeof(FrameworkElement))]
    public class Badged : ContentControl
    {
        private const string BadgeContentPartName = "PART_BadgeContent";
        private const string BadgePartName = "PART_Badge";

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetTemplatePartVisibility(IsShowed);
        }

        public object? Badge
        {
            get { return GetValue(BadgeProperty); }
            set { SetValue(BadgeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Badge"/> 
        /// dependency property.</summary>
        public static readonly DependencyProperty BadgeProperty =
            DependencyProperty.Register(nameof(Badge), typeof(object), typeof(Badged), new FrameworkPropertyMetadata(null));

        public Brush? BadgeForeground
        {
            get { return (Brush?)GetValue(BadgeForegroundProperty); }
            set { SetValue(BadgeForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="BadgeForeground"/> 
        /// dependency property.</summary>
        public static readonly DependencyProperty BadgeForegroundProperty =
            DependencyProperty.Register(nameof(BadgeForeground), typeof(Brush), typeof(Badged), new PropertyMetadata(null));

        public bool IsShowed
        {
            get { return (bool)GetValue(IsShowedProperty); }
            set { SetValue(IsShowedProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsShowed"/> 
        /// dependency property.</summary>
        public static readonly DependencyProperty IsShowedProperty =
            DependencyProperty.Register(nameof(IsShowed), typeof(bool), typeof(Badged), new PropertyMetadata(true, (s, e) => OnIsShowedChanged(s, e)));

        public event DependencyPropertyChangedEventHandler? IsShowedChanged;

        private void RaiseIsShowedChanged(DependencyPropertyChangedEventArgs e)
        {
            IsShowedChanged?.Invoke(this, e);
        }

        private static void OnIsShowedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Badged badged)
            {
                badged.SetTemplatePartVisibility((bool)e.NewValue);

                badged.RaiseIsShowedChanged(e);
            }
        }

        private void SetTemplatePartVisibility(bool visibility)
        {
            var templateBadgeContent = GetTemplateChild(BadgeContentPartName);
            var templateBadge = GetTemplateChild(BadgePartName);

            templateBadgeContent?.SetCurrentValue(VisibilityProperty, this.ToVisibleOrHidden(visibility));
            templateBadge?.SetCurrentValue(VisibilityProperty, this.ToVisibleOrHidden(visibility));
        }
    }
}
