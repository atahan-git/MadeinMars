using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneChangeMaster : MonoBehaviour {
    public static SceneChangeMaster s;
    
    [SerializeField] private SceneReference menu;
    [SerializeField] private SceneReference planetLevel;
    [SerializeField] private SceneReference starsLevel;
    [SerializeField] private SceneReference modRecipesLevel;
    
    public void Awake () {
        if (s != null) {
            Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
        }
        s = this;
    }


    public void OnDestroy() {
        s = null;
    }

    public void LoadMenu() {
        LoadScene(menu);
    }

    public void LoadPlanetLevel() {
        GameMaster.currentState = GameMaster.GameState.planet;
        LoadScene(planetLevel);
    }

    public void LoadStarsLevel() {
        GameMaster.currentState = GameMaster.GameState.stars;
        LoadScene(starsLevel);
    }

    public void LoadModRecipesLevel() {
        LoadScene(modRecipesLevel);
    }


    private void LoadScene(SceneReference sceneReference) {
        SceneManager.LoadSceneAsync(sceneReference);
    }
}
