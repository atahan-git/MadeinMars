using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



/// <summary>
/// Controls the settings UI panel
/// </summary>
public class GUI_SettingsController : MonoBehaviour
{
    public void ResetData () {
        DataSaver.mySave = null;
        DataSaver.s.dontSave = true;
        DataSaver.s.DeleteSave();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
