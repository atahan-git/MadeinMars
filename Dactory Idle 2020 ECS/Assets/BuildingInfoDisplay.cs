using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInfoDisplay : MonoBehaviour
{
    public BuildingCraftingController crafter;

    public Slider progressBar;
    public GameObject inputParent;
    public GameObject outputParent;

    public List<GameObject> inputs = new List<GameObject>();
    public List<GameObject> outputs = new List<GameObject>();

    public GameObject processDisplayPrefab;

    // Update is called once per frame
    void Update()
    {
        progressBar.value = (float)crafter.curCraftingProgress / (float)crafter.craftingProgressTickReq;
    }

    public void SetUp () {

    }

	public void UpdateValues (){

	}
}
