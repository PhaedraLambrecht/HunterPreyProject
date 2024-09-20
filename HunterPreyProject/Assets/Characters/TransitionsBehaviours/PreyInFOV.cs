using UnityEngine;


namespace BehaviourTree
{
    public class PreyInFOV : Node
    {
        private static int _preyLayer = 1 << 6;
        private Transform _agentTransform;
        private float _fovAngle;

        public bool IsSuccess => state == NodeState.Succes;


        public PreyInFOV(Transform agentTransform, float fovAngle)
        {
            _agentTransform = agentTransform;
            _fovAngle = fovAngle;
        }

        public override NodeState Evaluate()
        {
            object j = GetData("PreySmell");
            if (j != null)
            {
                Collider[] colliders = Physics.OverlapSphere(_agentTransform.position, _fovAngle, _preyLayer);

                foreach (Collider collider in colliders)
                {
                    Transform preyTransform = collider.transform;
                    if (IsPreyInFOV(_agentTransform, preyTransform, _fovAngle))
                    {
                        parent.parent.SetData("Prey", preyTransform);
                        state = NodeState.Succes;
                        parent.parent.ClearData("PreySmell");
                        return state;
                    }
                }

                state = NodeState.Failure;
                return state;
            }

            state = NodeState.Running;
            return state;
        }

        private bool IsPreyInFOV(Transform agent, Transform prey, float fovAngle)
        {
            Vector3 directionToPrey = prey.position - agent.position;
            float distanceToPreySqr = directionToPrey.sqrMagnitude;

            if (distanceToPreySqr > _fovAngle * _fovAngle) // use a range check to optimize
            {
                return false;
            }

            directionToPrey.Normalize();
            float angle = Vector3.Angle(agent.forward, directionToPrey);

            return angle < fovAngle;
        }

    }
}