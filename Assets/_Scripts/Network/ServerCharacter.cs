using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ServerCharacter : MonoBehaviour
{
#if SERVER
    public int team = -1;
    public string playerName;

    public bool isStunned;
    public float timeBeforeEndOfStun;
    private const float TIMEOFSTUN = 15.0f;

    public bool canStun;
    private float timeBeforeStunReload;
    private const float TIMEOFSTUNRELOAD = 20.0f;

    public NavMeshAgent navMeshAgent;
    private float baseSpeed;

    public ServerCarrier carrier;

    public bool isAttracted = false;
    public float attracttimebeforeend;
    public Transform attractDestination;

    public int isAttractedByData = 0;
    public Vector3 attractByDataDestination;

    public bool hasAPriorityDestination;
    public Vector3 priorityDestination;

    public Vector3 normalDestination;

    public Vector3 actualDestination;

    public Server server;

    public bool coroutineStarted = false;
    public bool destinationWasSet = false;

    public Coroutine pathCalculationCoroutine;

    // Start is called before the first frame update
    void Start()
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		isStunned = false;
        navMeshAgent = GetComponent<NavMeshAgent>();
        baseSpeed = navMeshAgent.speed;
        carrier = GetComponent<ServerCarrier>();
        server = FindObjectOfType<Server>();
    }

    public void getStun()
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		timeBeforeEndOfStun = TIMEOFSTUN;
        isStunned = true;
        navMeshAgent.speed = 0;
        GetComponent<ProgrammableObjectsData>().OnInput("OnStun");
        GetComponent<ServerCarrier>().StopTaking();
        GetComponent<ServerCarrier>().StopGiving();
    }

    public void doStun()
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		timeBeforeStunReload = TIMEOFSTUNRELOAD;
        canStun = false;
    }

    // Update is called once per frame
    void Update()
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		if (isStunned)
        {
            timeBeforeEndOfStun -= Time.deltaTime;
            if (timeBeforeEndOfStun <= 0)
            {
                isStunned = false;
                navMeshAgent.speed = baseSpeed;
                hasAPriorityDestination = false;
            }
        }
        else
        {
            if (!canStun)
            {
                timeBeforeStunReload -= Time.deltaTime;
                if (timeBeforeStunReload <= 0)
                {
                    canStun = true;
                }
            }

            if (isAttracted)
            {
                attracttimebeforeend -= Time.deltaTime;
                if (attracttimebeforeend > 0 && !attractDestination.GetComponent<ServerCharacter>().isStunned)
                {
                    if (Vector3.Distance(attractDestination.position, actualDestination) > 0.2)
                    {
                        actualDestination = attractDestination.position;
                        destinationWasSet = true; ;
                    }
                }
                else
                {
                    isAttracted = false;
                }
                hasAPriorityDestination = false;
            }
            else
            {
                if (isAttractedByData!=0)
                {
                    if((isAttractedByData>0 && carrier.charge < carrier.maxCharge) || (isAttractedByData < 0 && carrier.charge > 0.0f) && Vector3.Distance(this.transform.position, attractByDataDestination) < 35)
                    {
                        if (actualDestination != attractByDataDestination)
                        {
                            actualDestination = attractByDataDestination;
                            //navMeshAgent.ResetPath();
                            destinationWasSet = true;
                        }
                    }
                    else
                    {
                        isAttractedByData = 0;
                    }
                    hasAPriorityDestination = false;
                }
                else
                {
                    if (hasAPriorityDestination)
                    {
                        if(Vector3.Distance(this.transform.position, priorityDestination) < 5)
                        {
                            hasAPriorityDestination = false;
                        }
                        else
                        {
                            if (actualDestination != priorityDestination)
                            {
                                actualDestination = priorityDestination;
                                normalDestination = actualDestination;
                                //navMeshAgent.ResetPath();
                                destinationWasSet = true;
                            }
                        }
                    }
                    else
                    {
                        if (normalDestination != actualDestination)
                        {
                            actualDestination = normalDestination;
                            //navMeshAgent.ResetPath();
                            destinationWasSet = true;
                        }
                    }                    
                }
            }
        }

        if (destinationWasSet == true)
        {
            // if this character has a team, it's a player : display its path on screen
            if (team != -1)
            {
                NavMeshPath tmpPath = new NavMeshPath();

                if (pathCalculationCoroutine != null)
                    StopCoroutine(pathCalculationCoroutine);
                pathCalculationCoroutine = StartCoroutine(CalculatePath(tmpPath));

                //if (((navMeshAgent.pathPending == false) && (wasPending == true || setDestinationSuccess == true)) || navMeshAgent.isPathStale == true)// && navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
                if (navMeshAgent.pathPending == false && tmpPath.status != NavMeshPathStatus.PathInvalid)
                {
                    StopCoroutine(pathCalculationCoroutine);
                    NavMeshPath path = tmpPath;
                    navMeshAgent.SetPath(path);

                    Vector3[] pathAs3dPositions = new Vector3[path.corners.Length];
                    for (int i = 0; i < path.corners.Length - 1; i++)
                        pathAs3dPositions[i] = new Vector3(path.corners[i].x, path.corners[i].y, path.corners[i].z);

                    pathAs3dPositions[path.corners.Length - 1] = navMeshAgent.destination;
                    server.SendPath(pathAs3dPositions, server.GetNetworkConnectionFromPlayerTransform(transform));

                    if (coroutineStarted == false)
                        StartCoroutine(SendPathChange());
                }

                if (navMeshAgent.hasPath == false)
                {
                    StopAllCoroutines();
                    coroutineStarted = false;
                    server.SendPath(new Vector3[] { Vector3.zero }, server.GetNetworkConnectionFromPlayerTransform(transform));
                }
            }
            else
            {
                navMeshAgent.SetDestination(actualDestination);
            }

            destinationWasSet = false;
        }

        
    }

    public IEnumerator SendPathChange()
    {
        while(true)
        {
            coroutineStarted = true;
            NavMeshPath path = navMeshAgent.path;
            Vector3[] pathAs3dPositions = new Vector3[path.corners.Length];
            for (int i = 0; i < path.corners.Length - 1; i++)
                pathAs3dPositions[i] = new Vector3(path.corners[i].x, path.corners[i].y, path.corners[i].z);

            pathAs3dPositions[path.corners.Length - 1] = navMeshAgent.destination;
            server.SendPath(pathAs3dPositions, server.GetNetworkConnectionFromPlayerTransform(transform));

            yield return new WaitForSeconds(.1f);
        }
    }

    public IEnumerator CalculatePath(NavMeshPath path)
    {
        while (true)
        {
            navMeshAgent.CalculatePath(actualDestination, path);
            yield return null;
        }
    }
#endif
}
