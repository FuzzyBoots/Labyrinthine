using UnityEngine;

namespace MazeAsset.Player
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] Transform player;
        [SerializeField] float distance = 5f;
        [SerializeField] float height = 2f;
        [SerializeField] float rotationSpeed = 5f;
        [SerializeField] float zoomSpeed = 2f;
        [SerializeField] float minDistance = 2f;
        [SerializeField] float maxDistance = 10f;
        [SerializeField] float minClamp = -30f;
        [SerializeField] float maxClamp = 85f;

        private float directionX = 0f;
        private float directionY = 0f;

        void Update()
        {
            directionX += Input.GetAxis("Mouse Y") * rotationSpeed;
            directionY -= Input.GetAxis("Mouse X") * rotationSpeed;

            directionX = Mathf.Clamp(directionX, minClamp, maxClamp);

            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            distance -= scrollInput * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            Vector3 direction = new Vector3(0, 0, -distance);
            Quaternion rotation = Quaternion.Euler(-directionX, -directionY, 0);

            Vector3 newPosition = player.position + Vector3.up * height + rotation * direction;
            transform.position = newPosition;

            transform.LookAt(player.position + Vector3.up * height);
        }
    }
}