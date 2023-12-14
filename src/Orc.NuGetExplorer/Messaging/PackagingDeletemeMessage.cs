namespace Orc.NuGetExplorer.Messaging;

using Catel.Messaging;
using Packaging;

public class PackagingDeletemeMessage : MessageBase<PackagingDeletemeMessage, PackageOperationInfo>, INuGetExplorerServiceMessage
{
    public PackagingDeletemeMessage()
    {

    }

    public PackagingDeletemeMessage(PackageOperationInfo content)
        : base(content)
    {

    }
}
