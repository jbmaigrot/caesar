using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ServerCharacter : MonoBehaviour
{
#if SERVER
    public int team = -1;

    public bool isStunned;
    private float timeBeforeEndOfStun;
    private const float TIMEOFSTUN = 15.0f;

    public bool canStun;
    private float timeBeforeStunReload;
    private const float TIMEOFSTUNRELOAD = 20.0f;

    private NavMeshAgent navMeshAgent;
    private float baseSpeed;

    public ServerCarrier carrier;

    public bool isAttracted = false;
    public float attracttimebeforeend;
    public Vector3 attractDestination;

    public int isAttractedByData = 0;
    public Vector3 attractByDataDestination;

    public bool hasAPriorityDestination;
    public Vector3 priorityDestination;

    public Vector3 normalDestination;

    public Vector3 actualDestination;

    public Server server;


    // Start is called before the first frame update
    void Start()
    {
        isStunned = false;
        navMeshAgent = GetComponent<NavMeshAgent>();
        baseSpeed = navMeshAgent.speed;
        carrier = GetComponent<ServerCarrier>();
        server = FindObjectOfType<Server>();
    }

    public void getStun()
    {
        timeBeforeEndOfStun = TIMEOFSTUN;
        isStunned = true;
        navMeshAgent.speed = 0;
        GetComponent<ProgrammableObjectsData>().OnInput("OnStun");
    }

    public void doStun()
    {
        timeBeforeStunReload = TIMEOFSTUNRELOAD;
        canStun = false;
    }

    // Update is called once per frame
    void Update()
    {
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
                if (attracttimebeforeend > 0)
                {
                    if (Vector3.Distance(attractDestination, actualDestination) > 0.2)
                    {
                        actualDestination = attractDestination;
                        navMeshAgent.destination = actualDestination;
                        SendPathChange();
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
                    if((isAttractedByData>0 && carrier.charge < carrier.maxCharge) || (isAttractedByData < 0 && carrier.charge > 0.0f) && Vector3.Distance(this.transform.position, attractDestination) < 30)
                    {
                        if (actualDestination != attractByDataDestination)
                        {
                            actualDestination = attractByDataDestination;
                            navMeshAgent.ResetPath();
                            navMeshAgent.destination = actualDestination;
                            SendPathChange();
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
                                navMeshAgent.ResetPath();
                                navMeshAgent.destination = actualDestination;
                                SendPathChange();
                            }
                        }
                    }
                    else
                    {
                        if (normalDestination != actualDestination)
                        {
                            actualDestination = normalDestination;
                            navMeshAgent.ResetPath();
                            navMeshAgent.destination = actualDestination;
                            SendPathChange();
                        }
                    }                    
                }
            }
        }
    }

    public void SendPathChange()
    {
        if (team != -1)
        {
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(transform.position, navMeshAgent.destination, NavMesh.AllAreas, path))
            {
                Vector3[] pathAs3dPositions = new Vector3[path.corners.Length];
                for (int i = 0; i < path.corners.Length - 1; i++)
                    pathAs3dPositions[i] = new Vector3(path.corners[i].x, path.corners[i].y, path.corners[i].z);

                server.SendPath(pathAs3dPositions, server.GetNetworkConnectionFromPlayerTransform(transform));
            }
        }
    }
#endif
}
