using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MazeAsset.MazeGenerator
{
    public static class ImgToMazeConvertor
    {
        public static WallsStateEnum[,] GenerateMazeFromAlpha(Texture2D img)
        {
            if (img == null)
            {
                Debug.LogError("Input texture is not assigned.");
                return null;
            }


            if (!img.isReadable)
            {
                Debug.LogError($"Input texture {img.name}  must be readable. Check 'Read/Write Enabled' in the texture import settings.");
                return null;
            }

            var width = img.width;
            var height = img.height;
            var floor = new WallsStateEnum[width, height];

            Color[] pixels = img.GetPixels();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = pixels[y * width + x];
                    if (pixel.a == 0)
                        floor[x, y] = WallsStateEnum.Empty;
                    else floor[x, y] = WallsStateEnum.NotVisited;
                }
            }
            return floor;
        }
    }
}
