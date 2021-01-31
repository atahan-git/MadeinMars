using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class NodePortGfx : MonoBehaviour {

    public ValueRegion myRegion;
    
    public enum PortType {
        itemInput, itemOutput, craftInput, craftOutput
    }

    //[HideInInspector]
    public NodeGfx myMaster;

    public int myIndex {
        get { return transform.GetSiblingIndex(); }
    }

    //[HideInInspector]
    public PortType myType;

    public GameObject ConnectionInProgressShowObject;
    
    [HideInInspector]
    public NodePortGfx myConnection;
    UILineRenderer myRend;

    public Transform lineConnectionPoint;

    private int resolution = 20;

    bool isInput { get { return myType == PortType.itemInput || myType == PortType.craftInput; } }
    bool isCrafting {get { return myType == PortType.craftInput || myType == PortType.craftOutput; }}
    
    public Color craftInColor = Color.blue;
    public Color craftOutColor = Color.red;

    public Image colorChangeImage;

    CubicBezierCurve path = new CubicBezierCurve(new Vector3[4]);

    private bool pathActive = false;
    public void Update() {
        if (pathActive) {
            float canvasScale = Screen.width / RecipeTreeMaster.myCanvas.GetComponent<CanvasScaler>().referenceResolution.x;
            //canvasScale = 1;
            //canvasScale = 1f / canvasScale;
            float localScale = 1/myRend.transform.lossyScale.x;
            //print(canvasScale);
            Vector3 startWorldPoint = /*RecipeTreeMaster.mainCam.ScreenToWorldPoint*/(lineConnectionPoint.position-myRend.transform.position)*localScale /*+ Vector3.forward*5*/ ;
            Vector3 endWorldPoint = /*RecipeTreeMaster.mainCam.ScreenToWorldPoint*/(myConnection.lineConnectionPoint.position-myRend.transform.position)*localScale /*+ Vector3.forward*5*/;
            float pushDistance = Mathf.Abs(startWorldPoint.x - endWorldPoint.x)/2f;
            Vector3[] knots = new[] {
                startWorldPoint, 
                startWorldPoint + (isInput ? Vector3.left : Vector3.right) *pushDistance, 
                endWorldPoint + (!isInput ? Vector3.left : Vector3.right)*pushDistance,
                endWorldPoint
            };
            
            /*Debug.DrawLine(lineConnectionPoint.position, lineConnectionPoint.position+Vector3.up);
            Debug.DrawLine(lineConnectionPoint.position + (isInput? Vector3.left : Vector3.right), lineConnectionPoint.position + (isInput? Vector3.left : Vector3.right)+Vector3.up);
            Debug.DrawLine(myConnection.lineConnectionPoint.position + (isInput? Vector3.left : Vector3.right), myConnection.lineConnectionPoint.position + (isInput? Vector3.left : Vector3.right)+Vector3.up);
            Debug.DrawLine(myConnection.lineConnectionPoint.position, myConnection.lineConnectionPoint.position+Vector3.up);*/
            
            path.SetControlVerts(knots);
            for (int i = 0; i <= resolution; i++) {
                Vector3 pos = path.GetPoint(((float) i) / (float) resolution);
                myRend.m_points[i] = new Vector2(pos.x,pos.y);
            }
            myRend.SetAllDirty();
        }
    }

    public NodePortGfx Setup(NodeGfx master, PortType type, int value) {
        myMaster = master;
        myType = type;
        
        myRend = GetComponentInChildren<UILineRenderer>();
        ConnectionInProgressShowObject.SetActive(false);
        myRend.enabled = false;
        pathActive = false;
        myRend.m_points = new Vector2[resolution+1];

        if (isCrafting) {
            if (isInput) {
                colorChangeImage.color = craftInColor;
            } else {
                colorChangeImage.color = craftOutColor;
            }
        } else {
            if (!isInput) {
                colorChangeImage.color = craftInColor;
            } else {
                colorChangeImage.color = craftOutColor;
            }
        }

        if (myRegion) {
            switch (myType) {
                case PortType.itemInput:
                    myRegion.SetUp(ValueRegion.type.itemInput,null,this, value);
                    break;
                case PortType.itemOutput:
                    myRegion.SetUp(ValueRegion.type.itemOutput,null,this, value);
                    break;
                case PortType.craftInput:
                    myRegion.SetUp(ValueRegion.type.craftInput,null,this, value);
                    break;
                case PortType.craftOutput:
                    myRegion.SetUp(ValueRegion.type.craftOutput,null,this, value);
                    break;
            }
        }

        return this;
    }
    
    public NodePortGfx Setup(NodeGfx master,PortType type, int value, NodePortGfx connection) {
        Setup(master, type, value);
        
        AddConnection(connection);

        return this;
    }

    public void AddConnection(NodePortGfx target) {
        RemoveConnections();
        myConnection = target;
        if (myType == PortType.craftOutput || myType == PortType.craftInput) {
            myRend.enabled = true;
            pathActive = true;
        }

        ClickConnectDone();
    }

    public void DeleteSelf() {
        if(myConnection != null)
            myMaster.RemoveConnectionAtPort(myType, myIndex);
        Destroy(gameObject);
    }

    public void RemoveConnections() {
        if (myConnection != null) {
            myConnection.DeleteSelf();
            myMaster.RemoveConnectionAtPort(myType, myIndex);
        }
        myConnection = null;
        myRend.enabled = false;
        pathActive = false;
    }

    //private bool isDragging = false;
    public void BeginClickConnect() {
        ConnectionInProgressShowObject.SetActive(true);
        myMaster.BeginClickConnect(this);
    }

    public void ClickConnectDone() {
        ConnectionInProgressShowObject.SetActive(false);
    }


    public void ValueUpdated(ValueRegion.type type, int value) {
        (myMaster as CraftingNodeGfx).ValueUpdated(type, value, myIndex); // Only the crafting node has updateable value regions, so we can assume this
    }
}
