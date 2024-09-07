using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPursuit : MonoBehaviour
{
    [SerializeField] private Transform _prey = null;
//    [SerializeField] private float _searchRadius = 10.0f;


    private NavMeshAgent _agent;
    private bool _isPreyDetected = false;
    private Vector3 _lastknownPosition;
    private float _stopRadius = 2.0f;


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }


    // Update is called once per frame
    void Update()
    {
        if (_isPreyDetected)
        {
            float distanceFromPrey = Vector3.Distance(transform.position, _prey.position);

            if (distanceFromPrey > _stopRadius)
            {
                _agent.SetDestination(_prey.position);
                _lastknownPosition = _prey.position;
            }
        }
        else
        {
            _agent.SetDestination(_lastknownPosition);
          
            
            float distanceFromLastKnown = Vector3.Distance(transform.position, _lastknownPosition);
            if (distanceFromLastKnown > _stopRadius)
            {
                _agent.SetDestination(_lastknownPosition);
              
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Prey"))
        {
            _isPreyDetected = true;
            _lastknownPosition = _prey.position;

            // Prey is in the cone's vision
            Debug.Log("Prey detected at position: " + _prey.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Prey"))
        {
            _isPreyDetected = false;
            Debug.Log("Prey out of sight, moving to last known position." + _lastknownPosition);
        }
    }
}
