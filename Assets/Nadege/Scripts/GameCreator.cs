using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameCreator : MonoBehaviour
{
    public GameObject PnjPGameObject;
    public int NbPnj;
    public Button ResetButton;
    public Button MoveButton;

    public GameObject Player;

    public GameObject[] ListZone;

    private PnjClass[] _listPnj;
    private GameObject _container;

    private List<ZoneClass> _listZone = new List<ZoneClass>();



    void Start()
    {
        _container = GameObject.Find("Crowd");
        foreach (GameObject zone in ListZone)
        {
            ZoneClass zc = new ZoneClass();
            zc.ZoneGameObject = zone;
			zc.FillListSlot();
            
            _listZone.Add(zc);
        }

        foreach (ZoneClass zone in _listZone)
        {
            //zone.ZoneGameObject
        }

        ResetCrowd();
        ResetButton.onClick.AddListener(ResetCrowd);
        MoveButton.onClick.AddListener(MovePnj);
    }
    
    void Update()
    {
        /*foreach (Transform child in _container.transform)
        {
            child.Translate(0.5f * Time.deltaTime,0,0);
            child.Rotate(new Vector3(0, 0.5f * Time.deltaTime));
        }*/
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("test");
            if (Physics.Raycast(ray, out hit, 100))
            {
                Player.GetComponent<NavMeshAgent>().destination = hit.point;
            }
        }
    }

    private void ResetCrowd()
    {
        foreach (Transform child in _container.transform)
        {
            Destroy(child.gameObject);
        }
        _listPnj = new PnjClass[0];
        foreach (PnjClass pnjClass in _listPnj)
        {
            Destroy(pnjClass.PrefabPnj);
        }
        foreach (ZoneClass zone in _listZone)
        {
            zone.EmptyListSlot();
        }

        _listPnj = new PnjClass[NbPnj];
        for (int i = 0; i < NbPnj; i++)
        {
            PnjClass pnj = new PnjClass();
            pnj.PrefabPnj = Instantiate(PnjPGameObject, _container.transform); //PnjPGameObject;
            pnj.PrefabPnj.transform.position = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
            //pnj.PrefabPnj.transform.Rotate(new Vector3(0, Random.Range(-1f, 360f)));
            pnj.Type = i.ToString();
            _listPnj[i] = pnj;
            //Instantiate(pnj.PrefabPnj,_container.transform);
            
        }


       /* foreach (Transform child in _container.transform)
        {
			int rndZoneIndex = Random.Range(0, _listZone.Count);
			
            SlotClass sc = _listZone[rndZoneIndex].GetFreeSlot();
            if (sc != null)
            {
                child.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = sc.SlotGameObject.transform.position;
            }
			
        }*/
    }

    private void MovePnj()
    {
        foreach (ZoneClass zone in _listZone)
        {
            zone.EmptyListSlot();
        }
        foreach (PnjClass pnj in _listPnj)
        {
            int rndZoneIndex = Random.Range(0, _listZone.Count);

            SlotClass sc = _listZone[rndZoneIndex].GetFreeSlot();
            if (sc != null)
            {
                pnj.PrefabPnj.GetComponent<NavMeshAgent>().destination = sc.SlotGameObject.transform.position;
            }

        }
        //foreach (Transform child in _container.transform)
        //{
        //    int rndZoneIndex = Random.Range(0, _listZone.Count);

        //    SlotClass sc = _listZone[rndZoneIndex].GetFreeSlot();
        //    if (sc != null)
        //    {
        //        child.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = sc.SlotGameObject.transform.position;
        //    }

        //}
    }

    
}
