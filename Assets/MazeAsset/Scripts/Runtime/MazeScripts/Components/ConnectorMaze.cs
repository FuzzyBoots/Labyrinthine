using System;
using System.Collections.Generic;
using UnityEngine;


namespace MazeAsset.MazeGenerator
{
    public class ConnectorMaze
    {
        private List<WallsStateEnum[,]> mazeArray;
        private ShapeCellEnum shapeCell;

        internal ConnectorMaze(List<WallsStateEnum[,]> mazeArray, ShapeCellEnum shapeCell)
        {
            this.mazeArray = mazeArray;
            this.shapeCell = shapeCell;
        }

        internal List<WallsStateEnum[,]> ConnectComponents()
        {
            List<List<Vector3Int>> components = FindComponents();

            while (components.Count > 1)
            {
                var closestPair = FindNearestComponents(components);
                if (closestPair.Item1 == Vector3Int.zero && closestPair.Item2 == Vector3Int.zero)
                {
                    var result = FindSmallestFloorDimensions(0, mazeArray.Count);
                    var newComponent = new List<Vector3Int>()
                {
                    new Vector3Int(result.Item1, 0, result.Item2)
                };
                    mazeArray[0][result.Item1, result.Item2] = WallsStateEnum.NotVisited;
                    components.Add(newComponent);
                    continue;
                }
                var addetList = ConnectTwoComponents(closestPair.Item1, closestPair.Item2);
                var mergedComponent = MergeComponents(components[closestPair.Item3], components[closestPair.Item4], addetList);

                if (closestPair.Item3 > closestPair.Item4)
                {
                    components.RemoveAt(closestPair.Item3);
                    components.RemoveAt(closestPair.Item4);
                }
                else
                {
                    components.RemoveAt(closestPair.Item4);
                    components.RemoveAt(closestPair.Item3);
                }

                components.Add(mergedComponent);
            }
            return mazeArray;
        }

        private List<List<Vector3Int>> FindComponents()
        {
            List<List<Vector3Int>> components = new List<List<Vector3Int>>();
            HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

            for (int floorIndex = 0; floorIndex < mazeArray.Count; floorIndex++)
            {
                int rows = mazeArray[floorIndex].GetLength(0);
                int cols = mazeArray[floorIndex].GetLength(1);
                var finder = new FinderComponent();
                for (int j = 0; j < cols; j++)
                {
                    for (int i = 0; i < rows; i++)
                    {
                        Vector3Int startVector = new Vector3Int(i, floorIndex, j);

                        if (mazeArray[floorIndex][i, j] != WallsStateEnum.Empty && !visited.Contains(startVector))
                        {
                            components.Add(finder.FindComponentFromPoint(startVector, visited, shapeCell, mazeArray));
                        }
                    }
                }
            }
            return components;
        }

        private (Vector3Int, Vector3Int, int, int) FindNearestComponents(List<List<Vector3Int>> components)
        {
            int closestAIndex = 0;
            int closestBIndex = 1;
            Vector3Int closestA = Vector3Int.zero;
            Vector3Int closestB = Vector3Int.zero;
            float minDistance = float.MaxValue;

            for (int i = 0; i < components.Count; i++)
            {
                for (int j = i + 1; j < components.Count; j++)
                {
                    var result = FindMinDistance(components[i], components[j]);
                    if (result.Item1 < minDistance)
                    {
                        minDistance = result.Item1;
                        closestA = result.Item2;
                        closestB = result.Item3;
                        closestAIndex = i;
                        closestBIndex = j;
                    }
                }
            }
            return (closestA, closestB, closestAIndex, closestBIndex);
        }

        private (float, Vector3Int, Vector3Int) FindMinDistance(List<Vector3Int> componentA, List<Vector3Int> componentB)
        {
            float minDistance = float.MaxValue;
            Vector3Int closestA = componentA[0];
            Vector3Int closestB = componentB[0];
            foreach (var cellA in componentA)
            {
                foreach (var cellB in componentB)
                {
                    if (Mathf.Abs(cellA.y - cellB.y) >= 2)
                    {
                        if (!IsValidFloorConnection(cellA, cellB))
                        {
                            continue;
                        }
                    }
                    float distance = Vector3Int.Distance(cellA, cellB);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestA = cellA;
                        closestB = cellB;
                    }

                }
            }

            return (minDistance, closestA, closestB);
        }

        private List<Vector3Int> ConnectTwoComponents(Vector3Int closestA, Vector3Int closestB)
        {
            var addedList = new List<Vector3Int>();

            Vector3Int direction = closestB - closestA;
            Vector3Int step = new Vector3Int((int)Mathf.Sign(direction.x), (int)Mathf.Sign(direction.y), (int)Mathf.Sign(direction.z));
            Vector3Int current = closestA;

            while (current != closestB)
            {
                if (current.y != closestB.y && IsValidFloorSize(current.y + step.y, new Vector3Int(current.x, current.y + step.y, current.z)))
                {
                    current.y += step.y;
                    mazeArray[current.y][current.x, current.z] = WallsStateEnum.NotVisited;
                    addedList.Add(current);
                    continue;

                }
                if (shapeCell == ShapeCellEnum.Square)
                {
                    if (current.z != closestB.z)
                    {
                        current.z += step.z;
                        mazeArray[current.y][current.x, current.z] = WallsStateEnum.NotVisited;
                        addedList.Add(current);
                    }
                    if (current.x != closestB.x)
                    {
                        current.x += step.x;
                        mazeArray[current.y][current.x, current.z] = WallsStateEnum.NotVisited;
                        addedList.Add(current);
                    }
                }
                else if (shapeCell == ShapeCellEnum.Hexagon)
                {
                    if (current.x != closestB.x && current.z != closestB.z)
                    {
                        current.x += step.x;
                        if (step.z > 0)
                            current.z += current.x % 2 == 0 ? step.z : 0;
                        else
                            current.z += current.x % 2 == 1 ? step.z : 0;
                        mazeArray[current.y][current.x, current.z] = WallsStateEnum.NotVisited;
                        addedList.Add(current);

                    }
                    else if (current.x != closestB.x)
                    {

                        current.x += step.x;
                        mazeArray[current.y][current.x, current.z] = WallsStateEnum.NotVisited;
                        addedList.Add(current);
                    }
                    else
                    {
                        current.z += step.z;
                        mazeArray[current.y][current.x, current.z] = WallsStateEnum.NotVisited;
                        addedList.Add(current);
                    }
                }
            }
            return addedList;
        }

        private List<Vector3Int> MergeComponents(List<Vector3Int> componentA, List<Vector3Int> componentB, List<Vector3Int> jointComponent)
        {
            List<Vector3Int> mergedComponent = new List<Vector3Int>(componentA);
            mergedComponent.AddRange(componentB);
            mergedComponent.AddRange(jointComponent);
            return mergedComponent;
        }

        private bool IsValidFloorConnection(Vector3Int cellA, Vector3Int cellB)
        {
            int floorA = cellA.y;
            int floorB = cellB.y;

            if (floorA == floorB)
            {
                return mazeArray[floorA].GetLength(0) >= cellA.x && mazeArray[floorA].GetLength(1) >= cellA.z &&
                       mazeArray[floorB].GetLength(0) >= cellB.x && mazeArray[floorB].GetLength(1) >= cellB.z;
            }
            else
            {
                var minVector = new Vector3Int(Math.Min(cellA.x, cellB.x), 0, Math.Min(cellA.z, cellB.z));
                if (floorA < floorB)
                {
                    ;
                    for (int current = floorA; current < floorB; current++)
                    {
                        minVector.y = current;
                        if (!IsValidFloorSize(current, minVector)) return false;
                    }
                    return true;
                }
                else
                {
                    for (int current = floorB; current < floorA; current++)
                    {
                        minVector.y = current;
                        if (!IsValidFloorSize(current, minVector)) return false;
                    }
                    return true;
                }
            }
        }

        private bool IsValidFloorSize(int floor, Vector3Int cell)
        {
            return floor >= 0 && floor < mazeArray.Count &&
                   cell.x >= 0 && cell.x < mazeArray[floor].GetLength(0) &&
                   cell.z >= 0 && cell.z < mazeArray[floor].GetLength(1);
        }

        private (int width, int length) FindSmallestFloorDimensions(int floorA, int floorB)
        {
            int lowerFloor = Math.Min(floorA, floorB);
            int upperFloor = Math.Max(floorA, floorB);

            int minWidth = 99999;
            int minLength = 99999;

            for (int floor = lowerFloor; floor < upperFloor; floor++)
            {
                int width = mazeArray[floor].GetLength(0);
                int length = mazeArray[floor].GetLength(1);

                if (width * length < minWidth * minLength)
                {
                    minWidth = width;
                    minLength = length;
                }
            }

            return (minWidth - 1, minLength - 1);
        }
    }
}
