namespace Orc.NuGetExplorer.Models
{
    using System;

    internal interface IObservablePackage
    {
        event EventHandler<PackageModelStatusEventArgs> StatusChanged;
        bool IsDelisted { get; }
    }
}
