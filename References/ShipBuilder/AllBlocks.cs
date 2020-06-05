using UnityEngine;
using System.Collections;

public class AllBlocks : MonoBehaviour {

	public static AllBlocks blocks;
	public GameObject hull;
	public GameObject gun;

	public enum BlockTypes{
		none,
		hull,
		gun
	}

	void Awake(){

		if(blocks == null){
			DontDestroyOnLoad(gameObject);
			blocks = this;
		}else if(blocks != this){
			Destroy(gameObject);
		}

	}

}

