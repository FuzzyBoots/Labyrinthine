using UnityEngine;

public class MazeRightHandRule : MonoBehaviour
{
    public CharacterController controller;
    public float moveSpeed = 5f;
    public float rotationSpeed = 90f; // Degrees per second
    public float cellWidth = 5f; // Width of each maze cell
    public LayerMask wallLayer; // Layer containing maze walls

    private Vector3 currentDirection = Vector3.forward;
    private float targetRotation = 0f;
    private bool isRotating = false;

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
        if (CanMoveForward())
        {
            Vector3 moveDirection = currentDirection * moveSpeed * Time.deltaTime;
            controller.Move(moveDirection);
        }
        else
        {
            Debug.Log("Bonk");
            TurnLeft();
        }
    }

    bool CanMoveForward()
    {
        Vector3 checkPosition = transform.position + currentDirection * (cellWidth / 2f + 0.1f); // Add a small offset
        return !Physics.CheckBox(checkPosition, new Vector3(cellWidth / 2f, 1f, cellWidth / 2f), transform.rotation, wallLayer);
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
            currentDirection = Quaternion.Euler(0f, targetRotation - transform.eulerAngles.y, 0f) * currentDirection;
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
        Debug.DrawRay(checkPosition, rightDirection * cellWidth, Color.yellow);

        if (!Physics.CheckBox(checkPosition, new Vector3(cellWidth / 2f, 1f, cellWidth / 2f), transform.rotation, wallLayer))
        {
            TurnRight();
        }
    }

	void OnDrawGizmos() // Use OnDrawGizmos for drawing in the editor
	{
		if (Application.isPlaying) // Only draw gizmos while playing
		{
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