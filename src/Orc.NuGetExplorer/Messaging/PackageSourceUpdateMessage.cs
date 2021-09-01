namespace Orc.NuGetExplorer.Messaging
{
    using Catel.Messaging;
    using NuGet.Configuration;

    public class PackageSourceUpdateMessage : MessageBase<PackageSourceUpdateMessage, PackageSource>, INuGetExplorerMessage
    {
        public PackageSourceUpdateMessage()
        {
        }

        public PackageSourceUpdateMessage(PackageSource data) : base(data)
        {
        }
    }
}
