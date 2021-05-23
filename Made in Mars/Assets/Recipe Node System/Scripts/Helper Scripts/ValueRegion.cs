using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// A helper script controlling the node port value region.
/// </summary>
public class ValueRegion : MonoBehaviour {

    public Text fieldName;
    public GameObject upDown;
    public GameObject counterObject;
    public Text counterField;
    
    private type myType;

    public enum type {
        port,
        craftingType,
        craftingTier,
        timeCost
    }
    
    private string[] myDropdownOptions;

    private NodePortGfx myPortMaster;
    private CraftingNodeGfx myNodeMaster;
    public void SetUp(type _type, bool _isCounted, string _fieldName, string[] dropdownOptions, NodePortGfx master, int _counter) {
        myPortMaster = master;
        counter = _counter;
        isCounted = _isCounted;
        myType = _type;
        SetUp(isCounted, _fieldName, dropdownOptions);
    }
    
    public void SetUp(type _type, bool _isCounted, string _fieldName, string[] dropdownOptions, CraftingNodeGfx master, int _counter) {
        myNodeMaster = master;
        counter = _counter;
        isCounted = _isCounted;
        myType = _type;
        SetUp(isCounted, _fieldName, dropdownOptions);
    }

    void SetUp(bool _isCounted, string _fieldName, string[] dropdownOptions) {
        fieldName.text = _fieldName;
        if(dropdownOptions == null)
            myDropdownOptions = new string[0];
        else 
            myDropdownOptions = dropdownOptions;

        if (_isCounted)
            ModifyCounter(0);
        else
            DisableCounter();

        if (NodeItemTreeMakerMaster.s == null) {
            DisableModifier();
        }
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
        if (myDropdownOptions.Length > 0) {
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
            myNodeMaster.ValueUpdated(myType, counter);
        }

        if (myPortMaster != null) {
            myPortMaster.ValueUpdated(counter);
        }
    }

    public void DisableModifier() {
        upDown.SetActive(false);
    }
}
