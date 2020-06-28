using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{

    public static bool isGameLoadingDone = false;
    public delegate void LoadYourself ();
    public static event LoadYourself loadEvent;

    public void LoadGame () {
        if (DataSaver.s.Load()) {
            CreateBelts();
            CreateBuildings();
            loadEvent?.Invoke();
        }

        isGameLoadingDone = true;
    }


    void CreateBelts () {
        foreach (DataSaver.BeltData belt in DataSaver.mySave.beltData) {
            if (belt != null)
                ObjectBuilderMaster.BuildBeltFromSave(belt.inLocations, belt.outLocations, belt.myPos, belt.isBuildingBelt);
        }
    }
    void CreateBuildings () {
        foreach (DataSaver.BuildingSaveData building in DataSaver.mySave.buildingData) {
            if(building != null)
                ObjectBuilderMaster.BuildObjectFromSave(building.myUniqueName, building.myPos);
        }
    }
}
