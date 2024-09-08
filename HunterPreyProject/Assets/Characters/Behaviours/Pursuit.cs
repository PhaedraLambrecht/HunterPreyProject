using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    public class Pursuit : Node
    {
        private NavMeshAgent _agent;
        private Transform _prey;

        private float _stopRadius;
        private float _fieldOfViewAngle;
        private float _fieldOfViewDistance;
        private float _searchRadius;

        private Vector3 _lastKnownPosition;
        private bool _isPursuing = false;



        public Pursuit(NavMeshAgent agent, Transform prey, float stopRadius, float fieldOfViewAngle, float fieldOfViewDistance, float searchRadius)
        {
            _agent = agent;
            _prey = prey;
            _stopRadius = stopRadius;
            _fieldOfViewAngle = fieldOfViewAngle;
            _fieldOfViewDistance = fieldOfViewDistance;
            _searchRadius = searchRadius;
        }

        public override NodeState Evaluate()
        {
            if (IsPreyInFieldOfView(_prey))
            {
                _lastKnownPosition = _prey.position;
                _isPursuing = true;
                PursuitPrey();
                state = NodeState.Running;
            }
            else if (_isPursuing && Vector3.Distance(_agent.transform.position, _prey.position) <= _searchRadius)
            {
                // Continue pursuing if the prey was recently visible and is still within the search radius
                PursuitPrey();
                state = NodeState.Running;
            }
            else
            {
                // Stop pursuing and transition to search if the prey is out of view and beyond the search radius
                _agent.ResetPath();
                _isPursuing = false;
                state = NodeState.Failure;
            }

            return state;
        }


        private bool IsPreyInFieldOfView(Transform preyTransform)
        {
            Vector3 directionToPrey = preyTransform.position - _agent.transform.position;
            float distanceToPreySqr = directionToPrey.sqrMagnitude;

            if (distanceToPreySqr > _fieldOfViewDistance * _fieldOfViewDistance)
            {
                return false;
            }

            directionToPrey.Normalize();
            float angle = Vector3.Angle(_agent.transform.forward, directionToPrey);

            return angle < _fieldOfViewAngle / 2f;
        }

        private void PursuitPrey()
        {
            float distanceFromPrey = Vector3.Distance(_agent.transform.position, _prey.position);

            if (distanceFromPrey > _stopRadius)
            {
                Vector3 directionAwayFromPrey = (_agent.transform.position - _prey.position).normalized;
                Vector3 destination = _prey.position + (directionAwayFromPrey * _stopRadius);

                // Set the agent's destination
                _agent.SetDestination(destination);
            }
            else
            {
                _agent.ResetPath();
            }
        }
    }
}

