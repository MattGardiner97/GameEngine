using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class Layer
    {
        private static List<Layer> _layers = new List<Layer>();
        static Layer()
        {
            CreateLayer("default");
        }

        private List<GameObject> _layerObjects = new List<GameObject>();
        public string LayerName { get; private set; }
        public int LayerIndex { get; private set; }

        public static void CreateLayer(string LayerName)
        {
            //If a layer exists with the same name
            if (_layers.Any(x => x.LayerName == LayerName))
                return;

            Layer newLayer = new Layer();
            newLayer.LayerName = LayerName;
            newLayer.LayerIndex = _layers.Count;

            _layers.Add(newLayer);
        }

        public static Layer SetObjectLayer(GameObject go, int LayerIndex)
        {
            if (LayerIndex < 0 || LayerIndex >= _layers.Count)
                throw new Exception("Invalid LayerIndex");

            if (go.Layer != null)
            {
                if (go.Layer.LayerIndex == LayerIndex)
                    return go.Layer;

                go.Layer._layerObjects.Remove(go);
            }

            Layer l = _layers[LayerIndex];
            l._layerObjects.Add(go);

            return l;
        }
        public static Layer SetObjectLayer(GameObject go, string LayerName)
        {
            int layerIndex = _layers.First(x => x.LayerName == LayerName).LayerIndex;
            return SetObjectLayer(go, layerIndex);
        }
        public static GameObject[] GetAllObjectsInLayer(int LayerIndex)
        {
            return _layers[LayerIndex]._layerObjects.ToArray();
        }
        public static GameObject[] GetAllObjectsInLayer(string LayerName)
        {
            int layerIndex = _layers.First(x => x.LayerName == LayerName).LayerIndex;
            return GetAllObjectsInLayer(layerIndex);
        }



    }
}
