using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class GameObject
    {
        private static List<GameObject> ObjectList = new List<GameObject>();
        private static List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();
        private static MeshRenderer[] _rendererCache;

        //private List<Component> _components = new List<Component>();
        private List<Component> _components = new List<Component>();

        #region Properties
        public string Name { get; set; }
        public Transform Transform { get { return GetComponent<Transform>(); } }
        public GameObject Parent { get; set; }
        public Layer Layer { get; private set; }
        #endregion

        #region Constructors
        public GameObject() : this("Gameobject")
        {

        }
        public GameObject(string name)
        {
            this.Name = name;

            ObjectList.Add(this);
            AddComponent<Transform>();

            this.Layer = Layer.SetObjectLayer(this, 0);
        }
        #endregion

        #region Methods
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

        public void Dispose()
        {
            foreach (Component c in _components)
            {
                c.Dispose();
            }
        }

        public bool HasComponent<T>()
        {
            return _components.Any(x => x.GetType() == typeof(T));
        }

        public T AddComponent<T>() where T : Component
        {
            if (HasComponent<T>())
                return null;

            Component newComponent = Activator.CreateInstance<T>();
            _components.Add(newComponent);

            if (typeof(T) == typeof(MeshRenderer))
            {
                _meshRenderers.Add((MeshRenderer)newComponent);
                _rendererCache = _meshRenderers.ToArray();
            }

            newComponent.GameObject = this;

            newComponent.Start();

            return (T)newComponent;
        }

        public T GetComponent<T>() where T : Component
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

        public void SetLayer(int LayerIndex)
        {
            this.Layer = Layer.SetObjectLayer(this, LayerIndex);
        }
        public void SetLayer(string LayerName)
        {
            this.Layer = Layer.SetObjectLayer(this, LayerName);
        }
        #endregion

        #region StaticMethods
        public static GameObject Find(string Name)
        {
            for (int i = 0; i < ObjectList.Count; i++)
                if (ObjectList[i].Name == Name)
                    return ObjectList[i];

            return null;
        }

        public static GameObject[] GetAllWithComponent<T>() where T : Component
        {
            GameObject[] result = new GameObject[ObjectList.Count];
            int resultIndex = 0;
            for (int i = 0; i < ObjectList.Count; i++)
                if (ObjectList[i].GetComponent<T>() != null)
                    result[resultIndex++] = ObjectList[i];

            Array.Resize(ref result, resultIndex);

            return result;
        }

        public static T[] GetAllComponents<T>() where T : Component
        {
            T[] result = new T[ObjectList.Count];
            int resultIndex = 0;
            for (int i = 0; i < ObjectList.Count; i++)
            {
                T comp = ObjectList[i].GetComponent<T>();
                if (comp != null)
                    result[resultIndex++] = comp;
            }

            Array.Resize(ref result, resultIndex);
            return result;
        }

        public static GameObject[] GetAllObjects()
        {
            return ObjectList.ToArray();
        }

        public static MeshRenderer[] GetAllMeshRenderers()
        {
            return _rendererCache;
        }

        public static void UpdateAll()
        {
            foreach (GameObject go in ObjectList)
                go.Update();
        }
        #endregion
    }
}
