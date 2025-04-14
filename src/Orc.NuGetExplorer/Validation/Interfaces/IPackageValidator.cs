namespace Orc.NuGetExplorer
{
    using Catel.Data;
    using NuGet.Protocol.Core.Types;

    public interface IPackageValidator
    {
        IValidationContext Validate(IPackageSearchMetadata packageSearchMetadata);
    }
}
