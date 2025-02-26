using System.Linq;
using UnityEditor;

using UnityEngine;
namespace MazeAsset.MazeGenerator.Editor
{
    [CustomEditor(typeof(MazeColor))]
    public class MazeColorEditor : UnityEditor.Editor
    {
        private const int MaxArraySize = 13;

        public override void OnInspectorGUI()
        {
            MazeColor genColor = (MazeColor)target;
            DrawDefaultInspector();

        }
    }
}