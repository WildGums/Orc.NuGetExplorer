
namespace Orc.NuGetExplorer.Controls.Templating
{
    using System.Windows;
    using System.Windows.Controls;
    using NuGetExplorer.Enums;

    public class BadgeContentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NotAvailable { get; set; }

        public DataTemplate Available { get; set; }

        public DataTemplate Default { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is not null)
            {
                var state = (PackageStatus)item;

                if (state == PackageStatus.LastVersionInstalled)
                {
                    return NotAvailable;
                }
                if (state == PackageStatus.UpdateAvailable)
                {
                    return Available;
                }

                return Default;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
