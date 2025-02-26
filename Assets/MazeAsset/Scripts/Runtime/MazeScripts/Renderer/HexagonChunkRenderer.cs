using System;
using UnityEngine;

namespace MazeAsset.MazeGenerator
{
    public class HexagonChunkRenderer : IChunkRenderer
    {
        private ObjectInstantiatorService _objectInstantiator;
        private MazeDimensionsService _dimensionsService;
        private Mesh _hexagonFloorMesh;
        private Material _hexagonMaterial;
        private Material _elevatorMaterial;
        private Material _editMaterial;
        private Vector2 scaler;

        internal HexagonChunkRenderer(ObjectInstantiatorService objectInstantiator, MazeDimensionsService dimensionsService, Material hexagonMaterial, Material elevatorMaterial, Material editMaterial, Vector2 scaler, float scaleYWallPrefab)
        {
            _objectInstantiator = objectInstantiator;
            _dimensionsService = dimensionsService;
            HexagonCreate hexagonCreate = new HexagonCreate();
            _hexagonFloorMesh = hexagonCreate.CreateMeshHexagon(scaleYWallPrefab, 1);
            _hexagonMaterial = hexagonMaterial;
            _elevatorMaterial = elevatorMaterial;
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
            GameObject rootElevator = null;
            if (wallVisibilityStatus == WallVisibilityStatus.VisibleInNormalMode)
            {
                rootElevator = CreateRootObject($"{X},{numberFloor},{Y}", elevatorParent, X, Y, numberFloor, ChunkSize);
            }
            RenderVerticalWalls(w, h, X, Y, maxChunkX, maxChunkZ, offsetX, offsetY, wallVisibilityStatus, rootWalls.transform, mazeData, numberFloor, wallPrefab);
            RenderHorizontalWalls(w, h, X, Y, maxChunkZ, offsetX, offsetY, wallVisibilityStatus, rootWalls.transform, mazeData, numberFloor, wallPrefab);
            RenderFloorAndRoof(w, h, makeRoof, offsetX, offsetY, wallVisibilityStatus, rootWalls.transform, rootElevator?.transform, mazeData, numberFloor, elevator, wallPrefab);

            return (rootWalls, rootElevator);
        }

        private GameObject CreateRootObject(string name, GameObject parent, int X, int Y, int floor, int ChunkSize)
        {
            GameObject root = new GameObject(name);
            root.transform.parent = parent.transform;
            root.transform.position = new Vector3(X * ChunkSize * 1.5f * scaler.x, floor * scaler.y, Y * ChunkSize * Mathf.Sqrt(3) * scaler.x);
            return root;
        }

        private void RenderVerticalWalls(int w, int h, int X, int Y, int maxChunkX, int maxChunkZ, int offsetX, int offsetY,
            WallVisibilityStatus wallVisibilityStatus, Transform parent, MazeData mazeData, int floor, GameObject wallPrefab)
        {

            int hh = (Y == maxChunkZ - 1) ? h * 2 + 1 : h * 2;
            var wh = (X == maxChunkX - 1) ? w + 1 : w;
            for (int x = 0; x < wh; x++)
            {
                var isSecondAdd = (offsetX + x) % 2 == 1;
                for (int y = 0; y < hh; y++)
                {
                    var index = mazeData.getIndex(offsetX + x * 1, offsetY * 2 + y, mazeData.GetFloor(floor).sizeVertical.x);
                    if (mazeData.GetFloor(floor).listWallsVertical[index] == wallVisibilityStatus)
                    {
                        Vector3 localPosition = new Vector3(x * 1.5f, 0, y * Mathf.Sqrt(3f) / 2);
                        int degreeOrient = isSecondAdd ? -60 : 60;
                        InstantiateWall(wallPrefab, localPosition, Quaternion.Euler(0, degreeOrient, 0), parent, $"{x + offsetX},{floor},{offsetY * 2 + y},_v", wallVisibilityStatus);
                    }
                    isSecondAdd = !isSecondAdd;
                }
            }
        }

        private void RenderHorizontalWalls(int w, int h, int X, int Y, int maxChunkZ, int offsetX, int offsetY,
            WallVisibilityStatus wallVisibilityStatus, Transform parent, MazeData mazeData, int floor, GameObject wallPrefab)
        {

            int hh = (Y == maxChunkZ - 1) ? h + 1 : h;
            for (int y = 0; y < hh; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    var index = mazeData.getIndex(offsetX + x, offsetY + y, mazeData.GetFloor(floor).sizeHorizontal.x);
                    if (mazeData.GetFloor(floor).listWallsHorizontal[index] == wallVisibilityStatus)
                    {
                        Vector3 localPosition = new Vector3(x * 1.5f + 0.75f, 0, y * Mathf.Sqrt(3f) - (Mathf.Sqrt(3f) / 4));
                        if ((offsetX + x) % 2 == 1) localPosition.z += Mathf.Sqrt(3f) / 2;
                        InstantiateWall(wallPrefab, localPosition, Quaternion.Euler(0, 0, 0), parent, $"{x + offsetX},{floor},{offsetY + y},_h", wallVisibilityStatus);
                    }
                }
            }
        }

        private void RenderFloorAndRoof(int w, int h, bool makeRoof, int offsetX, int offsetY,
            WallVisibilityStatus wallVisibilityStatus, Transform parent, Transform elevatorParent, MazeData mazeData, int floor,
            IElevatorPlatform elevator, GameObject wallPrefab)
        {
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Vector3 offset = Vector3.zero;
                    if ((offsetX + x) % 2 == 1) offset.z += Mathf.Sqrt(3f) / 2;
                    bool needElevator = mazeData.NeedElevator(floor + 1, offsetX + x, offsetY + y);
                    var width = mazeData.GetFloor(floor).sizeFloors.x;
                    if (mazeData.GetFloor(floor).floorsWalls[mazeData.getIndex(offsetX + x, offsetY + y, width)] == wallVisibilityStatus)
                    {
                        var name = elevatorParent == null ? $"{offsetX + x},{floor},{offsetY + y},_e" : $"{offsetX + x},{floor},{offsetY + y},_f";
                        Vector3 localPosition = new Vector3(x * 1.5f + 0.75f, -0.5f, y * Mathf.Sqrt(3f) + (Mathf.Sqrt(3f) / 4));
                        localPosition.y += (this as IChunkRenderer).offsetY;
                        InstantiateHexagon(localPosition + offset, Quaternion.Euler(0, 0, 0), parent, name, wallVisibilityStatus);
                    }
                    if (elevatorParent != null && (mazeData.GetFloor(floor).floorsWalls[mazeData.getIndex(offsetX + x, offsetY + y, width)] == WallVisibilityStatus.VisibleInEditMode || needElevator))
                    {
                        Vector3 position = new Vector3((x * 1.5f + 0.75f), (-0.5f + elevator.GetOffsetY), (y * Mathf.Sqrt(3f) + (Mathf.Sqrt(3f) / 4)));
                        var platform = elevator.GetPlatform(scaler.y);
                        platform.transform.parent = elevatorParent;
                        position.y += (this as IChunkRenderer).offsetY;
                        position += offset;
                        position.x *= scaler.x;
                        position.y *= scaler.y;
                        position.z *= scaler.x;
                        platform.transform.localPosition = position;
                        platform.name = $"{offsetX + x},{floor},{offsetY + y},_e";
                        platform.GetComponent<Renderer>().sharedMaterial = _elevatorMaterial;
                        platform.transform.localScale = new Vector3(scaler.x, platform.transform.localScale.y, scaler.x);
                        elevator.AddGOToGOManager(platform, offsetX + x, floor, offsetY + y);
                    }

                    if (elevatorParent != null && makeRoof && mazeData.NeedsRoof(floor, offsetX + x, offsetY + y))
                    {
                        Vector3 localPosition = new Vector3(x * 1.5f + 0.75f, 0.5f, y * Mathf.Sqrt(3f) + (Mathf.Sqrt(3f) / 4));
                        localPosition.y -= ((this as IChunkRenderer).offsetY + (this as IChunkRenderer).offsetRoof);
                        InstantiateHexagon(localPosition + offset, Quaternion.Euler(0, 0, 0), parent, $"{offsetX + x},{floor},{offsetY + y},_r", wallVisibilityStatus);
                    }
                }
            }
        }

        private void InstantiateWall(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, string name, WallVisibilityStatus wallVisibilityStatus)
        {
            position.x *= scaler.x;
            position.y *= scaler.y;
            position.z *= scaler.x;
            var material = wallVisibilityStatus == WallVisibilityStatus.VisibleInNormalMode ? _hexagonMaterial : _editMaterial;

            var sc = _objectInstantiator.InstantiateObject(prefab, position, rotation, parent, name, material);
            sc.transform.localScale = new Vector3(scaler.x, scaler.y, sc.transform.localScale.z);

        }
        private void InstantiateHexagon(Vector3 position, Quaternion rotation, Transform parent, string name, WallVisibilityStatus wallVisibilityStatus)
        {
            var material = wallVisibilityStatus == WallVisibilityStatus.VisibleInNormalMode ? _hexagonMaterial : _editMaterial;
            GameObject hexagon = new GameObject(name);
            MeshFilter meshFilter = hexagon.AddComponent<MeshFilter>();
            var renderer = hexagon.AddComponent<MeshRenderer>();
            renderer.material = material;
            MeshCollider meshCollider = hexagon.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = _hexagonFloorMesh;
            meshFilter.mesh = _hexagonFloorMesh;
            hexagon.transform.parent = parent;
            position.x *= scaler.x;
            position.y *= scaler.y;
            position.z *= scaler.x;
            hexagon.transform.localPosition = position;
            hexagon.transform.localRotation = rotation;
            hexagon.transform.localScale = new Vector3(scaler.x, hexagon.transform.localScale.y, scaler.x);
        }
    }
}
