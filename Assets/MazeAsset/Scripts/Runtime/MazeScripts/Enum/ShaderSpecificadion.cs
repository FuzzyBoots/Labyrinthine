using System;
using UnityEngine;
namespace MazeAsset.MazeGenerator
{
    [System.Serializable]
    internal struct ShaderSpecificadion
    {
        [SerializeField, Range(0, 1)] internal float heightFloorTexture;
        [SerializeField, Range(0, 1)] internal float outline;
        [SerializeField, Range(0, 1)] internal float heightWallColor;
        [SerializeField] internal float scalerTexture;
        [SerializeField] internal Color colorOutline;
        [SerializeField] internal Color colorWall;
        [SerializeField] internal Texture2D floorTexture;
    }
}