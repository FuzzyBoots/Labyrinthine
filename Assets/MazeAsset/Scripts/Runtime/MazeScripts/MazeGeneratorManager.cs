using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;

#if UNITY_EDITOR
using UnityEditor;

#endif
using UnityEngine.UIElements;
namespace MazeAsset.MazeGenerator
{
    [Serializable]
    [RequireComponent(typeof(MazeVisibleChunk))]
    [RequireComponent(typeof(MazeColor))]
    [RequireComponent(typeof(MazeScaler))]
    public class MazeGeneratorManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text textInteraction;
        [SerializeField] private bool makeRoof;
        [SerializeField, HideInInspector] public MazeData mazeData;
        private const string NameForParent = "RootForChunk";
        private const string NameForElevatorParent = "RootForElevatorChunk";
        private const string NameForParentEdit = "RootForChunkEdit";

        [SerializeField, HideInInspector] private GameObjectManager wallsObjectManager = new GameObjectManager();
        [SerializeField, HideInInspector] private GameObjectManager elevatorObjectManager = new GameObjectManager();
        [SerializeField, HideInInspector] private GameObjectManager elObjectManager = new GameObjectManager();
        private GameObjectManager editObjectManager;

        [SerializeField] private GameObject wallPrefab;
        [SerializeField] public MethodGenerateEnum methodGenerate;
        [SerializeField, HideInInspector] internal ShapeCellEnum shapeCell;
        [SerializeField] internal ShapeCellEnum shapeMaze;

        [Range(2, 150), SerializeField] internal int ChunkSize;

        [SerializeField, HideInInspector] public List<Texture2D> imageForGenerateMaze;
        [SerializeField, HideInInspector] private GameObject root;
        [SerializeField, HideInInspector] private GameObject rootEdit;
        [SerializeField, HideInInspector] private GameObject rootElevator;
        private Material materialMaze;
        private Material materialElevator;
        private Material editMaterial;

        [SerializeField, HideInInspector] public bool editMode;
        //internal bool InPlayedMode;
        [SerializeField, HideInInspector] private Vector2 scaler;

        private MazeVisibleChunk visibleChunk;
        private IMazeGenerationService mazeGenerationService;
        private ICell cellShape;
        private IChunkRenderer chunkRenderer;
        private IElevatorPlatform elevatorPlatform;
        [SerializeField, HideInInspector] private Vector3Int centerChunk;

        private MazeDimensionsService dimensionsService;
#if UNITY_EDITOR
        private MoveGOFromChunkToOtherChunk moveGOFromChunkToOtherChunk;
#endif
        [SerializeField, HideInInspector]
        public List<Vector2Int> floorDataList;
        [SerializeField, HideInInspector]
        public int sizeArray;

        public void ChangeSizeArrayInMazeColor(int ListSize)
        {
            sizeArray = ListSize;
            GetComponent<MazeColor>().UpdateSpecificationArray();
        }

        internal void AddElevator(GameObject gameObject, int x, int y, int z)
        {
            elObjectManager.AddGameObject(gameObject, x, y, z);
        }
        internal GameObject GetElevator(int x, int y, int z)
        {
            return elObjectManager.GetGameObject(x, y, z);
        }

        internal bool RemoveElevator(int x, int y, int z)
        {
            return elObjectManager.RemoveGameObject(x, y, z);
        }
        public void GenerateMaze(bool removeWalls = false, bool createNewWals = true)
        {
            scaler = GetComponent<MazeScaler>().scalerVector;
            if (createNewWals) shapeCell = shapeMaze;
            var mazeColor = GetComponent<MazeColor>();
            mazeColor.hex = shapeCell == ShapeCellEnum.Hexagon;
            mazeColor.scaler = scaler;
            mazeColor.UpdateMaterial();

            visibleChunk = GetComponent<MazeVisibleChunk>();
            InitRootParent();
            InitData(createNewWals);

            if (createNewWals)
            {
                if (!mazeGenerationService.Initialize()) return;

                mazeGenerationService.Generate(removeWalls, visibleChunk.VisibleAllMaze);
                mazeData = mazeGenerationService.MazeData;
            }
            //Stopwatch stopwatch = Stopwatch.StartNew();
            if (visibleChunk.VisibleAllMaze)
                GenerateFullMaze();
            else
                GenerateMazeWithPosition();
            //stopwatch.Stop();
            //UnityEngine.Debug.Log($"Renderer walls {(float)stopwatch.ElapsedTicks / (float)Stopwatch.Frequency * 1000} ms");

        }

        private void GenerateFullMaze(bool edit = false)
        {
            for (int y = 0; y < mazeData.floors.Count; y++)
            {
                (int maxChunkX, int maxChunkZ) = dimensionsService.GetMaxChunks(y, mazeData);
                for (int z = 0; z < maxChunkZ; z++)
                {
                    for (int x = 0; x < maxChunkX; x++)
                    {
                        GenerateChunk(x, y, z, edit);
                    }
                }
            }
        }

        private void GenerateMazeWithPosition(bool edit = false)
        {
            if (visibleChunk == null || visibleChunk.transformPlayer == null) return;

            float widthChunk = cellShape.GetWidthChunk(ChunkSize, scaler);
            float heightChunk = cellShape.GetHeightChunk(ChunkSize, scaler);
            Vector3 positionPlayer = visibleChunk.transformPlayer.transform.position;

            int visible = visibleChunk.VisibleTerrain;
            int visibleFloor = visibleChunk.VisibleFloor;

            int startX = (int)(positionPlayer.x / widthChunk) - visible;
            int startZ = (int)(positionPlayer.z / heightChunk) - visible;
            int startY = (int)(positionPlayer.y / scaler.y) - visibleFloor;

            for (int numberFloor = startY; numberFloor <= startY + (2 * visibleFloor); numberFloor++)
            {
                if (numberFloor >= mazeData.floors.Count || numberFloor < 0) continue;

                (int maxChunkX, int maxChunkZ) = dimensionsService.GetMaxChunks(numberFloor, mazeData);
                for (int x = startX; x <= startX + (2 * visible); x++)
                {
                    for (int z = startZ; z <= startZ + (2 * visible); z++)
                    {
                        if (x >= maxChunkX || z >= maxChunkZ || x < 0 || z < 0) continue;

                        GenerateChunk(x, numberFloor, z, edit);
                    }
                }
            }
            centerChunk.x = (int)(positionPlayer.x / widthChunk);
            centerChunk.z = (int)(positionPlayer.z / heightChunk);
            centerChunk.y = (int)scaler.y;
        }


        private void InitRootParent()
        {
            root = ReInitObject(NameForParent);
            rootElevator = ReInitObject(NameForElevatorParent);
        }

        private GameObject ReInitObject(string nameObject)
        {
            var existingObject = GameObject.Find(nameObject);
            if (existingObject != null)
            {
                DestroyImmediate(existingObject);
            }
            return new GameObject(nameObject);
        }

        private void InitData(bool initData = true)
        {
            GetMaterial();
            visibleChunk = GetComponent<MazeVisibleChunk>();
            InitInterface();

            elevatorObjectManager = new GameObjectManager();
            elObjectManager = new GameObjectManager();
            wallsObjectManager = new GameObjectManager();
            if (initData)
            {
                mazeData?.ClearAllData();
                mazeData = new MazeData();
            }

        }

        private void GetMaterial()
        {
            //GetComponent<MazeColor>().scaler = scaler;
            var colorScript = GetComponent<MazeColor>();
            materialMaze = colorScript.mazeMaterial;
            materialElevator = colorScript.elevatorMaterial;
            editMaterial = colorScript.editMaterial;
        }

        private void InitInterface()
        {
            GetMaterial();
            elevatorPlatform?.DeleteElevator();
            dimensionsService = new MazeDimensionsService(ChunkSize);

            if (shapeCell == ShapeCellEnum.Square)
            {
                cellShape = new SquareCell();
                elevatorPlatform = new SquarePlatformElevator(this, textInteraction, new AnimatorElevatorService(scaler.y, scaler.x, shapeCell));
                chunkRenderer = new SquareChunkRenderer(new ObjectInstantiatorService(), dimensionsService, materialMaze, materialElevator, editMaterial, scaler);
            }
            else
            {
                cellShape = new HexagonCell();
                elevatorPlatform = new HexagonPlatformElevator(this, textInteraction, new AnimatorElevatorService(scaler.y, scaler.x, shapeCell));
                chunkRenderer = new HexagonChunkRenderer(new ObjectInstantiatorService(), dimensionsService, materialMaze, materialElevator, editMaterial, scaler, wallPrefab.transform.localScale.z);
            }

            mazeGenerationService = (methodGenerate == MethodGenerateEnum.Image) ?
                new MazeGenerationService(new ImageGenerationStrategy(imageForGenerateMaze), cellShape) :
                new MazeGenerationService(new RandomGenerationStrategy(floorDataList), cellShape);
        }

        public void StartEditMode()
        {
            rootElevator.SetActive(false);
            editObjectManager = new GameObjectManager();

            if (!TryGetComponent<MazeVisibleChunk>(out visibleChunk))
            {
                UnityEngine.Debug.LogError("Script \"MazeVisibleChunk\" not found.");
                return;
            }

            if (dimensionsService == null) ReInitInterface();

            editMode = true;
            rootEdit = ReInitObject(NameForParentEdit);

            if (visibleChunk.VisibleAllMaze)
                GenerateFullMaze(true);
            else
                GenerateMazeWithPosition(true);
        }

        private void ReInitInterface()
        {
            GetMaterial();
            dimensionsService = new MazeDimensionsService(ChunkSize);
            var finder = new ElevatorFinderController();
            elevatorPlatform = finder.Search(rootElevator, shapeCell);
            if (elevatorPlatform == null)
            {
                if (shapeCell == ShapeCellEnum.Square)
                {
                    elevatorPlatform = new SquarePlatformElevator(this, textInteraction, new AnimatorElevatorService(scaler.y, scaler.x, shapeCell));
                }
                else
                {
                    elevatorPlatform = new HexagonPlatformElevator(this, textInteraction, new AnimatorElevatorService(scaler.y, scaler.x, shapeCell));
                }
            }
            else
            {
                elevatorPlatform.ReinitData();
            }
            if (shapeCell == ShapeCellEnum.Square)
            {
                cellShape = new SquareCell();
                chunkRenderer = new SquareChunkRenderer(new ObjectInstantiatorService(), dimensionsService, materialMaze, materialElevator, editMaterial, scaler);
            }
            else
            {
                cellShape = new HexagonCell();
                chunkRenderer = new HexagonChunkRenderer(new ObjectInstantiatorService(), dimensionsService, materialMaze, materialElevator, editMaterial, scaler, wallPrefab.transform.localScale.z);
            }
        }

        public void StopEditMode()
        {
#if UNITY_EDITOR
            moveGOFromChunkToOtherChunk = null;
#endif
            rootElevator.SetActive(true);
            editMode = false;
            DestroyImmediate(rootEdit);
            editObjectManager = null;
        }

        public void HandleMouseClick(Vector2 mousePosition)
        {
#if UNITY_EDITOR
            if (elevatorPlatform == null) ReInitInterface();
            moveGOFromChunkToOtherChunk ??= new MoveGOFromChunkToOtherChunk(editMaterial, materialElevator, materialMaze, scaler, shapeCell, wallPrefab.transform.localScale.z);
            if (root == null) return;

            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject hitObject = hit.collider.gameObject;
                Mesh mesh;
                if (shapeCell == ShapeCellEnum.Hexagon)
                {
                    HexagonCreate hexagonCreate = new();
                    mesh = hexagonCreate.CreateMeshHexagon(wallPrefab.transform.localScale.z, 1);
                }
                else
                {
                    mesh = wallPrefab.GetComponent<MeshFilter>().sharedMesh;
                }
                var newParent = moveGOFromChunkToOtherChunk.ChangeWallVisibility(hitObject, wallsObjectManager, elevatorObjectManager, editObjectManager, mazeData.floors, elevatorPlatform, mesh);

                if (newParent != null)
                    hitObject.transform.SetParent(newParent.transform);
            }
#endif
        }

        internal void GenMazeWithPlayerPosition(Vector3 positionPlayer)
        {
            if (dimensionsService == null) ReInitInterface();
            float widthChunk = cellShape.GetWidthChunk(ChunkSize, scaler);
            float heightChunk = cellShape.GetHeightChunk(ChunkSize, scaler);
            float chunkHeightY = scaler.y;
            MazeVisibleChunk mazeVisibleChunk = GetComponent<MazeVisibleChunk>();
            if (mazeVisibleChunk.VisibleAllMaze) return;
            int visible = mazeVisibleChunk.VisibleTerrain;
            int visibleFloor = mazeVisibleChunk.VisibleFloor;
            if (centerChunk.y != (int)(positionPlayer.y / chunkHeightY))
            {
                int yDestruct;
                int yCreate;
                if ((int)(positionPlayer.y / chunkHeightY) < centerChunk.y)
                {
                    yDestruct = centerChunk.y + visibleFloor;
                    yCreate = centerChunk.y - visibleFloor - 1;
                }
                else
                {
                    yDestruct = centerChunk.y - visibleFloor;
                    yCreate = centerChunk.y + visibleFloor + 1;
                }
                int startX = (int)(mazeVisibleChunk.transformPlayer.position.x / widthChunk) - visible;
                int startZ = (int)(mazeVisibleChunk.transformPlayer.position.z / heightChunk) - visible;
                centerChunk.y = (int)(positionPlayer.y / chunkHeightY);
                if (yDestruct < mazeData.floors.Count && yDestruct >= 0)
                {
                    (int maxChunkX, int maxChunkZ) = dimensionsService.GetMaxChunks(yDestruct, mazeData);

                    for (int x = startX; x <= startX + (2 * visible); x++)
                    {
                        for (int z = startZ; z <= startZ + (2 * visible); z++)
                        {
                            if (x >= maxChunkX || z >= maxChunkZ || x < 0 || z < 0) continue;

                            DestroyChunkAtPosition(new Vector3(x, yDestruct, z));
                        }
                    }
                }
                if (yCreate < mazeData.floors.Count && yCreate >= 0)
                {
                    (int maxChunkX, int maxChunkZ) = dimensionsService.GetMaxChunks(yCreate, mazeData);
                    for (int x = startX; x <= startX + (2 * visible); x++)
                    {
                        for (int z = startZ; z <= startZ + (2 * visible); z++)
                        {
                            if (x >= maxChunkX || z >= maxChunkZ || x < 0 || z < 0) continue;
                            CreateChunkAtPosition(new Vector3(x, yCreate, z));
                        }
                    }
                }
            }

            if (centerChunk.x != (int)(positionPlayer.x / widthChunk))
            {
                int xDestruct;
                int xCreate;
                int _y = (int)(positionPlayer.z / heightChunk);
                if ((int)(positionPlayer.x / widthChunk) < centerChunk.x)
                {
                    xDestruct = centerChunk.x + visible;
                    xCreate = centerChunk.x - visible - 1;
                }
                else
                {
                    xDestruct = centerChunk.x - visible;
                    xCreate = centerChunk.x + visible + 1;
                }
                centerChunk.x = (int)(positionPlayer.x / widthChunk);
                int startY = (int)(mazeVisibleChunk.transformPlayer.position.y / chunkHeightY) - visibleFloor;

                for (int numberFloor = startY; numberFloor <= startY + (2 * visibleFloor); numberFloor++)
                {
                    if (numberFloor >= mazeData.floors.Count || numberFloor < 0) continue;
                    (int maxChunkX, int maxChunkZ) = dimensionsService.GetMaxChunks(numberFloor, mazeData);
                    for (int x = -visible; x <= visible; x++)
                    {
                        if (_y + x >= maxChunkZ || _y + x < 0) continue;
                        if (xDestruct >= maxChunkX || xDestruct < 0) continue;
                        DestroyChunkAtPosition(new Vector3(xDestruct, numberFloor, _y + x));
                    }
                }


                for (int numberFloor = startY; numberFloor <= startY + (2 * visibleFloor); numberFloor++)
                {
                    if (numberFloor >= mazeData.floors.Count || numberFloor < 0) continue;
                    (int maxChunkX, int maxChunkZ) = dimensionsService.GetMaxChunks(numberFloor, mazeData);
                    for (int x = -visible; x <= visible; x++)
                    {
                        if (_y + x >= maxChunkZ || _y + x < 0) continue;
                        if (xCreate >= maxChunkX || xCreate < 0) continue;

                        CreateChunkAtPosition(new Vector3(xCreate, numberFloor, _y + x));
                    }
                }

            }

            if (centerChunk.z != (int)(positionPlayer.z / heightChunk))
            {
                int zDestruct;
                int zCreate;
                int _x = (int)(positionPlayer.x / widthChunk);
                if ((int)(positionPlayer.z / heightChunk) < centerChunk.z)
                {
                    zDestruct = centerChunk.z + visible;
                    zCreate = centerChunk.z - visible - 1;
                }
                else
                {
                    zDestruct = centerChunk.z - visible;
                    zCreate = centerChunk.z + visible + 1;
                }
                centerChunk.z = (int)(positionPlayer.z / heightChunk);
                int startY = (int)(mazeVisibleChunk.transformPlayer.position.y / chunkHeightY) - visibleFloor;

                for (int numberFloor = startY; numberFloor <= startY + (2 * visibleFloor); numberFloor++)
                {
                    if (numberFloor >= mazeData.floors.Count || numberFloor < 0) continue;
                    (int maxChunkX, int maxChunkZ) = dimensionsService.GetMaxChunks(numberFloor, mazeData);
                    for (int x = -visible; x <= visible; x++)
                    {
                        if (_x + x >= maxChunkX || _x + x < 0) continue;
                        if (zDestruct >= maxChunkZ || zDestruct < 0) continue;
                        DestroyChunkAtPosition(new Vector3(_x + x, numberFloor, zDestruct));
                    }
                }


                for (int numberFloor = startY; numberFloor <= startY + (2 * visibleFloor); numberFloor++)
                {
                    if (numberFloor >= mazeData.floors.Count || numberFloor < 0) continue;
                    (int maxChunkX, int maxChunkZ) = dimensionsService.GetMaxChunks(numberFloor, mazeData);
                    for (int x = -visible; x <= visible; x++)
                    {
                        if (_x + x >= maxChunkX || _x + x < 0) continue;
                        if (zCreate >= maxChunkZ || zCreate < 0) continue;

                        CreateChunkAtPosition(new Vector3(_x + x, numberFloor, zCreate));
                    }
                }
            }
        }

        private void DestroyChunkAtPosition(Vector3 position)
        {
            wallsObjectManager.HideObject((int)position.x, (int)position.y, (int)position.z);

            elevatorObjectManager.HideObject((int)position.x, (int)position.y, (int)position.z);

            if (editMode)
            {
                editObjectManager.HideObject((int)position.x, (int)position.y, (int)position.z);
            }
        }
        private void GenerateChunk(int x, int y, int z, bool edit)
        {
            var wallVisibility = edit ? WallVisibilityStatus.VisibleInEditMode : WallVisibilityStatus.VisibleInNormalMode;
            var rootForRender = edit ? rootEdit : root;
            var rootForElevator = edit ? null : rootElevator;
            var chunks = chunkRenderer.RenderChunk(x, z, y, wallVisibility, mazeData, rootForRender, rootForElevator, makeRoof, elevatorPlatform, wallPrefab);

            AddChunkToManagers(chunks, x, y, z, edit);
        }

        private void CreateChunkAtPosition(Vector3 position)
        {
            var chunkWasGenerated = wallsObjectManager.VisibleObject((int)position.x, (int)position.y, (int)position.z);
            elevatorObjectManager.VisibleObject((int)position.x, (int)position.y, (int)position.z);
            if (chunkWasGenerated == false)
            {
                var chunks = chunkRenderer.RenderChunk((int)position.x, (int)position.z, (int)position.y, WallVisibilityStatus.VisibleInNormalMode, mazeData, root, rootElevator, makeRoof, elevatorPlatform, wallPrefab);
                AddChunkToManagers(chunks, (int)position.x, (int)position.y, (int)position.z, false);
            }
            if (editMode)
            {
                var chunkWasGenEdit = editObjectManager.GetGameObject((int)position.x, (int)position.y, (int)position.z);
                if (chunkWasGenEdit == false)
                {
                    var chunksEdi = chunkRenderer.RenderChunk((int)position.x, (int)position.z, (int)position.y, WallVisibilityStatus.VisibleInEditMode, mazeData, rootEdit, null, makeRoof, elevatorPlatform, wallPrefab);
                    AddChunkToManagers(chunksEdi, (int)position.x, (int)position.y, (int)position.z, true);
                }
            }
        }

        private void AddChunkToManagers((GameObject, GameObject) chunks, int x, int y, int z, bool edit)
        {
            if (edit)
            {
                editObjectManager.AddGameObject(chunks.Item1, x, y, z);
            }
            else
            {
                wallsObjectManager.AddGameObject(chunks.Item1, x, y, z);
                elevatorObjectManager.AddGameObject(chunks.Item2, x, y, z);
            }
        }
    }
}