using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// A recipe tree viewer without the editing functionality. Use this variant in game for seeing the relevant recipes.
/// This really needs an optimization pass though. Not this script, but the Unity UI implementation.
/// </summary>
public class RecipeTreeViewer : RecipeTreeMaster {

    private void Awake() {
        Setup();
        myCanvas = canvas.GetComponent<Canvas>();;
        mainCam = Camera.main;
        NodeGfx.snapMultUI = NodeGfx.snapMult * canvas.GetComponent<RectTransform>().localScale.x;
    }

    private void Start() {

        //Draw Nodes
        foreach (var itemNode in myRecipeSet.myItemNodes) {
            //Vector3 pos = itemNode.pos;
            var node = Instantiate(ItemNodePrefab, NodeParent);
            allNodeGfxs.Add(node.GetComponent<NodeGfx>());
            node.GetComponent<ItemNodeGfx>().SetUp(this, itemNode);
            (node.transform as RectTransform).anchoredPosition = itemNode.pos;
            //itemNode.pos = pos;

            Destroy(node.GetComponent<DragPanel>());
            node.AddComponent<NodeFocusCatcher>().Setup(this);
        }

        foreach (var craftingNode in myRecipeSet.myCraftingNodes) {
            //Vector3 pos = craftingNode.pos;
            var node = Instantiate(CraftingNodePrefab, NodeParent);
            allNodeGfxs.Add(node.GetComponent<NodeGfx>());
            node.GetComponent<CraftingNodeGfx>().SetUp(this, craftingNode);
            (node.transform as RectTransform).anchoredPosition = craftingNode.pos;
            //craftingNode.pos = pos;

            Destroy(node.GetComponent<DragPanel>());
            node.AddComponent<NodeFocusCatcher>().Setup(this);
        }

        foreach (var nodeGfx in allNodeGfxs) {
            nodeGfx.SetupPorts();
        }

        foreach (var nodeGfx in allNodeGfxs) {
            nodeGfx.SetupConnections();
        }
        
        foreach (var nodeGfx in allNodeGfxs) {
            ValueRegion[] allVals = nodeGfx.GetComponentsInChildren<ValueRegion>();
            foreach (var valRegion in allVals) {
                valRegion.DisableModifier();
            }
        }
        
        RescaleNodeArea();
        
        foreach (var nodeGfx in allNodeGfxs) {
            nodeGfx.OnDraggingNode();
        }

        Invoke("LateStart",0.5f);
    }

    void LateStart() {
        foreach (var nodeGfx in allNodeGfxs) {
            nodeGfx.OnDraggingNode();
        }
    }
    


    public override void BeginClickConnect(NodeGfx node, NodePortGfx port) {
        port.ClickConnectDone();
    }

    public override void DeleteNode(NodeGfx node) {
        
    }

    public void FocusOnNode(GameObject source) {
        StopAllCoroutines();
        StartCoroutine(FocusOnNodeSmoothTransition(source));
        
    }
    
    IEnumerator FocusOnNodeSmoothTransition(GameObject target) {
        Vector3 targetOffset = -(target.transform.position- target.transform.parent.parent.parent.parent.position);
        Vector3 targetPos = NodeAreaRect.position + targetOffset;
        float timer = 0;
        while (Vector3.Distance(NodeAreaRect.position, targetPos) > 10f) {
            NodeAreaRect.position = Vector3.Lerp(NodeAreaRect.position, targetPos, 10f * Time.deltaTime);
            timer += Time.deltaTime;
            
            if(timer > 1f)
                break;
            yield return null;
        }

        yield break;
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
}
