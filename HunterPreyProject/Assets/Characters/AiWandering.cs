using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiWandering : MonoBehaviour
{
    [SerializeField] private float _wanderRadius = 20f;
    [SerializeField] private float _maxWanderTime = 2f;
    [SerializeField] private float _stepLengthExponent = 2.0f; // typically between 1 and 3

    private static int numSteps = 10;
    private NavMeshAgent agent;
    private float timer;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = _maxWanderTime;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= _maxWanderTime)
        {
            Vector3 newPos = LevyFlight(transform.position, _wanderRadius, _stepLengthExponent);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    // Lévy Flight is composed of a series of small displacements, interspersed occasionally by a very large displacement.
    public static Vector3 LevyFlight(Vector3 origin, float dist, float stepLengthExponent)
    {
        Vector3 accumulatedDisplacement = Vector3.zero;

        for (int i = 0; i < numSteps; i++)
        {
            float randAngle = Random.Range(0, (2 * Mathf.PI)); // Get a random angle between 0 & 2pi


            float stepLength = Mathf.Pow(Random.value, -1f / stepLengthExponent);// Calculate step length.
            stepLength = Mathf.Min(stepLength, dist);

            Vector3 direction = new Vector3(Mathf.Cos(randAngle), 0, Mathf.Sin(randAngle)); // Calculate the direction vector
            Vector3 randomStep = direction * stepLength; // Calculate the random step

            accumulatedDisplacement += randomStep; // Accumulate the displacement
        }


        // Apply the scaling factor based on number of steps and exponent
        float scaleFactor = Mathf.Pow(numSteps, 1f / stepLengthExponent);
        accumulatedDisplacement /= scaleFactor;


        Vector3 newPos = origin + accumulatedDisplacement;

        // Ensure the new position is within the NavMesh
        NavMeshHit navHit;
        NavMesh.SamplePosition(newPos, out navHit, dist, -1);

        return navHit.position;

    }
}
