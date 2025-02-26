using UnityEngine;
using UnityEditor;
namespace MazeAsset.MazeGenerator.Editor
{
    [CustomEditor(typeof(SolvableChecker))]
    [InitializeOnLoad]
    public class SolvableCheckerEditor :  UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            var gen = (SolvableChecker)target;
            DrawDefaultInspector();
            if (GUILayout.Button("Check"))
            {
                var res = gen.IsMazeSolvable();
                if (res == false)
                {
                    Debug.LogError("Maze is not solvable");
                }
            }
        }

    }
}
