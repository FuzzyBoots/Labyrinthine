using System.Collections.Generic;
using UnityEngine;
namespace MazeAsset.MazeGenerator
{
    public class RandomGenerationStrategy : IMazeGenerationStrategy
    {
        List<Vector2Int> _sizeFloor;
        public RandomGenerationStrategy(List<Vector2Int> sizeFloor)
        {
            _sizeFloor = sizeFloor;
        }



        private void FillLayerRandomly(WallsStateEnum[,] layer)
        {
            for (int x = 0; x < layer.GetLength(0); x++)
            {
                for (int y = 0; y < layer.GetLength(1); y++)
                {
                    layer[x, y] = (Random.value > 0.5f) ? WallsStateEnum.NotVisited : WallsStateEnum.Empty;
                }
            }
        }

        MazeData IMazeGenerationStrategy.InitializeData()
        {
            var mazeData = new MazeData();
            mazeData.mazeArray = new List<WallsStateEnum[,]>();

            foreach (var size in _sizeFloor)
            {
                WallsStateEnum[,] layer = new WallsStateEnum[size.x, size.y];
                FillLayerRandomly(layer);
                mazeData.mazeArray.Add(layer);
            }

            return mazeData;
        }
    }
}
