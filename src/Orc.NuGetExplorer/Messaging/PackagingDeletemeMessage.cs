namespace Orc.NuGetExplorer.Messaging
{
    using Orc.NuGetExplorer.Packaging;

    public class PackagingDeletemeMessage : NuGetExplorerServiceMessage
    {
        public PackagingDeletemeMessage()
        {
        }

        public PackagingDeletemeMessage(PackageOperationInfo content)
            : base(content)
        {
        }
    }
}
