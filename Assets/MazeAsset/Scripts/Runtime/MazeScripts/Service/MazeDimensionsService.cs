using UnityEngine;

namespace MazeAsset.MazeGenerator
{
    internal class MazeDimensionsService
    {
        internal int ChunkSize;

        internal MazeDimensionsService(int chunkSize)
        {
            ChunkSize = chunkSize;
        }
        internal (int width, int height) GetWidthAndHeightFromFloor(int numberFloor, MazeData mazeData)
        {
            var mazeFloorDim = mazeData.GetFloor(numberFloor).sizeFloors;
            int width = mazeFloorDim.x;
            int height = mazeFloorDim.y;
            return (width, height);
        }
        internal (int maxChunkX, int maxChunkZ) GetMaxChunks(int numberFloor, MazeData mazeData)
        {

            if (numberFloor >= 0 && numberFloor < mazeData.floors.Count)
            {
                var mazeFloor = mazeData.GetFloor(numberFloor);
                var width = mazeFloor.sizeFloors.x;
                var height = mazeFloor.sizeFloors.y;
                int maxChunkX = (int)Mathf.Ceil((float)(width) / ChunkSize);
                int maxChunkZ = (int)Mathf.Ceil((float)(height) / ChunkSize);
                return (maxChunkX, maxChunkZ);
            }
            return (0, 0);
        }
    }
}