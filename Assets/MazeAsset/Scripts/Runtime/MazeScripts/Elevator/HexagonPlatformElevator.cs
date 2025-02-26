using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
namespace MazeAsset.MazeGenerator
{
    [System.Serializable]
    internal class HexagonPlatformElevator : IElevatorPlatform
    {
        bool animating = false;
        Mesh platformMesh;
        [SerializeField]
        private AnimatorElevatorService anim;
        [SerializeField]
        private bool isInteracting;
        [SerializeField]
        private MazeGeneratorManager coroutineOwner;
        private IEnumerator coroutine;
        private IEnumerator coroutine2;
        private PositionCalculatorService positionCalculator;
        private DirectionMoveEnum directionMove;
        [SerializeField]
        private InteractionElevatorTextController interactionElevatorTextController;
        [SerializeField]
        private TMP_Text textMeshPro;
        public HexagonPlatformElevator(MazeGeneratorManager coroutineOwner, TMP_Text textMeshPro, AnimatorElevatorService animatorElevatorService)
        {
            InitData(1);
            positionCalculator = new PositionCalculatorService();
            this.textMeshPro = textMeshPro;
            this.anim = animatorElevatorService;
            this.coroutineOwner = coroutineOwner;


        }

        void InitData(float width)
        {
            Mesh mesh = new Mesh();
            mesh.Clear();
            Vector3[] vertices = new Vector3[7];
            vertices[0] = Vector3.zero;
            for (int i = 0; i < 6; i++)
            {
                float angle = i * Mathf.PI * 2f / 6;
                vertices[i + 1] = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * width;
            }
            mesh.vertices = vertices;
            int[] triangles = new int[18];

            for (int i = 0; i < 6; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 2] = i + 1;
                triangles[i * 3 + 1] = (i + 1) % 6 + 1;
            }
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            platformMesh = mesh;
            interactionElevatorTextController = new InteractionElevatorTextController();
        }

        void IElevatorPlatform.AddGOToGOManager(GameObject elevator, int x, int y, int z)
        {
            coroutineOwner.AddElevator(elevator, x, y, z);
        }


        private IEnumerator CheckButtonPress(GameObject elevatorPlatform)
        {

            while (true)
            {
                if (animating)
                {
                    textMeshPro.text = "";
                    coroutineOwner.StopCoroutine(coroutine);

                }
                if (Input.GetKeyDown(KeyCode.Q) && (
                    directionMove == DirectionMoveEnum.Up ||
                    directionMove == DirectionMoveEnum.Both))
                {
                    var position = positionCalculator.CalculatePosition(elevatorPlatform);
                    var upper = GetGOFromGOManager(position.x, position.y + 1, position.z);
                    coroutine2 = anim.MoveUp(upper);
                    StartAnim(elevatorPlatform);
                }
                else if (Input.GetKeyDown(KeyCode.R) && (
                    directionMove == DirectionMoveEnum.Down ||
                    directionMove == DirectionMoveEnum.Both))
                {
                    var position = positionCalculator.CalculatePosition(elevatorPlatform);
                    var downer = GetGOFromGOManager(position.x, position.y - 1, position.z);
                    coroutine2 = anim.MoveDown(downer);
                    StartAnim(elevatorPlatform);
                }
                yield return null;
            }
        }
        internal void Interact(GameObject elevatorPlatform)
        {
            if (animating) return;
            if (positionCalculator == null) positionCalculator = new PositionCalculatorService();
            coroutine = CheckButtonPress(elevatorPlatform);
            coroutineOwner.StartCoroutine(coroutine);
            var position = positionCalculator.CalculatePosition(elevatorPlatform);
            var upper = GetGOFromGOManager(position.x, position.y + 1, position.z);
            if (upper != null) directionMove = DirectionMoveEnum.Up;
            var down = GetGOFromGOManager(position.x, position.y - 1, position.z);
            if (down != null)
                directionMove = directionMove == DirectionMoveEnum.Up ? DirectionMoveEnum.Both : DirectionMoveEnum.Down;

            textMeshPro.text = interactionElevatorTextController.GetInteractionText(directionMove);
        }

        private void StartAnim(GameObject elevatorPlatform)
        {
            elevatorPlatform.GetComponent<BoxCollider>().enabled = false;
            animating = true;

            anim.platform = elevatorPlatform;
            coroutineOwner.StartCoroutine(StartCoroutineWithCallback(coroutine2, () =>
            {
                animating = false;
            }));
            elevatorPlatform.GetComponent<BoxCollider>().enabled = true;
        }

        private IEnumerator StartCoroutineWithCallback(IEnumerator coroutine, System.Action onComplete)
        {
            yield return coroutineOwner.StartCoroutine(coroutine);
            onComplete?.Invoke();
        }

        internal void StopInteract(GameObject elevatorPlatform)
        {
            coroutineOwner.StopCoroutine(coroutine);
            textMeshPro.text = "";
            Debug.Log("stop Inteact");
        }


        GameObject GetGOFromGOManager(int x, int y, int z)
        {
            return coroutineOwner.GetElevator(x, y, z);
        }
        GameObject IElevatorPlatform.GetPlatform(float s)
        {
            var go = new GameObject();
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            var renderer = go.AddComponent<MeshRenderer>();
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshFilter.mesh = platformMesh;
            MeshCollider meshCollider = go.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = platformMesh;
            meshCollider.convex = true;
            BoxCollider boxCollider = go.AddComponent<BoxCollider>();
            var inter = this as IElevatorPlatform;
            boxCollider.size = new Vector3(1, (platformMesh.bounds.size.y + inter.HeightBoxForInteract) * anim.height, 1.73f);
            boxCollider.center = new Vector3(0, boxCollider.size.y / 2, 0);
            boxCollider.isTrigger = true;
            var interaction = go.AddComponent<ElevatorInteractionHex>();
            interaction.SetPlatform(this);
            return go;
        }

        void IElevatorPlatform.DeleteElevator()
        {
            GameObject.DestroyImmediate(anim.doors);
        }

        void IElevatorPlatform.ReinitData()
        {
            InitData(1);
        }

        bool IElevatorPlatform.RemoveGOToGOManager(int x, int y, int z)
        {
            return coroutineOwner.RemoveElevator(x, y, z);
        }

        GameObject IElevatorPlatform.GetGOFromManager(int x, int y, int z)
        {
            return GetGOFromGOManager(x, y, z);
        }
    }
}
