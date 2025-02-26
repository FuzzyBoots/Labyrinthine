using System.Collections.Generic;
using UnityEngine;

namespace MazeAsset.MazeGenerator
{
    public class ImageGenerationStrategy : IMazeGenerationStrategy
    {
        private List<Texture2D> _images;

        public ImageGenerationStrategy(List<Texture2D> images)
        {
            _images = images;
        }


        MazeData IMazeGenerationStrategy.InitializeData()
        {
            var mazeData = new MazeData();
            mazeData.mazeArray = new List<WallsStateEnum[,]>();

            foreach (var image in _images)
            {
                var mazeArray = ImgToMazeConvertor.GenerateMazeFromAlpha(image);
                if (mazeArray != null)
                {
                    mazeData.mazeArray.Add(mazeArray);
                }
            }
            return mazeData;
        }
    }
}