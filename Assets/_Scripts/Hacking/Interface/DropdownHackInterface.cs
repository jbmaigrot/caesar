﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropdownHackInterface : MonoBehaviour
{

    private int previousValue;



   

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

        if (this.GetComponent<Dropdown>().value != previousValue)
        {
            UpdateHackingGraph();

        }
    }

    public void UpdateOff()
    {
        this.GetComponent<Dropdown>().value = 0;
        previousValue = 0;
    }

    public void UpdateBlank(bool isInput)
    {
        UpdateOptions(isInput);
        this.GetComponent<Dropdown>().value = 0;
        previousValue = 0;
    }

    public void UpdateOn(bool isInput, bool isFixed, string code)
    {
        if (isFixed)
        {
            this.GetComponent<Dropdown>().ClearOptions();
            Dropdown.OptionData NewData; NewData = new Dropdown.OptionData();
            List<Dropdown.OptionData> ListNewData = new List<Dropdown.OptionData>();
            NewData.image = null;

            if (isInput)
            {
                NewData.text = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.DescribeCode(code, false, true);
            }
            else
            {
                NewData.text = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.DescribeCode(code, false, false);
            }
            ListNewData.Add(NewData);
            this.GetComponent<Dropdown>().AddOptions(ListNewData);
            this.GetComponent<Dropdown>().value = 0;
            previousValue = 0;
        }
        else
        {
            this.GetComponent<Dropdown>().value = UpdateOptions(isInput, code);
            previousValue = this.GetComponent<Dropdown>().value;
        }
    }

    private int UpdateOptions(bool isInput, string code = "")
    {
        this.GetComponent<Dropdown>().ClearOptions();

        Dropdown.OptionData NewData; NewData = new Dropdown.OptionData();
        List<Dropdown.OptionData> ListNewData = new List<Dropdown.OptionData>();

        int indice = 0;
        int retour = 0;

        if (HackInterface.SelectedGameObject != null)
        {
            if (isInput)
            {
                NewData.image = null;

                NewData.text = "";
                ListNewData.Add(NewData);
                foreach (string ryan in HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleInputCode)
                {
                    indice += 1;
                    NewData = new Dropdown.OptionData();
                    NewData.image = null;
                    NewData.text = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.DescribeCode(ryan, false, true);

                    ListNewData.Add(NewData);
                    if (ryan == code)
                    {
                        retour= indice;
                    }
                }
            }
            else
            {
                NewData.image = null;

                NewData.text = "";
                ListNewData.Add(NewData);
                foreach (string ryan in HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleOutputCode)
                {
                    indice += 1;
                    NewData = new Dropdown.OptionData();
                    NewData.image = null;
                    NewData.text = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.DescribeCode(ryan, false, false);

                    ListNewData.Add(NewData);
                    if (ryan == code)
                    {
                        retour= indice;
                    }
                }
            }
            this.GetComponent<Dropdown>().AddOptions(ListNewData);
            

        }
        return retour;
    }
}
