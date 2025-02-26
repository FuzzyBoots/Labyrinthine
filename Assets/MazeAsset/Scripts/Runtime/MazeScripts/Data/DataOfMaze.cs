using System;
using UnityEngine;
namespace MazeAsset.MazeGenerator
{
    [Serializable]
    public struct DataOfMaze
    {
        [SerializeField] internal WallVisibilityStatus[] listWallsHorizontal;
        [SerializeField] internal Vector2Int sizeHorizontal;

        [SerializeField] internal WallVisibilityStatus[] listWallsVertical;
        [SerializeField] internal Vector2Int sizeVertical;

        [SerializeField] internal WallVisibilityStatus[] floorsWalls;
        [SerializeField] internal Vector2Int sizeFloors;
        internal int getIndex(int x, int z, int width)
        {
            return x + z * width;
        }
    }
}