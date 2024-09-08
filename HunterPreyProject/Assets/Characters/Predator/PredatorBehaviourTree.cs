using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    public class PredatorBehaviourTree : Tree
    {
        public NavMeshAgent agent;
        public CoroutineManager coroutineManager;

        public Transform _prey;

        // Wandering
        public float wanderRadius = 20f;
        public float maxWanderTime = 2f;
        public float stepLengthExponent = 2.0f;

        // Searching
        public float searchRadius = 10f;
        public float searchTime = 5f; // Duration for search

        // Pursuit
        public float stopRadius = 5.0f;
        public float fieldOfViewAngle = 45f;
        public float fieldOfViewDistance = 20f;



        protected override Node SetUpTree()
        {
            // Behaviours
            Node wander = new Wander(agent, wanderRadius, maxWanderTime, stepLengthExponent);
            Node search = new Search(agent, coroutineManager, searchRadius, searchTime, _prey.position);

            Node pursuit = new Pursuit(agent, _prey, stopRadius, fieldOfViewAngle, fieldOfViewDistance, searchRadius);

            Node checkPreyDetected = new SmellingRangeCheck(agent.transform, _prey, searchRadius);
            Node checkPreyInView = new PreyInFOV(agent.transform, _prey, fieldOfViewAngle, fieldOfViewDistance);

            // Define sequences
            Sequance pursuitSequence = new Sequance(new List<Node> { checkPreyInView, pursuit });
            Sequance searchSequence = new Sequance(new List<Node> { checkPreyDetected, search });

            // Define selectors
            Selector searchOrWander = new Selector(new List<Node> { searchSequence, wander });
            Selector root = new Selector(new List<Node> { pursuitSequence, searchOrWander });


            return root;
        }
    }
}

