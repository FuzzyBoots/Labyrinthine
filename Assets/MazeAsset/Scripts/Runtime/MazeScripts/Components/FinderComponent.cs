using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MazeAsset.MazeGenerator
{
    public class FinderComponent
    {
        internal List<Vector3Int> FindComponentFromPoint(Vector3Int startVector, HashSet<Vector3Int> visited, ShapeCellEnum shapeCellEnum, List<WallsStateEnum[,]> mazeArray)
        {
            int[] directionsX, directionsY;

            if (shapeCellEnum == ShapeCellEnum.Square)
            {
                directionsX = new int[] { -1, 1, 0, 0 };
                directionsY = new int[] { 0, 0, -1, 1 };
            }
            else // Hexagon
            {
                directionsX = new int[] { -1, 1, 0, 0, -1, 1 };
                directionsY = new int[] { 0, 0, -1, 1, -1, -1 };
            }
            List<Vector3Int> component = new List<Vector3Int>();
            Queue<Vector3Int> queue = new Queue<Vector3Int>();
            queue.Enqueue(startVector);
            visited.Add(startVector);


            while (queue.Count > 0)
            {
                Vector3Int current = queue.Dequeue();
                component.Add(current);

                CheckNeighbors(current, directionsX, directionsY, queue, visited, mazeArray, shapeCellEnum);

                CheckFloor(current, current.y - 1, queue, visited, mazeArray);

                CheckFloor(current, current.y + 1, queue, visited, mazeArray);
            }

            return component;
        }

        internal void CheckNeighbors(Vector3Int current, int[] directionsX, int[] directionsY, Queue<Vector3Int> queue, HashSet<Vector3Int> visited, List<WallsStateEnum[,]> mazeArray, ShapeCellEnum shapeCell)
        {
            for (int d = 0; d < directionsX.Length; d++)
            {
                int newX = current.x + directionsX[d];
                int newY = current.z + directionsY[d];

                if (shapeCell == ShapeCellEnum.Hexagon && Mathf.Abs(directionsX[d]) == 1)
                {
                    if (current.x % 2 == 1 && directionsY[d] == -1) newY += 2;
                }

                Vector3Int newPos = new Vector3Int(newX, current.y, newY);
                if (IsValidFloorSize(current.y, newPos, mazeArray))
                {
                    if (mazeArray[current.y][newX, newY] != WallsStateEnum.Empty && !visited.Contains(newPos))
                    {
                        queue.Enqueue(newPos);
                        visited.Add(newPos);
                    }
                }
            }
        }

        internal void CheckFloor(Vector3Int current, int floorY, Queue<Vector3Int> queue, HashSet<Vector3Int> visited, List<WallsStateEnum[,]> mazeArray)
        {
            Vector3Int newPos = new Vector3Int(current.x, floorY, current.z);

            if (IsValidFloorSize(floorY, newPos, mazeArray))
            {
                if (mazeArray[floorY][current.x, current.z] != WallsStateEnum.Empty && !visited.Contains(newPos))
                {
                    queue.Enqueue(newPos);
                    visited.Add(newPos);
                }
            }
        }

        private bool IsValidFloorSize(int floor, Vector3Int cell, List<WallsStateEnum[,]> mazeArray)
        {
            return floor >= 0 && floor < mazeArray.Count &&
                   cell.x >= 0 && cell.x < mazeArray[floor].GetLength(0) &&
                   cell.z >= 0 && cell.z < mazeArray[floor].GetLength(1);
        }
    }
}