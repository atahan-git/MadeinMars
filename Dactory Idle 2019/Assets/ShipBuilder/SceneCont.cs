using UnityEngine;
using System.Collections;

public class SceneCont : MonoBehaviour {

	public static SceneCont scene;

	public string builder = "ship builder";
	public string level1 = "level 1";

	void Awake(){
		
		if(scene == null){
			DontDestroyOnLoad(gameObject);
			scene = this;
		}else if(scene != this){
			Destroy(gameObject);
		}
		
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//buttons
	public void LoadBuilder(){
		Application.LoadLevel (builder);
	}
	public void LoadLevel1(){
		Application.LoadLevel (level1);
	}
}
