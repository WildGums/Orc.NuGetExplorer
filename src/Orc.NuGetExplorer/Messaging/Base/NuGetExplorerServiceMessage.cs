namespace Orc.NuGetExplorer.Messaging
{
    using Catel.Messaging;
    using Orc.NuGetExplorer.Packaging;

    public class NuGetExplorerServiceMessage : MessageBase<NuGetExplorerServiceMessage, PackageOperationInfo>
    {
        public NuGetExplorerServiceMessage()
        {

        }

        public NuGetExplorerServiceMessage(PackageOperationInfo content)
            : base(content)
        {

        }
    }
}
