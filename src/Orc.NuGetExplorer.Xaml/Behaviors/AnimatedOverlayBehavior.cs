namespace Orc.NuGetExplorer.Behaviors
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;
    using Catel.IoC;
    using Catel.Windows;
    using Catel.Windows.Interactivity;
    using Orc.NuGetExplorer.Windows;

    internal class AnimatedOverlayBehavior : BehaviorBase<DataWindow>
    {
        private Grid _topInternalGrid;
        private SizeChangedEventHandler _sizeHandler;
        private Storyboard _overlayStoryboard;

        private IAnimationService AnimationService { get; set; }

        #region Properties

        public Grid OverlayGrid
        {
            get { return (Grid)GetValue(OverlayGridProperty); }
            set { SetValue(OverlayGridProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="OverlayGrid"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OverlayGridProperty =
            DependencyProperty.Register(nameof(OverlayGrid), typeof(Grid), typeof(AnimatedOverlayBehavior), new PropertyMetadata(null, (s, e) => OnOverlayGridChanged(s, e)));

        private static void OnOverlayGridChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var behavior = s as AnimatedOverlayBehavior;

            if (behavior.IsAssociatedObjectLoaded)
            {
                behavior.DetachOverlay(e.OldValue as UIElement);
                behavior.AttachOverlay(e.NewValue as UIElement);
            }
        }

        public Grid ActiveContentContainer
        {
            get { return (Grid)GetValue(ActiveContentContainerProperty); }
            set { SetValue(ActiveContentContainerProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ActiveContentContainer"/> 
        /// dependency property.</summary>
        public static readonly DependencyProperty ActiveContentContainerProperty =
            DependencyProperty.Register(nameof(ActiveContentContainer), typeof(Grid), typeof(AnimatedOverlayBehavior), new PropertyMetadata(null, (s, e) => OnActiveContentContainerChanged(s, e)));

        private static void OnActiveContentContainerChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var behavior = s as AnimatedOverlayBehavior;

            if (behavior.IsAssociatedObjectLoaded)
            {
                behavior.AttachActiveContainer(e.OldValue as UIElement);
                behavior.DetachActiveContainer(e.NewValue as UIElement);
            }
        }

        public UIElement OverlayContent
        {
            get { return (UIElement)GetValue(OverlayContentProperty); }
            set { SetValue(OverlayContentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="OverlayContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OverlayContentProperty =
            DependencyProperty.Register(nameof(OverlayContent), typeof(UIElement), typeof(AnimatedOverlayBehavior), new PropertyMetadata(null));

        #endregion

        protected override void OnAssociatedObjectLoaded()
        {
            base.OnAssociatedObjectLoaded();

            GetInternalGrid();
            AttachOverlay(OverlayGrid);
            AttachActiveContainer(ActiveContentContainer);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

#pragma warning disable IDISP004 // Don't ignore created IDisposable.
            AnimationService = this.GetServiceLocator().ResolveType<IAnimationService>();
#pragma warning restore IDISP004 // Don't ignore created IDisposable.
        }

        private void AttachOverlay(UIElement overlay)
        {
            _topInternalGrid.Children.Add(overlay);

            //manually hide overlay
            HideOverlay();
        }

        private void DetachOverlay(UIElement overlay)
        {
            _topInternalGrid.Children.Remove(overlay);
        }

        private void AttachActiveContainer(UIElement contentContainer)
        {
            _topInternalGrid.Children.Add(contentContainer);
        }

        private void DetachActiveContainer(UIElement contentContainer)
        {
            _topInternalGrid.Children.Add(contentContainer);
        }

        private void GetInternalGrid()
        {
            _topInternalGrid = AssociatedObject.FindVisualDescendantByName("_InternalGridName") as Grid;
        }

        protected override void OnIsEnabledChanged()
        {
            base.OnIsEnabledChanged();

            if (!IsAssociatedObjectLoaded)
            {
                return;
            }

            if (IsEnabled)
            {
                _sizeHandler = SetupAndShowOverlay(OverlayContent);
            }
            else
            {
                HideAnimatedOverlay();
                HideActiveContainer();
            }
        }

        private SizeChangedEventHandler SetupAndShowOverlay(UIElement overlayContent)
        {
            overlayContent.SetCurrentValue(Panel.ZIndexProperty, (int)OverlayGrid.GetValue(Panel.ZIndexProperty) + 1);

            overlayContent.SetCurrentValue(FrameworkElement.MinHeightProperty, AssociatedObject.ActualHeight / 4.0);
            overlayContent.SetCurrentValue(FrameworkElement.MaxHeightProperty, AssociatedObject.ActualHeight);

            SizeChangedEventHandler sizeHandler = (sender, args) =>
            {
                overlayContent.SetCurrentValue(FrameworkElement.MinHeightProperty, AssociatedObject.ActualHeight / 4.0);
                overlayContent.SetCurrentValue(FrameworkElement.MaxHeightProperty, AssociatedObject.ActualHeight);
            };

            AssociatedObject.SizeChanged += sizeHandler;

            AddContentToOverlay(overlayContent);
            ShowAnimatedOverlay();

            return sizeHandler;
        }

        private void AddContentToOverlay(UIElement overlayContent)
        {
            var activeContent = ActiveContentContainer.Children.Cast<UIElement>().SingleOrDefault();

            if (activeContent is not null)
            {
                ActiveContentContainer.Children.Remove(activeContent);
            }

            ActiveContentContainer.Children.Add(overlayContent);
        }

        private void ShowAnimatedOverlay()
        {
            if (OverlayGrid is null)
            {
                throw new InvalidOperationException("Cannot find overlay in Associated object");
            }

            if (OverlayGrid.Visibility == Visibility.Visible && _overlayStoryboard is null)
            {
                return;
            }

            if (ActiveContentContainer.Visibility == Visibility.Hidden)
            {
                ActiveContentContainer.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Visible);
            }

            Dispatcher.VerifyAccess();

            var storyboard = AnimationService.GetFadeInAnimation(OverlayGrid);

            _overlayStoryboard = storyboard;

            if (TryGetOverlayFadingStoryboardAnimation(storyboard, out var animation))
            {
                OverlayGrid.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Visible);

                animation.SetCurrentValue(DoubleAnimation.ToProperty, (double?)0.7);

                EventHandler completionHandler = null;
                completionHandler = (sender, args) =>
                {
                    storyboard.Completed -= completionHandler;
                    if (_overlayStoryboard == storyboard)
                    {
                        _overlayStoryboard = null;
                    }
                };

                storyboard.Completed += completionHandler;
                OverlayGrid.BeginStoryboard(storyboard);
            }
            else
            {
                ShowOverlay();
            }
        }

        private void HideAnimatedOverlay()
        {
            if (OverlayGrid is null)
            {
                throw new InvalidOperationException("Cannot find overlay in Associated object");
            }

            if (OverlayGrid.Visibility == Visibility.Visible && OverlayGrid.Opacity <= 0.0)
            {
                OverlayGrid.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Hidden);
                return;
            }

            Dispatcher.VerifyAccess();

            var storyboard = AnimationService.GetFadeOutAnimation(OverlayGrid);

            _overlayStoryboard = storyboard;

            if (TryGetOverlayFadingStoryboardAnimation(storyboard, out var animation))
            {
                animation.SetCurrentValue(DoubleAnimation.ToProperty, 0d);

                EventHandler completionHandler = null;
                completionHandler = (sender, args) =>
                {
                    storyboard.Completed -= completionHandler;
                    if (_overlayStoryboard == storyboard)
                    {
                        OverlayGrid.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Hidden);
                        _overlayStoryboard = null;
                    }
                };

                storyboard.Completed += completionHandler;
                OverlayGrid.BeginStoryboard(storyboard);
            }
            else
            {
                HideOverlay();
            }
        }

        private static bool TryGetOverlayFadingStoryboardAnimation(Storyboard storyBoard, out DoubleAnimation animation)
        {
            animation = null;

            if (storyBoard is null)
            {
                return false;
            }

            storyBoard.Dispatcher.VerifyAccess();

            animation = storyBoard.Children.OfType<DoubleAnimation>().FirstOrDefault();
            if (animation is null)
            {
                return false;
            }

            return storyBoard.IsPreparedToBegin() ||
                animation.IsPreparedToBegin();
        }

        private void HideActiveContainer()
        {
            ActiveContentContainer.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Hidden);
        }

        private void ShowOverlay()
        {
            OverlayGrid.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Visible);
            OverlayGrid.SetCurrentValue(Grid.OpacityProperty, 0.7);
        }

        private void HideOverlay()
        {
            OverlayGrid.SetCurrentValue(Grid.OpacityProperty, 0d);
            OverlayGrid.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Hidden);
        }
    }
}
