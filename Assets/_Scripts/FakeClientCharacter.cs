using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeClientCharacter : MonoBehaviour
{
    public MeshRenderer Body;
    public MeshRenderer Lens;

    private float floatingRange = 0.1f;
    private float floatingFreq = 0;
    public Transform mesh;
    private float startingY;

    public int state = 0;

    //Start
    private void Start()
    {
        floatingFreq = Random.Range(0.3f, 0.4f);
        mesh = transform.Find("Mesh").transform;
        startingY = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Floating animation
        mesh.localPosition = new Vector3(0, startingY - floatingRange / 2 + floatingRange * (Mathf.Sin(Time.time * 2 * Mathf.PI * floatingFreq)), 0);

        switch (state)
        {
            case 0:
                Body.material.SetFloat("_IsShining", 1f);
                Body.material.SetColor("_EmissiveColor", new Color(1f, 0f, 9f / 255f, 1f));
                Body.material.SetFloat("_WaveSpeed", 8.0f);
                Lens.material.SetColor("_EmissionColor", new Color(1f, 0f, 9f / 255f, 1f));
                break;

            case 1:
                Body.material.SetFloat("_IsShining", 0f);
                Body.material.SetColor("_EmissiveColor", new Color(0f, 0f, 0f, 1f));
                Body.material.SetFloat("_WaveSpeed", 0.0f);
                Lens.material.SetColor("_EmissionColor", new Color(146f / 255f, 214 / 255f, 1f, 1f));
                break;

            case 2:
                Body.material.SetFloat("_IsShining", 1f);
                Body.material.SetColor("_EmissiveColor", new Color(1f, 236f / 255f, 0f, 1f));
                Body.material.SetFloat("_WaveSpeed", 3.0f);
                Lens.material.SetColor("_EmissionColor", new Color(1f, 236f / 255f, 0f, 1f));
                break;

            default:
                break;

        }
    }
}
