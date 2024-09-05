using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputAsset;
    [SerializeField] private InputActionReference _movementAction;
  
    private MovementBehaviour _movementBehaviour;


    private void Awake()
    {
        _movementBehaviour = GetComponent<MovementBehaviour>();
        if (_inputAsset != null)
        {
            _inputAsset.Enable();
        }
    }

    private void OnEnable()
    {
        if (_inputAsset == null) return;

        _inputAsset.Enable();
    }
    private void OnDisable()
    {
        if (_inputAsset == null) return;

        _inputAsset.Disable();
    }


    private void Update()
    {
        HandleMovementInput();
    }
    void HandleMovementInput()
    {
        if (_movementAction == null || _movementBehaviour == null) return;

        Vector2 movementInput = _movementAction.action.ReadValue<Vector2>();
        Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y); // Assuming Z axis is forward
        _movementBehaviour.SetMovementDirection(movement);
    }
}
