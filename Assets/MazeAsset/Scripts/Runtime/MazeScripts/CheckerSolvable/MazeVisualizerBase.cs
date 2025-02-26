using System.Collections.Generic;
using UnityEngine;

namespace MazeAsset.MazeGenerator
{
    public abstract class MazeVisualizerBase : IMazeVisualizer
    {
        public abstract void Visualize(Transform parent, List<Vector3Int> points, Vector2 scaler);

        protected void CreateConnectingCube(Vector3 start, Vector3 end, Transform parent)
        {
            Vector3 direction = end - start;
            float distance = direction.magnitude;

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(0.1f, distance, 0.1f);
            cube.transform.position = start + direction / 2;
            cube.transform.up = direction.normalized;
            cube.transform.parent = parent;
        }
    }
}
