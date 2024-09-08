using System.Collections.Generic;


namespace BehaviourTree
{
    public enum NodeState
    {
        Running,
        Succes,
        Failure
    }

    public class Node
    {
        protected NodeState state;

        public Node parent;
        protected List<Node> children = new List<Node>();

        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();


        public Node()
        {
            parent = null;

        }

        public Node(List<Node> children)
        {
            this.children = children ?? new List<Node>();

            foreach (var child in this.children.ToArray())
            {
                Attach(child);
            }
        }


        // Attach the children with their corresponding parent
        private void Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        public virtual NodeState Evaluate() => NodeState.Failure;


        public void SetData(string key, object value)
        {
            _dataContext[key] = value;
        }

        public object GetData(string key)
        {
            object value = null;

            if (_dataContext.TryGetValue(key, out value))
                return value;

            Node node = parent;
            while (node != null)
            {
                value = node.GetData(key);
                if (value != null)
                    return value;

                node = node.parent;
            }

            return null;
        }

        public bool ClearData(string key)
        {
            if (_dataContext.ContainsKey(key))
            {
                _dataContext.Remove(key);
                return true;
            }


            Node node = parent;
            while (node != null)
            {
                bool cleared = node.ClearData(key);
                if (cleared)
                    return true;

                node = node.parent;
            }

            return false;
        }
    }
}