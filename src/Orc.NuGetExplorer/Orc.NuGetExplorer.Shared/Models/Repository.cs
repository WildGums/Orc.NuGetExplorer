// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Repository.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    public class Repository : IRepository
    {
        #region Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public PackageOperationType OperationType { get; set; }
        #endregion

        #region Methods
        private bool Equals(Repository other)
        {
            return Id == other.Id && string.Equals(Name, other.Name) && OperationType == other.OperationType;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) OperationType;
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