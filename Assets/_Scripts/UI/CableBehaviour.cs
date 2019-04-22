using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CableBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.001f;
        Debug.Log(GetComponent<Image>().alphaHitTestMinimumThreshold);
    }

    public void OnClick()
    {
        Debug.Log(name + "ONCLICK");
        Debug.Log(GetComponent<Image>().alphaHitTestMinimumThreshold);
    }

    void OnMouseDown()
    {
        Debug.Log(name + "OnMouseDown");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + " Was Clicked.");
    }
}
