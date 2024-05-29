using Ez.SystemModule;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ez.SystemModule
{
    public interface ISystem
    {
        void Init();
        void Update();
        void Uninit();
    }

    public class ExecuteOrderAttribute : System.Attribute
    {
        public int Order;
        public ExecuteOrderAttribute(int order)
        {
            this.Order = order;
        }
    }

    public class SystemRunner : MonoBehaviour
    {
        public List<string> systemTypes = new List<string>();
        private List<ISystem> systems = new List<ISystem>();

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            foreach (var type in systemTypes)
            {
                var t = GetType(type);
                var system = Activator.CreateInstance(t) as ISystem;
                if (system == null)
                {
                    Debug.LogError($"Failed to create instance of {type}");
                    continue;
                }
                systems.Add(system);
            }

            systems = systems.OrderBy(x =>
            {
                var attr = x.GetType().GetCustomAttributes(typeof(ExecuteOrderAttribute), false).FirstOrDefault() as ExecuteOrderAttribute;
                return attr == null ? 0 : attr.Order;
            }).ToList();

            systems.ForEach(x =>
            {
                SystemCache.Register(x);
            });

            systems.ForEach(x => x.Init());
        }

        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) 
                return type;

            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }

        private void Update()
        {
            systems.ForEach(x => x.Update());
        }

        private void OnDestroy()
        {
            Uninit();
        }

        public void Uninit()
        {
            systems.ForEach(x => x.Uninit());

            systems.ForEach(x =>
            {
                SystemCache.Unregister(x);
            });
        }
    }
}