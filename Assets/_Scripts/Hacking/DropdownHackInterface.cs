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
        UpdateOptions();
        SelectedGameObject = HackInterface.SelectedGameObject;
    }


    void UpdateOptions()
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
                    foreach (InputCode reynolds in HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.inputCodes)
                    {
                        if (HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].inputcode == reynolds.inputCode)
                        {
                            NewData.text = reynolds.descriptionWithParameter;
                        }
                    }

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
                        foreach (InputCode reynolds in HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.inputCodes)
                        {
                            if (ryan == reynolds.inputCode)
                            {
                                NewData.text = reynolds.descriptionWithParameter;
                            }
                        }

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
                    foreach (OutputCode reynolds in HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.outputCodes)
                    {
                        if (HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].outputcode == reynolds.outputCode)
                        {
                            NewData.text = reynolds.descriptionWithParameter;
                        }
                    }

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
                        foreach (OutputCode reynolds in HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.outputCodes)
                        {
                            if (ryan == reynolds.outputCode)
                            {
                                NewData.text = reynolds.descriptionWithParameter;
                            }
                        }

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
                        if (HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleInputCode[i] == HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].inputcode)
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
                        if (HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleOutputCode[i] == HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].outputcode)
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
                    InputHack NewInputHack = new InputHack();
                    NewInputHack.inputcode = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleInputCode[this.GetComponent<Dropdown>().value - 1];
                    HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes.Add(NewInputHack);
                }
                else
                {
                    OutputHack NewOutputHack = new OutputHack();
                    NewOutputHack.outputcode = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleOutputCode[this.GetComponent<Dropdown>().value - 1];
                    HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes.Add(NewOutputHack);
                }
            }
            else
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().isInput)
                {
                    HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].inputcode = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleInputCode[this.GetComponent<Dropdown>().value - 1];
                }
                else
                {
                    HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].outputcode = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleOutputCode[this.GetComponent<Dropdown>().value - 1];
                }
            }
        }

        UpdateOptions();
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
