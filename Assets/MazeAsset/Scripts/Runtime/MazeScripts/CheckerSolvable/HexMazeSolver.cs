using System.Collections.Generic;
using UnityEngine;
namespace MazeAsset.MazeGenerator
{
    public class HexMazeSolver : MazeSolverBase
    {
        internal HexMazeSolver(List<DataOfMaze> mazeData) : base(mazeData) { }

        protected override List<Vector3Int> GetNeighbors(Vector3Int cell)
        {
            int x = cell.x;
            int y = cell.z;
            int floorIndex = cell.y;

            int newy = y * 2 + x % 2;

            var neighbors = new List<Vector3Int>();
            var floor = mazeData[floorIndex];

            // Check up neighbor
            if (floor.listWallsHorizontal[floor.getIndex(x, y + 1, floor.sizeHorizontal.x)] == WallVisibilityStatus.VisibleInEditMode)
            {
                neighbors.Add(new Vector3Int(x, floorIndex, y + 1));
            }

            // Check upper right neighbor
            if (floor.listWallsVertical[floor.getIndex(x + 1, newy + 1, floor.sizeVertical.x)] == WallVisibilityStatus.VisibleInEditMode)
            {
                neighbors.Add(new Vector3Int(x + 1, floorIndex, y + x % 2));
            }

            // Check down right neighbor
            if (floor.listWallsVertical[floor.getIndex(x + 1, newy, floor.sizeVertical.x)] == WallVisibilityStatus.VisibleInEditMode)
            {
                neighbors.Add(new Vector3Int(x + 1, floorIndex, y - (x + 1) % 2));
            }

            // Check down neighbor
            if (floor.listWallsHorizontal[floor.getIndex(x, y, floor.sizeHorizontal.x)] == WallVisibilityStatus.VisibleInEditMode)
            {
                neighbors.Add(new Vector3Int(x, floorIndex, y - 1));
            }

            // Check down left neighbor
            if (floor.listWallsVertical[floor.getIndex(x, newy, floor.sizeVertical.x)] == WallVisibilityStatus.VisibleInEditMode)
            {
                neighbors.Add(new Vector3Int(x - 1, floorIndex, y - (x + 1) % 2));
            }

            // Check upper left neighbor
            if (floor.listWallsVertical[floor.getIndex(x, newy + 1, floor.sizeVertical.x)] == WallVisibilityStatus.VisibleInEditMode)
            {
                neighbors.Add(new Vector3Int(x - 1, floorIndex, y + x % 2));
            }

            // Check upper floor
            if (floorIndex < mazeData.Count - 1 &&
                mazeData[floorIndex + 1].sizeFloors.x > x &&
                mazeData[floorIndex + 1].sizeFloors.y > y &&
                mazeData[floorIndex + 1].floorsWalls[floor.getIndex(x, y, mazeData[floorIndex + 1].sizeFloors.x)] == WallVisibilityStatus.VisibleInEditMode)
            {
                neighbors.Add(new Vector3Int(x, floorIndex + 1, y));
            }

            // Check lower floor
            if (floor.floorsWalls[floor.getIndex(x, y, floor.sizeFloors.x)] == WallVisibilityStatus.VisibleInEditMode)
            {
                neighbors.Add(new Vector3Int(x, floorIndex - 1, y));
            }

            return neighbors;
        }
    }
}
