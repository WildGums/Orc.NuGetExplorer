namespace Orc.NuGetExplorer.Messaging;

using Catel.Messaging;

public class ActivatedExplorerTabMessage : MessageBase<ActivatedExplorerTabMessage, ExplorerTab>
{
    public ActivatedExplorerTabMessage()
    {
    }

    public ActivatedExplorerTabMessage(ExplorerTab data)
        : base(data)
    {

    }
}