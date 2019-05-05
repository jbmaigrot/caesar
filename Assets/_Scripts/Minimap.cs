using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public float mapSize = 300;
    public float worldSize = 220;
    public RectTransform mapPlayer;
    public Transform player;
    public RectTransform mapSource;

    // Update is called once per frame
    void Update()
    {
        if (player != null)
            mapPlayer.localPosition = new Vector2(player.position.x / worldSize * mapSize, player.position.z / worldSize * mapSize);
    }
}
