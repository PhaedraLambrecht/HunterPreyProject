using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    public class Search : Node
    {
        private NavMeshAgent _agent;
        private CoroutineManager _coroutineManager;

        private float _searchRadius;
        private float _searchTime;

        private Coroutine _searchCoroutine = null;

        private bool _isSearching;
        private Vector3 _lastKnownPosition;

        private float _startTime;


        public Search(NavMeshAgent agent, CoroutineManager coroutineManager, float searchRadius, float searchTime, Vector3 lastKnownPosition)
        {
            _agent = agent;
            _coroutineManager = coroutineManager;
            _searchRadius = searchRadius;
            _searchTime = searchTime;
            _lastKnownPosition = lastKnownPosition;
        }


        public override NodeState Evaluate()
        {
            if (_isSearching)
            {
                // Check if the coroutine is still running
                if (_searchCoroutine == null)
                {
                    _isSearching = false;
                    state = NodeState.Succes; // Search completed successfully
                }
                else
                {
                    // Check if the timeout has been exceeded
                    if (Time.time - _startTime > _searchTime)
                    {
                        _isSearching = false;
                        state = NodeState.Failure; // Search timed out
                    }
                    else
                    {
                        state = NodeState.Running; // Continue running
                    }
                }

                return state;
            }

            _isSearching = true;
            _startTime = Time.time; // Start the timer
            _searchCoroutine = _coroutineManager.StartManagedCoroutine(SearchForPrey(_lastKnownPosition));
            return NodeState.Running;
        }

        private IEnumerator SearchForPrey(Vector3 lastKnownPosition)
        {
            float elapsedTime = 0f;

            yield return RotationSearch();

            while (elapsedTime < _searchTime)
            {
                yield return WalkSearch(lastKnownPosition);

                elapsedTime += Time.deltaTime;

                // Check if the timeout has been exceeded
                if (elapsedTime >= _searchTime)
                {
                    break;
                }
            }

            // Reset the agent path after the search is complete
            _agent.ResetPath();
            Debug.Log("Search complete, resetting path.");

            _isSearching = false;
            _searchCoroutine = null;
        }

        private IEnumerator RotationSearch()
        {
            // Randomize rotation speed and duration
            float rotationSpeed = Random.Range(30f, 60f); // Degrees per second
            float totalRotation = Random.Range(180f, 360f);
            float rotationDuration = totalRotation / rotationSpeed;

            Quaternion initialRotation = _agent.transform.rotation;
            bool rotateClockwise = Random.value > 0.5f;

            float rotationElapsedTime = 0f;

            while (rotationElapsedTime < rotationDuration)
            {
                Debug.Log("Rotation search happening.");
                float rotationAmount = rotationSpeed * Time.deltaTime;

                // Rotate in the chosen direction
                if (rotateClockwise)
                {
                    _agent.transform.Rotate(Vector3.up, rotationAmount);
                }
                else
                {
                    _agent.transform.Rotate(Vector3.up, -rotationAmount);
                }

                rotationElapsedTime += Time.deltaTime;
                yield return null;
            }

            _agent.transform.rotation = Quaternion.Euler(0, (initialRotation.eulerAngles.y + totalRotation) % 360, 0);
        }

        private IEnumerator WalkSearch(Vector3 lastKnownPosition)
        {
            // Randomize walk search time
            float searchTime = Random.Range(_searchTime * 0.5f, _searchTime * 1.5f);
            float elapsedTime = 0f;

            while (elapsedTime < searchTime)
            {
                Debug.Log("Walking search happening.");
                Vector3 randomDirection = Random.insideUnitSphere * _searchRadius;
                randomDirection += lastKnownPosition;

                RaycastHit hit;
                bool isHitFound = Physics.Raycast(randomDirection + Vector3.up * 50, Vector3.down, out hit, 100f, NavMesh.AllAreas);

                if (isHitFound)
                {
                    Vector3 destination = hit.point + Vector3.up * 0.5f; // Add a small offset to ensure the agent moves above the ground
                    _agent.SetDestination(destination);

                    while (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance)
                    {
                        yield return null;
                    }
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}