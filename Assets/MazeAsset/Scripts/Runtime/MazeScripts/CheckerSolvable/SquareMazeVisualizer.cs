using System;
using System.Collections.Generic;
using UnityEngine;

namespace MazeAsset.MazeGenerator
{
    public class SquareMazeVisualizer : MazeVisualizerBase
    {
        Vector2 scaler;
        public override void Visualize(Transform parent, List<Vector3Int> points, Vector2 scaler)
        {
            this.scaler = scaler;
            for (int i = 0; i < points.Count; i++)
            {

                Vector3 position = ApplyScaler(points[i]);
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.localScale = Vector3.one * 0.2f;
                sphere.transform.localPosition = position;
                sphere.transform.parent = parent;

                if (i < points.Count - 1)
                {
                    Vector3 nextPosition = ApplyScaler(points[i + 1]);
                    CreateConnectingCube(position, nextPosition, parent);
                }
            }
        }

        private Vector3 ApplyScaler(Vector3Int vector3Int)
        {
            return new Vector3(vector3Int.x * scaler.x, vector3Int.y * scaler.y, (vector3Int.z + 0.5f) * scaler.x);
        }
    }
}