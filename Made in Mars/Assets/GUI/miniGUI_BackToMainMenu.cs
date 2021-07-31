using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class miniGUI_BackToMainMenu : MonoBehaviour {
   public void BackToMainMenu() {
      SceneChangeMaster.s.LoadMenu();
   }
}
