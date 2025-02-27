using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class RobotMovement : MonoBehaviour
{
    // Basic idea behind the movement is that it will have a roam mode and a charge mode
    // In roam mode, it will operate on either the right-hand or left-hand rule.
    // If it sees the player, it will charge forward until it hits a wall and then resume.

    enum MovementStates
    {
        Roaming,
        Charging,
        Rotating,
        CounterRotating
    }

    [SerializeField] bool _followsRightHandRule;

    [SerializeField] float _travelSpeed = 5f;

    [SerializeField] float _chargeSpeed = 8f;

    [SerializeField] float _turnSpeed = 90f;

    MovementStates _state = MovementStates.Roaming;

    CharacterController _characterController;
    private Vector3 _boxCheckSize;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        switch (_state)
        {
            case MovementStates.Roaming:
                HandleRoaming();
                break;
            case MovementStates.Charging:
                HandleCharging();
                break;
            case MovementStates.Rotating:
                HandleRotating();
                break;
            case MovementStates.CounterRotating:
                HandleRotating(true);
                break;
            default:
                Debug.Log("Somehow entered bad movement state.");
                _state = MovementStates.Roaming;
        }
    }

    private void HandleRotating(bool reverse = false)
    {
        // Rotate until we've made a 90 degree turn.

    }

    private void HandleCharging()
    {

    }

    private void HandleRoaming()
    {
        CollisionFlags collision = _characterController.Move(transform.forward * _travelSpeed * Time.deltaTime);
        if (collision == CollisionFlags.Sides)
        {
            _state = MovementStates.CounterRotating;
            return;
        }

        //if (Physics.BoxCast(transform.position, _characterController.radius, transform.forward, ))
        //{

        //}

        Vector3 direction = _followsRightHandRule ? transform.right : -transform.right;

        if (!Physics.CheckBox(transform.position + direction * _characterController.radius, _boxCheckSize))
        {
            _state = MovementStates.Rotating;
        }


    }
}
