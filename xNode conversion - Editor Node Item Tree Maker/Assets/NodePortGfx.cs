using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.UI;

public class NodePortGfx : MonoBehaviour
{

    public enum PortType {
        itemInput, itemOutput, craftInput, craftOutput
    }

    [HideInInspector]
    public NodeGfx myMaster;
    [HideInInspector]
    public PortType myType;

    public GameObject ConnectionInProgressShowObject;
    
    [HideInInspector]
    public NodePortGfx myConnection;
    LineRenderer myRend;

    public Transform lineConnectionPoint;

    private int resolution = 20;

    bool isInput { get { return myType == PortType.itemInput || myType == PortType.craftInput; } }
    bool isCrafting {get { return myType == PortType.craftInput || myType == PortType.craftOutput; }}
    
    public Color craftInColor = Color.blue;
    public Color craftOutColor = Color.red;

    public Image colorChangeImage;

    CubicBezierCurve path = new CubicBezierCurve(new Vector3[4]);
    
    public void Update() {
        if (myRend.enabled) {
            Vector3 startWorldPoint = NodeItemTreeMakerMaster.s.mycam.ScreenToWorldPoint(lineConnectionPoint.position) + Vector3.forward*5;
            Vector3 endWorldPoint = NodeItemTreeMakerMaster.s.mycam.ScreenToWorldPoint(myConnection.lineConnectionPoint.position)+ Vector3.forward*5;
            Vector3[] knots = new[] {
                startWorldPoint, 
                startWorldPoint + (isInput ? Vector3.left : Vector3.right), 
                endWorldPoint + (!isInput ? Vector3.left : Vector3.right),
                endWorldPoint
            };
            
            /*Debug.DrawLine(lineConnectionPoint.position, lineConnectionPoint.position+Vector3.up);
            Debug.DrawLine(lineConnectionPoint.position + (isInput? Vector3.left : Vector3.right), lineConnectionPoint.position + (isInput? Vector3.left : Vector3.right)+Vector3.up);
            Debug.DrawLine(myConnection.lineConnectionPoint.position + (isInput? Vector3.left : Vector3.right), myConnection.lineConnectionPoint.position + (isInput? Vector3.left : Vector3.right)+Vector3.up);
            Debug.DrawLine(myConnection.lineConnectionPoint.position, myConnection.lineConnectionPoint.position+Vector3.up);*/
            
            path.SetControlVerts(knots);
            for (int i = 0; i <= resolution; i++) {
                myRend.SetPosition(i, path.GetPoint(((float)i)/(float)resolution));
            }
        }
    }

    public NodePortGfx Setup(NodeGfx master,PortType type) {
        myMaster = master;
        myType = type;
        
        myRend = GetComponentInChildren<LineRenderer>();
        ConnectionInProgressShowObject.SetActive(false);
        myRend.enabled = false;
        myRend.positionCount = resolution+1;

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

        return this;
    }
    
    public NodePortGfx Setup(NodeGfx master,PortType type, NodePortGfx connection) {
        Setup(master, type);
        
        AddConnection(connection);

        return this;
    }

    public void AddConnection(NodePortGfx target) {
        myConnection = target;
        if(myType == PortType.craftOutput || myType == PortType.craftInput)
            myRend.enabled = true;
        ClickConnectDone();
    }

    public void RemoveConnections() {
        myConnection = null;
        myRend.enabled = false;
    }

    //private bool isDragging = false;
    public void BeginClickConnect() {
        ConnectionInProgressShowObject.SetActive(true);
        myMaster.BeginClickConnect(this);
    }

    public void ClickConnectDone() {
        ConnectionInProgressShowObject.SetActive(false);
    }
    
}
