using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Controls the settings UI panel
/// </summary>
public class GUI_SettingsController : MonoBehaviour
{
    public void ResetData () {
        DataSaver.s.ClearSave();
        DataSaver.s.dontSave = true;
        DataSaver.s.DeleteSave();
        SceneChangeMaster.s.LoadPlanetLevel();
    }
}
