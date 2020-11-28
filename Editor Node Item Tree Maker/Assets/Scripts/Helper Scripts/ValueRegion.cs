using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueRegion : MonoBehaviour {

    public Text fieldName;
    public GameObject upDown;
    public GameObject counterObject;
    public Text counterField;

    private type myType;
    
    public enum type {
        itemInput,
        itemOutput,
        craftInput,
        craftOutput,
        craftingType,
        craftingTier,
        timeCost
    }

    private string[] myDropdownOptions;

    private NodePortGfx myPortMaster;
    private CraftingNodeGfx myNodeMaster;
    public void SetUp(type _type, string[] dropdownOptions, NodePortGfx master, int _counter) {
        myPortMaster = master;
        counter = _counter;
        SetUp(_type, dropdownOptions);
    }
    
    public void SetUp(type _type, string[] dropdownOptions, CraftingNodeGfx master, int _counter) {
        myNodeMaster = master;
        counter = _counter;
        SetUp(_type, dropdownOptions);
    }

    void SetUp(type _type, string[] dropdownOptions) {
        myType = _type;
        switch (myType) {
            case type.itemInput:
                DisableCounter();
                fieldName.text = "Result"; 
                break;
            case type.itemOutput:
                DisableCounter();
                fieldName.text = "Ingredient"; 
                break;
            case type.craftInput:
                fieldName.text = "Ingredient"; 
                break;
            case type.craftOutput:
                fieldName.text = "Result"; 
                break;
            case type.craftingType:
            case type.craftingTier:
            case type.timeCost:
                break;
        }
        myDropdownOptions = dropdownOptions;
        if(isCounted)
        ModifyCounter(0);
    }

    private bool isCounted = true;
    void DisableCounter() {
        isCounted = false;
        upDown.SetActive(false);
        counterObject.SetActive(false);
    }

    private int counter = 0;
    public void ModifyCounter(int amount) {
        counter += amount;
        if (myType == type.craftingType && myDropdownOptions.Length > 0) {
            if (counter < 0)
                counter = myDropdownOptions.Length-1;
            if (counter >= myDropdownOptions.Length)
                counter = 0;

            counterField.text = myDropdownOptions[counter];
        } else {
            if (counter < 0)
                counter = 0;
            counterField.text = counter.ToString();
        }

        if (myNodeMaster != null) {
            myNodeMaster.ValueUpdated(myType, counter, 0);
        }

        if (myPortMaster != null) {
            myPortMaster.ValueUpdated(myType, counter);
        }
    }

    public void DisableModifier() {
        upDown.SetActive(false);
    }
}
