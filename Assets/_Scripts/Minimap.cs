using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public float mapSize = 300;
    public float worldSize = 220;

    public RectTransform mapPlayer;
    public List<RectTransform> mapAllies = new List<RectTransform>();
    public RectTransform mapOrangeRelay;
    public RectTransform mapBlueRelay;

    public Transform player;
    public List<Transform> allies = new List<Transform>();
    public Transform orangeRelay;
    public Transform blueRelay;

    // Update is called once per frame
    void Update()
    {
        if (player != null)
            mapPlayer.localPosition = new Vector2(player.position.x / worldSize * mapSize, player.position.z / worldSize * mapSize);
    }
}

//minimap.mapSource.localPosition = new Vector2(sources[i].transform.position.x / minimap.worldSize * minimap.mapSize, sources[i].transform.position.z / minimap.worldSize * minimap.mapSize);