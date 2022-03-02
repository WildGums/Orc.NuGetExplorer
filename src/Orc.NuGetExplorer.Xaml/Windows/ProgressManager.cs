namespace Orc.NuGetExplorer.Windows
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Windows;
    using Catel.Windows.Interactivity;
    using Microsoft.Xaml.Behaviors;
    using NuGetExplorer.Behaviors;

    internal class ProgressManager : IProgressManager
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<IViewModel, DataWindow> _storedManagedWindows = new Dictionary<IViewModel, DataWindow>();

        public void ShowBar(IViewModel vm)
        {
            var window = GetCurrentActiveDataWindow();

            foreach (var behavior in GetOverlayBehaviors(window))
            {
                behavior.SetCurrentValue(BehaviorBase<DataWindow>.IsEnabledProperty, true);

                _storedManagedWindows.Add(vm, window);
            }
        }

        public void HideBar(IViewModel vm)
        {
            DataWindow window = null;

            if (_storedManagedWindows.TryGetValue(vm, out window))
            {
                foreach (var behavior in GetOverlayBehaviors(window))
                {
                    behavior.SetCurrentValue(BehaviorBase<DataWindow>.IsEnabledProperty, false);
                    _storedManagedWindows.Remove(vm);
                }
            }
        }

        private DataWindow GetCurrentActiveDataWindow()
        {
            return Application.Current.Windows.OfType<DataWindow>().FirstOrDefault(x => x.IsActive);
        }

        private IEnumerable<AnimatedOverlayBehavior> GetOverlayBehaviors(DataWindow window)
        {
            var behaviors = Interaction.GetBehaviors(window);

            return behaviors.OfType<AnimatedOverlayBehavior>();
        }
    }
}
