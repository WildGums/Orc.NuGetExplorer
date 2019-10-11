namespace Orc.NuGetExplorer.Windows.Service
{
    using System.Windows;
    using System.Windows.Media.Animation;

    public interface IAnimationService
    {
        Storyboard GetFadeInAnimation(DependencyObject dependencyObject);

        Storyboard GetFadeOutAnimation(DependencyObject dependencyObject);
    }
}
