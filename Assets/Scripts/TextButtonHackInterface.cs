using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextButtonHackInterface : MonoBehaviour, IPointerDownHandler
{
    public bool isInput;
    public int numero;
    private bool isAWorkingNode;
    private bool isVisible;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnPointerDown(PointerEventData pointerEvent)
    {
        if (isAWorkingNode)
        {

        }
        else
        {
            if (isInput)
            {
                HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes.Add(new InputHack());

            }
            else
            {
                HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes.Add(new OutputHack());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isInput)
        {
            if (numero > HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes.Count)
            {
                this.GetComponentInChildren<Text>().text = "";
                isAWorkingNode = false;
                if(numero> HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes.Count + 1)
                {
                    isVisible = false;
                    this.GetComponent<CanvasGroup>().alpha = 0f;
                    this.GetComponent<CanvasGroup>().blocksRaycasts = false;
                }
                else
                {
                    isVisible = true;
                    this.GetComponent<CanvasGroup>().alpha = 1f;
                    this.GetComponent<CanvasGroup>().blocksRaycasts = true;
                }
            }
            else
            {
                this.GetComponentInChildren<Text>().text = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes[numero - 1].inputcode;
                isAWorkingNode = true;
                isVisible = true;
                this.GetComponent<CanvasGroup>().alpha = 1f;
                this.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }
        else
        {
            if (numero > HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes.Count)
            {
                this.GetComponentInChildren<Text>().text = "";
                isAWorkingNode = false;
                if (numero > HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes.Count + 1)
                {
                    isVisible = false;
                    this.GetComponent<CanvasGroup>().alpha = 0f;
                    this.GetComponent<CanvasGroup>().blocksRaycasts = false;
                }
                else
                {
                    isVisible = true;
                    this.GetComponent<CanvasGroup>().alpha = 1f;
                    this.GetComponent<CanvasGroup>().blocksRaycasts = true;
                }
            }
            else
            {
                this.GetComponentInChildren<Text>().text = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes[numero - 1].outputcode;
                isAWorkingNode = true;
                isVisible = true;
                this.GetComponent<CanvasGroup>().alpha = 1f;
                this.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }

        if (isVisible)
        {
            this.GetComponentInChildren<Dropdown>().ClearOptions();
            if (isAWorkingNode)
            {
                this.GetComponentInChildren<CanvasGroup>().alpha = 0f;
            }
            else
            {
                this.GetComponentInChildren<CanvasGroup>().alpha = 1f;
                Dropdown.OptionData NewData;NewData=new Dropdown.OptionData();
                List<Dropdown.OptionData> ListNewData= new List<Dropdown.OptionData>();
                if (isInput)
                {
                    foreach (string ryan in HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleInputCode)
                    {
                        NewData = new Dropdown.OptionData();
                        NewData.image = null;
                        NewData.text = ryan;
                        ListNewData.Add(NewData);

                    }
                }
                else
                {
                    foreach (string ryan in HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleOutputCode)
                    {
                        NewData = new Dropdown.OptionData();
                        NewData.image = null;
                        NewData.text = ryan;
                        ListNewData.Add(NewData);

                    }
                }
                this.GetComponentInChildren<Dropdown>().AddOptions(ListNewData);

            }
           
        }
    }
}
