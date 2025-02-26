using UnityEngine;

namespace MazeAsset.MazeGenerator
{
    [System.Serializable]
    public class PositionCalculatorService
    {
        public Vector3Int CalculatePosition(GameObject gameObject)
        {
            if (gameObject == null)
                return Vector3Int.back;

            string objectName = gameObject.name;
            string[] objectCoords = objectName.Split(',');
            if (objectCoords.Length < 3 || objectCoords.Length > 5)
                return Vector3Int.back;


            if (!int.TryParse(objectCoords[0], out int localX) ||
                !int.TryParse(objectCoords[1], out int localY) ||
                !int.TryParse(objectCoords[2], out int localZ))
            {
                return Vector3Int.back;
            }
            return new Vector3Int(localX, localY, localZ);
        }

        public (Vector3Int position, string name) CalculatePositionWithName(GameObject gameObject)
        {
            string[] objectCoords = gameObject.name.Split(',');
            if (objectCoords.Length != 4)
                return (Vector3Int.back, "");
            return (CalculatePosition(gameObject), gameObject.name.Split(",")[3]);

        }

    }
}