[assembly: System.Resources.NeutralResourcesLanguageAttribute("en-US")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleToAttribute("Orc.NuGetExplorer.Tests")]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.6", FrameworkDisplayName=".NET Framework 4.6")]
[assembly: System.Windows.Markup.XmlnsDefinitionAttribute("http://schemas.wildgums.com/orc/nugetexplorer", "Orc.NuGetExplorer")]
[assembly: System.Windows.Markup.XmlnsDefinitionAttribute("http://schemas.wildgums.com/orc/nugetexplorer", "Orc.NuGetExplorer.Controls")]
[assembly: System.Windows.Markup.XmlnsDefinitionAttribute("http://schemas.wildgums.com/orc/nugetexplorer", "Orc.NuGetExplorer.Converters")]
[assembly: System.Windows.Markup.XmlnsDefinitionAttribute("http://schemas.wildgums.com/orc/nugetexplorer", "Orc.NuGetExplorer.Views")]
[assembly: System.Windows.Markup.XmlnsPrefixAttribute("http://schemas.wildgums.com/orc/nugetexplorer", "orcnugetexplorer")]
[assembly: System.Windows.ThemeInfoAttribute(System.Windows.ResourceDictionaryLocation.SourceAssembly, System.Windows.ResourceDictionaryLocation.SourceAssembly)]
public class static ModuleInitializer
{
    public static void Initialize() { }
}
namespace Orc.NuGetExplorer.Converters
{
    public class NullableBooleanTrueConverter : Catel.MVVM.Converters.ValueConverterBase<System.Nullable<bool>>
    {
        public NullableBooleanTrueConverter() { }
        protected override object Convert(System.Nullable<bool> value, System.Type targetType, object parameter) { }
        protected override object ConvertBack(object value, System.Type targetType, object parameter) { }
    }
    public class PackageDetailsCollectionToSelectablePackageDetailsViewModelCollectionConverter : Catel.MVVM.Converters.ValueConverterBase<System.Collections.ObjectModel.ObservableCollection<Orc.NuGetExplorer.IPackageDetails>, System.Collections.ObjectModel.ObservableCollection<Orc.NuGetExplorer.ViewModels.SelectablePackageDetailsViewModel>>
    {
        public PackageDetailsCollectionToSelectablePackageDetailsViewModelCollectionConverter() { }
        protected override object Convert(System.Collections.ObjectModel.ObservableCollection<Orc.NuGetExplorer.IPackageDetails> packageDetailsCollection, System.Type targetType, object parameter) { }
    }
    public class SelectablePackageDetailsViewModelToPackageDetailsConverter : Catel.MVVM.Converters.ValueConverterBase<Orc.NuGetExplorer.IPackageDetails, Orc.NuGetExplorer.ViewModels.SelectablePackageDetailsViewModel>
    {
        public SelectablePackageDetailsViewModelToPackageDetailsConverter() { }
        protected override object Convert(Orc.NuGetExplorer.IPackageDetails value, System.Type targetType, object parameter) { }
        protected override object ConvertBack(Orc.NuGetExplorer.ViewModels.SelectablePackageDetailsViewModel value, System.Type targetType, object parameter) { }
    }
}
namespace Orc.NuGetExplorer
{
    public interface IImageResolveService
    {
        System.Windows.Media.ImageSource ResolveImageFromUri(System.Uri uri, string defaultUrl = null);
    }
    public class static InlineCollectionExtensions
    {
        public static void AddIfNotNull(this System.Windows.Documents.InlineCollection inlineCollection, System.Windows.Documents.Inline inline) { }
    }
    public class static InlineExtensions
    {
        public static System.Windows.Documents.Inline Append(this System.Windows.Documents.Inline inline, System.Windows.Documents.Inline inlineToAdd) { }
        public static System.Windows.Documents.Inline AppendRange(this System.Windows.Documents.Inline inline, System.Collections.Generic.IEnumerable<System.Windows.Documents.Inline> inlines) { }
        public static System.Windows.Documents.Bold Bold(this System.Windows.Documents.Inline inline) { }
        public static System.Windows.Documents.Inline Insert(this System.Windows.Documents.Inline inline, System.Windows.Documents.Inline inlineToAdd) { }
        public static System.Windows.Documents.Inline InsertRange(this System.Windows.Documents.Inline inline, System.Collections.Generic.IEnumerable<System.Windows.Documents.Inline> inlines) { }
    }
    public interface IPackageCommandService
    {
        bool CanExecute(Orc.NuGetExplorer.PackageOperationType operationType, Orc.NuGetExplorer.IPackageDetails package);
        void Execute(Orc.NuGetExplorer.PackageOperationType operationType, Orc.NuGetExplorer.IPackageDetails packageDetails, Orc.NuGetExplorer.IRepository sourceRepository = null, bool allowedPrerelease = False);
        string GetActionName(Orc.NuGetExplorer.PackageOperationType operationType);
        string GetPluralActionName(Orc.NuGetExplorer.PackageOperationType operationType);
        bool IsRefreshRequired(Orc.NuGetExplorer.PackageOperationType operationType);
    }
    public class static IPleaseWaitServiceExtensions
    {
        public static System.IDisposable WaitingScope(this Catel.Services.IPleaseWaitService pleaseWaitService) { }
    }
    public class PackagesBatch
    {
        public PackagesBatch() { }
        public Orc.NuGetExplorer.PackageOperationType OperationType { get; set; }
        public System.Collections.ObjectModel.ObservableCollection<Orc.NuGetExplorer.IPackageDetails> PackageList { get; set; }
    }
    public class static StringExtensions
    {
        public static System.Windows.Documents.Inline ToInline(this string text) { }
        public static System.Windows.Documents.Inline ToInline(this string text, System.Windows.Media.Brush brush) { }
    }
    public class XamlPleaseWaitInterruptService : Orc.NuGetExplorer.IPleaseWaitInterruptService
    {
        public XamlPleaseWaitInterruptService(Catel.Services.IPleaseWaitService pleaseWaitService) { }
        public System.IDisposable InterruptTemporarily() { }
    }
}
namespace Orc.NuGetExplorer.ViewModels
{
    public class SelectablePackageDetailsViewModel : Catel.MVVM.ViewModelBase
    {
        public SelectablePackageDetailsViewModel(Orc.NuGetExplorer.IPackageDetails packageDetails) { }
        public System.Collections.Generic.IList<string> AvailableVersions { get; }
        public string Description { get; }
        public System.Uri IconUrl { get; }
        public System.Nullable<bool> IsInstalled { get; set; }
        public Orc.NuGetExplorer.IPackageDetails PackageDetails { get; }
        public string SelectedVersion { get; set; }
        public System.Windows.Input.ICommand SelectPackageVersionCommand { get; }
        public override string Title { get; }
    }
}
namespace Orc.NuGetExplorer.Views
{
    public sealed class PackageSourceSettingControl : Catel.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector
    {
        public static readonly System.Windows.DependencyProperty DefaultFeedProperty;
        public static readonly System.Windows.DependencyProperty DefaultSourceNameProperty;
        public static readonly System.Windows.DependencyProperty PackageSourcesProperty;
        public PackageSourceSettingControl() { }
        [Catel.MVVM.Views.ViewToViewModelAttribute("", MappingType=Catel.MVVM.Views.ViewToViewModelMappingType.ViewToViewModel)]
        public string DefaultFeed { get; set; }
        [Catel.MVVM.Views.ViewToViewModelAttribute("", MappingType=Catel.MVVM.Views.ViewToViewModelMappingType.ViewToViewModel)]
        public string DefaultSourceName { get; set; }
        [Catel.MVVM.Views.ViewToViewModelAttribute("", MappingType=Catel.MVVM.Views.ViewToViewModelMappingType.TwoWayViewWins)]
        public System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IPackageSource> PackageSources { get; set; }
        public void InitializeComponent() { }
    }
}
namespace XamlGeneratedNamespace
{
    public sealed class GeneratedInternalTypeHelper : System.Windows.Markup.InternalTypeHelper
    {
        public GeneratedInternalTypeHelper() { }
        protected override void AddEventHandler(System.Reflection.EventInfo eventInfo, object target, System.Delegate handler) { }
        protected override System.Delegate CreateDelegate(System.Type delegateType, object target, string handler) { }
        protected override object CreateInstance(System.Type type, System.Globalization.CultureInfo culture) { }
        protected override object GetPropertyValue(System.Reflection.PropertyInfo propertyInfo, object target, System.Globalization.CultureInfo culture) { }
        protected override void SetPropertyValue(System.Reflection.PropertyInfo propertyInfo, object target, object value, System.Globalization.CultureInfo culture) { }
    }
}