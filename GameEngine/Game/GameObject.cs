using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class GameObject
    {
        internal static List<GameObject> ObjectList = new List<GameObject>();

        //private List<Component> _components = new List<Component>();
        private Dictionary<Type,Component> _components = new Dictionary<Type, Component>();

        public string Name { get; set; }
        public Transform Transform { get { return GetComponeont<Transform>(); } }
        public GameObject Parent { get; set; }

        public GameObject() : this("Gameobject")
        {

        }
        public GameObject(string name)
        {
            this.Name = name;

            ObjectList.Add(this);
            AddComponent<Transform>();
        }

        public void Start()
        {

        }
        public void Update()
        {
            foreach (Component c in _components.Values)
            {
                c.Update();
            }
        }

        public void Dispose()
        {
            foreach (Component c in _components.Values)
            {
                c.Dispose();
            }
        }

        private bool ComponentExists<T>()
        {
            return _components.ContainsKey(typeof(T));
        }

        public T AddComponent<T>() where T : Component
        {
            if (ComponentExists<T>())
                return null;

            Component newComponent = Activator.CreateInstance<T>();
            _components.Add(typeof(T),newComponent);

            newComponent.GameObject = this;

            newComponent.Start();

            return (T)newComponent;
        }

        public T GetComponeont<T>() where T : Component
        {
            foreach (Component c in _components.Values)
            {
                if (c is T)
                {
                    return (T)c;
                }
            }
            return null;
        }

        public static GameObject Find(string Name)
        {
            for (int i = 0; i < ObjectList.Count; i++)
                if (ObjectList[i].Name == Name)
                    return ObjectList[i];

            return null;
            //return Engine.Current.ObjectList.First(x => x.Name == Name);
        }

        public static GameObject[] GetAllWithComponent<T>() where T : Component
        {
            GameObject[] result = new GameObject[ObjectList.Count];
            int resultIndex = 0;
            for (int i = 0; i < ObjectList.Count; i++)
                if (ObjectList[i].GetComponeont<T>() != null)
                    result[resultIndex++] = ObjectList[i];

            Array.Resize(ref result, resultIndex);

            return result;
        }

        public static T[] GetAllComponents<T>() where T : Component
        {
            T[] result = new T[ObjectList.Count];
            int resultIndex = 0; 
            for(int i = 0; i < ObjectList.Count;i++)
            {
                T comp = ObjectList[i].GetComponeont<T>();
                if (comp != null)
                    result[resultIndex++] = comp;
            }

            Array.Resize(ref result, resultIndex);
            return result;
        }
    }
}
