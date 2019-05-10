﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public float mapSize = 300;
    public float worldSize = 220;

    public RectTransform mapPlayer;
    public List<RectTransform> mapAllies = new List<RectTransform>();
    public RectTransform mapRedRelay;
    public RectTransform mapBlueRelay;
    public RectTransform mapMessage;
    public GameObject mapPingPrefab;
    public GameObject mapAllyPrefab;

    public Transform player;
    public List<Transform> allies = new List<Transform>();
    public Transform orangeRelay;
    public Transform blueRelay;

#if CLIENT
    private bool isPointerOver = false;
    private Client client;
    private HackInterface hackInterface;

    private float messageStartingTime;
    private float messageFadeTime = 2f;

    // Start
    private void Start()
    {
        client = FindObjectOfType<Client>();
        hackInterface = FindObjectOfType<HackInterface>();
        mapMessage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        //move player icon
        if (player != null)
            mapPlayer.localPosition = worldToMap(player.position);

        for (int i = 0; i < allies.Count; i++)
        {
            if (allies[i] != null && mapAllies[i] != null)
                mapAllies[i].localPosition = worldToMap(allies[i].position);
        }

        //update message display
        if (Time.time - messageStartingTime > messageFadeTime)
        {
            mapMessage.gameObject.SetActive(false);
        }

        //inputs: move or ping
        if (isPointerOver)
        {
            if (!Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0) && !hackInterface.GetComponent<CanvasGroup>().blocksRaycasts)
            {
                client.SetDestination(mapToWorld(screenToMap(Input.mousePosition)));
            }
            else if (!Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(1))
            {
                client.Ping(screenToMap(Input.mousePosition));
            }
        }
    }

    // Ping
    public void Ping(Vector2 mapPos)
    {
        GameObject newPing = Instantiate(mapPingPrefab, transform);
        newPing.GetComponent<RectTransform>().localPosition = mapPos;
    }

    // Show message source
    public void ShowMessage(Vector3 sourcePosition)
    {
        mapMessage.gameObject.SetActive(true);
        mapMessage.localPosition = worldToMap(sourcePosition);
        messageStartingTime = Time.time;
    }

    // Update relays
    public void UpdateRelays(bool redIsVisible, bool blueIsVisible, Vector3 redPos, Vector3 bluePos, int team)
    {
        if (/*redIsVisible ||*/ team == 0)
        {
            mapRedRelay.gameObject.SetActive(true);
            mapRedRelay.localPosition = worldToMap(redPos);
        }
        else
        {
            mapRedRelay.gameObject.SetActive(false);
        }

        if (/*blueIsVisible ||*/ team == 1)
        {
            mapBlueRelay.gameObject.SetActive(true);
            mapBlueRelay.localPosition = worldToMap(bluePos);
        }
        else
        {
            mapBlueRelay.gameObject.SetActive(false);
        }
    }

    // Bind new ally object
    public void AddAlly(Transform newAlly, Color color)
    {
        allies.Add(newAlly);

        GameObject newMapAlly = Instantiate(mapAllyPrefab, transform);
        newMapAlly.GetComponent<Image>().color = color;
        mapAllies.Add(newMapAlly.GetComponent<RectTransform>());
    }

    // Set isPointerOver
    public void SetIsPointerOver(bool b)
    {
        isPointerOver = b;
    }
#endif
    // Convert coordinates
    public Vector2 worldToMap (Vector3 worldPosition)
    {
        return new Vector2(worldPosition.x / worldSize * mapSize, worldPosition.z / worldSize * mapSize);
    }

    public Vector3 mapToWorld (Vector2 mapPosition)
    {
        return new Vector3(mapPosition.x * worldSize / mapSize, 0, mapPosition.y * worldSize / mapSize);
    }

    public Vector2 screenToMap (Vector3 mousePosition)
    {
        float angle = - GetComponent<RectTransform>().localEulerAngles.z * Mathf.Deg2Rad;
        Vector2 mapPos = GetComponent<RectTransform>().position;

        return new Vector2(((mousePosition.x - mapPos.x) * Mathf.Cos(angle) - (mousePosition.y - mapPos.y) * Mathf.Sin(angle)) * 1920 / Screen.width,
                           ((mousePosition.x - mapPos.x) * Mathf.Sin(angle) + (mousePosition.y - mapPos.y) * Mathf.Cos(angle)) * 1080 / Screen.height);
    }

}
