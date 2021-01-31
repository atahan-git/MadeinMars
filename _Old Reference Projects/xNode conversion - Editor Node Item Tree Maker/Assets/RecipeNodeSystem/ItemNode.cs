using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class ItemNode : Node {
    
    [HideInInspector]
    public string myUniqueName = "New Item";

    [Input] public ItemOutput CraftingResult;
    [Output] public ItemInput CraftingInput;
}
