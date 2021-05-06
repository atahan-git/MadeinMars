using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;


/// <summary>
/// The main class that draws and controls the node item tree making process.
/// See the relevant "Node Item Tree Making Scene" to see this script in action
/// </summary>
public class NodeItemTreeMakerMaster : RecipeTreeMaster {
    public static NodeItemTreeMakerMaster s;

    public float DeadZonePercent = 0.1f;
    public float DeadZone;

    public RectTransform ItemsParent;
    public GameObject ItemListItemPrefab;

    private int lastIdGiven = 0;

    private void Awake() {
        Setup();
        s = this;
        myCanvas = canvas.GetComponent<Canvas>();
        mainCam = Camera.main;
        NodeGfx.snapMultUI = NodeGfx.snapMult * canvas.localScale.x;
        DeadZone = Screen.height * DeadZonePercent;
    }

    private void Start() {
        // Draw menu for items
        foreach (ItemSet myItemSet in myRecipeSet.myItemSets) {
            foreach (Item myItem in myItemSet.items) {
                GameObject itemListItem = Instantiate(ItemListItemPrefab, ItemsParent);
                itemListItem.GetComponent<DragMe>().myItem = myItem;
                itemListItem.transform.GetChild(0).GetComponent<Image>().sprite = myItem.mySprite;
                itemListItem.transform.GetChild(1).GetComponent<Text>().text = myItem.name;
            }
        }
        // Draw menu for ores is not needed, because ores should exist as items anyways! Use the corresponding item for ore mining

        // Draw menu for buildings
        foreach (BuildingData myBuildingData in myRecipeSet.myBuildings) {
            GameObject itemListItem = Instantiate(ItemListItemPrefab, ItemsParent);
            Item buildingTempItem = new Item();
            buildingTempItem.uniqueName = myBuildingData.uniqueName;
            buildingTempItem.mySprite = myBuildingData.gfxSprite;
            itemListItem.GetComponent<DragMe>().myItem = buildingTempItem;
            itemListItem.transform.GetChild(0).GetComponent<Image>().sprite = buildingTempItem.mySprite;
            itemListItem.transform.GetChild(1).GetComponent<Text>().text = buildingTempItem.name;
        }

        //Draw Nodes
        foreach (var itemNode in myRecipeSet.myItemNodes) {
            //Vector3 pos = itemNode.pos;
            var node = Instantiate(ItemNodePrefab, NodeParent);
            allNodeGfxs.Add(node.GetComponent<NodeGfx>());
            node.GetComponent<ItemNodeGfx>().SetUp(this, itemNode);
            (node.transform as RectTransform).anchoredPosition = itemNode.pos;
            //itemNode.pos = pos;

            lastIdGiven = Mathf.Max(lastIdGiven, itemNode.id);
        }

        foreach (var craftingNode in myRecipeSet.myCraftingNodes) {
            //Vector3 pos = craftingNode.pos;
            var node = Instantiate(CraftingNodePrefab, NodeParent);
            allNodeGfxs.Add(node.GetComponent<NodeGfx>());
            node.GetComponent<CraftingNodeGfx>().SetUp(this, craftingNode);
            (node.transform as RectTransform).anchoredPosition = craftingNode.pos;
            //craftingNode.pos = pos;

            lastIdGiven = Mathf.Max(lastIdGiven, craftingNode.id);
        }

        foreach (var nodeGfx in allNodeGfxs) {
            nodeGfx.SetupPorts();
        }

        foreach (var nodeGfx in allNodeGfxs) {
            nodeGfx.SetupConnections();
        }
        
        foreach (var nodeGfx in allNodeGfxs) {
            nodeGfx.OnDraggingNode();
        }

        Invoke("LateStart",0.5f);
        RescaleNodeArea();
        
        //UpdateValues();
    }

    void UpdateValues() {
        foreach (var nodeGfx in allNodeGfxs) {
            if (nodeGfx is ItemNodeGfx) {
                var node = nodeGfx.myNode as ItemNode;
                node.inputIds = new List<int>();
                node.outputIds = new List<int>();
            } else {
                var node = nodeGfx.myNode as CraftingNode;
                node.inputs = new List<CountedItemNode>();
                node.outputs = new List<CountedItemNode>();
            }
        }
    }


    void LateStart() {
        foreach (var nodeGfx in allNodeGfxs) {
            nodeGfx.OnDraggingNode();
        }
    }


    public void CreateItemNodeAtPosition(PointerEventData data) {
        var originalObj = data.pointerDrag;
        if (originalObj == null)
            return;

        var dragMe = originalObj.GetComponent<DragMe>();
        if (dragMe == null)
            return;

        var myItem = dragMe.myItem;
        if (myItem == null)
            return;
        GameObject node;
        lastIdGiven += 1;
        if (myItem.uniqueName == "CraftingProcess") {
            node = Instantiate(CraftingNodePrefab, NodeParent);
            var craftingNode = new CraftingNode(lastIdGiven);
            myRecipeSet.myCraftingNodes.Add(craftingNode);
            node.GetComponent<CraftingNodeGfx>().SetUp(this, craftingNode);
            node.GetComponent<CraftingNodeGfx>().SetupPorts();
            node.GetComponent<CraftingNodeGfx>().PositionUpdated();

        } else {
            node = Instantiate(ItemNodePrefab, NodeParent);
            var itemNode = new ItemNode(lastIdGiven, myItem);
            myRecipeSet.myItemNodes.Add(itemNode);
            node.GetComponent<ItemNodeGfx>().SetUp(this, itemNode);
            node.GetComponent<ItemNodeGfx>().SetupPorts();
            node.GetComponent<ItemNodeGfx>().PositionUpdated();
        }

        allNodeGfxs.Add(node.GetComponent<NodeGfx>());
        //node.transform.position = mycam.ScreenToWorldPoint(data.position);
        node.transform.position = data.position;
        node.transform.position = new Vector3(node.transform.position.x, node.transform.position.y, 0);
        node.transform.localScale = Vector3.one;
    }

    private NodePortGfx prevPort;
    private NodeGfx prevNode;

    public override void BeginClickConnect(NodeGfx node, NodePortGfx port) {

        if (prevNode == null) {
            prevPort = port;
            prevNode = node;
        } else if (prevPort == port) {
            prevNode = null;
            prevPort.ClickConnectDone();
            prevPort = null;
        } else {
            bool canConnect = false;

            switch (port.myType) {
                case NodePortGfx.PortType.craftInput:
                    if (prevPort.myType == NodePortGfx.PortType.itemOutput)
                        canConnect = true;
                    break;
                case NodePortGfx.PortType.craftOutput:
                    if (prevPort.myType == NodePortGfx.PortType.itemInput)
                        canConnect = true;
                    break;
                case NodePortGfx.PortType.itemInput:
                    if (prevPort.myType == NodePortGfx.PortType.craftOutput)
                        canConnect = true;
                    break;
                case NodePortGfx.PortType.itemOutput:
                    if (prevPort.myType == NodePortGfx.PortType.craftInput)
                        canConnect = true;
                    break;
            }

            if (canConnect) {
                CraftingNode craft;
                ItemNode item;
                bool isInput;

                // Figure out what port is which, and whether we are dealing with an input or not
                if (prevNode is CraftingNodeGfx) {
                    craft = prevNode.myNode as CraftingNode;
                    item = node.myNode as ItemNode;
                    if (prevPort.myType == NodePortGfx.PortType.craftInput)
                        isInput = true;
                    else
                        isInput = false;

                } else {
                    craft = node.myNode as CraftingNode;
                    item = prevNode.myNode as ItemNode;
                    if (port.myType == NodePortGfx.PortType.craftInput)
                        isInput = true;
                    else
                        isInput = false;
                }

                bool isPrevConnected = prevPort.myConnection != null;
                bool isThisConnected = port.myConnection != null;


                // Actually add the input/ outputs as necessary.

                ConnectPorts(prevPort, port);

                if (prevNode is CraftingNodeGfx) {
                    if (!isPrevConnected)
                        (prevNode as CraftingNodeGfx).AddPort(isInput);
                    if (!isThisConnected)
                        (node as ItemNodeGfx).AddPort(isInput);
                } else {
                    if (!isThisConnected)
                        (node as CraftingNodeGfx).AddPort(isInput);
                    if (!isPrevConnected)
                        (prevNode as ItemNodeGfx).AddPort(isInput);
                }

                if (isInput) {
                    craft.inputs.Add(new CountedItemNode(item, 1));
                    item.outputIds.Add(craft.id);
                } else {
                    craft.outputs.Add(new CountedItemNode(item, 1));
                    item.inputIds.Add(craft.id);
                }

                if (prevNode is CraftingNodeGfx)
                    (prevNode as CraftingNodeGfx).UpdateVisuals();
                else
                    (node as CraftingNodeGfx).UpdateVisuals();

                prevNode = null;
                prevPort = null;
            } else {
                port.ClickConnectDone();
                bool isDeletion = false;
                if (prevPort) {
                    prevPort.ClickConnectDone();
                    if (prevNode = node) {
                        //If we have selected another port on the same node, delete the port if its not the last port
                        if (prevPort.transform.GetSiblingIndex() + 1 < prevPort.transform.parent.childCount) {
                            prevPort.RemoveConnections();
                            prevPort.DeleteSelf();
                            isDeletion = true;
                        }
                    }
                }

                if (!isDeletion) {
                    prevPort = port;
                    prevNode = node;
                    prevPort.BeginClickConnect();
                } else {
                    prevPort = null;
                    prevNode = null;
                }
            }
        }
    }
    

    public void ConnectPorts(NodePortGfx first, NodePortGfx second) {
        first.AddConnection(second);
        second.AddConnection(first);
    }


    public override void DeleteNode(NodeGfx node) {
        allNodeGfxs.Remove(node);
        if (node.myNode is ItemNode) {
            myRecipeSet.myItemNodes.Remove(node.myNode as ItemNode);
        } else {
            myRecipeSet.myCraftingNodes.Remove(node.myNode as CraftingNode);
        }
    }

    const float changeIncrements = 500;

    public override void RescaleNodeArea() {
        float xmin = float.MaxValue, xmax = float.MinValue, ymin = float.MaxValue, ymax = float.MinValue;
        foreach (var nodeGfx in allNodeGfxs) {
            xmin = Mathf.Min(nodeGfx.GetComponent<RectTransform>().anchoredPosition.x, xmin);
            xmax = Mathf.Max(nodeGfx.GetComponent<RectTransform>().anchoredPosition.x, xmax);
            ymin = Mathf.Min(nodeGfx.GetComponent<RectTransform>().anchoredPosition.y, ymin);
            ymax = Mathf.Max(nodeGfx.GetComponent<RectTransform>().anchoredPosition.y, ymax);
        }

        /*print("min and max values");
        print(xmin);
        print(xmax);
        print(ymin);
        print(ymax);*/

        var rect = NodeAreaInnerRect.rect;
        //var scale = NodeAreaRect.localScale.x;
        var scale = 1;

        float leftSide = -NodeParent.anchoredPosition.x - rect.width / 2;
        float rightSide = -NodeParent.anchoredPosition.x + rect.width / 2;
        float topSide = -NodeParent.anchoredPosition.y + rect.height / 2;
        float bottomSide = -NodeParent.anchoredPosition.y - rect.height / 2;

        /*print("box edges");
        print(leftSide);
        print(rightSide);
        print(topSide);
        print(bottomSide);*/

        //NodeParent.SetParent(canvas);

        bool madeShift = false;
        Vector2 totalShift = Vector2.zero;
        if (xmin < leftSide) {
            NodeAreaRect.sizeDelta = new Vector2(((NodeAreaRect.sizeDelta.x / changeIncrements) + 1) * changeIncrements, NodeAreaRect.sizeDelta.y);
            NodeAreaInnerRect.sizeDelta = new Vector2(((NodeAreaInnerRect.sizeDelta.x / changeIncrements) + 1) * changeIncrements, NodeAreaInnerRect.sizeDelta.y);
            NodeAreaRect.anchoredPosition += new Vector2(-changeIncrements / 2, 0) * scale;
            totalShift += new Vector2(-changeIncrements / 2, 0) * scale;
            madeShift = true;
            print("enlarging to the leftSide");

        } else if (xmin > leftSide + changeIncrements) {
            if (NodeAreaRect.sizeDelta.x > 2500) {
                NodeAreaRect.sizeDelta = new Vector2(((NodeAreaRect.sizeDelta.x / changeIncrements) - 1) * changeIncrements, NodeAreaRect.sizeDelta.y);
                NodeAreaInnerRect.sizeDelta = new Vector2(((NodeAreaInnerRect.sizeDelta.x / changeIncrements) - 1) * changeIncrements, NodeAreaInnerRect.sizeDelta.y);
                NodeAreaRect.anchoredPosition -= new Vector2(-changeIncrements / 2, 0) * scale;
                totalShift -= new Vector2(-changeIncrements / 2, 0) * scale;
                madeShift = true;
                print("reducing to the leftSide");
            }
        }

        if (xmax > rightSide) {
            NodeAreaRect.sizeDelta = new Vector2(((NodeAreaRect.sizeDelta.x / changeIncrements) + 1) * changeIncrements, NodeAreaRect.sizeDelta.y);
            NodeAreaInnerRect.sizeDelta = new Vector2(((NodeAreaInnerRect.sizeDelta.x / changeIncrements) + 1) * changeIncrements, NodeAreaInnerRect.sizeDelta.y);
            NodeAreaRect.anchoredPosition += new Vector2(+changeIncrements / 2, 0) * scale;
            totalShift += new Vector2(+changeIncrements / 2, 0) * scale;
            madeShift = true;
            print("enlarging to the rightSide");

        } else if (xmax < rightSide - changeIncrements) {
            if (NodeAreaRect.sizeDelta.x > 2500) {
                NodeAreaRect.sizeDelta = new Vector2(((NodeAreaRect.sizeDelta.x / changeIncrements) - 1) * changeIncrements, NodeAreaRect.sizeDelta.y);
                NodeAreaInnerRect.sizeDelta = new Vector2(((NodeAreaInnerRect.sizeDelta.x / changeIncrements) - 1) * changeIncrements, NodeAreaInnerRect.sizeDelta.y);
                NodeAreaRect.anchoredPosition -= new Vector2(+changeIncrements / 2, 0) * scale;
                totalShift -= new Vector2(+changeIncrements / 2, 0) * scale;
                madeShift = true;
                print("reducing to the rightSide");
            }
        }

        if (ymin < bottomSide) {
            NodeAreaRect.sizeDelta = new Vector2(NodeAreaRect.sizeDelta.x, ((NodeAreaRect.sizeDelta.y / changeIncrements) + 1) * changeIncrements);
            NodeAreaInnerRect.sizeDelta = new Vector2(NodeAreaInnerRect.sizeDelta.x, ((NodeAreaInnerRect.sizeDelta.y / changeIncrements) + 1) * changeIncrements);
            NodeAreaRect.anchoredPosition += new Vector2(0, -changeIncrements / 2) * scale;
            totalShift += new Vector2(0, -changeIncrements / 2) * scale;
            madeShift = true;
            print("enlarging to the bottomSide");

        } else if (ymin > bottomSide + changeIncrements) {
            if (NodeAreaRect.sizeDelta.y > 2500) {
                NodeAreaRect.sizeDelta = new Vector2(NodeAreaRect.sizeDelta.x, ((NodeAreaRect.sizeDelta.y / changeIncrements) - 1) * changeIncrements);
                NodeAreaInnerRect.sizeDelta = new Vector2(NodeAreaInnerRect.sizeDelta.x, ((NodeAreaInnerRect.sizeDelta.y / changeIncrements) - 1) * changeIncrements);
                NodeAreaRect.anchoredPosition -= new Vector2(0, -changeIncrements / 2) * scale;
                totalShift -= new Vector2(0, -changeIncrements / 2) * scale;
                madeShift = true;
                print("reducing to the bottomSide");
            }
        }

        if (ymax > topSide) {
            NodeAreaRect.sizeDelta = new Vector2(NodeAreaRect.sizeDelta.x, ((NodeAreaRect.sizeDelta.y / changeIncrements) + 1) * changeIncrements);
            NodeAreaInnerRect.sizeDelta = new Vector2(NodeAreaInnerRect.sizeDelta.x, ((NodeAreaInnerRect.sizeDelta.y / changeIncrements) + 1) * changeIncrements);
            NodeAreaRect.anchoredPosition += new Vector2(0, +changeIncrements / 2) * scale;
            totalShift += new Vector2(0, +changeIncrements / 2) * scale;
            madeShift = true;
            print("enlarging to the topSide");

        } else if (ymax < topSide - changeIncrements) {
            if (NodeAreaRect.sizeDelta.y > 2500) {
                NodeAreaRect.sizeDelta = new Vector2(NodeAreaRect.sizeDelta.x, ((NodeAreaRect.sizeDelta.y / changeIncrements) - 1) * changeIncrements);
                NodeAreaInnerRect.sizeDelta = new Vector2(NodeAreaInnerRect.sizeDelta.x, ((NodeAreaInnerRect.sizeDelta.y / changeIncrements) - 1) * changeIncrements);
                NodeAreaRect.anchoredPosition -= new Vector2(0, +changeIncrements / 2) * scale;
                totalShift -= new Vector2(0, +changeIncrements / 2) * scale;
                madeShift = true;
                print("reducing to the topSide");
            }
        }

        NodeParent.anchoredPosition -= totalShift;
        //NodeParent.SetParent(NodeAreaRect);
        if (madeShift) {
            RescaleNodeArea();
        }

        //NodeParent.anchoredPosition = Vector3.zero;
    }

    public void BackToMenu() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

}

//}


public abstract class RecipeTreeMaster : MonoBehaviour {

    public static float xBorderMin = -1f;
    public static float xBorderMax = -1f;
    public static float yBorderMin = -1f;
    public static float yBorderMax = -1f;
    
    public static Camera mainCam;
    public static Canvas myCanvas;
    
    public RecipeSet myRecipeSet;
    
    public RectTransform canvas;
    
    public RectTransform NodeParent;
    public GameObject ItemNodePrefab;
    public GameObject CraftingNodePrefab;
    
    public RectTransform NodeAreaRect;
    public RectTransform NodeAreaInnerRect;

    public abstract void BeginClickConnect(NodeGfx node, NodePortGfx port);
    public abstract void RescaleNodeArea();
    public abstract void DeleteNode(NodeGfx node);
    
    
    public List<NodeGfx> allNodeGfxs = new List<NodeGfx>();
    public NodeGfx GetNodeGfxFromNode(int nodeId) {
        foreach (var nodeGfx in allNodeGfxs) {
            if (nodeGfx.myNode.id == nodeId) {
                return nodeGfx;
            }
        }

        print("not node!");
        return null;
    }

    public RectTransform ViewPort;

    protected void Setup() {
        //print(ViewPort.anchoredPosition);
        //print(ViewPort.rect);
        Rect rect = ViewPort.rect;
        xBorderMin = ViewPort.anchoredPosition.x - rect.width / 2;
        xBorderMax = ViewPort.anchoredPosition.x + rect.width / 2;
        yBorderMin = ViewPort.anchoredPosition.y - rect.height / 2;
        yBorderMax = ViewPort.anchoredPosition.y + rect.height / 2;
    }
}