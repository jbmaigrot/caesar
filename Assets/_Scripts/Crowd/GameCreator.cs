using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameCreator : MonoBehaviour
{
    public GameObject PnjPGameObject;
    public int NbPnj;
    public Button ResetButton;
    public Button MoveButton;
    public Text ChanText;

    public GameObject ScrollPanel;
    public GameObject ClickableTweet;

    public GameObject Player;

    public GameObject[] ListZone;

    private PnjClass[] _listPnj;
    private GameObject _container;
    private GameObject _tweetContainer;

    private List<ZoneClass> _listZone = new List<ZoneClass>();

    private bool tweetIsWaiting;
    private TweetList _tweets = new TweetList();





    void Start()
    {
        _container = GameObject.Find("Crowd");
        _tweetContainer = GameObject.Find("Content");
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

        StartCoroutine(GetTextTwitter());
    }

    IEnumerator GetTextTwitter()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://www.nadege-bourguignon.com/PROJETS/CAESAR/tweet.json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string text = www.downloadHandler.text;
            string[] tweets = www.downloadHandler.text.Split(char.Parse(","));


            TweetList loadedData = JsonUtility.FromJson<TweetList>(text);
            foreach (Tweet tweet in loadedData.tweets)
            {
                //Debug.Log(tweet.tweet);
                //ChanText.text += tweet.tweet;
                //ChanText.text += "\n";
            }
            _tweets = loadedData;
        }

    }

    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 200))
                {
                Player.GetComponent<NavMeshAgent>().destination = hit.point;
            }
        }


        //FACE A L'OBJET
        foreach (PnjClass pnj in _listPnj)
        {
            NavMeshAgent agent = pnj.PrefabPnj.GetComponent<NavMeshAgent>();
            if (agent.remainingDistance <= agent.stoppingDistance )
            {

                if (!agent.hasPath || Mathf.Abs(agent.velocity.sqrMagnitude) < float.Epsilon)
                {
                    Vector3 targetDir = pnj.DestinationGameObject.transform.position - pnj.PrefabPnj.transform.position;
                    float step = 2 * Time.deltaTime;
                    Vector3 newDir = Vector3.RotateTowards(pnj.PrefabPnj.transform.forward, targetDir, step, 0.0f);
                    pnj.PrefabPnj.transform.rotation =  Quaternion.LookRotation(newDir);
                }

                    

            }

        }


        if (!tweetIsWaiting)
        {
            tweetIsWaiting = true;
            StartCoroutine(WriteTweet());
        }
    }

    IEnumerator WriteTweet()
    {
        float wait = Random.Range(1f, 10f);
        yield return new WaitForSeconds(wait);
        if (_tweets.tweets.Length > 0)
        {
            float index = Random.Range(0f, _tweets.tweets.Length - 1);
            //ChanText.text += _tweets.tweets[(int)index].tweet;
            //ChanText.text += "\n";
            tweetIsWaiting = false;

            Tweet tw = new Tweet();
            tw.tweet = _tweets.tweets[(int)index].tweet;
            tw.TweetGameObject = Instantiate(ClickableTweet, ScrollPanel.transform);
            tw.pnj = _listPnj[Random.Range(0, _listPnj.Length - 1)];
            tw.TweetGameObject.GetComponentInChildren<Text>().text = tw.tweet;
            tw.TweetGameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate { ClickOnTweet(tw); });


            

        }
        
    }

    void ClickOnTweet(Tweet tw)
    {
        Debug.Log(tw.pnj.Name + " : " +tw.tweet);
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
            pnj.PrefabPnj = Instantiate(PnjPGameObject, _container.transform); 

            int rndZoneIndex = Random.Range(0, _listZone.Count);

            SlotClass sc = _listZone[rndZoneIndex].GetFreeSlot();
            if (sc != null && sc.SlotGameObject != null)
            {
                pnj.PrefabPnj.transform.position = sc.SlotGameObject.GetComponent<Renderer>().bounds.center;
                pnj.DestinationGameObject = sc.ConnectedGameObject;
            }
            
            pnj.Name = i.ToString();
            _listPnj[i] = pnj;
        }

        
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
            if (sc != null && sc.SlotGameObject != null)
            {
                pnj.PrefabPnj.GetComponent<NavMeshAgent>().destination = sc.SlotGameObject.transform.position;
                pnj.DestinationGameObject = sc.ConnectedGameObject;
            }

        }
    }

    
}
