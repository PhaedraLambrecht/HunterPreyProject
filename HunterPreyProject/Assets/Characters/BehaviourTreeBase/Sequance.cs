using System.Collections.Generic;


namespace BehaviourTree
{
    public class Sequance : Node
    {
        public Sequance() : base() { }
        public Sequance(List<Node> children) : base(children) { }


        public override NodeState Evaluate()
        {
            bool anyChildRunning = false;

            foreach (var node in this.children.ToArray())
            {
                switch (node.Evaluate())
                {
                    case NodeState.Failure:
                        state = NodeState.Failure;
                        return state;
                    case NodeState.Succes:
                        continue;
                    case NodeState.Running:
                        anyChildRunning = true;
                        continue;
                    default:
                        state = NodeState.Succes;
                        return state;
                }
            }

            state = anyChildRunning ? NodeState.Running : NodeState.Succes;
            return state;
        }
    }
}
