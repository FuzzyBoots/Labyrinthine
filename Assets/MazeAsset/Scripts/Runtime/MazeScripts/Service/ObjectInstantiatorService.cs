using UnityEngine;

namespace MazeAsset.MazeGenerator
{
    internal class ObjectInstantiatorService
    {
        internal GameObject InstantiateObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, string Name, Material material)
        {
            var t = Object.Instantiate(prefab);
            t.name = Name;
            t.transform.parent = parent;
            t.transform.localPosition = position;
            t.transform.localRotation = rotation;
            var renderer = t.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }
            return t;
        }
    }
}