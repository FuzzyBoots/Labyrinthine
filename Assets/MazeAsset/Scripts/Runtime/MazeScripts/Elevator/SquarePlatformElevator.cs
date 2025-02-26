using System;
using System.Collections;
using TMPro;
using UnityEngine;


namespace MazeAsset.MazeGenerator
{
    [System.Serializable]
    internal class SquarePlatformElevator : IElevatorPlatform
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
        [SerializeField]
        private PositionCalculatorService positionCalculator;
        private DirectionMoveEnum directionMove;
        [SerializeField]
        private InteractionElevatorTextController interactionElevatorTextController;
        [SerializeField]
        private TMP_Text textMeshPro;

        internal SquarePlatformElevator(MazeGeneratorManager coroutineOwner, TMP_Text textMeshPro, AnimatorElevatorService animatorElevatorService)
        {
            InitData(1);
            positionCalculator = new PositionCalculatorService();
            this.textMeshPro = textMeshPro;
            this.anim = animatorElevatorService;
            this.coroutineOwner = coroutineOwner;
        }

        private void InitData(float width)
        {
            Mesh mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = new Vector3[]
            {
            new Vector3(-width / 2, 0, -width / 2),
            new Vector3(width / 2, 0, -width / 2),
            new Vector3(width / 2, 0, width / 2),
            new Vector3(-width / 2, 0, width / 2)
            };
            mesh.triangles = new int[] { 0, 2, 1, 0, 3, 2 };
            mesh.RecalculateNormals();

            platformMesh = mesh;
            interactionElevatorTextController = new InteractionElevatorTextController();
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
                if (animating) coroutineOwner.StopCoroutine(coroutine);
                if (Input.GetKeyDown(KeyCode.Q) && (
                    directionMove == DirectionMoveEnum.Up ||
                    directionMove == DirectionMoveEnum.Both))
                {
                    var position = positionCalculator.CalculatePosition(elevatorPlatform);
                    var upper = GetGOFromGOManager(position.x, position.y + 1, position.z);
                    Debug.Log(upper.name);
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
            BoxCollider collider = go.AddComponent<BoxCollider>();
            collider.size = new Vector3(platformMesh.bounds.size.x, platformMesh.bounds.size.y + 1f, platformMesh.bounds.size.z);
            collider.isTrigger = true;
            var interaction = go.AddComponent<ElevatorInteractionSquare>();
            interaction.SetPlatform(this);
            return go;
        }

        internal void Interact(GameObject elevatorPlatform)
        {
            if (animating) return;
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
        }


        GameObject GetGOFromGOManager(int x, int y, int z)
        {
            return coroutineOwner.GetElevator(x, y, z);
        }
        void IElevatorPlatform.DeleteElevator()
        {
            GameObject.DestroyImmediate(anim.doors);
        }

        void IElevatorPlatform.AddGOToGOManager(GameObject elevator, int x, int y, int z)
        {
            coroutineOwner.AddElevator(elevator, x, y, z);
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