using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ServerCharacter : MonoBehaviour
{
#if SERVER
    public bool isStunned;
    private float timeBeforeEndOfStun;
    private const float TIMEOFSTUN = 15.0f;

    public bool canStun;
    private float timeBeforeStunReload;
    private const float TIMEOFSTUNRELOAD = 20.0f;

    private NavMeshAgent navMeshAgent;
    private float baseSpeed;

    public ServerCarrier carrier;

    public bool isAttracted =false;
    public float attracttimebeforeend;
    public Vector3 attractDestination;

    public int isAttractedByData = 0;
    public Vector3 attractByDataDestination;

    public bool hasAPriorityDestination;
    public Vector3 priorityDestination;

    public Vector3 normalDestination;

    public Vector3 actualDestination;


    // Start is called before the first frame update
    void Start()
    {
        isStunned = false;
        navMeshAgent = GetComponent<NavMeshAgent>();
        baseSpeed = navMeshAgent.speed;
        carrier = GetComponent<ServerCarrier>();
        
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
                        this.GetComponent<NavMeshAgent>().destination = actualDestination;
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
                    if((isAttractedByData>0 && this.GetComponent<ServerCarrier>().charge< this.GetComponent<ServerCarrier>().maxCharge) || (isAttractedByData < 0 && this.GetComponent<ServerCarrier>().charge > 0.0f) && Vector3.Distance(this.transform.position, attractDestination) < 30)
                    {
                        if (actualDestination != attractByDataDestination)
                        {
                            actualDestination = attractByDataDestination;
                            this.GetComponent<NavMeshAgent>().ResetPath();
                            this.GetComponent<NavMeshAgent>().destination = actualDestination;
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
                                this.GetComponent<NavMeshAgent>().ResetPath();
                                this.GetComponent<NavMeshAgent>().destination = actualDestination;
                            }
                        }
                    }
                    else
                    {
                        if (normalDestination != actualDestination)
                        {
                            actualDestination = normalDestination;
                            this.GetComponent<NavMeshAgent>().ResetPath();
                            this.GetComponent<NavMeshAgent>().destination = actualDestination;
                        }
                    }                    
                }
            }
        }
    }
#endif
}
