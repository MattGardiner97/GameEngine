using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class GameObject
    {
        private List<Component> _components = new List<Component>();

        public string Name
        {
            get;
            set;
        }
        public Transform Transform
        {
            get
            {
                return GetComponeont<Transform>();
            }
        }

        public GameObject() : this("Gameobject")
        {

        }
        public GameObject(string name)
        {
            this.Name = name;

            Engine.ObjectList.Add(this);
            AddComponent<Transform>();
        }

        public void Start()
        {

        }
        public void Update()
        {
            foreach(Component c in _components)
            {
                c.Update();
            }
        }
        public void Draw()
        {
            foreach(Component c in _components)
            {
                c.Draw();
            }
        }

        public void Dispose()
        {
            foreach(Component c in _components)
            {
                c.Dispose();
            }
        }

        public void AddComponent<T>() where T : Component
        {
            if(_components.Any(x => x is T))
            {
                return;
            }

            Component c = Activator.CreateInstance<T>();
            _components.Add(c);

            c._parent = this;

            c.Start();
        }

        public T GetComponeont<T>() where T : Component
        {
            foreach(Component c in _components)
            {
                if(c is T)
                {
                    return (T)c;
                }
            }
            return null;
        }
    }
}
