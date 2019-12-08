using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public GameObject[] lines;
    public Color col;
    public ServerCarrier battery;
    private Text text;

#if CLIENT
    // Start is called before the first frame update
    void Start()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		text = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		text.text = Mathf.Floor(battery.clientCharge * 100) + "%";

        for (int i = 0; i < lines.Length; i++)
        {
            if(battery.clientCharge >= (i+1)*0.2)
                lines[i].GetComponent<SVGImage>().color = col;
            else
                lines[i].GetComponent<SVGImage>().color = Color.Lerp(Color.white, col, (battery.clientCharge - i * 0.2f)*3); // 0 to 60%
                //Debug.Log(Color.Lerp(Color.white, col, 0.5f * battery.clientCharge));
        }
    }

	/*public void fillLine(int num)
    {
		if (!GameState.CLIENT) return; // replacement for preprocessor

        lines[num].GetComponent<SVGImage>().color = col;
    }*/
#endif
}
