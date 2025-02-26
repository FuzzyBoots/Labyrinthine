using UnityEngine;

namespace MazeAsset.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float walkSpeed = 5f;
        public float sprintSpeed = 10f;
        public float jumpHeight = 2f;
        public float gravity = -9.81f;
        public Camera playerCamera;

        private CharacterController controller;
        private Vector3 velocity;
        private bool isGrounded;

        private float currentSpeed;
        private Transform currentPlatform;
        private Vector3 lastPlatformPosition;

        void Start()
        {
            controller = GetComponent<CharacterController>();
            currentSpeed = walkSpeed;
        }

        void Update()
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.5f);

            if (isGrounded)
            {
                if (hit.collider != null)
                {
                    Transform platform = hit.collider.transform;

                    if (currentPlatform != platform)
                    {
                        currentPlatform = platform;
                        lastPlatformPosition = currentPlatform.position;
                    }
                }
                else
                {
                    currentPlatform = null;
                }

                if (velocity.y < 0)
                {
                    velocity.y = -2f;
                }
            }

            // Movement
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 forward = playerCamera.transform.forward;
            Vector3 right = playerCamera.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 move = forward * moveZ + right * moveX;
            controller.Move(move * currentSpeed * Time.deltaTime);

            // Sprinting
            if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
            {
                currentSpeed = sprintSpeed;
            }
            else
            {
                currentSpeed = walkSpeed;
            }

            // Jumping
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // Gravity
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            if (currentPlatform != null)
            {
                Vector3 platformMovement = currentPlatform.position - lastPlatformPosition;
                controller.Move(platformMovement);
                lastPlatformPosition = currentPlatform.position;
            }
        }
    }
}