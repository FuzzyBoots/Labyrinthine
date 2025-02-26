using System.Collections.Generic;
using System.Diagnostics;

namespace MazeAsset.MazeGenerator
{
    public abstract class MazeAlgorithmBase
    {
        protected List<WallsStateEnum[,]> mazeArray;
        internal List<DataOfMaze> floors = new List<DataOfMaze>();
        protected int currentRow;
        protected int currentColumn;
        protected int currentFloor;

        protected MazeAlgorithmBase(List<WallsStateEnum[,]> mazeArray)
        {
            this.mazeArray = mazeArray;
        }


        internal List<DataOfMaze> GetWallsList(bool removeWalls)
        {
            InitializeWalls();
            if (removeWalls) GenerateMaze();
            return floors;

        }

        internal abstract DataOfMaze CreateNewFloor(int width, int heigh);
        protected abstract List<bool> GetNeighbor(int floorIndex, int x, int y, WallsStateEnum statusForSearch);
        protected abstract void RemoveWall(int direction, bool updateDirection = true);
        internal abstract void SetVisibilityWall(DataOfMaze floor, int x, int y, int width);

        void GenerateMaze()
        {
            bool needJoin = false;

            for (int index = 0; index < mazeArray.Count; index++)
            {
                ProcessMazeFloor(index, ref needJoin);
            }
        }

        void ProcessMazeFloor(int index, ref bool needJoin)
        {
            var mazeFloor = mazeArray[index];
            if (mazeFloor == null) return;

            for (int y = 0; y < mazeFloor.GetLength(1); y++)
            {
                for (int x = 0; x < mazeFloor.GetLength(0); x++)
                {
                    if (mazeFloor[x, y] == WallsStateEnum.NotVisited)
                    {
                        currentFloor = index;
                        currentColumn = y;
                        currentRow = x;

                        if (needJoin)
                            JoinToVisitedCell();
                        else
                            mazeArray[index][x, y] = WallsStateEnum.Visited;

                        RepeatUntilHaveUnvisitedNeighbor();
                        needJoin = true;
                    }
                }
            }
        }

        void JoinToVisitedCell()
        {
            var neighbors = GetNeighbor(currentFloor, currentRow, currentColumn, WallsStateEnum.Visited);
            int indexTrue = neighbors.Contains(true) ? GetRandomIndexWhereTrue(neighbors) : -1;



            if (indexTrue < 0)
                FindAndProcessUnvisitedCell(ref indexTrue);

            RemoveWall(indexTrue, false);
        }

        protected (int, int, int) FindCellWhoHasVisitedNeighbour(int row, int column)
        {
            for (int index = 0; index < mazeArray.Count; index++)
            {
                var mazeFloor = mazeArray[index];
                if (mazeFloor == null) continue;

                for (int y = 0; y < mazeFloor.GetLength(1); y++)
                {
                    for (int x = 0; x < mazeFloor.GetLength(0); x++)
                    {
                        if (mazeFloor[x, y] == WallsStateEnum.NotVisited)
                        {
                            var neighbors = GetNeighbor(index, x, y, WallsStateEnum.Visited);
                            if (neighbors.Contains(true))
                                return (index, x, y);
                        }
                    }
                }
            }

            return (-1, -1, -1);
        }

        protected void InitializeWalls()
        {
            floors.Clear();

            for (int index = 0; index < mazeArray.Count; index++)
            {
                var mazeFloor = mazeArray[index];
                if (mazeFloor == null) continue;

                ProcessFloor(index, mazeFloor);
            }
        }

        protected void ProcessFloor(int index, WallsStateEnum[,] mazeFloor)
        {
            int width = mazeFloor.GetLength(0);
            int height = mazeFloor.GetLength(1);

            var floor = CreateNewFloor(width, height);
            InitializeWallsForFloor(mazeFloor, floor, width, height);

            floors.Add(floor);
        }

        protected void RepeatUntilHaveUnvisitedNeighbor()
        {
            var unvisitedList = GetNeighbor(currentFloor, currentRow, currentColumn, WallsStateEnum.NotVisited);
            int removeWallIndex = GetRandomIndexWhereTrue(unvisitedList);

            while (removeWallIndex >= 0)
            {
                RemoveWall(removeWallIndex);
                unvisitedList = GetNeighbor(currentFloor, currentRow, currentColumn, WallsStateEnum.NotVisited);
                removeWallIndex = GetRandomIndexWhereTrue(unvisitedList);
            }
        }

        protected int GetRandomIndexWhereTrue(List<bool> boolList)
        {
            List<int> trueIndices = new List<int>();
            for (int i = 0; i < boolList.Count; i++)
            {
                if (boolList[i])
                {
                    trueIndices.Add(i);
                }
            }

            if (trueIndices.Count == 0)
            {
                return -1;
            }

            int randomIndex = UnityEngine.Random.Range(0, trueIndices.Count);
            return trueIndices[randomIndex];
        }

        protected void FindAndProcessUnvisitedCell(ref int indexTrue)
        {
            var savedRow = currentRow;
            var savedColumn = currentColumn;
            var savedFloor = currentFloor;

            while (indexTrue < 0)
            {
                var cellWithVisitedNeighbor = FindCellWhoHasVisitedNeighbour(savedRow, savedColumn);
                if (cellWithVisitedNeighbor.Item1 == -1)
                    break;

                currentFloor = cellWithVisitedNeighbor.Item1;
                currentRow = cellWithVisitedNeighbor.Item2;
                currentColumn = cellWithVisitedNeighbor.Item3;

                var neighbors = GetNeighbor(currentFloor, currentRow, currentColumn, WallsStateEnum.Visited);
                indexTrue = GetRandomIndexWhereTrue(neighbors);
                RemoveWall(indexTrue, false);
                RepeatUntilHaveUnvisitedNeighbor();

                neighbors = GetNeighbor(savedFloor, savedRow, savedColumn, WallsStateEnum.Visited);
                indexTrue = GetRandomIndexWhereTrue(neighbors);
            }

            currentRow = savedRow;
            currentColumn = savedColumn;
            currentFloor = savedFloor;
        }

        private void InitializeWallsForFloor(WallsStateEnum[,] mazeFloor, DataOfMaze floor, int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (mazeFloor[x, y] != WallsStateEnum.Empty)
                    {
                        SetVisibilityWall(floor, x, y, width);
                    }
                }
            }
        }
    }
}
