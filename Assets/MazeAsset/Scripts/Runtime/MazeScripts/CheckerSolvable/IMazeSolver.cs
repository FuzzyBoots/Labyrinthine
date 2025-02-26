using System.Collections.Generic;
using UnityEngine;
namespace MazeAsset.MazeGenerator
{
    public interface IMazeSolver
    {
        List<Vector3Int> FindPath(Vector3Int start, Vector3Int target);
    }
}
