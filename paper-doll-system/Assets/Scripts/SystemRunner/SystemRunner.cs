using Ez.SystemModule;
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

        public void Init()
        {
            foreach (var type in systemTypes)
            {
                var component = System.Activator.CreateInstance(System.Type.GetType(type)) as ISystem;
                if (component == null)
                {
                    Debug.LogError($"Failed to create instance of {type}");
                    continue;
                }
                systems.Add(component);
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

        private void Start()
        {
            Init();
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