using UnityEngine;


namespace BehaviuorTree
{
    public class SmellingRangeCheck : Node
    {
        private Transform _prey;
        private float _detectionRange;
        private Transform _agentTransform; // Add this field

        public SmellingRangeCheck(Transform agentTransform, Transform prey, float detectionRange)
        {
            _agentTransform = agentTransform; // Initialize agent transform
            _prey = prey;
            _detectionRange = detectionRange;
        }

        public override NodeState Evaluate()
        {
            float distance = Vector3.Distance(_prey.position, _agentTransform.position); // Use agentTransform instead of _agent

            if (distance <= _detectionRange)
            {
                state = NodeState.Succes;
            }
            else
            {
                state = NodeState.Failure;
            }

            return state;
        }
    }
}