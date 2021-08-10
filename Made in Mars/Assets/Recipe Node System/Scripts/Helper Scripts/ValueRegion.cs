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
    public void SetUp(type _type, bool _isCounted, string _fieldName, string[] dropdownOptions, NodePortGfx master, int _counter, bool isInteractable) {
        myPortMaster = master;
        counter = _counter;
        isCounted = _isCounted;
        myType = _type;
        SetUp(isCounted, _fieldName, dropdownOptions, isInteractable);
    }
    
    public void SetUp(type _type, bool _isCounted, string _fieldName, string[] dropdownOptions, CraftingNodeGfx master, int _counter, bool isInteractable) {
        myNodeMaster = master;
        counter = _counter;
        isCounted = _isCounted;
        myType = _type;
        SetUp(isCounted, _fieldName, dropdownOptions,  isInteractable);
    }

    void SetUp(bool _isCounted, string _fieldName, string[] dropdownOptions, bool _isInteractable) {
        fieldName.text = _fieldName;
        if(dropdownOptions == null)
            myDropdownOptions = new string[0];
        else 
            myDropdownOptions = dropdownOptions;
        
        
        isCounted = _isCounted;
        isInteractable = _isInteractable;

        SetCounterState();
        
        if (_isCounted)
            ModifyCounter(0);

    }

    private bool isCounted = true;
    private bool isInteractable = true;
    void SetCounterState() {
        upDown.SetActive(isCounted && isInteractable);
        counterObject.SetActive(isCounted);
    }

    private int counter = 0;
    public void ModifyCounter(int amount) {
        if (counter < 15) {
            counter += amount;
        } else if(counter < 50){
            counter += amount * 5;
        } else if (counter < 150) {
            counter += amount * 10;
        } else {
            counter += amount * 50;
        }

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
}
