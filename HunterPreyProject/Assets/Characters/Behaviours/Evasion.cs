using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace BehaviourTree
{
    public class Evasion : Node
    {
        private NavMeshAgent _agent;

        private float _predictionTime;


        public Evasion(NavMeshAgent agent, float predictionTime)
        {
            _agent = agent;
            _predictionTime = predictionTime;
        }


        public override NodeState Evaluate()
        {
            Debug.Log("Evasion - Never heard of her.");


            Transform target = (Transform)GetData("Predator");
            Vector3 predictedPosition = PredictTargetPosition(target);

            // Direction
            Vector3 evasionDirection = _agent.transform.position - predictedPosition;
            evasionDirection.Normalize();

            float evasionDistance = Vector3.Distance(_agent.transform.position, target.position);
            float targetSpeed = GetTargetSpeed(target);

            float urgency = CalculateUrgency(evasionDistance, targetSpeed);


            _agent.speed = 10f * urgency; // Adjust the agent's speed based on urgency
            Vector3 evadeDestination = _agent.transform.position + evasionDirection * _agent.speed;
            _agent.SetDestination(evadeDestination);

            state = NodeState.Running;
            return state;

        }



        private Vector3 PredictTargetPosition(Transform target)
        {
            Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
            if (targetRigidbody != null)
            {
                Vector3 targetVelocity = targetRigidbody.velocity;
                return target.position + targetVelocity * _predictionTime;
            }
            else
            {
                // Handle the case where the target object doesn't have a Rigidbody
                // For example, you could return the current position of the target
                return target.position;
            }
        }
       
        private float GetTargetSpeed(Transform target)
        {
            Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
            if (targetRigidbody != null)
            {
                return targetRigidbody.velocity.magnitude;
            }
            else
            {
                return 5f;
            }
        }

        private float CalculateUrgency(float distance, float speed)
        {
            if (distance < 20f && speed > 5f)
            {
                return 1f;  // High urgency
            }
            else if (distance < 50f)
            {
                return 0.5f;  // Medium urgency
            }
            else
            {
                return 0.2f;  // Low urgency
            }
        }
    }
}