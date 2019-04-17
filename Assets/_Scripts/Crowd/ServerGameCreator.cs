using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if SERVER
public class ServerGameCreator : MonoBehaviour
{
    public int NbPnj;
    public GameObject PnjGameObject;

    public GameObject[] ListZone;

    [Range(0, 100)]
    public int ChanceToGoToObject = 60;
    [Range(0, 50)]
    public int MinTimeMoving = 4;
    [Range(0, 50)]
    public int MaxTimeMoving = 16;

    private GameObject[] _listFloor;
    private ProgrammableObjectsContainer _containerNPC;
    private PnjClass[] _listPnj;
    

    private List<ZoneClass> _listZone = new List<ZoneClass>();

    void Start()
    {
        _listFloor = GameObject.FindGameObjectsWithTag("Floor");
        ListZone = GameObject.FindGameObjectsWithTag("ConnectedObject");
        foreach (GameObject zone in ListZone)
        {
            ZoneClass zc = new ZoneClass();
            zc.ZoneGameObject = zone;
            zc.ConnectedGameObject = zone;
            //zc.FillListSlot();
            _listZone.Add(zc);
        }


        _containerNPC = FindObjectOfType<ProgrammableObjectsContainer>();
        FillCrowd();
    }
    

    void Update()
    {
        for (int i = 0; i < NbPnj; i++)
        {
            _listPnj[i].Time += Time.deltaTime;

            if (_listPnj[i].Time >= _listPnj[i].MovingTime)
            {
                //todo beuuurk
                //foreach (ZoneClass zone in _listZone)
                //{
                //    zone.EmptyListSlot();
                //}
                //foreach (ZoneClass zoneClass in _listZone)
                //{
                //    if (_listPnj[i].DestinationGameObject != null && zoneClass.ConnectedGameObject == _listPnj[i].DestinationGameObject)
                //    {
                //        zoneClass.EmptySlot(_listPnj[i].DestinationGameObject);
                //    }
                //}

                int chanceGoZone = Random.Range(0, 100);
                if (chanceGoZone <= ChanceToGoToObject)
                {
                    int rndZoneIndex = Random.Range(0, _listZone.Count);
                    {
                        _listPnj[i].Time = 0;

                        _listPnj[i].PrefabPnj.GetComponent<NavMeshAgent>().ResetPath();
                        Vector3 pos = _listZone[rndZoneIndex].ConnectedGameObject.transform.position;
                        pos.x += Random.Range(-5, 5);
                        pos.z += Random.Range(-5, 5);
                        _listPnj[i].PrefabPnj.GetComponent<NavMeshAgent>().destination = pos;

                        //SlotClass sc = _listZone[rndZoneIndex].GetFreeSlot();
                        //if (sc != null && sc.SlotGameObject != null)
                        //{
                        //    _listPnj[i].PrefabPnj.GetComponent<NavMeshAgent>().ResetPath();
                        //    _listPnj[i].PrefabPnj.GetComponent<NavMeshAgent>().destination = sc.SlotGameObject.transform.position;
                        //    _listPnj[i].DestinationGameObject = sc.ConnectedGameObject;
                        //}
                        // Add time to simulate hacking
                        _listPnj[i].MovingTime = Random.Range(MinTimeMoving + 5, MaxTimeMoving + 5);
                    }
                }
                else
                {
                    int rndZoneIndex = Random.Range(0, _listFloor.Length);
                    _listPnj[i].Time = 0;
                    _listPnj[i].PrefabPnj.GetComponent<NavMeshAgent>().ResetPath();
                    _listPnj[i].PrefabPnj.GetComponent<NavMeshAgent>().destination = _listFloor[rndZoneIndex].transform.position;
                    _listPnj[i].DestinationGameObject = _listFloor[rndZoneIndex];
                    _listPnj[i].MovingTime = Random.Range(MinTimeMoving, MaxTimeMoving);
                }
            }

            
        }
        
            
            //TODO Version Zone
            //int rndZoneIndex = Random.Range(0, _listZone.Count);



            //_listPnj[i].Time += Time.deltaTime;
            //if (_listPnj[i].Time >= _listPnj[i].MovingTime)
            //{
            //    //todo beuuurk
            //    foreach (ZoneClass zone in _listZone)
            //    {
            //        zone.EmptyListSlot();
            //    }
            //    /*foreach (ZoneClass zoneClass in _listZone)
            //    {
            //        zoneClass.EmptySlot(_listPnj[i].DestinationGameObject);
            //    }*/

            //    Debug.Log("pnj n°"+i+" is going to move");
            //    _listPnj[i].Time = 0;

            //    SlotClass sc = _listZone[rndZoneIndex].GetFreeSlot();
            //    if (sc != null && sc.SlotGameObject != null)
            //    {
            //        _listPnj[i].PrefabPnj.GetComponent<NavMeshAgent>().ResetPath();
            //        _listPnj[i].PrefabPnj.GetComponent<NavMeshAgent>().destination = sc.SlotGameObject.transform.position;
            //        _listPnj[i].DestinationGameObject = sc.ConnectedGameObject;
            //    }
            //    else { Debug.Log("plus de place");}
            //    _listPnj[i].MovingTime = Random.Range(2, 8);
            //}
        
    }

    void FillCrowd()
    {
        _listPnj = new PnjClass[NbPnj];
        for (int i = 0; i < NbPnj; i++)
        {
            PnjClass pnj = new PnjClass();
            pnj.PrefabPnj = Instantiate(PnjGameObject, _containerNPC.transform);

            int rndZoneIndex = Random.Range(0, _listZone.Count);

            SlotClass sc = _listZone[rndZoneIndex].GetFreeSlot();
            if (sc != null && sc.SlotGameObject != null)
            {
                pnj.PrefabPnj.transform.position = sc.SlotGameObject.GetComponent<Renderer>().bounds.center;
                sc.PnjInSlot = pnj;
                pnj.DestinationGameObject = sc.ConnectedGameObject;
            }

            pnj.Name = i.ToString();
            pnj.MovingTime = Random.Range(2, 8);
            pnj.Time = 0;
            GetComponent<Server>().characters.Add(pnj.PrefabPnj.transform);
            _listPnj[i] = pnj;
            _containerNPC.objectListServer.Add(pnj.PrefabPnj.GetComponent<ProgrammableObjectsData>());
        }        
    }
   
}
#endif