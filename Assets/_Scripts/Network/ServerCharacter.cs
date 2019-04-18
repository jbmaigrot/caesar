using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ServerCharacter : MonoBehaviour
{
    public bool isStunned;
    private float timeBeforeEndOfStun;
    private const float TIMEOFSTUN = 15.0f;

    public bool canStun;
    private float timeBeforeStunReload;
    private const float TIMEOFSTUNRELOAD = 20.0f;

    private NavMeshAgent navMeshAgent;
    private float baseSpeed;

    public int[] inventory = new int[3];

    // Start is called before the first frame update
    void Start()
    {
        isStunned = false;
        navMeshAgent = GetComponent<NavMeshAgent>();
        baseSpeed = navMeshAgent.speed;
        inventory[0] = InventoryConstants.Attract;
        inventory[1] = InventoryConstants.Stunbox;
        inventory[2] = InventoryConstants.Powerpump;
    }

    public void getStun()
    {
        timeBeforeEndOfStun = TIMEOFSTUN;
        isStunned = true;
        navMeshAgent.speed = 0;
        this.GetComponent<ProgrammableObjectsData>().OnInput("OnStun");
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
        }
    }
}
