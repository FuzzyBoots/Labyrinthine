using System;
using System.Collections.Generic;
using System.Numerics;
namespace MazeAsset.MazeGenerator
{
    public interface ICell
    {
        internal Vector2 GetSizeCell(UnityEngine.Vector2 scaler);
        internal float GetWidthChunk(int chunkSize, UnityEngine.Vector2 scaler);
        internal float GetHeightChunk(int chunkSize, UnityEngine.Vector2 scaler);
        internal ShapeCellEnum GetCellType { get; }
        internal List<DataOfMaze> GetWallsList(List<WallsStateEnum[,]> mazeArray, bool removeWalls);
    }
}
