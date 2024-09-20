using UnityEngine;


namespace BehaviourTree
{
    public class CheckPreyInSmellRange : Node
    {
        private static int _preyLayer = 1 << 6;
        private Transform _agentTransform; // Add this field
        private float _fovRange;

        public CheckPreyInSmellRange(Transform agentTransform, float fovRange)
        {
            _agentTransform = agentTransform;
            _fovRange = fovRange;
        }


        public override NodeState Evaluate()
        {
            object j = GetData("PreySmell");
            if (j == null)
            {
                object preyJ = GetData("Prey");
                if(preyJ == null)
                {
                    Collider[] collider = Physics.OverlapSphere(_agentTransform.position, _fovRange, _preyLayer);

                    if (collider.Length > 0)
                    {
                        parent.parent.SetData("PreySmell", collider[0].transform);
                        state = NodeState.Succes;
                        return state;
                    }

                }


                state = NodeState.Failure;
                return state;
            }

            state = NodeState.Running;
            return state;
        }
    }
}
