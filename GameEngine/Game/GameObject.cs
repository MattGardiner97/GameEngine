using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class GameObject
    {
        private List<Component> _components = new List<Component>();

        public string Name { get; set; }
        public Transform Transform { get { return GetComponeont<Transform>(); } }
        public GameObject Parent { get; set; }

        public GameObject() : this("Gameobject")
        {

        }
        public GameObject(string name)
        {
            this.Name = name;

            Engine.Current.ObjectList.Add(this);
            AddComponent<Transform>();
        }

        public void Start()
        {

        }
        public void Update()
        {
            foreach (Component c in _components)
            {
                c.Update();
            }
        }
        public void Draw()
        {
            foreach (Component c in _components)
            {
                c.Draw();
            }
        }

        public void Dispose()
        {
            foreach (Component c in _components)
            {
                c.Dispose();
            }
        }

        private bool ComponentExists<T>()
        {
            for (int i = 0; i < _components.Count; i++)
                if (_components[i] is T)
                    return true;

            return false;
        }

        public T AddComponent<T>() where T : Component
        {
            if (ComponentExists<T>())
                return null;

            Component newComponent = Activator.CreateInstance<T>();
            _components.Add(newComponent);

            newComponent.GameObject = this;

            newComponent.Start();

            return (T)newComponent;
        }

        public T GetComponeont<T>() where T : Component
        {
            foreach (Component c in _components)
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
            for (int i = 0; i < Engine.Current.ObjectList.Count; i++)
                if (Engine.Current.ObjectList[i].Name == Name)
                    return Engine.Current.ObjectList[i];

            return null;
            //return Engine.Current.ObjectList.First(x => x.Name == Name);
        }
    }
}
