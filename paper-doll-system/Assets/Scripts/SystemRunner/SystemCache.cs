using System.Collections.Generic;
using System.Linq;

namespace Ez.SystemModule
{
    public static class SystemCache
    {
        private static List<ISystem> systems = new List<ISystem>();

        public static void Register(ISystem system)
        {
            systems.Add(system);
        }

        public static void Unregister(ISystem system)
        {
            systems.Remove(system);
        }

        public static bool TryGetSystem<T>(out T system) where T : class, ISystem
        {
            system = systems.FirstOrDefault(x => x is T) as T;
            return system != null;
        }

        public static T GetT<T>() where T : class, ISystem
        {
            return systems.FirstOrDefault(x => x is T) as T;
        }
    }

    public static class SystemCache<T> where T : class, ISystem
    {
        private static T system;

        public static T System
        {
            get
            {
                if (system == null)
                {
                    SystemCache.TryGetSystem(out system);
                }
                return system;
            }
        }
    }
}