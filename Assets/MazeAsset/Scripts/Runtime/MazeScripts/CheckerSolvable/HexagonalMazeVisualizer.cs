using System.Collections.Generic;
using UnityEngine;

namespace MazeAsset.MazeGenerator
{
    internal class HexagonalMazeVisualizer : MazeVisualizerBase
    {
        private Vector2 scaler;
        public override void Visualize(Transform parent, List<Vector3Int> points, Vector2 scaler)
        {
            this.scaler = scaler;
            for (int i = 0; i < points.Count; i++)
            {
                Vector3 position = HexToWorldPosition(points[i]);
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.localScale = Vector3.one * 0.2f;
                sphere.transform.localPosition = position;
                sphere.transform.parent = parent;

                if (i < points.Count - 1)
                {
                    Vector3 nextPosition = HexToWorldPosition(points[i + 1]);
                    CreateConnectingCube(position, nextPosition, parent);
                }
            }
        }

        private Vector3 HexToWorldPosition(Vector3Int hex)
        {
            float x = (hex.x * 1.5f + 0.75f) * scaler.x;
            float z = (hex.z * Mathf.Sqrt(3) + 0.5f) * scaler.x;

            if (hex.x % 2 == 1)
            {
                z += Mathf.Sqrt(3) / 2 * scaler.x;
            }

            return new Vector3(x, hex.y * scaler.y, z);
        }
    }
}