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
public class NodeItemTreeMakerMaster : RecipeTreeViewer {
    public static NodeItemTreeMakerMaster s;

    public float DeadZonePercent = 0.1f;
    public float DeadZone;

    public RectTransform ItemsParent;
    public GameObject ItemListItemPrefab;

    private int lastIdGiven = 0;

    private void Awake() {
        base.Awake();
        s = this;
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
            if (myBuildingData != null) {
                GameObject itemListItem = Instantiate(ItemListItemPrefab, ItemsParent);
                Item buildingTempItem = new Item().MakeBuildingDataDummyItem(myBuildingData.uniqueName, myBuildingData.gfxSprite);
                itemListItem.GetComponent<DragMe>().myItem = buildingTempItem;
                itemListItem.transform.GetChild(0).GetComponent<Image>().sprite = buildingTempItem.mySprite;
                itemListItem.transform.GetChild(1).GetComponent<Text>().text = buildingTempItem.name;
            }
        }

        ReDrawAllNodes();
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
        var position = new Vector3(data.position.x, data.position.y, 0);
        if (myItem.uniqueName == "CraftingProcess") {
            var craftingNode = myRecipeSet.AddCraftingNode(position);
        }else if(myItem.uniqueName == "Research") {
            var researchNode = myRecipeSet.AddResearchNode(position);
        } else {
            var itemNode = myRecipeSet.AddItemNode(position, myItem);
        }
        
        ReDrawAllNodes();
    }

    private NodePortGfx prevPort;
    private NodeGfx prevNode;

    public void BeginClickConnect(NodeGfx node, NodePortGfx port) {

        if (prevNode == null) {
            prevPort = port;
            prevNode = node;
        } else if (prevPort == port) {
            prevNode = null;
            prevPort.ClickConnectDone();
            prevPort = null;
        } else {
            
            if (RecipeSet.ConnectAdapters(prevPort.myAdapterGroup, port.myAdapterGroup)) {

                prevNode = null;
                prevPort = null;
                ReDrawAllNodes();

            } else {
                
                port.ClickConnectDone();
                bool isDeletion = false;
                if (prevPort) {
                    prevPort.ClickConnectDone();
                    if (prevNode == node) {
                        //If we have selected another port on the same node, delete the port if its not the last port
                        if (port.myConnection != null) {
                            myRecipeSet.RemoveConnection(port.myAdapterGroup, port.myConnection);
                        }
                        isDeletion = true;
                    }
                }

                if (!isDeletion) {
                    prevPort = port;
                    prevNode = node;
                    prevPort.BeginClickConnect();
                } else {
                    prevPort = null;
                    prevNode = null;
                    ReDrawAllNodes();
                }
            }
        }
    }

    public void UpdateNodePosition(NodeGfx node, Vector3 pos) {
        node.myNode.pos = pos;
    }

    public void DeleteNode(NodeGfx node) {
        myRecipeSet.RemoveNode(node.myNode);
        ReDrawAllNodes();
    }

    const float changeIncrements = 500;


    public void BackToMenu() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

}