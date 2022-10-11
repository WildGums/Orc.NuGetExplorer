
namespace Orc.NuGetExplorer.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using NuGet.Configuration;

    internal class NuGetSettingsSection : SettingSection
    {
        /// <summary>
        /// Empty settings section
        /// </summary>
        /// <param name="name"></param>
        public NuGetSettingsSection(string name) 
            : base(name, new Dictionary<string, string>(), new List<SettingItem>())
        {

        }

        /// <summary>
        /// Settings section without attributes
        /// </summary>
        /// <param name="name"></param>
        /// <param name="children"></param>
        public NuGetSettingsSection(string name, IEnumerable<SettingItem> children) 
            : base(name, new Dictionary<string, string>(), children)
        {

        }

        public NuGetSettingsSection(string name, IReadOnlyDictionary<string, string> attributes, IEnumerable<SettingItem> children) : base(name, attributes, children)
        {
        }

        public override SettingBase Clone()
        {
            return new NuGetSettingsSection(ElementName, Items.Select(child => child.Clone() as SettingItem));
        }
    }
}
