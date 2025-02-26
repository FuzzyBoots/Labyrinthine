using MazeAsset.CustomAttribute;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MazeAsset.MazeGenerator
{
    [ExecuteInEditMode]
    internal class MazeVisibleChunk : MonoBehaviour
    {
        [SerializeField]
        internal bool VisibleAllMaze;
        [SerializeField, VisibleIf("VisibleAllMaze", false)]
        internal Transform transformPlayer;
        internal Vector3 InitialPosition { get; set; }
        [SerializeField, VisibleIf("VisibleAllMaze", false), Range(1, 10)]
        internal int VisibleTerrain = 1;
        [SerializeField, VisibleIf("VisibleAllMaze", false), Range(2, 10)]
        internal int VisibleFloor = 2;



        private void Update()
        {
            if (transformPlayer == null) return;

            Vector3 currentPosition = transformPlayer.position;
            if (currentPosition != InitialPosition)
            {
                InitialPosition = currentPosition;
                if (!TryGetComponent<MazeGeneratorManager>(out var maze)) return;
                if (!VisibleAllMaze) maze.GenMazeWithPlayerPosition(currentPosition);
            }

        }
    }
}
