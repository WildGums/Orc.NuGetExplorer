using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;

// All other assembly info is defined in SolutionAssemblyInfo.cs

[assembly: AssemblyTitle("Orc.NuGetExplorer.Xaml")]
[assembly: AssemblyDescription("Orc NuGetExplorer Xaml Library")]
[assembly: AssemblyProduct("Orc.NuGetExplorer.Xaml")]
[assembly: NeutralResourcesLanguage("en-US")]

[assembly: InternalsVisibleTo("Orc.NuGetExplorer.Tests")]

[assembly: XmlnsPrefix("http://schemas.wildgums.com/orc/nugetexplorer", "orcnugetexplorer")]
[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/nugetexplorer", "Orc.NuGetExplorer")]
[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/nugetexplorer", "Orc.NuGetExplorer.Controls")]
[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/nugetexplorer", "Orc.NuGetExplorer.Converters")]
[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/nugetexplorer", "Orc.NuGetExplorer.Views")]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.SourceAssembly, //where theme specific resource dictionaries are located
                                               //(used if a resource is not found in the page, 
                                               // or application resource dictionaries)

    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page, 
                                              // app, or any theme specific resource dictionaries)
)]
