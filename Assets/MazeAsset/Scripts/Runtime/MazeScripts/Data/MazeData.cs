using System.Collections.Generic;
using System;
using UnityEngine;
using System.Reflection;
namespace MazeAsset.MazeGenerator
{
    [System.Serializable]
    public class MazeData
    {
        [SerializeField]
        public List<DataOfMaze> floors = new List<DataOfMaze>();

        [NonSerialized]

        public List<WallsStateEnum[,]> mazeArray;


        public bool NeedsRoof(int numberFloor, int x, int z)
        {
            var floorWall = GetFloor(numberFloor).floorsWalls;
            if (floorWall[getIndex(x, z, GetFloor(numberFloor).sizeFloors.x)] == WallVisibilityStatus.Empty)
            {
                return false;
            }
            if (numberFloor == floors.Count - 1) return true;

            if (numberFloor < floors.Count - 1)
            {
                var upperFloor = GetFloor(numberFloor + 1).floorsWalls;
                var dimSize = GetFloor(numberFloor + 1).sizeFloors;
                if (x >= 0 && x < dimSize.x && z >= 0 && z < dimSize.y)
                {
                    if (upperFloor[getIndex(x, z, dimSize.x)] == WallVisibilityStatus.Empty)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public DataOfMaze GetFloor(int index)
        {
            return floors[index];
        }

        public void ClearAllData()
        {
            floors.Clear();
            mazeArray?.Clear();
        }
        internal int getIndex(int x, int z, int width)
        {
            if (x < 0 || z < 0 || x >= width)
            {
                Debug.LogError($"Invalid index calculation: x={x}, y={z}, width={width}");
                return -1;
            }
            return x + z * width;
        }

        internal bool NeedElevator(int numberFloor, int x, int y)
        {
            if (floors.Count <= numberFloor || numberFloor < 0)
                return false;
            var floor = GetFloor(numberFloor);
            if (floors.Count > numberFloor &&
                x >= 0 && x < floor.sizeFloors.x &&
                y >= 0 && y < floor.sizeFloors.y &&
                floor.floorsWalls[getIndex(x, y, floor.sizeFloors.x)] == WallVisibilityStatus.VisibleInEditMode
                )
            {
                return true;
            }
            return false;
        }
    }
}