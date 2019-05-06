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
            mapPlayer.localPosition = worldToMap(player.position);
    }


    // Convert coordinates
    public Vector2 worldToMap (Vector3 worldPosition)
    {
        return new Vector2(worldPosition.x / worldSize * mapSize, worldPosition.z / worldSize * mapSize);
    }

    public Vector3 mapToWorld(Vector2 mapPosition)
    {
        return new Vector3(mapPosition.x * worldSize / mapSize, 0, mapPosition.y * worldSize / mapSize);
    }
}

//minimap.mapSource.localPosition = new Vector2(sources[i].transform.position.x / minimap.worldSize * minimap.mapSize, sources[i].transform.position.z / minimap.worldSize * minimap.mapSize);