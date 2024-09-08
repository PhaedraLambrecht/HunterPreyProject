using UnityEngine;


namespace BehaviourTree
{
    public class PreyInFOV : Node
    {
        private Transform _predator;
        private Transform _prey;
        private float _fieldOfViewAngle;
        private float _fieldOfViewDistance;
   
        public bool IsSuccess => state == NodeState.Succes;


        public PreyInFOV(Transform predator, Transform prey, float fieldOfViewAngle, float fieldOfViewDistance)
        {
            _predator = predator;
            _prey = prey;
            _fieldOfViewAngle = fieldOfViewAngle;
            _fieldOfViewDistance = fieldOfViewDistance;
        }

        public override NodeState Evaluate()
        {
            if (IsPreyInFieldOfView(_predator, _prey, _fieldOfViewAngle, _fieldOfViewDistance))
            {
                state = NodeState.Succes;
            }
            else
            {
                state = NodeState.Failure;
            }

            return state;
        }

        public static bool IsPreyInFieldOfView(Transform predator, Transform prey, float fieldOfViewAngle, float fieldOfViewDistance)
        {
            Vector3 directionToPrey = prey.position - predator.position;
            float distanceToPreySqr = directionToPrey.sqrMagnitude;

            if (distanceToPreySqr > fieldOfViewDistance * fieldOfViewDistance)
            {
                return false;
            }

            directionToPrey.Normalize();
            float angle = Vector3.Angle(predator.forward, directionToPrey);

            return angle < fieldOfViewAngle / 2f;
        }

    }
}