using System;
using UnityEngine;

namespace MazeAsset.MazeGenerator
{
    public class SquareChunkRenderer : IChunkRenderer
    {
        private ObjectInstantiatorService _objectInstantiator;
        private MazeDimensionsService _dimensionsService;
        private Material _squareMaterial;
        private Material _elevatorMaterial;
        private Material _editMaterial;
        private Vector2 scaler;

        internal SquareChunkRenderer(ObjectInstantiatorService objectInstantiator, MazeDimensionsService dimensionsService, Material materialMaze, Material elevatorMaterial, Material editMaterial, Vector2 scaler)
        {
            _objectInstantiator = objectInstantiator;
            _dimensionsService = dimensionsService;
            this._squareMaterial = materialMaze;
            this._elevatorMaterial = elevatorMaterial;
            _editMaterial = editMaterial;
            this.scaler = scaler;
        }



        (GameObject, GameObject) IChunkRenderer.RenderChunk(
            int X, int Y, int numberFloor, WallVisibilityStatus wallVisibilityStatus,
            MazeData mazeData, GameObject parent, GameObject elevatorParent, bool makeRoof, IElevatorPlatform elevator, GameObject wallPrefab)
        {
            var ChunkSize = _dimensionsService.ChunkSize;
            (int maxChunkX, int maxChunkZ) = _dimensionsService.GetMaxChunks(numberFloor, mazeData);
            var (width, height) = _dimensionsService.GetWidthAndHeightFromFloor(numberFloor, mazeData);
            int w = Math.Min(ChunkSize, width - ChunkSize * X);
            int h = Math.Min(ChunkSize, height - ChunkSize * Y);
            int offsetX = X * ChunkSize;
            int offsetY = Y * ChunkSize;

            GameObject rootWalls = CreateRootObject($"{X},{numberFloor},{Y}", parent, X, Y, numberFloor, ChunkSize);
            GameObject rootElevatorWalls = null;
            if (elevatorParent != null)
            {
                rootElevatorWalls = CreateRootObject($"{X},{numberFloor},{Y}", elevatorParent, X, Y, numberFloor, ChunkSize);
            }

            RenderHorizontalWalls(w, h, X, Y, maxChunkZ, offsetX, offsetY, wallVisibilityStatus, rootWalls.transform, mazeData, numberFloor, wallPrefab);
            RenderVerticalWalls(w, h, X, Y, maxChunkX, offsetX, offsetY, wallVisibilityStatus, rootWalls.transform, mazeData, numberFloor, wallPrefab);
            RenderFloorAndRoof(w, h, makeRoof, offsetX, offsetY, wallVisibilityStatus, rootWalls.transform, rootElevatorWalls?.transform
        , mazeData, numberFloor, elevator, wallPrefab);
            return (rootWalls, rootElevatorWalls);
        }

        private GameObject CreateRootObject(string name, GameObject parent, int X, int Y, int floor, int ChunkSize)
        {
            GameObject root = new GameObject(name);
            root.transform.parent = parent.transform;
            root.transform.position = new Vector3(X * ChunkSize * scaler.x, floor * scaler.y, Y * ChunkSize * scaler.x);
            return root;
        }

        private void RenderHorizontalWalls(int w, int h, int X, int Y, int maxChunkZ, int offsetX, int offsetY,
            WallVisibilityStatus status, Transform parent, MazeData mazeData, int floor, GameObject wallPrefab)
        {
            int hh = (Y == maxChunkZ - 1) ? h + 1 : h;
            for (int y = 0; y < hh; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (mazeData.GetFloor(floor).listWallsHorizontal[mazeData.getIndex(offsetX + x, offsetY + y, mazeData.GetFloor(floor).sizeHorizontal.x)] == status)
                    {
                        InstantiateWall(wallPrefab, new Vector3(x, 0, y), Quaternion.identity, parent, $"{offsetX + x},{floor},{offsetY + y},_h", status, false);
                    }
                }
            }
        }

        private void RenderVerticalWalls(int w, int h, int X, int Y, int maxChunkX, int offsetX, int offsetY,
            WallVisibilityStatus status, Transform parent, MazeData mazeData, int floor, GameObject wallPrefab)
        {
            int ww = (X == maxChunkX - 1) ? w + 1 : w;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < ww; x++)
                {
                    if (mazeData.GetFloor(floor).listWallsVertical[mazeData.getIndex(offsetX + x, offsetY + y, mazeData.GetFloor(floor).sizeVertical.x)] == status)
                    {
                        InstantiateWall(wallPrefab, new Vector3(x - 0.5f, 0, y + 0.5f), Quaternion.Euler(0, -90, 0), parent, $"{offsetX + x},{floor},{offsetY + y},_v", status, false);
                    }
                }
            }
        }

        private void RenderFloorAndRoof(int w, int h, bool makeRoof, int offsetX, int offsetY,
            WallVisibilityStatus status, Transform parent, Transform elevatorParent, MazeData mazeData, int floor,
            IElevatorPlatform elevator, GameObject wallPrefab)
        {
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    bool needElevator = mazeData.NeedElevator(floor + 1, offsetX + x, offsetY + y);
                    var width = mazeData.GetFloor(floor).sizeFloors.x;
                    if (mazeData.GetFloor(floor).floorsWalls[mazeData.getIndex(offsetX + x, offsetY + y, width)] == status)
                    {
                        var name = elevatorParent == null ? $"{offsetX + x},{floor},{offsetY + y},_e" : $"{offsetX + x},{floor},{offsetY + y},_f";
                        InstantiateWall(wallPrefab, new Vector3(x, -0.5f + (this as IChunkRenderer).offsetY, y + 0.5f), Quaternion.Euler(90, 0, 0), parent, name, status, true);
                        if (elevatorParent == null)
                        {

                        }
                    }
                    if (status != WallVisibilityStatus.VisibleInEditMode && (mazeData.GetFloor(floor).floorsWalls[mazeData.getIndex(offsetX + x, offsetY + y, width)] == WallVisibilityStatus.VisibleInEditMode || needElevator))
                    {
                        Vector3 position = new Vector3(x * scaler.x, (-0.5f + elevator.GetOffsetY) * scaler.y, (y + 0.5f) * scaler.x);
                        var platform = elevator.GetPlatform(scaler.y);
                        platform.transform.localScale = new Vector3(scaler.x, 1, scaler.x);
                        platform.transform.parent = elevatorParent;
                        position.y += (this as IChunkRenderer).offsetY;
                        platform.transform.localPosition = position;
                        platform.name = $"{offsetX + x},{floor},{offsetY + y},_e";
                        elevator.AddGOToGOManager(platform, offsetX + x, floor, offsetY + y);
                        platform.GetComponent<Renderer>().sharedMaterial = _elevatorMaterial;
                    }

                    if (status != WallVisibilityStatus.VisibleInEditMode && (makeRoof && mazeData.NeedsRoof(floor, offsetX + x, offsetY + y)))
                    {
                        InstantiateWall(wallPrefab, new Vector3(x, 0.5f - (this as IChunkRenderer).offsetY - (this as IChunkRenderer).offsetRoof, y + 0.5f), Quaternion.Euler(90, 0, 0), parent, $"{offsetX + x} , {floor} , {offsetY + y},_r", status, true);
                    }
                }
            }
        }

        private void InstantiateWall(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, string name, WallVisibilityStatus status, bool isFloor)
        {
            position.x *= scaler.x;
            position.y *= scaler.y;
            position.z *= scaler.x;
            var material = status == WallVisibilityStatus.VisibleInEditMode ? _editMaterial : _squareMaterial;
            var sc = _objectInstantiator.InstantiateObject(prefab, position, rotation, parent, name, material);
            Vector3 scalerObject = isFloor ?
                new Vector3(scaler.x, scaler.x, sc.transform.localScale.z) :
                new Vector3(scaler.x, scaler.y, sc.transform.localScale.z);
            sc.transform.localScale = scalerObject;
        }

    }
}