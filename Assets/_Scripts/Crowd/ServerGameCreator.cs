using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ServerGameCreator : MonoBehaviour
{

    public int NbPnj;
    public GameObject PnjGameObject;

    public GameObject[] ListZone;

    private GameObject _containerNPC;
    private PnjClass[] _listPnj;
    

    private List<ZoneClass> _listZone = new List<ZoneClass>();

    void Start()
    {
        foreach (GameObject zone in ListZone)
        {
            ZoneClass zc = new ZoneClass();
            zc.ZoneGameObject = zone;
            zc.FillListSlot();

            _listZone.Add(zc);
        }
        _containerNPC = GameObject.Find("NPC_crowd");
        FillCrowd();
    }
    

    void Update()
    {
        for (int i = 0; i < NbPnj; i++)
        {

            int rndZoneIndex = Random.Range(0, _listZone.Count);

            
            
            _listPnj[i].Time += Time.deltaTime;
            if (_listPnj[i].Time >= _listPnj[i].MovingTime)
            {
                //todo beuuurk
                foreach (ZoneClass zone in _listZone)
                {
                    zone.EmptyListSlot();
                }
                /*foreach (ZoneClass zoneClass in _listZone)
                {
                    zoneClass.EmptySlot(_listPnj[i].DestinationGameObject);
                }*/

                Debug.Log("pnj n°"+i+" is going to move");
                _listPnj[i].Time = 0;
                
                SlotClass sc = _listZone[rndZoneIndex].GetFreeSlot();
                if (sc != null && sc.SlotGameObject != null)
                {
                    _listPnj[i].PrefabPnj.GetComponent<NavMeshAgent>().ResetPath();
                    _listPnj[i].PrefabPnj.GetComponent<NavMeshAgent>().destination = sc.SlotGameObject.transform.position;
                    _listPnj[i].DestinationGameObject = sc.ConnectedGameObject;
                }
                else { Debug.Log("plus de place");}
                _listPnj[i].MovingTime = Random.Range(2, 8);
            }
        }
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
        }
        
    }
}
