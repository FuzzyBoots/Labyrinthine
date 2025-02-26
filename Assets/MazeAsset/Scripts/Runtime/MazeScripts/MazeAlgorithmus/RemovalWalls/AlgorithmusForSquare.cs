using System;
using System.Collections.Generic;
using UnityEngine;

namespace MazeAsset.MazeGenerator
{
    public class AlgorithmusForSquare : MazeAlgorithmBase
    {
        public AlgorithmusForSquare(List<WallsStateEnum[,]> mazeArray) : base(mazeArray) { }

        protected override List<bool> GetNeighbor(int floorIndex, int x, int y, WallsStateEnum statusForSearch)
        {
            var neighbors = new List<bool>();
            var currentFloor = mazeArray[floorIndex];
            if (currentFloor == null) return neighbors;

            // Check up
            neighbors.Add(y < (currentFloor.GetLength(1) - 1) && currentFloor[x, y + 1] == statusForSearch);

            // Check right
            neighbors.Add(x < (currentFloor.GetLength(0) - 1) && currentFloor[x + 1, y] == statusForSearch);

            // Check down
            neighbors.Add(y > 0 && currentFloor[x, y - 1] == statusForSearch);

            // Check left
            neighbors.Add(x > 0 && currentFloor[x - 1, y] == statusForSearch);

            // Check upper floor
            if (floorIndex < mazeArray.Count - 1)
            {
                var upperFloor = mazeArray[floorIndex + 1];
                neighbors.Add(x < upperFloor.GetLength(0) && y < upperFloor.GetLength(1) &&
                              upperFloor[x, y] == statusForSearch);
            }
            else
            {
                neighbors.Add(false);
            }

            // Check lower floor
            if (floorIndex > 0)
            {
                var lowerFloor = mazeArray[floorIndex - 1];
                neighbors.Add(x < lowerFloor.GetLength(0) && y < lowerFloor.GetLength(1) &&
                              lowerFloor[x, y] == statusForSearch);
            }
            else
            {
                neighbors.Add(false);
            }

            return neighbors;
        }

        protected override void RemoveWall(int direction, bool updateDirection = true)
        {
            int widthHorizontal = floors[currentFloor].sizeHorizontal.x;

            int widthVertical = floors[currentFloor].sizeVertical.x;
            int widthFloors = floors[currentFloor].sizeFloors.x;

            if (direction == 0) // up
            {
                var upIndex = floors[currentFloor].getIndex(currentRow, currentColumn + 1, widthHorizontal);
                floors[currentFloor].listWallsHorizontal[upIndex] = WallVisibilityStatus.VisibleInEditMode;
                if (updateDirection) currentColumn++;
            }
            else if (direction == 1) // right
            {
                var rightIndex = floors[currentFloor].getIndex(currentRow + 1, currentColumn, widthVertical);
                floors[currentFloor].listWallsVertical[rightIndex] = WallVisibilityStatus.VisibleInEditMode;
                if (updateDirection) currentRow++;
            }
            else if (direction == 2) // down
            {
                var downIndex = floors[currentFloor].getIndex(currentRow, currentColumn, widthHorizontal);
                floors[currentFloor].listWallsHorizontal[downIndex] = WallVisibilityStatus.VisibleInEditMode;
                if (updateDirection) currentColumn--;
            }
            else if (direction == 3) // left
            {
                var leftIndex = floors[currentFloor].getIndex(currentRow, currentColumn, widthVertical);
                floors[currentFloor].listWallsVertical[leftIndex] = WallVisibilityStatus.VisibleInEditMode;
                if (updateDirection) currentRow--;
            }
            else if (direction == 4) // upper floor
            {
                var upperFloorIndex = floors[currentFloor + 1].getIndex(currentRow, currentColumn, floors[currentFloor + 1].sizeFloors.x);
                floors[currentFloor + 1].floorsWalls[upperFloorIndex] = WallVisibilityStatus.VisibleInEditMode;
                if (updateDirection) currentFloor++;
            }
            else if (direction == 5) // lower floor
            {
                var lowerFloorIndex = floors[currentFloor].getIndex(currentRow, currentColumn, widthFloors);
                floors[currentFloor].floorsWalls[lowerFloorIndex] = WallVisibilityStatus.VisibleInEditMode;
                if (updateDirection) currentFloor--;
            }
            mazeArray[currentFloor][currentRow, currentColumn] = WallsStateEnum.Visited;
        }

        internal override DataOfMaze CreateNewFloor(int width, int heigh)
        {
            return new DataOfMaze
            {
                floorsWalls = new WallVisibilityStatus[width * heigh],
                listWallsHorizontal = new WallVisibilityStatus[width * (heigh + 1)],
                listWallsVertical = new WallVisibilityStatus[(width + 1) * heigh],
                sizeHorizontal = new Vector2Int(width, heigh + 1),
                sizeVertical = new Vector2Int(width + 1, heigh),
                sizeFloors = new Vector2Int(width, heigh)
            };
        }

        internal override void SetVisibilityWall(DataOfMaze floor, int x, int y, int width)
        {
            floor.listWallsHorizontal[floor.getIndex(x, y + 1, width)] = WallVisibilityStatus.VisibleInNormalMode; // up
            floor.listWallsVertical[floor.getIndex(x + 1, y, width + 1)] = WallVisibilityStatus.VisibleInNormalMode; // right
            floor.listWallsHorizontal[floor.getIndex(x, y, width)] = WallVisibilityStatus.VisibleInNormalMode; // down
            floor.listWallsVertical[floor.getIndex(x, y, width + 1)] = WallVisibilityStatus.VisibleInNormalMode; // left
            floor.floorsWalls[floor.getIndex(x, y, width)] = WallVisibilityStatus.VisibleInNormalMode; // floor
        }
    }
}