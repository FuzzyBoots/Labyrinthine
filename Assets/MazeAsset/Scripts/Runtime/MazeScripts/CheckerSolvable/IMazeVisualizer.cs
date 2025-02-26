using System.Collections.Generic;
using UnityEngine;
namespace MazeAsset.MazeGenerator
{
    public interface IMazeVisualizer
    {
        void Visualize(Transform parent, List<Vector3Int> points, Vector2 scaler);
    }
}
