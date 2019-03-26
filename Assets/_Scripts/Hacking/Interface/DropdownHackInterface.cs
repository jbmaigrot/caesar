using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropdownHackInterface : MonoBehaviour
{
    private GameObject SelectedGameObject;
    private int previousValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void UpdateOptions()
    {
        this.GetComponent<Dropdown>().ClearOptions();

        Dropdown.OptionData NewData; NewData = new Dropdown.OptionData();
        List<Dropdown.OptionData> ListNewData = new List<Dropdown.OptionData>();



        if (HackInterface.SelectedGameObject != null)
        {
            if (this.GetComponentInParent<TextButtonHackInterface>().isInput)
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().numero <= HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes.Count && HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].is_fixed)
                {
                    NewData = new Dropdown.OptionData();
                    NewData.image = null;
                    NewData.text = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.DescribeCode(HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].code, false, true);

                    ListNewData.Add(NewData);
                }
                else
                {
                    NewData.image = null;

                    NewData.text = "";
                    ListNewData.Add(NewData);
                    foreach (string ryan in HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleInputCode)
                    {
                        NewData = new Dropdown.OptionData();
                        NewData.image = null;
                        NewData.text = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.DescribeCode(ryan, false, true);

                        ListNewData.Add(NewData);

                    }
                }

            }
            else
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().numero <= HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes.Count && HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].is_fixed)
                {
                    NewData = new Dropdown.OptionData();
                    NewData.image = null;
                    NewData.text = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.DescribeCode(HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].code, false, false);

                    ListNewData.Add(NewData);
                }
                else
                {
                    NewData.image = null;

                    NewData.text = "";
                    ListNewData.Add(NewData);
                    foreach (string ryan in HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleOutputCode)
                    {
                        NewData = new Dropdown.OptionData();
                        NewData.image = null;
                        NewData.text = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.DescribeCode(ryan, false, false);

                        ListNewData.Add(NewData);

                    }
                }


            }
            this.GetComponent<Dropdown>().AddOptions(ListNewData);


            if (this.GetComponentInParent<TextButtonHackInterface>().isInput)
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().numero > HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes.Count)
                {
                    this.GetComponent<Dropdown>().value = 0;
                }
                else
                {
                    for (int i = 0; i < HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleInputCode.Count; i++)
                    {
                        if (HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleInputCode[i] == HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].code)
                        {
                            this.GetComponent<Dropdown>().value = i + 1;
                        }
                    }

                }
            }
            else
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().numero > HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes.Count)
                {
                    this.GetComponent<Dropdown>().value = 0;
                }
                else
                {
                    for (int i = 0; i < HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleOutputCode.Count; i++)
                    {
                        if (HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleOutputCode[i] == HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].code)
                        {
                            this.GetComponent<Dropdown>().value = i + 1;
                        }
                    }
                }
            }
        }
    }

    void UpdateHackingGraph()
    {
        if (this.GetComponent<Dropdown>().value == 0)
        {
            if (this.GetComponentInParent<TextButtonHackInterface>().isInput)
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().numero <= HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes.Count)
                {
                    HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes.RemoveAt(this.GetComponentInParent<TextButtonHackInterface>().numero - 1);
                }
            }
            else
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().numero <= HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes.Count)
                {
                    HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes.RemoveAt(this.GetComponentInParent<TextButtonHackInterface>().numero - 1);
                }
            }
        }
        else
        {
            if (previousValue == 0)
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().isInput)
                {
                    InOutVignette NewInputHack = new InOutVignette();
                    NewInputHack.code = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleInputCode[this.GetComponent<Dropdown>().value - 1];
                    HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes.Add(NewInputHack);
                }
                else
                {
                    InOutVignette NewOutputHack = new InOutVignette();
                    NewOutputHack.code = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleOutputCode[this.GetComponent<Dropdown>().value - 1];
                    HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes.Add(NewOutputHack);
                }
            }
            else
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().isInput)
                {
                    HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].code = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleInputCode[this.GetComponent<Dropdown>().value - 1];
                }
                else
                {
                    HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].code = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleOutputCode[this.GetComponent<Dropdown>().value - 1];
                }
            }
        }
        this.GetComponentInParent<HackInterface>().reloadInterface();
        previousValue = this.GetComponent<Dropdown>().value;

    }

    // Update is called once per frame
    void Update()
    {
        if (SelectedGameObject != HackInterface.SelectedGameObject)
        {
            UpdateOptions();
            SelectedGameObject = HackInterface.SelectedGameObject;
            previousValue = this.GetComponent<Dropdown>().value;
        }

        if (this.GetComponent<Dropdown>().value != previousValue)
        {
            UpdateHackingGraph();

        }
    }


}
