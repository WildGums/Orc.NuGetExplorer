namespace Orc.NuGetExplorer.Configuration
{
    using System;
    using System.Collections.Generic;

    public static class ConfigurationIdGenerator
    {
        /// <summary>
        /// The dictionary containing the unique identifiers per type.
        /// </summary>
        private static readonly HashSet<Guid> _occupiedIdenitfiers = new HashSet<Guid>();

        private static readonly object _lockObject = new object();

        private static readonly Dictionary<Guid, Guid> _remappedValues = new Dictionary<Guid, Guid>();

        /// <summary>
        /// Gets a unique identifier for the specified type.
        /// </summary>
        public static Guid GetUniqueIdentifier()
        {
            lock (_lockObject)
            {
                while (true)
                {
                    var guid = Guid.NewGuid();

                    if (!_occupiedIdenitfiers.Contains(guid))
                    {
                        _occupiedIdenitfiers.Add(guid);
                        return guid;
                    }
                }
            }
        }

        public static bool TryTakeUniqueIdentifier(Guid guid, out Guid collisionResolve)
        {
            lock (_lockObject)
            {
                bool isFree = true;

                var actualGuid = guid;

                isFree = !_remappedValues.TryGetValue(guid, out collisionResolve);

                if (_occupiedIdenitfiers.Contains(isFree ? guid : collisionResolve))
                {
                    isFree = false;
                    collisionResolve = GetUniqueIdentifier();

                    _remappedValues[guid] = collisionResolve;
                }

                _occupiedIdenitfiers.Add(guid);

                return isFree;
            }
        }

        public static Guid GetOriginalIdentifier(Guid gui)
        {
            lock (_lockObject)
            {
                if (_remappedValues.TryGetValue(gui, out Guid collidedGuid))
                {
                    return collidedGuid;
                }
                else throw new InvalidOperationException("No collisions stored for this identifier");
            }
        }

        public static bool IsCollision(Guid guid)
        {
            return _remappedValues.ContainsKey(guid);
        }
    }
}
