using System.Collections.Generic;
using UnityEngine;

namespace MazeAsset.MazeGenerator
{
    public class CheckerConector
    {
        internal bool CheckIfMazeConnected(MazeData mazeData, ICell cellShape, bool autoConnect = false)
        {
            Vector3Int? startVector = null;
            for (int f = 0; f < mazeData.mazeArray.Count; f++)
            {
                var floor = mazeData.mazeArray[f];
                for (int i = 0; i < floor.GetLength(0); i++)
                {
                    for (int j = 0; j < floor.GetLength(1); j++)
                    {
                        if (floor[i, j] != WallsStateEnum.Empty)
                        {
                            startVector = new Vector3Int(i, f, j);
                            break;
                        }
                    }
                    if (startVector.HasValue) break;
                }
                if (startVector.HasValue) break;
            }

            if (!startVector.HasValue) return false;

            var finder = new FinderComponent();
            HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
            finder.FindComponentFromPoint(startVector.Value, visited, cellShape.GetCellType, mazeData.mazeArray);

            foreach (var floorArray in mazeData.mazeArray)
            {
                for (int i = 0; i < floorArray.GetLength(0); i++)
                {
                    for (int j = 0; j < floorArray.GetLength(1); j++)
                    {
                        if (floorArray[i, j] != WallsStateEnum.Empty)
                        {
                            Vector3Int cellPosition = new Vector3Int(i, mazeData.mazeArray.IndexOf(floorArray), j);
                            if (!visited.Contains(cellPosition))
                            {
                                if (autoConnect)
                                {
                                    mazeData.mazeArray = new ConnectorMaze(mazeData.mazeArray, cellShape.GetCellType).ConnectComponents();
                                }
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}