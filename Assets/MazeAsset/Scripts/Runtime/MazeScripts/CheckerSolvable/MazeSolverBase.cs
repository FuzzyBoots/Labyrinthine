using System.Collections.Generic;
using UnityEngine;
namespace MazeAsset.MazeGenerator
{
    public abstract class MazeSolverBase : IMazeSolver
    {
        internal List<DataOfMaze> mazeData;

        internal MazeSolverBase(List<DataOfMaze> mazeData)
        {
            this.mazeData = mazeData;
        }

        protected abstract List<Vector3Int> GetNeighbors(Vector3Int cell);

        public List<Vector3Int> FindPath(Vector3Int start, Vector3Int target)
        {
            var visited = new HashSet<Vector3Int>();
            var queue = new Queue<(Vector3Int position, List<Vector3Int> path)>();

            queue.Enqueue((start, new List<Vector3Int> { start }));

            while (queue.Count > 0)
            {
                var (currentPosition, currentPath) = queue.Dequeue();

                if (currentPosition == target)
                {
                    return currentPath;
                }

                if (visited.Contains(currentPosition))
                    continue;

                visited.Add(currentPosition);

                var neighbors = GetNeighbors(currentPosition);

                foreach (var neighbor in neighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        var newPath = new List<Vector3Int>(currentPath) { neighbor };
                        queue.Enqueue((neighbor, newPath));
                    }
                }
            }

            return null;
        }
    }
}
