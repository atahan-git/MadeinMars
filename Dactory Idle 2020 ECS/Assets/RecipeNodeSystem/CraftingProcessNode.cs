using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class CraftingProcessNode : Node {

    public enum cTypes { Miner, Furnace, ProcessorSingle, ProcessorDouble, Press, Coiler, Cutter, Lab, Building};
    public cTypes CraftingType;
    public int CraftingTier;
    public float timeCost = 1;

  
    [HideInInspector]
    public List<string> inputItemUniqueNames = new List<string>();
    [HideInInspector]
    public List<int> inputItemCounts = new List<int>();

    [HideInInspector]
    public List<string> outputItemUniqueNames = new List<string>();
    [HideInInspector]
    public List<int> outputItemCounts = new List<int>();

    // Start is called before the first frame update
    public override object GetValue (NodePort port) {
        return null;
    }
}