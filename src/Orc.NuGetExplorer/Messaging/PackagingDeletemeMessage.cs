namespace Orc.NuGetExplorer.Messaging
{
    using Catel.Messaging;
    using Orc.NuGetExplorer.Packaging;

    public class PackagingDeletemeMessage : MessageBase<PackagingDeletemeMessage, PackageOperationInfo>, INuGetExplorerMessage
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
