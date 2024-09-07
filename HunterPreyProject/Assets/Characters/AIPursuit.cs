using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPursuit : MonoBehaviour
{
    [SerializeField] private Transform _prey = null;
    [SerializeField] private float _stopRadius = 5.0f;

    [SerializeField] private float _searchRadius = 20.0f;
    [SerializeField] private float _searchTime = 2.0f;

    [SerializeField] private float _fieldOfViewAngle = 45f;
    [SerializeField] private float _fieldOfViewDistance = 20f;


    private NavMeshAgent _agent;

    private bool _isPreyDetected = false;
    private Vector3 _lastknownPosition;
    private Coroutine _searchCoroutine = null;



    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    

    // Update is called once per frame
    void Update()
    {
        if (IsPreyInFieldOfView(_prey))
        {
            _isPreyDetected = true;
            _lastknownPosition = _prey.position;
            Debug.Log("Prey detected in detection radius!");
        }
        else
        {
            _isPreyDetected = false;
            SearchPrey();
        }
      

        if (_isPreyDetected)
        {
            PursuitPrey();
        }
    }

    private void PursuitPrey()
    {
        float distanceFromPrey = Vector3.Distance(transform.position, _prey.position);
        if (distanceFromPrey > _stopRadius)
        {
            Vector3 directionAwayFromPrey = (transform.position - _prey.position).normalized;
            Vector3 destination = _prey.position + (directionAwayFromPrey * _stopRadius);

            // Set the agent's destination
            _agent.SetDestination(destination);
        }
        else
        {
            _agent.ResetPath();
        }
    }

    private void SearchPrey()
    {
        if (_searchCoroutine == null)
        {
            //  a coroutine is a method that can pause execution and return control to Unity
            //  but then continue where it left off on the following frame
            _searchCoroutine = StartCoroutine(SearchForPrey());
        }
    }

    private bool IsPreyInFieldOfView(Transform preyTransform)
    {
        // Calculate direction to prey
        Vector3 directionToPrey = preyTransform.position - transform.position;
        float distanceToPreySqr = directionToPrey.sqrMagnitude;

        if (distanceToPreySqr > _fieldOfViewDistance * _fieldOfViewDistance)
        {
            return false;
        }

        
        directionToPrey.Normalize();
        float angle = Vector3.Angle(transform.forward, directionToPrey);

        return angle < _fieldOfViewAngle / 2f; // Check if prey is within the field of view angle
    }

    private void OnDrawGizmos()
    {
        // Draw the FOV cone
        {
            Gizmos.color = Color.green;

            // Calculate the left and right boundaries of the cone
            Vector3 leftBoundary = Quaternion.Euler(0, -_fieldOfViewAngle / 2, 0) * transform.forward * _fieldOfViewDistance;
            Vector3 rightBoundary = Quaternion.Euler(0, _fieldOfViewAngle / 2, 0) * transform.forward * _fieldOfViewDistance;

            Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
            Gizmos.DrawLine(transform.position + leftBoundary, transform.position + rightBoundary);
        }

    }



    private IEnumerator SearchForPrey()
    {
        yield return StartCoroutine(RotationSearch());
        yield return StartCoroutine(WalkSearch());


        // Reset the agent path after the search is complete
        _agent.ResetPath();
        Debug.Log("Search complete, resetting path.");
        _searchCoroutine = null; // Reset the coroutine reference
    }

    private IEnumerator RotationSearch()
    {
        float rotationSpeed = 45; // Degrees per second
        float totalRotation = 360f;
        float rotationDuration = totalRotation / rotationSpeed;

        Quaternion initialRotation = transform.rotation;
        bool rotateClockwise = Random.value > 0.5f; // Randomly choose clockwise or counter-clockwise
       
        
        float rotationElapsedTime = 0f;

        while (rotationElapsedTime < rotationDuration)
        {
            float rotationAmount = rotationSpeed * Time.deltaTime;

            // Rotate in the chosen direction
            if (rotateClockwise)
            {
                transform.Rotate(Vector3.up, rotationAmount);
            }
            else
            {
                transform.Rotate(Vector3.up, -rotationAmount);
            }

            // Check if the prey comes back into view during the rotation
            if (CheckForPreyAndHandleDetection())
            {
                yield break; // Exit coroutine if prey is found
            }

            rotationElapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        transform.rotation = Quaternion.Euler(0, (initialRotation.eulerAngles.y + totalRotation) % 360, 0);
    }

    private IEnumerator WalkSearch()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _searchTime)
        {
            Vector3 randomDirection = Random.insideUnitSphere * _searchRadius;
            randomDirection += _lastknownPosition;

            RaycastHit hit;// Cast a ray downwards to find a valid point on the NavMesh
            bool isHitFound = Physics.Raycast(randomDirection + Vector3.up * 50, Vector3.down, out hit, 100f, NavMesh.AllAreas);


            if (isHitFound)
            {
                Vector3 destination = hit.point;
                _agent.SetDestination(destination);

                while (_agent.pathPending || _agent.remainingDistance > _stopRadius)
                {
                    if (CheckForPreyAndHandleDetection())
                    {
                        yield break; // Exit coroutine if prey is found
                    }

                    yield return null; // Wait until the next frame
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }
    }

    private bool CheckForPreyAndHandleDetection()
    {
        if (IsPreyInFieldOfView(_prey))
        {
            _isPreyDetected = true;
            _lastknownPosition = _prey.position;
            Debug.Log("Prey found during search!");

            _agent.ResetPath();
            _searchCoroutine = null; // Reset the coroutine reference

            return true; // Prey found
        }
        return false; // Prey not found
    }
}
