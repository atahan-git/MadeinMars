using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUI_MainMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    public void StartGame() {
        SceneManager.LoadScene(1);
    }

    public void ResetProgress() {
        DataSaver.DeleteSave(DataSaver.saveName);
    }
}
