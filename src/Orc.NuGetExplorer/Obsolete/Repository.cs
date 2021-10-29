// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Repository.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.NuGetExplorer
{
    using Catel;
    using NuGet.Configuration;

    [ObsoleteEx(ReplacementTypeOrMember = "NuGet.Configuration.PackageSource/NuGet.Protocol.Core.Types.SourceRepository", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
    public sealed class Repository : IRepository
    {
        public Repository(string name, string source)
            : this(new PackageSource(source, name))
        {
            Argument.IsNotNull(() => name);
            Argument.IsNotNull(() => source);
        }

        public Repository(PackageSource packageSource)
        {
            PackageSource = packageSource;
        }

        #region Properties
        public int Id { get; set; }
        public string Name => PackageSource.Name;
        public string Source => PackageSource.Source;

        public bool IsLocal => PackageSource.IsLocal;

        public PackageSource PackageSource { get; set; }
        #endregion

        #region Methods
        private bool Equals(Repository other)
        {
            return Id == other.Id && string.Equals(Name, other.Name)
                && string.Equals(Source, other.Source);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (Name is not null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Source is not null ? Source.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            var repository = obj as Repository;
            if (repository is null)
            {
                return false;
            }

            return Equals(repository);
        }
        #endregion
    }
}
