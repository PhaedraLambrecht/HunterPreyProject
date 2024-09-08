using System.Collections;
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
                    state = NodeState.Running; // Continue running
                }
                return state;
            }

            _isSearching = true;
            _searchCoroutine = _coroutineManager.StartManagedCoroutine(SearchForPrey(_lastKnownPosition));
            return NodeState.Running;
        }

        private IEnumerator SearchForPrey(Vector3 lastKnownPosition)
        {
            yield return RotationSearch();
            yield return WalkSearch(lastKnownPosition);

            // Reset the agent path after the search is complete
            _agent.ResetPath();
            Debug.Log("Search complete, resetting path.");

            _isSearching = false;
            _searchCoroutine = null;
        }

        private IEnumerator RotationSearch()
        {
            float rotationSpeed = 45f; // Degrees per second
            float totalRotation = 360f;
            float rotationDuration = totalRotation / rotationSpeed;

            Quaternion initialRotation = _agent.transform.rotation;
            bool rotateClockwise = Random.value > 0.5f;

            float rotationElapsedTime = 0f;

            while (rotationElapsedTime < rotationDuration)
            {
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
            float elapsedTime = 0f;

            while (elapsedTime < _searchTime)
            {
                Vector3 randomDirection = Random.insideUnitSphere * _searchRadius;
                randomDirection += lastKnownPosition;

                RaycastHit hit;
                bool isHitFound = Physics.Raycast(randomDirection + Vector3.up * 50, Vector3.down, out hit, 100f, NavMesh.AllAreas);

                if (isHitFound)
                {
                    Vector3 destination = hit.point;
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