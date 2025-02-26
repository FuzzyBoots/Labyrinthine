using System.Collections.Generic;
using UnityEngine;

namespace MazeAsset.MazeGenerator
{
    public class MoveGOFromChunkToOtherChunk
    {
        private float widthSquare;
        private PositionCalculatorService positionCalculator;
        private Material _editMaterial;
        private Material _elevatorMaterial;
        private Material _mazeMaterial;
        private Vector2 _scaler;
        private ShapeCellEnum _shapeCell;
        internal MoveGOFromChunkToOtherChunk(Material editMaterial, Material elevatorMaterial, Material mazeMaterial, Vector2 scaler, ShapeCellEnum shapeCell, float widthSquare)
        {
            _editMaterial = editMaterial;
            _elevatorMaterial = elevatorMaterial;
            _mazeMaterial = mazeMaterial;
            _scaler = scaler;
            _shapeCell = shapeCell;
            this.widthSquare = widthSquare;
        }

        internal GameObject ChangeWallVisibility(GameObject wallGameObject, GameObjectManager wallsObjectManager, GameObjectManager elevatorObjectManager, GameObjectManager editObjectManager, List<DataOfMaze> dataOfMaze, IElevatorPlatform elevatorPlatform, Mesh mesh)
        {
            positionCalculator ??= new PositionCalculatorService();
            var (position, name) = positionCalculator.CalculatePositionWithName(wallGameObject);

            if (position == Vector3Int.back || wallGameObject.transform.parent == null)
                return null;

            var (x, y, z) = (position.x, position.y, position.z);
            var floor = dataOfMaze[y];
            Vector3Int chunk = positionCalculator.CalculatePosition(wallGameObject.transform.parent.gameObject);
            if (chunk == Vector3Int.back) return null;

            return HandleWallByName(name, wallGameObject, floor, x, y, z, chunk, dataOfMaze, wallsObjectManager, elevatorObjectManager, editObjectManager, elevatorPlatform, mesh);
        }

        private GameObject HandleWallByName(string name, GameObject wallGameObject, DataOfMaze floor, int x, int y, int z, Vector3Int chunk, List<DataOfMaze> dataOfMaze, GameObjectManager wallsObjectManager, GameObjectManager elevatorObjectManager, GameObjectManager editObjectManager, IElevatorPlatform elevatorPlatform, Mesh mesh)
        {
            return name switch
            {
                "_h" => HandleHorizontalWall(floor, x, z, chunk, wallsObjectManager, editObjectManager, wallGameObject),
                "_v" => HandleVerticalWall(floor, x, z, chunk, wallsObjectManager, editObjectManager, wallGameObject),
                "_f" => HandleFloor(wallGameObject, floor, dataOfMaze, x, y, z, chunk, elevatorPlatform, editObjectManager, elevatorObjectManager),
                "_e" => HandleElevator(wallGameObject, floor, dataOfMaze, x, y, z, chunk, wallsObjectManager, elevatorObjectManager, mesh, elevatorPlatform),
                _ => null
            };
        }

        private GameObject HandleHorizontalWall(DataOfMaze floor, int x, int z, Vector3Int chunk, GameObjectManager wallsObjectManager, GameObjectManager editObjectManager, GameObject wallGameObject)
        {
            if (z == 0 || z + 1 == floor.sizeHorizontal.y) return null;

            if (IsHorizontalWallEmpty(floor, x, z)) return null;

            var wallIndex = floor.getIndex(x, z, floor.sizeHorizontal.x);
            var newStatus = ToggleWallVisibilityStatus(floor.listWallsHorizontal, wallIndex);
            return UpdateWallMaterialAndManager(newStatus, wallGameObject, chunk, wallsObjectManager, editObjectManager);
        }

        private GameObject HandleVerticalWall(DataOfMaze floor, int x, int z, Vector3Int chunk, GameObjectManager wallsObjectManager, GameObjectManager editObjectManager, GameObject wallGameObject)
        {
            if (IsVerticalWallOutOfBounds(floor, x, z)) return null;

            if (IsVerticalWallEmpty(floor, x, z)) return null;

            var wallIndex = floor.getIndex(x, z, floor.sizeVertical.x);
            var newStatus = ToggleWallVisibilityStatus(floor.listWallsVertical, wallIndex);
            return UpdateWallMaterialAndManager(newStatus, wallGameObject, chunk, wallsObjectManager, editObjectManager);
        }

        private GameObject HandleFloor(GameObject wallGameObject, DataOfMaze floor, List<DataOfMaze> dataOfMaze, int x, int y, int z, Vector3Int chunk, IElevatorPlatform elevatorPlatform, GameObjectManager editObjectManager, GameObjectManager elevatorObjectManager)
        {
            if (IsElevatorFloorOutOfBounds(dataOfMaze, y, x, z)) return null;
            Vector3 localPos = wallGameObject.transform.localPosition;
            DestroyWallGameObject(wallGameObject);

            return CreateElevatorPlatform(floor, dataOfMaze, x, y, z, chunk, elevatorPlatform, editObjectManager, elevatorObjectManager, localPos);
        }

        private GameObject HandleElevator(GameObject wallGameObject, DataOfMaze floor, List<DataOfMaze> dataOfMaze, int x, int y, int z, Vector3Int chunk, GameObjectManager wallsObjectManager, GameObjectManager elevatorObjectManager, Mesh mesh, IElevatorPlatform elevatorPlatform)
        {
            Vector3 localPos = wallGameObject.transform.localPosition;
            var rot = wallGameObject.transform.localRotation;
            var objectInRoot = elevatorPlatform.GetGOFromManager(x, y, z);
            if (objectInRoot == null) return null;
            DestroyWallGameObject(objectInRoot);
            DestroyWallGameObject(wallGameObject);
            elevatorPlatform.RemoveGOToGOManager(x, y, z);
            HandleLowerElevatorRemoval(dataOfMaze, y, x, z, chunk, elevatorObjectManager, elevatorPlatform);

            return CreateFloor(floor, dataOfMaze, x, y, z, chunk, wallsObjectManager, elevatorObjectManager, mesh, elevatorPlatform, localPos, rot);
        }

        private bool IsHorizontalWallEmpty(DataOfMaze floor, int x, int z)
        {
            return floor.floorsWalls[floor.getIndex(x, z, floor.sizeFloors.x)] == WallVisibilityStatus.Empty ||
                   floor.floorsWalls[floor.getIndex(x, z - 1, floor.sizeFloors.x)] == WallVisibilityStatus.Empty;
        }

        private bool IsVerticalWallOutOfBounds(DataOfMaze floor, int x, int z)
        {
            return x == 0 || x + 1 == floor.sizeVertical.x || z >= floor.sizeVertical.y;
        }

        private bool IsVerticalWallEmpty(DataOfMaze floor, int x, int z)
        {
            if (_shapeCell == ShapeCellEnum.Hexagon)
            {
                if (x == 0 || z == 0) return true;
                var newZLeft = (int)((z - (x + 1) % 2) / 2);
                var newZRight = (int)((z - (x) % 2) / 2);
                var index1 = floor.getIndex(x - 1, newZLeft, floor.sizeFloors.x);
                var index2 = floor.getIndex(x, newZRight, floor.sizeFloors.x);
                return (index1 < floor.floorsWalls.Length &&
                    floor.floorsWalls[index1] == WallVisibilityStatus.Empty) ||
                    (index2 < floor.floorsWalls.Length &&
                    floor.floorsWalls[index2] == WallVisibilityStatus.Empty);
            }
            else
            {
                return floor.floorsWalls[floor.getIndex(x - 1, z, floor.sizeFloors.x)] == WallVisibilityStatus.Empty ||
                       floor.floorsWalls[floor.getIndex(x, z, floor.sizeFloors.x)] == WallVisibilityStatus.Empty;
            }
        }

        private bool IsElevatorFloorOutOfBounds(List<DataOfMaze> dataOfMaze, int y, int x, int z)
        {
            return y < 1 || x >= dataOfMaze[y - 1].sizeFloors.x || z >= dataOfMaze[y - 1].sizeFloors.y;
        }

        private WallVisibilityStatus ToggleWallVisibilityStatus(IList<WallVisibilityStatus> wallList, int wallIndex)
        {
            return wallList[wallIndex] = wallList[wallIndex] == WallVisibilityStatus.VisibleInEditMode
                ? WallVisibilityStatus.VisibleInNormalMode
                : WallVisibilityStatus.VisibleInEditMode;
        }

        private GameObject UpdateWallMaterialAndManager(WallVisibilityStatus newStatus, GameObject wallGameObject, Vector3Int chunk, GameObjectManager wallsObjectManager, GameObjectManager editObjectManager)
        {
            wallGameObject.GetComponent<Renderer>().sharedMaterial = newStatus == WallVisibilityStatus.VisibleInEditMode ? _editMaterial : _mazeMaterial;
            return newStatus == WallVisibilityStatus.VisibleInEditMode
                ? editObjectManager.GetGameObject(chunk.x, chunk.y, chunk.z)
                : wallsObjectManager.GetGameObject(chunk.x, chunk.y, chunk.z);
        }

        private void DestroyWallGameObject(GameObject wallGameObject)
        {
            Object.DestroyImmediate(wallGameObject);
        }

        private GameObject CreateElevatorPlatform(DataOfMaze floor, List<DataOfMaze> dataOfMaze, int x, int y, int z, Vector3Int chunk, IElevatorPlatform elevatorPlatform, GameObjectManager editObjectManager, GameObjectManager elevatorObjectManager, Vector3 localPos)
        {
            var platform = elevatorPlatform.GetPlatform(_scaler.y);
            SetupElevatorPlatform(platform, x, y, z, chunk, elevatorPlatform, editObjectManager, elevatorObjectManager, localPos);

            floor.floorsWalls[floor.getIndex(x, z, floor.sizeFloors.x)] = WallVisibilityStatus.VisibleInEditMode;

            HandleLowerElevator(dataOfMaze, y, x, z, chunk, platform, elevatorPlatform, elevatorObjectManager);
            return null;
        }

        private void SetupElevatorPlatform(GameObject platform, int x, int y, int z, Vector3Int chunk, IElevatorPlatform elevatorPlatform, GameObjectManager editObjectManager, GameObjectManager elevatorObjectManager, Vector3 localPos)
        {
            platform.transform.localPosition = localPos;
            platform.transform.localScale = new Vector3(_scaler.x, platform.transform.localScale.y, _scaler.x);
            platform.transform.SetParent(elevatorObjectManager.GetGameObject(chunk.x, chunk.y, chunk.z).transform, false);
            platform.name = $"{x},{y},{z},_e";
            platform.GetComponent<Renderer>().sharedMaterial = _elevatorMaterial;
            elevatorPlatform.AddGOToGOManager(platform, x, y, z);
            Duplicate(platform, editObjectManager.GetGameObject(chunk.x, chunk.y, chunk.z).transform, _editMaterial);
            platform.transform.localPosition += (Vector3.up * elevatorPlatform.GetOffsetY) * _scaler.y;
        }

        private void HandleLowerElevator(List<DataOfMaze> dataOfMaze, int y, int x, int z, Vector3Int chunk, GameObject platform, IElevatorPlatform elevatorPlatform, GameObjectManager elevatorObjectManager)
        {
            if (dataOfMaze[y - 1].floorsWalls[dataOfMaze[y - 1].getIndex(x, z, dataOfMaze[y - 1].sizeFloors.x)] == WallVisibilityStatus.VisibleInNormalMode)
            {
                var lowerChunkParent = elevatorObjectManager.GetGameObject(chunk.x, chunk.y - 1, chunk.z).transform;
                var t = DuplicateAndName(platform, $"{x},{y - 1},{z},_e", lowerChunkParent, _elevatorMaterial);
                elevatorPlatform.AddGOToGOManager(t, x, y - 1, z);
            }
        }

        private GameObject CreateFloor(DataOfMaze floor, List<DataOfMaze> dataOfMaze, int x, int y, int z, Vector3Int chunk, GameObjectManager wallsObjectManager, GameObjectManager elevatorObjectManager, Mesh mesh, IElevatorPlatform elevatorPlatform, Vector3 localPos, Quaternion rot)
        {
            var goFloor = InstantiateFromMesh(localPos, rot, wallsObjectManager.GetGameObject(chunk.x, chunk.y, chunk.z).transform, $"{x},{y},{z},_f", mesh, _mazeMaterial);
            if (_shapeCell == ShapeCellEnum.Square)
            {
                goFloor.transform.localScale = new Vector3(_scaler.x, _scaler.x, widthSquare);
                goFloor.transform.eulerAngles = new Vector3(90, 0, 0);

            }
            else
                goFloor.transform.localScale = new Vector3(_scaler.x, 1, _scaler.x);

            floor.floorsWalls[floor.getIndex(x, z, floor.sizeFloors.x)] = WallVisibilityStatus.VisibleInNormalMode;

            return null;
        }

        private void HandleLowerElevatorRemoval(List<DataOfMaze> dataOfMaze, int y, int x, int z, Vector3Int chunk, GameObjectManager elevatorObjectManager, IElevatorPlatform elevatorPlatform)
        {
            if (y >= 1)
            {
                var lowerChunkParent = elevatorObjectManager.GetGameObject(chunk.x, chunk.y - 1, chunk.z).transform;
                var index = dataOfMaze[y - 1].getIndex(x, z, dataOfMaze[y - 1].sizeFloors.x);
                if (index < dataOfMaze[y - 1].floorsWalls.Length && dataOfMaze[y - 1].floorsWalls[index] != WallVisibilityStatus.VisibleInEditMode)
                {
                    Object.DestroyImmediate(lowerChunkParent.Find($"{x},{y - 1},{z},_e")?.gameObject);
                    elevatorPlatform.RemoveGOToGOManager(x, y - 1, z);
                }
            }
        }

        private GameObject Duplicate(GameObject original, Transform parent, Material material)
        {
            if (original == null) return null;

            var duplicate = Object.Instantiate(original);
            duplicate.transform.SetParent(parent, false);
            duplicate.GetComponent<Renderer>().sharedMaterial = material;
            duplicate.name = original.name;
            return duplicate;
        }

        private GameObject DuplicateAndName(GameObject original, string newName, Transform parent, Material material)
        {
            var duplicate = Duplicate(original, parent, material);
            if (duplicate != null) duplicate.name = newName;

            return duplicate;
        }

        private GameObject InstantiateFromMesh(Vector3 position, Quaternion rotation, Transform parent, string name, Mesh mesh, Material material)
        {
            var gameObject = new GameObject(name);
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            var meshCollider = gameObject.AddComponent<MeshCollider>();
            meshRenderer.sharedMaterial = material;
            meshCollider.sharedMesh = mesh;
            meshFilter.mesh = mesh;

            gameObject.transform.SetParent(parent, false);
            gameObject.transform.localPosition = position;
            gameObject.transform.localRotation = rotation;

            return gameObject;
        }
    }
}