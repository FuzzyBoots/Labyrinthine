using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MazeAsset.MazeGenerator
{
    [RequireComponent(typeof(MazeGeneratorManager))]
    [RequireComponent(typeof(MazeScaler))]
    public class SolvableChecker : MonoBehaviour
    {
        private const string NameOfRoot = "RootForPath";
        private GameObject root;
        [SerializeField]
        internal GameObject startFloor;
        [SerializeField]
        internal GameObject endFloor;
        IMazeSolver mazeSolver;
        IMazeVisualizer visualizer;

        void OnValidate()
        {
            if (!CheckObjet(startFloor))
            {
                startFloor = null;
            }

            if (!CheckObjet(endFloor))
            {
                endFloor = null;
            }

        }

        private void FindRoot()
        {
            GameObject.Find(NameOfRoot);
        }

        private void DestroyRoot()
        {
            DestroyImmediate(root);
        }

        private void CreateRoot()
        {
            root = new GameObject(NameOfRoot);
        }


        private void ReInit()
        {
            FindRoot();
            DestroyRoot();
            CreateRoot();
        }

        internal bool CheckObjet(GameObject objectForValid)
        {
            if (objectForValid == null) return true;
            var splitString = objectForValid.name.Split(",");
            if (splitString.Length == 4 && splitString[3] == "_f")
            {
                return true;
            }
            Debug.LogError($"{objectForValid.name} is not valid object. Objet name must end with _f");
            return false;
        }
        public bool IsMazeSolvable()
        {
            ReInit();
            if (startFloor == null || endFloor == null)
            {
                Debug.LogError("The start or end finish not set");
                return false;
            }
            var mazeGenerator = GetComponent<MazeGeneratorManager>();
            if (mazeGenerator == null) return false;
            var data = mazeGenerator.mazeData.floors;
            if (data == null) return false;
            if (mazeGenerator.shapeCell == ShapeCellEnum.Square)
            {
                mazeSolver = new SquareMazeSolver(data);
                visualizer = new SquareMazeVisualizer();
            }
            else if (mazeGenerator.shapeCell == ShapeCellEnum.Hexagon)
            {
                mazeSolver = new HexMazeSolver(data);
                visualizer = new HexagonalMazeVisualizer();
            }
            var posCalculator = new PositionCalculatorService();
            var start = posCalculator.CalculatePosition(startFloor);
            var end = posCalculator.CalculatePosition(endFloor);
            if (start == Vector3Int.back || end == Vector3Int.back) return false;
            var res = mazeSolver.FindPath(start, end);
            if (res == null) return false;
            var scaler = GetComponent<MazeScaler>().scalerVector;
            visualizer.Visualize(root.transform, res, scaler);
            return true;
        }
    }
}
