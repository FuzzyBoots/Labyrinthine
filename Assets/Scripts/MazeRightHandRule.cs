using UnityEngine;

public class MazeRightHandRule : MonoBehaviour
{
	[SerializeField] private CharacterController controller;
	[SerializeField] private float moveSpeed = 5f;
	[SerializeField] private float rotationSpeed = 90f; // Degrees per second
	[SerializeField] private float cellWidth = 5f; // Width of each maze cell
	[SerializeField] private LayerMask wallLayer; // Layer containing maze walls

    private Vector3 currentDirection = Vector3.forward;
    private float targetRotation = 0f;
    private bool isRotating = false;

    float _canTurn = 0f;
    [SerializeField] private float _timeToTurn = 1f;

    void Update()
    {
        if (isRotating)
        {
            RotateCharacter();
        }
        else
        {
            MoveCharacter();
        }		
	}

    void MoveCharacter()
    {
        Vector3 moveDirection = currentDirection * moveSpeed * Time.deltaTime;
        CollisionFlags collisionFlags = controller.Move(moveDirection);

        if (collisionFlags == CollisionFlags.Sides) { 
            TurnLeft();
        }
    }

    void TurnRight()
    {
        targetRotation = transform.eulerAngles.y + 90f;
        isRotating = true;
    }

    void TurnLeft()
    {
        targetRotation = transform.eulerAngles.y - 90f;
        isRotating = true;
    }

    void RotateCharacter()
    {
        float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(0f, angle, 0f);

        if (Mathf.Approximately(angle, targetRotation))
        {
            isRotating = false;
            _canTurn = Time.time + _timeToTurn;
            currentDirection = transform.forward;
            currentDirection.Normalize();
        }
    }

    void FixedUpdate()
    {
        if (!isRotating)
        {
            CheckRightWall();
        }
    }

    void CheckRightWall()
    {
        Vector3 rightDirection = Quaternion.Euler(0f, 90f, 0f) * currentDirection;
        Vector3 checkPosition = transform.position + rightDirection * (cellWidth / 2f + 0.1f);

        Debug.Log(checkPosition);

        if (!Physics.CheckBox(checkPosition, new Vector3(cellWidth / 2f, 1f, cellWidth / 2f), transform.rotation, wallLayer) 
            && Time.time > _canTurn)
        {
            TurnRight();
        }
    }

	void OnDrawGizmos() // Use OnDrawGizmos for drawing in the editor
	{
		if (Application.isPlaying) // Only draw gizmos while playing
		{
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.forward * 5);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, currentDirection * 5);
            // Debug visualization (draw the forward check box)
            Vector3 forwardCheckPosition = transform.position + currentDirection * (cellWidth / 2f + 0.1f);
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(forwardCheckPosition, new Vector3(cellWidth, 2f, cellWidth));

			// Debug visualization (draw the right check box)
			Vector3 rightDirection = Quaternion.Euler(0f, 90f, 0f) * currentDirection;
			Vector3 rightCheckPosition = transform.position + rightDirection * (cellWidth / 2f + 0.1f);
			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube(rightCheckPosition, new Vector3(cellWidth, 2f, cellWidth));
		}
	}
}