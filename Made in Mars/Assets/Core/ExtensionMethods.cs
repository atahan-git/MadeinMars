using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//It is common to create a class to contain all of your
//extension methods. This class must be static.
public static class ExtensionMethods {

    public static void ResetTransformation (this Transform trans) {
        trans.position = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = new Vector3(1, 1, 1);
    }

    public static Vector3 vector3 (this Vector2 v2) {
        return new Vector3(v2.x, v2.y, Position.defaultPositionVector3Z);
    }

    public static Vector2 vector2 (this Vector3 v3) {
        return new Vector2(v3.x, v3.y);
    }

    public static bool isEmpty(this Item o) {
        return o == null || o.uniqueName.Length == 0;
    }

    public static bool isEmpty(this ResearchNode o) {
        return o == null || o.researchUniqueName.Length == 0|| o.researchUniqueName == "rUnnamed";
}
    
    public static List<Position> CoveredPositions(this ArrayLayout layout, Position location) {
        var coveredPositions = new List<Position>();
        for (int y = 0; y < layout.column.Length; y++) {
            for (int x = 0; x < layout.column[y].row.Length; x++) {
                if (layout.column[y].row[x]) {
                    var pos = new Position(x, y) + location - BuildingData.center;
                    //print(new Position(x, y) + center - BuildingData.center);
                    //myTile.itemPlaceable = false;
                    coveredPositions.Add(pos);
                }
            }
        }

        return coveredPositions;
    }

    public static int GetTotalAmountOfItems(this List<InventoryItemSlot> slots) {
        if (slots == null) {
            return 0;
        } else {
            var count = 0;
            foreach (var slot in slots) {
                count += slot.count;
            }

            return count;
        }
    }


    public static void DeleteAllChildren(this Transform transform) {
        int childs = transform.childCount;
        for (int i = childs - 1; i >= 0; i--) {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }
    }
}