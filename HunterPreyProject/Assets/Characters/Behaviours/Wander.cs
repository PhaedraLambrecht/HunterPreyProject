using UnityEngine;
using UnityEngine.AI;

namespace BehaviuorTree
{
    public class Wander : Node
    {
        private NavMeshAgent _agent;
        private float _wanderRadius;
        private float _maxWanderTime;
        private float _stepLengthExponent;
        private float _timer;
        private static int _numSteps = 10;

        public Wander(NavMeshAgent agent, float wanderRadius, float maxWanderTime, float stepLengthExponent)
        {
            _agent = agent;
            _wanderRadius = wanderRadius;
            _maxWanderTime = maxWanderTime;
            _stepLengthExponent = stepLengthExponent;
            _timer = maxWanderTime; // Start with the timer ready to trigger the first move
        }

        public override NodeState Evaluate()
        {
            _timer += Time.deltaTime;

            if (_timer >= _maxWanderTime)
            {
                Vector3 newPos = LevyFlight(_agent.transform.position, _wanderRadius, _stepLengthExponent);
                _agent.SetDestination(newPos);
                _timer = 0;
            }

            // Return Running while the agent is moving
            state = NodeState.Running;
            return state;
        }

        // Lévy Flight implementation
        private Vector3 LevyFlight(Vector3 origin, float dist, float stepLengthExponent)
        {
            Vector3 accumulatedDisplacement = Vector3.zero;

            for (int i = 0; i < _numSteps; i++)
            {
                float randAngle = Random.Range(0, (2 * Mathf.PI)); // Get a random angle between 0 & 2pi
                float stepLength = Mathf.Pow(Random.value, -1f / stepLengthExponent); // Calculate step length
                stepLength = Mathf.Min(stepLength, dist);

                Vector3 direction = new Vector3(Mathf.Cos(randAngle), 0, Mathf.Sin(randAngle)); // Calculate the direction vector
                Vector3 randomStep = direction * stepLength; // Calculate the random step

                accumulatedDisplacement += randomStep; // Accumulate the displacement
            }

            // Apply the scaling factor based on number of steps and exponent
            float scaleFactor = Mathf.Pow(_numSteps, 1f / stepLengthExponent);
            accumulatedDisplacement /= scaleFactor;

            Vector3 newPos = origin + accumulatedDisplacement;

            // Ensure the new position is within the NavMesh
            NavMeshHit navHit;
            NavMesh.SamplePosition(newPos, out navHit, dist, -1);

            return navHit.position;
        }
    }
}