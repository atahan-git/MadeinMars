using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    public void LoadGame () {
        if (DataSaver.s.Load()) {
            CreateBelts();
            CreateBuildings();
        }
    }

    void CreateBuildings () {
        foreach (DataSaver.BuildingSaveData building in DataSaver.mySave.buildingData) {
            if(building != null)
                ObjectBuilderMaster.BuildObjectFromSave(building.myUniqueName, building.myPos);
        }
    }

    void CreateBelts () {
        foreach (DataSaver.BeltData belt in DataSaver.mySave.beltData) {
            if (belt != null)
                ObjectBuilderMaster.BuildBeltFromSave(belt.inLocations, belt.outLocations, belt.myPos, belt.isBuildingBelt);
        }
    }
}
