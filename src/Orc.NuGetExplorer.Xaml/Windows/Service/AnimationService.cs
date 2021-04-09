namespace Orc.NuGetExplorer.Windows
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Animation;

    internal class AnimationService : IAnimationService
    {
        private readonly Themes.Animations _resourceDictionary;

        public AnimationService()
        {
            _resourceDictionary = new Themes.Animations();
            _resourceDictionary.InitializeComponent();
        }

        public Storyboard GetFadeInAnimation(DependencyObject dependencyObject)
        {
            var sb = (_resourceDictionary["FastFadeIn"] as Storyboard)?.Clone();

            ValidateFadeAnimation(sb, dependencyObject, "FastFadeIn");

            return sb;
        }

        public Storyboard GetFadeOutAnimation(DependencyObject dependencyObject)
        {
            var sb = (_resourceDictionary["FastFadeOut"] as Storyboard)?.Clone();

            ValidateFadeAnimation(sb, dependencyObject, "FastFadeOut");

            return sb;
        }

        private void ValidateFadeAnimation(Storyboard sb, DependencyObject dependencyObject, string key)
        {
            if (sb is not null && sb.Children.Count > 0)
            {
                Storyboard.SetTarget(sb.Children.First(), dependencyObject);
            }
            else
            {
                throw new InvalidOperationException($"Resource under key '{key}' is not exist or can't be used as animation");
            }
        }
    }
}
