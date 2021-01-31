using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class BuildingNode : Node {
    
    [HideInInspector]
    public string myUniqueName = "New Building";

    [Input] public ItemOutput CraftingResult;
    [Output] public ItemInput CraftingInput;
}
