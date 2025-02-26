using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace MazeAsset.MazeGenerator
{
    [Serializable]
    public class AnimatorElevatorService
    {
        private const string NameOfRootForCube = "RootForCubeDooor";
        internal GameObject platform { get; set; }
        [SerializeField]
        internal GameObject doors;
        private float width;
        [SerializeField]
        internal float height;
        private int doorSize;

        internal AnimatorElevatorService(float height, float width, ShapeCellEnum shapeCellEnum)
        {
            this.height = height;
            this.width = width;
            if (shapeCellEnum == ShapeCellEnum.Square)
            {
                doorSize = 4;
            }
            else
            {
                this.width *= 1.5f;
                doorSize = 6;
            }
            doors = CreateCubesAsDoors();
            doors.transform.position = new Vector3(99999, 99999, 99999);
        }

        internal GameObject CreateCubesAsDoors()
        {
            GameObject.DestroyImmediate(GameObject.Find(NameOfRootForCube));
            var doors = new GameObject(NameOfRootForCube);
            for (int i = 0; i < doorSize; i++)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                cube.transform.SetParent(doors.transform);

                float angle = i * Mathf.PI * 2f / doorSize;
                Vector3 position = new Vector3(
                    Mathf.Cos(angle) * (width / 2.5f),
                    height / 2 - height * 0.1f,
                    Mathf.Sin(angle) * (width / 2.5f)
                );

                cube.transform.localPosition = position;
                cube.transform.localScale = new Vector3(width * .1f, width * .1f, width * .1f);
                cube.transform.localRotation = Quaternion.identity;
            }
            return doors;
        }
        public IEnumerator MoveUp(GameObject platform2)
        {
            Vector3 previos = platform.transform.position;
            platform2.SetActive(false);
            SetDoor();
            yield return MoveDoor(Vector3.up * height / 2);
            yield return MoveElevator(Vector3.up * height);
            yield return MoveDoor(Vector3.down * height / 2);
            platform2.SetActive(true);
            platform.transform.position = previos;
            doors.transform.position = new Vector3(99999, 99999, 99999);
        }

        public IEnumerator MoveDown(GameObject platform2)
        {
            platform2.SetActive(false);
            Vector3 previos = platform.transform.position;
            SetDoor();
            yield return MoveDoor(Vector3.up * height / 2);
            yield return MoveElevator(Vector3.down * height);
            yield return MoveDoor(Vector3.down * height / 2);
            platform2.SetActive(true);

            platform.SetActive(false);
            platform.transform.position = previos;
            platform.SetActive(true);
            doors.transform.position = new Vector3(99999, 99999, 99999);
        }

        private void SetDoor()
        {
            doors.transform.localPosition = platform.transform.position - (Vector3.up * (height / 2));
        }



        private IEnumerator MoveDoor(Vector3 direction)
        {
            Vector3 doorOpenPositions = doors.transform.localPosition + direction;
            Vector3 startPos = doors.transform.localPosition;
            float startTime = Time.time;
            float duration = 1f * height;
            while (Time.time - startTime < duration)
            {

                float t = (Time.time - startTime) / duration;
                doors.transform.localPosition = Vector3.Lerp(startPos, doorOpenPositions, t);

                yield return Time.deltaTime;
            }
            doors.transform.localPosition = doorOpenPositions;

        }

        private IEnumerator MoveElevator(Vector3 direction)
        {
            Vector3 doorOpenPositions = doors.transform.localPosition + direction;
            Vector3 doorStartPos = doors.transform.localPosition;

            Vector3 startPos = platform.transform.localPosition;
            Vector3 targetPos = startPos + direction;
            float startTime = Time.time;
            float duration = 3f * height;
            while (Time.time - startTime < duration)
            {
                float t = (Time.time - startTime) / duration;
                platform.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
                doors.transform.localPosition = Vector3.Lerp(doorStartPos, doorOpenPositions, t);

                yield return Time.deltaTime;
            }
            doors.transform.localPosition = doorOpenPositions;
            platform.transform.localPosition = targetPos;
        }

        internal bool IsSafeToMoveDoors()
        {
            SetDoor();
            foreach (Transform door in doors.transform)
            {
                Vector3 boxSize = door.GetComponent<Collider>().bounds.size;
                Vector3 movePosition = door.position + Vector3.up * height;

                Collider[] colliders = Physics.OverlapBox(movePosition, boxSize / 2, Quaternion.identity);

                foreach (Collider hit in colliders)
                {
                    if (hit.CompareTag("Player"))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
