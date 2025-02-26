using System.Collections.Generic;
using UnityEngine;
namespace MazeAsset.MazeGenerator
{
    public class SquareMazeSolver : MazeSolverBase
    {
        internal SquareMazeSolver(List<DataOfMaze> mazeData) : base(mazeData) { }

        protected override List<Vector3Int> GetNeighbors(Vector3Int cell)
        {
            int x = cell.x;
            int y = cell.z;
            int floorIndex = cell.y;

            var neighbors = new List<Vector3Int>();
            var floor = mazeData[floorIndex];

            // Up
            if (floor.listWallsHorizontal[floor.getIndex(x, y + 1, floor.sizeHorizontal.x)] == WallVisibilityStatus.VisibleInEditMode)
            {
                neighbors.Add(new Vector3Int(x, floorIndex, y + 1));
            }

            // Right
            if (floor.listWallsVertical[floor.getIndex(x + 1, y, floor.sizeVertical.x)] == WallVisibilityStatus.VisibleInEditMode)
            {
                neighbors.Add(new Vector3Int(x + 1, floorIndex, y));
            }

            // Down
            if (floor.listWallsHorizontal[floor.getIndex(x, y, floor.sizeHorizontal.x)] == WallVisibilityStatus.VisibleInEditMode)
            {
                neighbors.Add(new Vector3Int(x, floorIndex, y - 1));
            }

            // Left
            if (floor.listWallsVertical[floor.getIndex(x, y, floor.sizeVertical.x)] == WallVisibilityStatus.VisibleInEditMode)
            {
                neighbors.Add(new Vector3Int(x - 1, floorIndex, y));
            }

            // Upper floor
            if (floorIndex < mazeData.Count - 1 &&
                mazeData[floorIndex + 1].sizeFloors.x > x &&
                mazeData[floorIndex + 1].sizeFloors.y > y &&
                mazeData[floorIndex + 1].floorsWalls[floor.getIndex(x, y, mazeData[floorIndex + 1].sizeFloors.x)] == WallVisibilityStatus.VisibleInEditMode)
            {
                neighbors.Add(new Vector3Int(x, floorIndex + 1, y));
            }

            // Downer floor
            if (floor.floorsWalls[floor.getIndex(x, y, floor.sizeFloors.x)] == WallVisibilityStatus.VisibleInEditMode)
            {
                neighbors.Add(new Vector3Int(x, floorIndex - 1, y));
            }

            return neighbors;
        }
    }
}