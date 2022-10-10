namespace Orc.NuGetExplorer
{
    using System;

    public sealed class Repository : IRepository
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public PackageOperationType OperationType { get; set; }

        public bool IsLocal => new Uri(Source)?.IsLoopback ?? false;

        private bool Equals(Repository other)
        {
            return Id == other.Id && string.Equals(Name, other.Name)
                && string.Equals(Source, other.Source)
                && OperationType == other.OperationType;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (Name is not null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Source is not null ? Source.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)OperationType;
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
    }
}
