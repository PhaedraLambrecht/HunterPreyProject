using System;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    public class Pursuit : Node
    {
        private NavMeshAgent _agent;

        private float _stopRadius;
        private float _predictionTime;
   

        public Pursuit(NavMeshAgent agent, float stopRadius, float predictiontime)
        {
            _agent = agent;
            _stopRadius = stopRadius;
            _predictionTime = predictiontime;
        }


        public override NodeState Evaluate()
        {
            Debug.Log("pursuit - Who is she?");

            Transform target = (Transform)GetData("Prey");

            Vector3 predictedPosition = PredictTargetPosition(target);
            float distanceToPredicted = Vector3.Distance(_agent.transform.position, predictedPosition); // calculate distance to predicted position


            // if close enough, stop pursuing
            if (distanceToPredicted <= _stopRadius)
            {
                state = NodeState.Succes;
                return state;
            }


            _agent.transform.position = Vector3.MoveTowards(_agent.transform.position, predictedPosition, 5.0f * Time.deltaTime);
          
            Vector3 lookAtDirection = CalculateLookAtDirection(_agent.transform.position, predictedPosition);

            _agent.transform.LookAt(predictedPosition, lookAtDirection);


            state = NodeState.Running;
            return state;
        }


        private Vector3 CalculateLookAtDirection(Vector3 from, Vector3 to)
        {
            Vector3 direction = (to - from).normalized;
            float yaw = Mathf.Atan2(direction.z, direction.x);

            // Create a new direction vector with the y component fixed at 0.5
            Vector3 lookAtDirection = new Vector3(Mathf.Cos(yaw), 1.0f, Mathf.Sin(yaw));

            return lookAtDirection.normalized;
        }


        private Vector3 PredictTargetPosition(Transform target)
        {
            Vector3 targetVelocity = target.GetComponent<Rigidbody>().velocity; // assume prey has a Rigidbody
            return target.position + targetVelocity * _predictionTime; // predict prey's future position
        }
    }
}
