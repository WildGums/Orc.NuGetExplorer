[assembly: System.Resources.NeutralResourcesLanguageAttribute("en-US")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleToAttribute("Orc.NuGetExplorer.Tests")]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.6", FrameworkDisplayName=".NET Framework 4.6")]
[assembly: System.Windows.Markup.XmlnsDefinitionAttribute("http://schemas.wildgums.com/orc/nugetexplorer", "Orc.NuGetExplorer")]
[assembly: System.Windows.Markup.XmlnsDefinitionAttribute("http://schemas.wildgums.com/orc/nugetexplorer", "Orc.NuGetExplorer.Controls")]
[assembly: System.Windows.Markup.XmlnsDefinitionAttribute("http://schemas.wildgums.com/orc/nugetexplorer", "Orc.NuGetExplorer.Converters")]
[assembly: System.Windows.Markup.XmlnsDefinitionAttribute("http://schemas.wildgums.com/orc/nugetexplorer", "Orc.NuGetExplorer.Views")]
[assembly: System.Windows.Markup.XmlnsPrefixAttribute("http://schemas.wildgums.com/orc/nugetexplorer", "orcnugetexplorer")]
[assembly: System.Windows.ThemeInfoAttribute(System.Windows.ResourceDictionaryLocation.SourceAssembly, System.Windows.ResourceDictionaryLocation.SourceAssembly)]
public class static LoadAssembliesOnStartup { }
public class static ModuleInitializer
{
    public static void Initialize() { }
}
namespace Orc.NuGetExplorer.Controls
{
    [System.Windows.TemplatePartAttribute(Name="PART_Badge", Type=typeof(System.Windows.FrameworkElement))]
    [System.Windows.TemplatePartAttribute(Name="PART_BadgeContent", Type=typeof(System.Windows.FrameworkElement))]
    public class Badged : System.Windows.Controls.ContentControl
    {
        public static readonly System.Windows.DependencyProperty BadgeForegroundProperty;
        public static readonly System.Windows.DependencyProperty BadgeProperty;
        public static readonly System.Windows.DependencyProperty IsShowedProperty;
        public Badged() { }
        public object Badge { get; set; }
        public System.Windows.Media.Brush BadgeForeground { get; set; }
        public bool IsShowed { get; set; }
        public event System.Windows.DependencyPropertyChangedEventHandler IsShowedChanged;
        public override void OnApplyTemplate() { }
    }
    public class InfiniteScrollListBox : System.Windows.Controls.ListBox
    {
        public static readonly System.Windows.DependencyProperty CommandParameterProperty;
        public static readonly System.Windows.DependencyProperty CommandProperty;
        public static readonly System.Windows.DependencyProperty IsCommandExecutingProperty;
        public static readonly System.Windows.DependencyProperty ScrollSizeProperty;
        public InfiniteScrollListBox() { }
        public Catel.MVVM.TaskCommand Command { get; set; }
        public object CommandParameter { get; set; }
        public bool IsCommandExecuting { get; set; }
        public int ScrollSize { get; set; }
        protected System.Threading.Tasks.Task ExecuteLoadingItemsCommandAsync() { }
    }
    public class RotationProgressBar : System.Windows.Controls.ProgressBar
    {
        public static readonly System.Windows.DependencyProperty IconDataProperty;
        public static readonly System.Windows.DependencyProperty IsInProgressProperty;
        public static readonly System.Windows.DependencyProperty SpeedProperty;
        public static readonly System.Windows.DependencyProperty SuccessProperty;
        public RotationProgressBar() { }
        public System.Windows.Shapes.Path IconData { get; set; }
        public bool IsInProgress { get; set; }
        public double Speed { get; set; }
        public bool Success { get; set; }
    }
    public class TabControllerButton : System.Windows.Controls.RadioButton
    {
        public static readonly System.Windows.DependencyProperty IsFirstProperty;
        public static readonly System.Windows.DependencyProperty NextProperty;
        public static readonly System.Windows.DependencyProperty TabSourceProperty;
        public TabControllerButton() { }
        public bool IsFirst { get; set; }
        public Orc.NuGetExplorer.Controls.TabControllerButton Next { get; set; }
        public System.Windows.Controls.TabControl TabSource { get; set; }
    }
}
namespace Orc.NuGetExplorer.Controls.Helpers
{
    public class WpfHelper
    {
        public WpfHelper() { }
        public static childItem FindVisualChild<childItem>(System.Windows.DependencyObject obj)
            where childItem : System.Windows.DependencyObject { }
    }
    public class XamlExportHelper
    {
        public XamlExportHelper() { }
        public static string Save(object element) { }
    }
}
namespace Orc.NuGetExplorer.Controls.Templating
{
    public class BadgeContentTemplateSelector : System.Windows.Controls.DataTemplateSelector
    {
        public BadgeContentTemplateSelector() { }
        public System.Windows.DataTemplate Available { get; set; }
        public System.Windows.DataTemplate Default { get; set; }
        public System.Windows.DataTemplate NotAvailable { get; set; }
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container) { }
    }
}
namespace Orc.NuGetExplorer.Converters
{
    public class BoolToIntConverter : Catel.MVVM.Converters.ValueConverterBase<bool, int>
    {
        public BoolToIntConverter() { }
        protected override object Convert(bool value, System.Type targetType, object parameter) { }
    }
    [System.Windows.Data.ValueConversionAttribute(typeof(System.Collections.ICollection), typeof(System.Windows.Visibility))]
    public class EmptyCollectionToVisibleConverter : Catel.MVVM.Converters.ValueConverterBase<System.Collections.ICollection, System.Windows.Visibility>
    {
        public EmptyCollectionToVisibleConverter() { }
        protected override object Convert(System.Collections.ICollection value, System.Type targetType, object parameter) { }
    }
    public class NuGetFrameworkToStringConverter : Catel.MVVM.Converters.ValueConverterBase<NuGet.Frameworks.NuGetFramework, string>
    {
        public NuGetFrameworkToStringConverter() { }
        protected override object Convert(NuGet.Frameworks.NuGetFramework value, System.Type targetType, object parameter) { }
    }
    public class NuGetFrameworkToVisibilityConverter : Catel.MVVM.Converters.ValueConverterBase<NuGet.Frameworks.NuGetFramework, System.Windows.Visibility>
    {
        public NuGetFrameworkToVisibilityConverter() { }
        protected override object Convert(NuGet.Frameworks.NuGetFramework value, System.Type targetType, object parameter) { }
    }
    public class NuGetVersionToStringConverter : Catel.MVVM.Converters.ValueConverterBase<NuGet.Versioning.NuGetVersion, string>
    {
        public NuGetVersionToStringConverter() { }
        protected override object Convert(NuGet.Versioning.NuGetVersion value, System.Type targetType, object parameter) { }
    }
    public class NullableBooleanTrueConverter : Catel.MVVM.Converters.ValueConverterBase<System.Nullable<bool>>
    {
        public NullableBooleanTrueConverter() { }
        protected override object Convert(System.Nullable<bool> value, System.Type targetType, object parameter) { }
        protected override object ConvertBack(object value, System.Type targetType, object parameter) { }
    }
    public class PackageStatusEnumToBoolConverter : Catel.MVVM.Converters.ValueConverterBase<Orc.NuGetExplorer.Enums.PackageStatus, bool>
    {
        public PackageStatusEnumToBoolConverter() { }
        protected override object Convert(Orc.NuGetExplorer.Enums.PackageStatus value, System.Type targetType, object parameter) { }
    }
    public class PackageStatusEnumToBrushConverter : Catel.MVVM.Converters.ValueConverterBase<Orc.NuGetExplorer.Enums.PackageStatus, System.Windows.Media.Brush>
    {
        public PackageStatusEnumToBrushConverter() { }
        protected override object Convert(Orc.NuGetExplorer.Enums.PackageStatus value, System.Type targetType, object parameter) { }
    }
    public class UriToBitmapConverter : Catel.MVVM.Converters.ValueConverterBase<System.Uri, System.Windows.Media.Imaging.BitmapImage>
    {
        public UriToBitmapConverter() { }
        protected override object Convert(System.Uri value, System.Type targetType, object parameter) { }
    }
}
namespace Orc.NuGetExplorer
{
    public class ExplorerTab
    {
        public static Orc.NuGetExplorer.ExplorerTab Browse;
        public static Orc.NuGetExplorer.ExplorerTab Installed;
        public static Orc.NuGetExplorer.ExplorerTab Update;
        public string Name { get; }
    }
    public interface IImageResolveService
    {
        System.Windows.Media.ImageSource ResolveImageFromUri(System.Uri uri, string defaultUrl = null);
        System.Threading.Tasks.Task<System.Windows.Media.ImageSource> ResolveImageFromUriAsync(System.Uri uri, string defaultUrl = null);
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
    public interface INuGetConfigurationResetService
    {
        System.Threading.Tasks.Task Reset();
    }
    public interface IPackageCommandService
    {
        System.Threading.Tasks.Task<bool> CanExecuteAsync(Orc.NuGetExplorer.PackageOperationType operationType, Orc.NuGetExplorer.IPackageDetails package);
        [System.ObsoleteAttribute("Use `ExecuteAsync` instead. Will be removed in version 5.0.0.", true)]
        System.Threading.Tasks.Task ExecuteAsync(Orc.NuGetExplorer.PackageOperationType operationType, Orc.NuGetExplorer.IPackageDetails packageDetails, Orc.NuGetExplorer.IRepository sourceRepository = null, bool allowedPrerelease = False);
        System.Threading.Tasks.Task ExecuteAsync(Orc.NuGetExplorer.PackageOperationType operationType, Orc.NuGetExplorer.IPackageDetails packageDetails);
        System.Threading.Tasks.Task ExecuteInstallAsync(Orc.NuGetExplorer.IPackageDetails packageDetails, System.Threading.CancellationToken token);
        System.Threading.Tasks.Task ExecuteUninstallAsync(Orc.NuGetExplorer.IPackageDetails packageDetails, System.Threading.CancellationToken token);
        System.Threading.Tasks.Task ExecuteUpdateAsync(Orc.NuGetExplorer.IPackageDetails packageDetails, System.Threading.CancellationToken token);
        string GetActionName(Orc.NuGetExplorer.PackageOperationType operationType);
        string GetPluralActionName(Orc.NuGetExplorer.PackageOperationType operationType);
        bool IsRefreshRequired(Orc.NuGetExplorer.PackageOperationType operationType);
    }
    public interface IPackageMetadataMediaDownloadService
    {
        System.Threading.Tasks.Task DownloadMediaForMetadataAsync(NuGet.Protocol.Core.Types.IPackageSearchMetadata packageMetadata);
    }
    public interface IPackagesUIService
    {
        string SettingsTitle { get; set; }
        System.Threading.Tasks.Task ShowPackagesExplorerAsync();
        System.Threading.Tasks.Task ShowPackagesExplorerAsync(Orc.NuGetExplorer.ExplorerTab openTab, Orc.NuGetExplorer.PackageSearchParameters searchParameters);
        System.Threading.Tasks.Task<System.Nullable<bool>> ShowPackagesSourceSettingsAsync();
    }
    public class static IPleaseWaitServiceExtensions
    {
        public static System.IDisposable WaitingScope(this Catel.Services.IPleaseWaitService pleaseWaitService) { }
    }
    public class static ObservableCollectionExtensions
    {
        public static void MoveDown<T>(this System.Collections.ObjectModel.ObservableCollection<T> collection, T item) { }
        public static void MoveUp<T>(this System.Collections.ObjectModel.ObservableCollection<T> collection, T item) { }
    }
    public class PackageMetadataMediaDownloadService : Orc.NuGetExplorer.IImageResolveService, Orc.NuGetExplorer.IPackageMetadataMediaDownloadService
    {
        public PackageMetadataMediaDownloadService(Orc.NuGetExplorer.Providers.IApplicationCacheProvider appCacheProvider) { }
        public System.Threading.Tasks.Task DownloadMediaForMetadataAsync(NuGet.Protocol.Core.Types.IPackageSearchMetadata packageMetadata) { }
        public System.Windows.Media.ImageSource ResolveImageFromUri(System.Uri uri, string defaultUrl = null) { }
        public System.Threading.Tasks.Task<System.Windows.Media.ImageSource> ResolveImageFromUriAsync(System.Uri uri, string defaultUrl = null) { }
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
    public class SynchronizationContextScopeManager
    {
        public SynchronizationContextScopeManager() { }
        public static System.IDisposable OutOfContext() { }
    }
    public class SynchronizeInvoker : System.ComponentModel.ISynchronizeInvoke
    {
        public SynchronizeInvoker(System.Windows.Threading.Dispatcher dispatcher) { }
        public bool InvokeRequired { get; }
        public System.IAsyncResult BeginInvoke(System.Delegate method, object[] args) { }
        public object EndInvoke(System.IAsyncResult result) { }
        public object Invoke(System.Delegate method, object[] args) { }
    }
    public class XamlPleaseWaitInterruptService : Orc.NuGetExplorer.IPleaseWaitInterruptService
    {
        public XamlPleaseWaitInterruptService(Catel.Services.IPleaseWaitService pleaseWaitService) { }
        public System.IDisposable InterruptTemporarily() { }
    }
}
namespace Orc.NuGetExplorer.Logging
{
    public class NuGetLogListener : Orc.NuGetExplorer.PackageManagerLogListenerBase, Catel.Logging.ILogListener
    {
        public NuGetLogListener(Orc.NuGetExplorer.INuGetLogListeningSevice nuGetLogListeningSevice) { }
        public bool IgnoreCatelLogging { get; set; }
        public bool IsDebugEnabled { get; set; }
        public bool IsErrorEnabled { get; set; }
        public bool IsInfoEnabled { get; set; }
        public bool IsStatusEnabled { get; set; }
        public bool IsWarningEnabled { get; set; }
        public Catel.Logging.TimeDisplay TimeDisplay { get; set; }
        public event System.EventHandler<Catel.Logging.LogMessageEventArgs> LogMessage;
        public void Debug(Catel.Logging.ILog log, string message, object extraData, Catel.Logging.LogData logData, System.DateTime time) { }
        public void Error(Catel.Logging.ILog log, string message, object extraData, Catel.Logging.LogData logData, System.DateTime time) { }
        public void Info(Catel.Logging.ILog log, string message, object extraData, Catel.Logging.LogData logData, System.DateTime time) { }
        protected override void OnDebug(object sender, Orc.NuGetExplorer.NuGetLogRecordEventArgs e) { }
        protected override void OnError(object sender, Orc.NuGetExplorer.NuGetLogRecordEventArgs e) { }
        protected override void OnInfo(object sender, Orc.NuGetExplorer.NuGetLogRecordEventArgs e) { }
        protected override void OnWarning(object sender, Orc.NuGetExplorer.NuGetLogRecordEventArgs e) { }
        public void Status(Catel.Logging.ILog log, string message, object extraData, Catel.Logging.LogData logData, System.DateTime time) { }
        public void Warning(Catel.Logging.ILog log, string message, object extraData, Catel.Logging.LogData logData, System.DateTime time) { }
        public void Write(Catel.Logging.ILog log, string message, Catel.Logging.LogEvent logEvent, object extraData, Catel.Logging.LogData logData, System.DateTime time) { }
    }
}
namespace Orc.NuGetExplorer.MVVM
{
    public class BindingProxy : System.Windows.Freezable
    {
        public static readonly System.Windows.DependencyProperty DataProperty;
        public BindingProxy() { }
        public object Data { get; set; }
        protected override System.Windows.Freezable CreateInstanceCore() { }
    }
}
namespace Orc.NuGetExplorer.Providers
{
    public class ExplorerSettingsContainerModelProvider : Orc.NuGetExplorer.Providers.ModelProvider<Orc.NuGetExplorer.Models.ExplorerSettingsContainer>
    {
        public ExplorerSettingsContainerModelProvider(Catel.IoC.ITypeFactory typeFactory, Orc.NuGetExplorer.INuGetConfigurationService nugetConfigurationService) { }
        public bool IsInitialized { get; set; }
        public override Orc.NuGetExplorer.Models.ExplorerSettingsContainer Model { get; set; }
        public override Orc.NuGetExplorer.Models.ExplorerSettingsContainer Create() { }
    }
    public class NuGetProjectContextProvider : Orc.NuGetExplorer.INuGetProjectContextProvider
    {
        public NuGetProjectContextProvider(Catel.IoC.ITypeFactory typeFactory) { }
        public NuGet.ProjectManagement.INuGetProjectContext GetProjectContext(NuGet.ProjectManagement.FileConflictAction fileConflictAction) { }
    }
}
namespace Orc.NuGetExplorer.Services
{
    public class NuGetExplorerInitializationService : Orc.NuGetExplorer.Services.INuGetExplorerInitializationService
    {
        public NuGetExplorerInitializationService(Catel.Services.ILanguageService languageService, Orc.NuGetExplorer.ICredentialProviderLoaderService credentialProviderLoaderService, Orc.NuGetExplorer.Services.INuGetProjectUpgradeService nuGetProjectUpgradeService, Catel.MVVM.IViewModelLocator vmLocator, Catel.IoC.ITypeFactory typeFactory) { }
        public string DefaultSourceKey { get; }
        public virtual System.Threading.Tasks.Task<bool> UpgradeNuGetPackagesIfNeededAsync() { }
    }
}
namespace Orc.NuGetExplorer.ViewModels
{
    public class FeedDetailViewModel : Catel.MVVM.ViewModelBase
    {
        public static readonly Catel.Data.PropertyData FeedProperty;
        public static readonly Catel.Data.PropertyData NameProperty;
        public static readonly Catel.Data.PropertyData SourceProperty;
        public FeedDetailViewModel(Orc.NuGetExplorer.Models.NuGetFeed feed, Orc.NuGetExplorer.Providers.IModelProvider<Orc.NuGetExplorer.Models.NuGetFeed> modelProvider) { }
        [Catel.MVVM.ModelAttribute()]
        public Orc.NuGetExplorer.Models.NuGetFeed Feed { get; set; }
        [Catel.MVVM.ViewModelToModelAttribute("", "")]
        public string Name { get; set; }
        public Catel.MVVM.Command OpenChooseLocalPathToSourceDialog { get; set; }
        [Catel.MVVM.ViewModelToModelAttribute("", "")]
        public string Source { get; set; }
        public Catel.MVVM.Command UpdateFeed { get; set; }
        protected override System.Threading.Tasks.Task<bool> SaveAsync() { }
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
namespace Orc.NuGetExplorer.Web
{
    public class IconDownloader
    {
        public IconDownloader() { }
        public byte[] GetByUrl(System.Uri uri, System.Net.WebClient client) { }
        public System.Threading.Tasks.Task<byte[]> GetByUrlAsync(System.Uri uri, System.Net.WebClient client) { }
    }
}
namespace Orc.NuGetExplorer.Windows
{
    public class DialogHost : Catel.Windows.DataWindow, System.Windows.Markup.IComponentConnector
    {
        public static readonly System.Windows.DependencyProperty DialogCommandProperty;
        public DialogHost(Catel.MVVM.IViewModel vm) { }
        public System.Windows.Input.ICommand DialogCommand { get; set; }
        protected override void Initialize() { }
        public void InitializeComponent() { }
    }
    public interface IProgressManager
    {
        void HideBar(Catel.MVVM.IViewModel vm);
        void ShowBar(Catel.MVVM.IViewModel vm);
    }
}
namespace Themes
{
    public class Animations : System.Windows.ResourceDictionary, System.Windows.Markup.IComponentConnector
    {
        public Animations() { }
        public void InitializeComponent() { }
    }
    public class Brushes : System.Windows.ResourceDictionary, System.Windows.Markup.IComponentConnector
    {
        public Brushes() { }
        public void InitializeComponent() { }
    }
    public class Compiled : System.Windows.ResourceDictionary, System.Windows.Markup.IComponentConnector
    {
        public Compiled() { }
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