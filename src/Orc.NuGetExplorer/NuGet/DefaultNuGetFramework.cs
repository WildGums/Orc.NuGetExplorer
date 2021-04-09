namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Catel;
    using Catel.Logging;
    using Microsoft.Win32;
    using NuGet.Frameworks;

    public class DefaultNuGetFramework : IDefaultNuGetFramework
    {
        private const string BaseFrameworkName = ".NETFramework, Version=";
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly FrameworkReducer _frameworkReducer = new FrameworkReducer();
        private readonly IFrameworkNameProvider _frameworkNameProvider;
        private readonly IList<NuGetFramework> _nuGetFrameworks = new List<NuGetFramework>();

        public DefaultNuGetFramework(IFrameworkNameProvider frameworkNameProvider)
        {
            Argument.IsNotNull(() => frameworkNameProvider);

            _frameworkNameProvider = frameworkNameProvider;

            LoadAvailableFrameworks();
        }

        public IEnumerable<NuGetFramework> GetLowest()
        {
            return _frameworkReducer.ReduceDownwards(_nuGetFrameworks);
        }

        public IEnumerable<NuGetFramework> GetHighest()
        {
            return _frameworkReducer.ReduceUpwards(_nuGetFrameworks);
        }

        
        private void LoadAvailableFrameworks()
        {
            var version = Environment.Version;
            var revision = version.Revision;

            var frameworkStringList = new List<string>();

            if (revision < 42000 && revision > 0)
            {
                GetOlderFrameworkVersionsFromRegistry(frameworkStringList);
            }
            else
            {
                GetNewerFrameworkVersionsFromRegistry(frameworkStringList);
            }

#if NETCORE 
            var netCoreDescription = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
            if (netCoreDescription.Contains(".NET Core"))
            {
                frameworkStringList.Add($".NETCoreApp,Version=v{version.Major}.{version.Minor}");
            }
            else
            {
                if (version.Major == 5)
                {
                    // Support .NET 5 
                    frameworkStringList.Add($".NETCoreApp,Version=v{version.Major}.0");
                }
            }
#endif
            foreach (var frameworkName in frameworkStringList.Distinct())
            {
                var targetFramework = FrameworkParser.TryParseFrameworkName(frameworkName, _frameworkNameProvider);

                if (targetFramework != NuGetFramework.UnsupportedFramework)
                {
                    targetFramework = _frameworkNameProvider.GetFullNameReplacement(targetFramework);

                    _nuGetFrameworks.Add(targetFramework);
                }
            }
        }

        private static void GetOlderFrameworkVersionsFromRegistry(List<string> frameworkList)
        {
            // Opens the registry key for the .NET Framework entry.
            using (RegistryKey ndpKey =
                    RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).
                    OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
            {
                foreach (var versionKeyName in ndpKey.GetSubKeyNames())
                {
                    // Skip .NET Framework 4.5 version information.
                    if (versionKeyName == "v4")
                    {
                        continue;
                    }

                    if (versionKeyName.StartsWith("v"))
                    {

                        RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);

                        // Get the .NET Framework version value.
                        var name = (string)versionKey.GetValue("Version", "");
                        // Get the service pack (SP) number.
                        var sp = versionKey.GetValue("SP", "").ToString();

                        // Get the installation flag, or an empty string if there is none.
                        var install = versionKey.GetValue("Install", "").ToString();

                        if (string.IsNullOrEmpty(install))
                        {
                            // No install info; it must be in a child subkey.
                            frameworkList.Add($"{versionKeyName} {name}");
                        }
                        else
                        {
                            if (!(string.IsNullOrEmpty(sp)) && install == "1")
                            {
                                frameworkList.Add($"{versionKeyName} {name} SP{sp}");
                            }
                        }
                        if (!string.IsNullOrEmpty(name))
                        {
                            continue;
                        }

                        foreach (var subKeyName in versionKey.GetSubKeyNames())
                        {
                            RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
                            name = (string)subKey.GetValue("Version", "");
                            if (!string.IsNullOrEmpty(name))
                                sp = subKey.GetValue("SP", "").ToString();

                            install = subKey.GetValue("Install", "").ToString();

                            if (string.IsNullOrEmpty(install))
                            {
                                //No install info; it must be later.
                                frameworkList.Add($"{versionKeyName} {name}");
                            }
                            else
                            {
                                if (install == "1")
                                {
                                    if (!(string.IsNullOrEmpty(sp)))
                                    {
                                        frameworkList.Add($"{subKeyName} {name} SP{sp}");
                                    }
                                    else
                                    {
                                        frameworkList.Add($" {subKeyName} {name}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void GetNewerFrameworkVersionsFromRegistry(List<string> frameworkList)
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

            using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                var ndpKeyRelease = ndpKey.GetValue("Release");
                if (ndpKey is not null && ndpKeyRelease is not null)
                {
                    var version = CheckFor45PlusVersion((int)ndpKeyRelease);

                    frameworkList.Add(version);
                }
                else
                {
                    Log.Info(".NET Framework Version 4.5 or later is not detected.");
                }
            }

            // Checking the version using >= enables forward compatibility.
            static string CheckFor45PlusVersion(int releaseKey)
            {
                if (releaseKey >= 528040)
                {
                    return $"{BaseFrameworkName}v4.7.2";    //current version of nuget lib doesn't know about 4.8 
                }
                if (releaseKey >= 461808)
                {
                    return $"{BaseFrameworkName}v4.7.2";
                }
                if (releaseKey >= 461308)
                {
                    return $"{BaseFrameworkName}v4.7.1";
                }
                if (releaseKey >= 460798)
                {
                    return $"{BaseFrameworkName}v4.7";
                }
                if (releaseKey >= 394802)
                {
                    return $"{BaseFrameworkName}v4.6.2";
                }
                if (releaseKey >= 394254)
                {
                    return $"{BaseFrameworkName}v4.6.1";
                }
                if (releaseKey >= 393295)
                {
                    return $"{BaseFrameworkName}v4.6.0";
                }
                if (releaseKey >= 379893)
                {
                    return $"{BaseFrameworkName}v4.5.2";
                }
                if (releaseKey >= 378675)
                {
                    return $"{BaseFrameworkName}v4.5.1";
                }
                if (releaseKey >= 378389)
                {
                    return $"{BaseFrameworkName}v4.5";
                }

                // This code should never execute. A non-null release key should mean
                // that 4.5 or later is installed.
                return string.Empty;
            }
        }
    }
}
