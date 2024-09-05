using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBehaviour : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 5.0f;
    [SerializeField] private float rotationSpeed = 10.0f; // Adjust rotation speed as needed

    private Rigidbody _rigedbody;
    private Vector3 _movementDirection = Vector3.zero;

    private void Awake()
    {
        _rigedbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        if (_rigedbody == null) return;

        Vector3 movement = _movementDirection * _movementSpeed * Time.fixedDeltaTime;
        _rigedbody.MovePosition(_rigedbody.position + movement);
    }
    private void HandleRotation()
    {
        if (_movementDirection != Vector3.zero)
        {
            // Calculate the desired rotation
            Quaternion targetRotation = Quaternion.LookRotation(_movementDirection.normalized, Vector3.up);
            // Smoothly rotate towards the desired rotation
            _rigedbody.rotation = Quaternion.Slerp(_rigedbody.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    public void SetMovementDirection(Vector3 direction)
    {
        _movementDirection = direction;
    }
}
