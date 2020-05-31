using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltGfxLookupTableMaker : MonoBehaviour
{

    [Header("Start the scene to set this up")]
    public BeltGfxLookupTable myTable;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Setting up the gfx lookup table");

        BeltObject[] allBeltsInScene = GameObject.FindObjectsOfType<BeltObject>();

        for (int i = 0; i < allBeltsInScene.Length; i++) {
            int spriteIndex = Mathf.RoundToInt((allBeltsInScene[i].transform.position.x / 2) + (allBeltsInScene[i].transform.position.y / -2 * 8));

            if (!myTable.mapping.ContainsKey(BeltGfxLookupTable.beltToKey(allBeltsInScene[i]))) {
                myTable.mapping.Add(BeltGfxLookupTable.beltToKey(allBeltsInScene[i]), myTable.sprites[spriteIndex]);
            } else {
                myTable.mapping[BeltGfxLookupTable.beltToKey(allBeltsInScene[i])] = myTable.sprites[spriteIndex];
            }
        }
        myTable.isSetUp = true;

        foreach (KeyValuePair<string, Sprite> attachStat in myTable.mapping) {
            //Now you can access the key and value both separately from this attachStat as:
            Debug.Log(attachStat.Key.ToString() + ", " + attachStat.Value.ToString());
        }
    }
}
