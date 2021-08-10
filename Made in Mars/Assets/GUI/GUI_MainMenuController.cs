using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Controls the main menu in the main menu scene
/// </summary>
public class GUI_MainMenuController : MonoBehaviour {

    public GameObject tutorialPanel;
    public Toggle tutShowOnStartToggle;

    private void Start() {
        tutShowOnStartToggle.isOn = PlayerPrefs.GetInt("showTutOnStart", 1) == 1;
        if (PlayerPrefs.GetInt("showTutOnStart", 1) == 0) {
            OpenTutorialPanel();
        }
    }

    public void TutorialShowValueChanged() {
        PlayerPrefs.SetInt("showTutOnStart", tutShowOnStartToggle.isOn ? 1 : 0);
    }

    public void StartGame() {
        SceneChangeMaster.s.LoadPlanetLevel();
    }

    public void OpenModRecipes() {
        SceneChangeMaster.s.LoadModRecipesLevel();
    }
    
    public void OpenTutorialPanel() {
        tutorialPanel.SetActive(true);
    }

    public void ResetProgress() {
        DataSaver.s.DeleteSave();
    }
}
