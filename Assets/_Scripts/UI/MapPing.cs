using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPing : MonoBehaviour
{
    private float startingTime;
    private float fadeTime = 2f;

    // Start is called before the first frame update
    void Start()
    {
        startingTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startingTime > fadeTime)
        {
            Destroy(gameObject);
        }
        else
        {
            Image pingImg = GetComponent<Image>();
            pingImg.color = new Color(pingImg.color.r, pingImg.color.g, pingImg.color.b, 1 - (Time.time - startingTime) / fadeTime);
        }
    }
}
