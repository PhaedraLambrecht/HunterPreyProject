using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    protected MovementBehaviour _movementBehaviour;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        _movementBehaviour = GetComponent<MovementBehaviour>();
    }
}
