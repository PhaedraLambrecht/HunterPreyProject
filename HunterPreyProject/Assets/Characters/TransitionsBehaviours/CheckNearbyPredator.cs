using UnityEngine;


namespace BehaviourTree
{
    public class CheckNearbyPredator : Node
    {
        private static int _preyLayer = 1 << 7;
        private Transform _agentTransform; // Add this field
        private float _fovRange;

        public CheckNearbyPredator(Transform agentTransform, float fovRange)
        {
            _agentTransform = agentTransform;
            _fovRange = fovRange;
        }


        public override NodeState Evaluate()
        {
            object j = GetData("Predator");
            if (j == null)
            {
                Collider[] collider = Physics.OverlapSphere(_agentTransform.position, _fovRange, _preyLayer);

                if (collider.Length > 0)
                {
                    parent.parent.SetData("Predator", collider[0].transform);
                    state = NodeState.Succes;
                    return state;
                }

                state = NodeState.Failure;
                return state;
            }

            state = NodeState.Succes;
            return state;
        }
    }
}
