using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedRosace : MonoBehaviour
{
    private Image image;
    private float remainingTime;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        image.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else
        {
            image.enabled = false;
        }
    }

    // Show
    public void DisplayRedRosace()
    {
        image.enabled = true;
        remainingTime = 2;
    }
}
