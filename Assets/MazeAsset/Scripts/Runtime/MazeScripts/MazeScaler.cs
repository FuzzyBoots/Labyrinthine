using MazeAsset.CustomAttribute;
using UnityEngine;
namespace MazeAsset.MazeGenerator
{
    public class MazeScaler : MonoBehaviour
    {
        [LabeledVector2("Scaler X & Z", "Scaler Y")]
        public Vector2 scalerVector;
    }
}