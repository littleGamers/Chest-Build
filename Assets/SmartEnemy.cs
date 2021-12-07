using UnityEngine;
using UnityEngine.AI;

/*
 * This script is used to define enemies behaviour.
 * Set the role and see what happens.
 */

[RequireComponent(typeof(NavMeshAgent))]
public class SmartEnemy : MonoBehaviour
{
    private enum Roles { Coward, Brave, Chaser, EngineDestroyer };
    [Tooltip("Choose enemy role")]
    [SerializeField] private Roles enemyRole;

    [Tooltip("Add player object to chase (used for 'Chaser' role")]
    [SerializeField] private Transform player;

    [Tooltip("Add engine object to chase and destroy (used for 'Engine Destroyer' role")]
    [SerializeField] private Transform engine;

    [Tooltip("When enemy enters this radius from the engine - the engine is destoryed (used for 'Engine Destroyer' role")]
    [SerializeField] private float destroyRadius = 5f;

    [Tooltip("Minimum time to wait at target between running to the next target")]
    [SerializeField] private float minWaitAtTarget = 7f;

    [Tooltip("Maximum time to wait at target between running to the next target")]
    [SerializeField] private float maxWaitAtTarget = 15f;


    [Tooltip("A game object whose children have a Target component. Each child represents a target.")]
    [SerializeField] private Transform targetFolder = null;
    private Target[] allTargets = null;
    
    private Target currentTarget = null;
    private float timeToWaitAtTarget = 0;
    private NavMeshAgent navMeshAgent;
    private float rotationSpeed = 5f;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        allTargets = targetFolder.GetComponentsInChildren<Target>(false); // false = get components in active children only
        Debug.Log("Found " + allTargets.Length + " active targets.");

        SelectNewTarget();
    }

    private void SelectNewTarget()
    {
        // This function chooses the new target based on the enemy role.
        switch(enemyRole)
        {
            case Roles.Coward:
                setCowardTarget();
                break;
            case Roles.Brave:
                setBraveTarget();
                break;
            case Roles.Chaser:
                setChaserTarget();
                break;
            case Roles.EngineDestroyer:
                setEngineDestroyerTarget();
                break;
        }
    }

    private void setCowardTarget()
    {
        // This function finds the farthest target from the player.
        currentTarget = allTargets[0];
        float maxDistance = Vector3.Distance(currentTarget.transform.position, player.position);

        for (int i = 1; i < allTargets.Length; i++)
        {
            float distance = Vector3.Distance(allTargets[i].transform.position, player.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                currentTarget = allTargets[i];
            }
        }

        Debug.Log("New target: " + currentTarget.name);
        navMeshAgent.SetDestination(currentTarget.transform.position);

        timeToWaitAtTarget = Random.Range(minWaitAtTarget, maxWaitAtTarget);
    }

    private void setBraveTarget()
    {
        // This function finds the closest target from the player.

        currentTarget = allTargets[0];
        float minDistance = Vector3.Distance(currentTarget.transform.position, player.position);

        for (int i = 1; i < allTargets.Length; i++)
        {
            float distance = Vector3.Distance(allTargets[i].transform.position, player.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                currentTarget = allTargets[i];
            }
        }

        Debug.Log("New target: " + currentTarget.name);
        navMeshAgent.SetDestination(currentTarget.transform.position);

        timeToWaitAtTarget = Random.Range(minWaitAtTarget, maxWaitAtTarget);
    }

    private void setChaserTarget()
    {
        // This function sets the player as target.
        navMeshAgent.SetDestination(player.position);
    }
    private void setEngineDestroyerTarget()
    {
        // This function sets the engine as target.
        navMeshAgent.SetDestination(engine.position);
    }

    

    private void Update()
    {
        // Check which role the enemy is set to.
        switch (enemyRole)
        {
            // Chaser should find the player's current position and face him.
            case Roles.Chaser:
                setChaserTarget();
                FaceDestination();
                break;

            // Engine Destroyer should face the target until it finds it.
            // If he is within the destroy radius - he will destroy the engine.
            case Roles.EngineDestroyer:
                float distance = Vector3.Distance(transform.position, engine.position);
                if (distance > destroyRadius)
                {
                    FaceDestination();
                }
                else
                    engine.gameObject.SetActive(false);
                break;

            // Brave and Cowers act the same.
            // They face the right target (farthest or closest to the player).
            // When they reach the target they wait for the amount of time generated and sets a new point.
            default: // Brave or Coward - same behaviour
                if (navMeshAgent.hasPath)
                {
                    FaceDestination();
                }
                else
                {   // we are at the target
                    timeToWaitAtTarget -= Time.deltaTime;
                    if (timeToWaitAtTarget <= 0)
                        SelectNewTarget();
                }
                break;
        }
        
    }

    private void FaceDestination()
    {
        // This function makes the enemy face the target he's navigating to.
        Vector3 directionToDestination = (navMeshAgent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToDestination.x, 0, directionToDestination.z));

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed); // Gradual rotation
    }


}
