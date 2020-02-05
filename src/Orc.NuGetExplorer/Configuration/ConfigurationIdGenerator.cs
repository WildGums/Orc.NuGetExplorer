namespace Orc.NuGetExplorer.Configuration
{
    using System;
    using System.Collections.Generic;

    internal static class ConfigurationIdGenerator
    {
        /// <summary>
        /// The dictionary containing the unique identifiers per type.
        /// </summary>
        private static readonly HashSet<Guid> OccupiedIdentifiers = new HashSet<Guid>();

        private static readonly object LockObject = new object();

        private static readonly Dictionary<Guid, Guid> RemappedValues = new Dictionary<Guid, Guid>();

        /// <summary>
        /// Gets a unique identifier for the specified type.
        /// </summary>
        public static Guid GetUniqueIdentifier()
        {
            lock (LockObject)
            {
                while (true)
                {
                    var guid = Guid.NewGuid();

                    if (!OccupiedIdentifiers.Add(guid))
                    {
                        return guid;
                    }
                }
            }
        }

        public static bool TryTakeUniqueIdentifier(Guid guid, out Guid collisionResolve)
        {
            lock (LockObject)
            {
                bool isFree = true;

                var actualGuid = guid;

                isFree = !RemappedValues.TryGetValue(guid, out collisionResolve);

                if (OccupiedIdentifiers.Contains(isFree ? guid : collisionResolve))
                {
                    isFree = false;
                    collisionResolve = GetUniqueIdentifier();

                    RemappedValues[guid] = collisionResolve;
                }

                OccupiedIdentifiers.Add(guid);

                return isFree;
            }
        }

        public static Guid GetOriginalIdentifier(Guid gui)
        {
            lock (LockObject)
            {
                if (RemappedValues.TryGetValue(gui, out Guid collidedGuid))
                {
                    return collidedGuid;
                }

                throw new InvalidOperationException("No collisions stored for this identifier");
            }
        }

        public static bool IsCollision(Guid guid)
        {
            return RemappedValues.ContainsKey(guid);
        }
    }
}
