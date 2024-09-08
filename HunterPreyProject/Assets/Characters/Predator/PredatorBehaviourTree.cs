using System.Collections.Generic;
using UnityEngine;


namespace BehaviuorTree
{
    public class PredatorBehaviourTree : Tree
    {
        public UnityEngine.AI.NavMeshAgent agent;
        public CoroutineManager coroutineManager;

        public Transform _prey;

        // Wandering
        public float wanderRadius = 20f;
        public float maxWanderTime = 2f;
        public float stepLengthExponent = 2.0f; // Min 1 max 3

        // Searching
        public float searchRadius = 10f;
        public float searchTime = 5f; // Duration for search
 


        protected override Node SetUpTree()
        {
            // Behaviours
            Node wander = new Wander(agent, wanderRadius, maxWanderTime, stepLengthExponent); // Create the Wander node
            Node search = new Search(agent, coroutineManager, searchRadius, searchTime, _prey.position); // Create the Search node

            Node checkPreyDetected = new SmellingRangeCheck(agent.transform, _prey, searchRadius);



            BehaviuorTree.Sequance searchSequence = new BehaviuorTree.Sequance(new List<Node> { checkPreyDetected, search });

            Selector searchOrWander = new Selector(new List<Node> { searchSequence, wander });
            Selector root = new Selector(new List<Node> { searchOrWander });



            return root;
        }
    }
}

