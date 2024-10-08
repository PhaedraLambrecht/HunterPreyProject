using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    public class PredatorBehaviourTree : Tree
    {
        public NavMeshAgent agent;
        public Transform _prey;

        // Wandering
        public float wanderRadius = 20f;
        public float maxWanderTime = 2f;
        public float stepLengthExponent = 2.0f;

        // Searching
        public float fovRange = 5.0f;


        //// Pursuit
        public float stopRadius = 5.0f;

        // Search
        public CoroutineManager coroutineManager;
        public float searchTime = 0;
        public float searchRadius = 10.0f;

        private Search _search;
        private Pursuit _pursuit;
        private Wander _wander;


        protected override Node SetUpTree()
        {
            // Behaviours
            _search = new Search(agent, coroutineManager, searchRadius, searchTime, _prey.position);
            _pursuit = new Pursuit(agent, stopRadius);
            _wander = new Wander(agent, wanderRadius, maxWanderTime, stepLengthExponent);


            // Sequances
            Sequance pursuitSequence = new Sequance(new List<Node> { new PreyInFOV(agent.transform, fovRange), _pursuit });
            Sequance searchSequance = new Sequance(new List<Node> { new CheckPreyInSmellRange(agent.transform, searchRadius), _search });

      
            // Root
            Node root = new Selector(new List<Node>
            {
                pursuitSequence,
                searchSequance,
                _wander
            });

            return root;
        }
    }
}

