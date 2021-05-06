using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



/// <summary>
/// Controls the main menu in the main menu scene
/// </summary>
public class GUI_MainMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    public void StartGame() {
        SceneManager.LoadScene(1);
    }

    public void OpenModRecipes() {
        SceneManager.LoadScene(2);
    }

    public void ResetProgress() {
        DataSaver.DeleteSave(DataSaver.saveName);
    }
}
