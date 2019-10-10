// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Repository.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer
{
    public sealed class Repository : IRepository
    {
        #region Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public PackageOperationType OperationType { get; set; }
        #endregion

        #region Methods
        private bool Equals(Repository other)
        {
            return Id == other.Id && string.Equals(Name, other.Name) && string.Equals(Source, other.Source) && OperationType == other.OperationType;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Source != null ? Source.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)OperationType;
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            var repository = obj as Repository;
            if (repository == null)
            {
                return false;
            }

            return Equals(repository);
        }
        #endregion
    }
}
