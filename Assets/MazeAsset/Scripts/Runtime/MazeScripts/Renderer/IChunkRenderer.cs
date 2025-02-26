using UnityEngine;

namespace MazeAsset.MazeGenerator
{
    public interface IChunkRenderer
    {
        internal float offsetY => 0.000001f;
        internal float offsetRoof => 0.1f;
        internal (GameObject, GameObject) RenderChunk(int X, int Y, int numberFloor, WallVisibilityStatus wallVisibilityStatus, MazeData mazeData, GameObject parent, GameObject elevatorParent, bool makeRoof, IElevatorPlatform elevator, GameObject wallPrefab);
    }
}
