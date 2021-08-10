using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// A recipe tree viewer. The editor should also use and extend this functionality
/// This really needs an optimization pass though. Not this script, but the Unity UI implementation.
/// </summary>
public class RecipeTreeViewer : MonoBehaviour {
    
    public float xBorderMin = -1f;
    public float xBorderMax = -1f;
    public float yBorderMin = -1f;
    public float yBorderMax = -1f;
    
    public Camera mainCam;
    public Canvas myCanvas;
    
    public RecipeSet myRecipeSet;
    
    public RectTransform canvas;
    
    public RectTransform NodeParent;
    public GameObjectObjectPool ItemNodePool;
    public GameObjectObjectPool craftingNodePool;
    public GameObjectObjectPool researchNodePool;
    public GameObjectObjectPool leftPortPool;
    public GameObjectObjectPool rightPortPool;
    
    
    public RectTransform NodeAreaRect;
    public RectTransform NodeAreaInnerRect;

    public Color[] PortColors;
    public Color[] lineColors;

    public bool isInteractable = false;


    public List<NodeGfx> allNodeGfxs = new List<NodeGfx>();
    /*public NodeGfx GetNodeGfxFromNode(int nodeId) {
        foreach (var nodeGfx in allNodeGfxs) {
            if (nodeGfx.myNode.id == nodeId) {
                return nodeGfx;
            }
        }

        print("not node!");
        return null;
    }*/

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

    protected void Awake() {
        Setup();
        myCanvas = canvas.GetComponent<Canvas>();;
        mainCam = Camera.main;
        NodeGfx.snapMultUI = NodeGfx.snapMult * canvas.GetComponent<RectTransform>().localScale.x;
    }

    private void Start() {
        ReDrawAllNodes();
    }

    protected void ReDrawAllNodes() {
        int childCount = allNodeGfxs.Count;
        myRecipeSet.FixRecipes();

        for (int i = 0; i < childCount; i++) {
            allNodeGfxs[i].GetComponent<PooledGameObject>().DestroyPooledObject();
        }

        allNodeGfxs.Clear();

        //Draw Nodes
        foreach (var itemNode in myRecipeSet.GetItemNodes()) {
            //Vector3 pos = itemNode.pos;
            var node = ItemNodePool.Spawn().gameObject;
            allNodeGfxs.Add(node.GetComponent<NodeGfx>());
            node.GetComponent<ItemNodeGfx>().ReDrawnNode(this, itemNode, isInteractable);
            (node.transform as RectTransform).anchoredPosition = itemNode.pos;
            //itemNode.pos = pos;
            if (node.GetComponent<NodeFocusCatcher>()) {
                Destroy(node.GetComponent<NodeFocusCatcher>());
            }
            node.AddComponent<NodeFocusCatcher>().Setup(this);
        }

        foreach (var craftingNode in myRecipeSet.GetCraftingNodes()) {
            //Vector3 pos = craftingNode.pos;
            var node = craftingNodePool.Spawn().gameObject;
            allNodeGfxs.Add(node.GetComponent<NodeGfx>());
            node.GetComponent<CraftingNodeGfx>().ReDrawnNode(this, craftingNode, isInteractable);
            (node.transform as RectTransform).anchoredPosition = craftingNode.pos;
            //craftingNode.pos = pos;
            
            if (node.GetComponent<NodeFocusCatcher>()) {
                Destroy(node.GetComponent<NodeFocusCatcher>());
            }
            node.AddComponent<NodeFocusCatcher>().Setup(this);
        }
        
        foreach (var researchNode in myRecipeSet.getResearchNodes()) {
            //Vector3 pos = craftingNode.pos;
            var node = researchNodePool.Spawn().gameObject;
            allNodeGfxs.Add(node.GetComponent<NodeGfx>());
            node.GetComponent<ResearchNodeGfx>().ReDrawnNode(this, researchNode, isInteractable);
            (node.transform as RectTransform).anchoredPosition = researchNode.pos;
            //craftingNode.pos = pos;
            
            if (node.GetComponent<NodeFocusCatcher>()) {
                Destroy(node.GetComponent<NodeFocusCatcher>());
            }
            node.AddComponent<NodeFocusCatcher>().Setup(this);
        }
        
        RescaleNodeArea();

        foreach (var node in allNodeGfxs) {
            node.ReDrawConnections();
        }
        
        Invoke("LateRedraw",0.5f);

#if UNITY_EDITOR
        EditorUtility.SetDirty(myRecipeSet);
#endif
    }

    void LateRedraw() {
        foreach (var node in allNodeGfxs) {
            foreach (var port in node.allPorts) {
                port.OnPositionUpdated();
            }
        }
    }

    public NodeGfx GetNodeWithId(int id) {
        foreach (var node in allNodeGfxs) {
            if (node.myNode.id == id) {
                return node;
            }
        }

        return null;
    }
    

    public void FocusOnNode(GameObject source) {
        StopAllCoroutines();
        StartCoroutine(FocusOnNodeSmoothTransition(source));
    }

    public float NodeFocusSpeed = 2f;
    
    IEnumerator FocusOnNodeSmoothTransition(GameObject target) {
        Vector3 targetOffset = -(target.transform.position- target.transform.parent.parent.parent.parent.position);
        Vector3 targetPos = NodeAreaRect.position + targetOffset;
        float timer = 0;
        while (Vector3.Distance(NodeAreaRect.position, targetPos) > 10f) {
            NodeAreaRect.position = Vector3.Lerp(NodeAreaRect.position, targetPos, NodeFocusSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            
            if(timer > 1f)
                break;
            yield return null;
        }

        yield break;
    }

    const float changeIncrements = 1000;

    public void RescaleNodeArea() {
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
            //print("enlarging to the leftSide");

        } else if (xmin > leftSide + changeIncrements) {
            if (NodeAreaRect.sizeDelta.x > 2500) {
                NodeAreaRect.sizeDelta = new Vector2(((NodeAreaRect.sizeDelta.x / changeIncrements) - 1) * changeIncrements, NodeAreaRect.sizeDelta.y);
                NodeAreaInnerRect.sizeDelta = new Vector2(((NodeAreaInnerRect.sizeDelta.x / changeIncrements) - 1) * changeIncrements, NodeAreaInnerRect.sizeDelta.y);
                NodeAreaRect.anchoredPosition -= new Vector2(-changeIncrements / 2, 0) * scale;
                totalShift -= new Vector2(-changeIncrements / 2, 0) * scale;
                madeShift = true;
                //print("reducing to the leftSide");
            }
        }

        if (xmax > rightSide) {
            NodeAreaRect.sizeDelta = new Vector2(((NodeAreaRect.sizeDelta.x / changeIncrements) + 1) * changeIncrements, NodeAreaRect.sizeDelta.y);
            NodeAreaInnerRect.sizeDelta = new Vector2(((NodeAreaInnerRect.sizeDelta.x / changeIncrements) + 1) * changeIncrements, NodeAreaInnerRect.sizeDelta.y);
            NodeAreaRect.anchoredPosition += new Vector2(+changeIncrements / 2, 0) * scale;
            totalShift += new Vector2(+changeIncrements / 2, 0) * scale;
            madeShift = true;
            //print("enlarging to the rightSide");

        } else if (xmax < rightSide - changeIncrements) {
            if (NodeAreaRect.sizeDelta.x > 2500) {
                NodeAreaRect.sizeDelta = new Vector2(((NodeAreaRect.sizeDelta.x / changeIncrements) - 1) * changeIncrements, NodeAreaRect.sizeDelta.y);
                NodeAreaInnerRect.sizeDelta = new Vector2(((NodeAreaInnerRect.sizeDelta.x / changeIncrements) - 1) * changeIncrements, NodeAreaInnerRect.sizeDelta.y);
                NodeAreaRect.anchoredPosition -= new Vector2(+changeIncrements / 2, 0) * scale;
                totalShift -= new Vector2(+changeIncrements / 2, 0) * scale;
                madeShift = true;
                //print("reducing to the rightSide");
            }
        }

        if (ymin < bottomSide) {
            NodeAreaRect.sizeDelta = new Vector2(NodeAreaRect.sizeDelta.x, ((NodeAreaRect.sizeDelta.y / changeIncrements) + 1) * changeIncrements);
            NodeAreaInnerRect.sizeDelta = new Vector2(NodeAreaInnerRect.sizeDelta.x, ((NodeAreaInnerRect.sizeDelta.y / changeIncrements) + 1) * changeIncrements);
            NodeAreaRect.anchoredPosition += new Vector2(0, -changeIncrements / 2) * scale;
            totalShift += new Vector2(0, -changeIncrements / 2) * scale;
            madeShift = true;
            //print("enlarging to the bottomSide");

        } else if (ymin > bottomSide + changeIncrements) {
            if (NodeAreaRect.sizeDelta.y > 2500) {
                NodeAreaRect.sizeDelta = new Vector2(NodeAreaRect.sizeDelta.x, ((NodeAreaRect.sizeDelta.y / changeIncrements) - 1) * changeIncrements);
                NodeAreaInnerRect.sizeDelta = new Vector2(NodeAreaInnerRect.sizeDelta.x, ((NodeAreaInnerRect.sizeDelta.y / changeIncrements) - 1) * changeIncrements);
                NodeAreaRect.anchoredPosition -= new Vector2(0, -changeIncrements / 2) * scale;
                totalShift -= new Vector2(0, -changeIncrements / 2) * scale;
                madeShift = true;
                //print("reducing to the bottomSide");
            }
        }

        if (ymax > topSide) {
            NodeAreaRect.sizeDelta = new Vector2(NodeAreaRect.sizeDelta.x, ((NodeAreaRect.sizeDelta.y / changeIncrements) + 1) * changeIncrements);
            NodeAreaInnerRect.sizeDelta = new Vector2(NodeAreaInnerRect.sizeDelta.x, ((NodeAreaInnerRect.sizeDelta.y / changeIncrements) + 1) * changeIncrements);
            NodeAreaRect.anchoredPosition += new Vector2(0, +changeIncrements / 2) * scale;
            totalShift += new Vector2(0, +changeIncrements / 2) * scale;
            madeShift = true;
            //print("enlarging to the topSide");

        } else if (ymax < topSide - changeIncrements) {
            if (NodeAreaRect.sizeDelta.y > 2500) {
                NodeAreaRect.sizeDelta = new Vector2(NodeAreaRect.sizeDelta.x, ((NodeAreaRect.sizeDelta.y / changeIncrements) - 1) * changeIncrements);
                NodeAreaInnerRect.sizeDelta = new Vector2(NodeAreaInnerRect.sizeDelta.x, ((NodeAreaInnerRect.sizeDelta.y / changeIncrements) - 1) * changeIncrements);
                NodeAreaRect.anchoredPosition -= new Vector2(0, +changeIncrements / 2) * scale;
                totalShift -= new Vector2(0, +changeIncrements / 2) * scale;
                madeShift = true;
                //print("reducing to the topSide");
            }
        }

        NodeParent.anchoredPosition -= totalShift;
        //NodeParent.SetParent(NodeAreaRect);
        if (madeShift) {
            RescaleNodeArea();
        }

        //NodeParent.anchoredPosition = Vector3.zero;
    }

    public Item GetItemOfNode(ItemNode node) {
        var item = myRecipeSet.GetItem(node.itemUniqueName);


        if (item == null) {
            var building = myRecipeSet.GetBuilding(node.itemUniqueName);
            if (building != null) {
                item = new Item();
                item.uniqueName = building.uniqueName;
                item.mySprite = building.gfxSprite;
            } else {
                var shipCard = myRecipeSet.GetShipCard(node.itemUniqueName);
                if (shipCard != null) {
                    item = new Item();
                    item.uniqueName = shipCard.uniqueName;
                    item.mySprite = shipCard.GetImage();
                }
            }
        }

        return item;
    }
}
