using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    public class PreyBehaviourTree : Tree
    {
        public NavMeshAgent agent;

        // Wandering
        public float wanderRadius = 20f;
        public float maxWanderTime = 2f;
        public float stepLengthExponent = 2.0f;

        // Searching
        public float fovRange = 5.0f;

        // Pursuit
        public float predictiontime = 0;


        private Evasion _evasion;
        private Wander _wander;


        protected override Node SetUpTree()
        {
            // Behaviours
            _evasion = new Evasion(agent, predictiontime);
            _wander = new Wander(agent, wanderRadius, maxWanderTime, stepLengthExponent);


            // Sequances
            Sequance pursuitSequence = new Sequance(new List<Node> { new CheckNearbyPredator(agent.transform, fovRange), _evasion });

            // Root
            Node root = new Selector(new List<Node>
            {
               pursuitSequence,
               _wander
            });

            return root;
        }
    }
}

