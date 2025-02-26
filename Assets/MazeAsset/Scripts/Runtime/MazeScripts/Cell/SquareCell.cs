using System.Collections.Generic;
using System.Numerics;
namespace MazeAsset.MazeGenerator
{
    public class SquareCell : ICell
    {
        ShapeCellEnum ICell.GetCellType => ShapeCellEnum.Square;

        float ICell.GetHeightChunk(int chunkSize, UnityEngine.Vector2 scaler)
        {
            return (float)(chunkSize * scaler.x);
        }

        Vector2 ICell.GetSizeCell(UnityEngine.Vector2 scaler)
        {
            return new Vector2(scaler.x, scaler.x);
        }

        List<DataOfMaze> ICell.GetWallsList(List<WallsStateEnum[,]> mazeArray, bool removeWalls)
        {
            var squareScript = new AlgorithmusForSquare(mazeArray);
            return squareScript.GetWallsList(removeWalls);
        }

        float ICell.GetWidthChunk(int chunkSize, UnityEngine.Vector2 scaler)
        {
            return (float)(chunkSize * scaler.x);
        }
    }
}