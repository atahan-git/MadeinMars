using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.UI;
using UnityEngine.UI.Extensions;


/// <summary>
/// This is a helper method for the graphics of the node "ports", the square things you connect between each of the nodes.
/// </summary>
public class NodePortGfx : MonoBehaviour {

    public ValueRegion myRegion;

    //[HideInInspector]
    public NodeGfx myMaster;

    public AdapterGroup myAdapterGroup;
    public AdapterGroup.AdapterConnection myConnection;

    public GameObject ConnectionInProgressShowObject;
    
    //[HideInInspector]
    public NodePortGfx connectedPortGfx;
    UILineRenderer myRend;

    public Transform lineConnectionPoint;

    private int resolution = 20;

    public Image colorChangeImage;

    CubicBezierCurve path = new CubicBezierCurve(new Vector3[4]);

    private bool pathActive = false;
    
    public void OnPositionUpdated() {
        if (pathActive) {
            float canvasScale = Screen.width / myMaster.myViewer.myCanvas.GetComponent<CanvasScaler>().referenceResolution.x;
            //canvasScale = 1;
            //canvasScale = 1f / canvasScale;
            float localScale = 1/myRend.transform.lossyScale.x;
            //print(canvasScale);
            Vector3 startWorldPoint = /*RecipeTreeMaster.mainCam.ScreenToWorldPoint*/(lineConnectionPoint.position-myRend.transform.position)*localScale /*+ Vector3.forward*5*/ ;
            Vector3 endWorldPoint = /*RecipeTreeMaster.mainCam.ScreenToWorldPoint*/(connectedPortGfx.lineConnectionPoint.position-myRend.transform.position)*localScale /*+ Vector3.forward*5*/;
            float pushDistance = Mathf.Abs(Vector3.Distance(startWorldPoint,endWorldPoint))/2f;
            //pushDistance = Mathf.Max(pushDistance, 50f*localScale);
            Vector3[] knots = new[] {
                startWorldPoint, 
                startWorldPoint + (myAdapterGroup.isLeftAdapter ? Vector3.left : Vector3.right) *pushDistance, 
                endWorldPoint + (!myAdapterGroup.isLeftAdapter ? Vector3.left : Vector3.right)*pushDistance,
                endWorldPoint
            };

            resolution = Mathf.CeilToInt(pushDistance / 7f);
            
            /*Debug.DrawLine(lineConnectionPoint.position, lineConnectionPoint.position+Vector3.up);
            Debug.DrawLine(lineConnectionPoint.position + (isInput? Vector3.left : Vector3.right), lineConnectionPoint.position + (isInput? Vector3.left : Vector3.right)+Vector3.up);
            Debug.DrawLine(connectedPortGfx.lineConnectionPoint.position + (isInput? Vector3.left : Vector3.right), connectedPortGfx.lineConnectionPoint.position + (isInput? Vector3.left : Vector3.right)+Vector3.up);
            Debug.DrawLine(connectedPortGfx.lineConnectionPoint.position, connectedPortGfx.lineConnectionPoint.position+Vector3.up);*/
            
            path.SetControlVerts(knots);
            
            var points = new Vector2[resolution+1];
            for (int i = 0; i <= resolution; i++) {
                Vector3 pos = path.GetPoint(((float) i) / (float) resolution);
                points[i] = new Vector2(pos.x,pos.y);
            }

            myRend.Points = points;
            myRend.SetAllDirty();
        }
    }

    public NodePortGfx Setup(NodeGfx master, AdapterGroup adapterGroup,  AdapterGroup.AdapterConnection connection, int value) {
        myMaster = master;
        myAdapterGroup = adapterGroup;
        myConnection = connection;
        
        myRend = GetComponentInChildren<UILineRenderer>();
        ConnectionInProgressShowObject.SetActive(false);
        myRend.enabled = false;
        pathActive = false;
        myRend.Points = new Vector2[resolution+1];

        if (myAdapterGroup.type >= 0 && myAdapterGroup.type < myMaster.myViewer.PortColors.Length) {
            colorChangeImage.color = myMaster.myViewer.PortColors[myAdapterGroup.type];
            myRend.color = myMaster.myViewer.lineColors[myAdapterGroup.type];
        } else {
            colorChangeImage.color = Color.white;
            myRend.color = Color.white;
        }

        if (myRegion) {
            myRegion.SetUp(
                ValueRegion.type.port, 
                adapterGroup.myType == AdapterGroup.AdapterType.counted, 
                adapterGroup.isLeftAdapter ? "Ingredient" : "Result", 
                null, this, value);
        }

        return this;
    }

    //private bool isDragging = false;
    public void BeginClickConnect() {
        ConnectionInProgressShowObject.SetActive(true);
        myMaster.BeginClickConnect(this);
    }

    public void ClickConnectDone() {
        ConnectionInProgressShowObject.SetActive(false);
    }

    public void SetConnection(NodePortGfx target) {
        connectedPortGfx = target;
        if (myAdapterGroup.isLeftAdapter) {
            myRend.enabled = true;
            pathActive = true;
            OnPositionUpdated();
        }
    }


    public void ValueUpdated(int value) {
        myMaster.AdapterConnectionValueUpdated(myConnection, value); // Only the crafting node has updateable value regions, so we can assume this
    }
}
