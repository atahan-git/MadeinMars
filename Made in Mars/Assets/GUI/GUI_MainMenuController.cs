using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Controls the main menu in the main menu scene
/// </summary>
public class GUI_MainMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    public void StartGame() {
        SceneChangeMaster.s.LoadPlanetLevel();
    }

    public void OpenModRecipes() {
        SceneChangeMaster.s.LoadModRecipesLevel();
    }

    public void ResetProgress() {
        DataSaver.s.DeleteSave();
    }
}
