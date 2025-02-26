using UnityEngine;
namespace MazeAsset.MazeGenerator
{
    public class ElevatorFinderController
    {
        internal IElevatorPlatform Search(GameObject gameObject, ShapeCellEnum shapeCell)
        {
            if (shapeCell == ShapeCellEnum.Hexagon)
            {
                var t = gameObject.GetComponentInChildren<ElevatorInteractionHex>(true);
                if (t != null)
                    return t.platformReference as IElevatorPlatform;
            }
            else if (shapeCell == ShapeCellEnum.Square)
            {
                var t = gameObject.GetComponentInChildren<ElevatorInteractionSquare>(true);
                if (t != null)
                    return t.platformReference as IElevatorPlatform;
            }
            return null;
        }
    }
}