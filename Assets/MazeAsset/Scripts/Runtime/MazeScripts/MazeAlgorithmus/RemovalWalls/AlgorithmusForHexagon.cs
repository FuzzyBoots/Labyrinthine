using System.Collections.Generic;
using UnityEngine;

namespace MazeAsset.MazeGenerator
{
    public class AlgorithmusForHexagon : MazeAlgorithmBase
    {
        public AlgorithmusForHexagon(List<WallsStateEnum[,]> mazeArray) : base(mazeArray) { }



        internal override DataOfMaze CreateNewFloor(int width, int height)
        {
            return new DataOfMaze
            {
                floorsWalls = new WallVisibilityStatus[width * height],
                listWallsVertical = new WallVisibilityStatus[(width + 1) * (height * 2 + 1)],
                listWallsHorizontal = new WallVisibilityStatus[width * (height + 1)],
                sizeVertical = new Vector2Int(width + 1, height * 2 + 1),
                sizeHorizontal = new Vector2Int(width, height + 1),
                sizeFloors = new Vector2Int(width, height),
            };
        }



        protected override List<bool> GetNeighbor(int floorIndex, int x, int y, WallsStateEnum statusForSearch)
        {
            var neighbor = new List<bool>();
            var currentFloor = mazeArray[floorIndex];
            if (currentFloor == null) return neighbor;

            int rows = currentFloor.GetLength(0);
            int cols = currentFloor.GetLength(1);

            // upper left
            int newX = x - 1;
            int newY = y + (x % 2);
            neighbor.Add(IsInBounds(newX, newY, rows, cols) && currentFloor[newX, newY] == statusForSearch); ;

            // up
            newX = x;
            newY = y + 1;
            neighbor.Add(IsInBounds(newX, newY, rows, cols) && currentFloor[newX, newY] == statusForSearch);

            // upper right
            newX = x + 1;
            newY = y + (x % 2);
            neighbor.Add(IsInBounds(newX, newY, rows, cols) && currentFloor[newX, newY] == statusForSearch);

            // lower right
            newX = x + 1;
            newY = x % 2 == 0 ? y - 1 : y;
            neighbor.Add(IsInBounds(newX, newY, rows, cols) && currentFloor[newX, newY] == statusForSearch);

            // down
            newX = x;
            newY = y - 1;
            neighbor.Add(IsInBounds(newX, newY, rows, cols) && currentFloor[newX, newY] == statusForSearch);

            // lower left
            newX = x - 1;
            newY = x % 2 == 0 ? y - 1 : y;
            neighbor.Add(IsInBounds(newX, newY, rows, cols) && currentFloor[newX, newY] == statusForSearch);

            // Check upper floor
            if (floorIndex < mazeArray.Count - 1)
            {
                var upperFloor = mazeArray[floorIndex + 1];
                if (x < upperFloor.GetLength(0) && y < upperFloor.GetLength(1) &&
                    upperFloor[x, y] == statusForSearch)
                {
                    neighbor.Add(true);
                }
                else
                {
                    neighbor.Add(false);
                }
            }
            else
            {
                neighbor.Add(false);
            }

            // Check down floor
            if (floorIndex > 0)
            {
                var lowerFloor = mazeArray[floorIndex - 1];
                if (x < lowerFloor.GetLength(0) && y < lowerFloor.GetLength(1) && lowerFloor[x, y] == statusForSearch)
                {
                    neighbor.Add(true);
                }
                else
                {
                    neighbor.Add(false);
                }
            }
            else
            {
                neighbor.Add(false);
            }
            return neighbor;

        }

        private bool IsInBounds(int x, int y, int rows, int cols)
        {
            return x >= 0 && x < rows && y >= 0 && y < cols;
        }


        protected override void RemoveWall(int direction, bool updateDirection = true)
        {
            int newX = currentRow;
            int newY = currentColumn;
            int newFloor = currentFloor;

            int widthVertical = floors[currentFloor].sizeVertical.x;
            int widthHorizontal = floors[currentFloor].sizeHorizontal.x;
            int widthFloors = floors[currentFloor].sizeFloors.x;

            // upper left
            if (direction == 0)
            {
                if (currentRow % 2 == 0)
                {
                    floors[currentFloor].listWallsVertical[floors[0].getIndex(currentRow, currentColumn * 2 + 1, widthVertical)] = WallVisibilityStatus.VisibleInEditMode;
                }
                else
                {
                    floors[currentFloor].listWallsVertical[floors[0].getIndex(currentRow, currentColumn * 2 + 2, widthVertical)] = WallVisibilityStatus.VisibleInEditMode;
                }
                newX = currentRow - 1;
                newY = currentRow % 2 == 0 ? currentColumn : currentColumn + 1;
            }
            // up
            else if (direction == 1)
            {
                floors[currentFloor].listWallsHorizontal[floors[0].getIndex(currentRow, currentColumn + 1, widthHorizontal)] = WallVisibilityStatus.VisibleInEditMode;
                newX = currentRow;
                newY = currentColumn + 1;
            }
            // upper right
            else if (direction == 2)
            {
                if (currentRow % 2 == 0)
                {
                    floors[currentFloor].listWallsVertical[floors[0].getIndex(currentRow + 1, currentColumn * 2 + 1, widthVertical)] = WallVisibilityStatus.VisibleInEditMode;
                }
                else
                {
                    floors[currentFloor].listWallsVertical[floors[0].getIndex(currentRow + 1, currentColumn * 2 + 2, widthVertical)] = WallVisibilityStatus.VisibleInEditMode;
                }
                newX = currentRow + 1;
                newY = currentRow % 2 == 0 ? currentColumn : currentColumn + 1;
            }
            // lower right
            else if (direction == 3)
            {
                if (currentRow % 2 == 0)
                {
                    floors[currentFloor].listWallsVertical[floors[0].getIndex(currentRow + 1, currentColumn * 2, widthVertical)] = WallVisibilityStatus.VisibleInEditMode;
                }
                else
                {
                    floors[currentFloor].listWallsVertical[floors[0].getIndex(currentRow + 1, currentColumn * 2 + 1, widthVertical)] = WallVisibilityStatus.VisibleInEditMode;
                }
                newX = currentRow + 1;
                newY = currentRow % 2 == 0 ? currentColumn - 1 : currentColumn;
            }
            // down
            else if (direction == 4)
            {
                floors[currentFloor].listWallsHorizontal[floors[0].getIndex(currentRow, currentColumn, widthHorizontal)] = WallVisibilityStatus.VisibleInEditMode;
                newX = currentRow;
                newY = currentColumn - 1;
            }
            // lower left
            else if (direction == 5)
            {
                if (currentRow % 2 == 0)
                {
                    floors[currentFloor].listWallsVertical[floors[0].getIndex(currentRow, currentColumn * 2, widthVertical)] = WallVisibilityStatus.VisibleInEditMode;
                }
                else
                {
                    floors[currentFloor].listWallsVertical[floors[0].getIndex(currentRow, currentColumn * 2 + 1, widthVertical)] = WallVisibilityStatus.VisibleInEditMode;
                }
                newX = currentRow - 1;
                newY = currentRow % 2 == 0 ? currentColumn - 1 : currentColumn;
            }
            // upper floor
            else if (direction == 6)
            {
                var index = floors[0].getIndex(currentRow, currentColumn, floors[currentFloor + 1].sizeFloors.x);
                floors[currentFloor + 1].floorsWalls[index] = WallVisibilityStatus.VisibleInEditMode;
                if (updateDirection)
                {
                    currentFloor++;
                }
                newFloor++;
            }
            else if (direction == 7)
            {
                var index = floors[0].getIndex(currentRow, currentColumn, widthFloors);

                floors[currentFloor].floorsWalls[index] = WallVisibilityStatus.VisibleInEditMode;
                newFloor--;
            }

            if (updateDirection)
            {
                currentRow = newX;
                currentColumn = newY;
                currentFloor = newFloor;
            }

            mazeArray[currentFloor][currentRow, currentColumn] = WallsStateEnum.Visited;

        }

        internal override void SetVisibilityWall(DataOfMaze floor, int x, int y, int width)
        {
            floor.listWallsHorizontal[floor.getIndex(x, y + 1, width)] = WallVisibilityStatus.VisibleInNormalMode; // up
            floor.listWallsHorizontal[floor.getIndex(x, y, width)] = WallVisibilityStatus.VisibleInNormalMode; // down

            floor.floorsWalls[floor.getIndex(x, y, width)] = WallVisibilityStatus.VisibleInNormalMode; // floor

            // Vertical walls
            if (x % 2 == 0)
            {
                floor.listWallsVertical[floor.getIndex(x, y * 2 + 1, width + 1)] = WallVisibilityStatus.VisibleInNormalMode;
                floor.listWallsVertical[floor.getIndex(x + 1, y * 2 + 1, width + 1)] = WallVisibilityStatus.VisibleInNormalMode;
                floor.listWallsVertical[floor.getIndex(x, y * 2, width + 1)] = WallVisibilityStatus.VisibleInNormalMode;
                floor.listWallsVertical[floor.getIndex(x + 1, y * 2, width + 1)] = WallVisibilityStatus.VisibleInNormalMode;
            }
            else
            {
                floor.listWallsVertical[floor.getIndex(x, y * 2 + 2, width + 1)] = WallVisibilityStatus.VisibleInNormalMode;
                floor.listWallsVertical[floor.getIndex(x + 1, y * 2 + 2, width + 1)] = WallVisibilityStatus.VisibleInNormalMode;
                floor.listWallsVertical[floor.getIndex(x, y * 2 + 1, width + 1)] = WallVisibilityStatus.VisibleInNormalMode;
                floor.listWallsVertical[floor.getIndex(x + 1, y * 2 + 1, width + 1)] = WallVisibilityStatus.VisibleInNormalMode;
            }
        }
    }
}
