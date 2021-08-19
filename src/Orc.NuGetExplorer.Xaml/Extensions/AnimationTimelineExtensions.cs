namespace Orc.NuGetExplorer
{
    using System.Windows.Media.Animation;

    internal static class AnimationTimelineExtensions
    {
        public static bool IsPreparedToBegin(this Timeline animationTimeline)
        {
            // TODO verify condition (shoould be OR or AND ?)
            return animationTimeline.Duration.HasTimeSpan && animationTimeline.Duration.TimeSpan.Ticks > 0 ||
                animationTimeline.AccelerationRatio > 0 ||
                animationTimeline.DecelerationRatio > 0;
        }
    }
}
