using UnityEngine;
using System.Collections;

public class PlacedItemBaseScript : MonoBehaviour {

	public int x = 0;
	public int y = 0;
	public int type = 0;

	public int n_out = 0;
	public int n_in = 0;

	public TileBaseScript[] tilesCovered = new TileBaseScript[99];
	public int n_cover = 0;

	public GameObject movingItem;


	public bool shouldGiveItem = false;
	// Use this for initialization
	void Start () {
		//BeltPulseControl.secondPulse += SecondPulse;
		DataSaver.saveEvent += SaveYourself;
	}
	
	// Update is called once per frame
	public void DestroyYourself () {
		//foreach (TileBaseScript myTile in tilesCovered) {
		//	if (myTile != null)
		//		myTile.areThereItem = false;
		//}
		Destroy (gameObject);
	}

	public bool TryAccepting (MovingObject myObject, Vector3 offset) {
		//check if we can accept it
		//if yes store it and stuff
		//if not return false
		myObject.SelfDestruct(offset);
		return true;
	}

	
	void SaveYourself () {
		//DataSaver.ItemsToBeSaved[DataSaver.n] = new DataSaver.BuildingSaveData(type, x, y);
		//DataSaver.n++;
	}

	void OnDestroy () {
		DataSaver.saveEvent -= SaveYourself;
	}

	/*public void SecondPulse(){
		foreach (BeltScript bS in outConveyors) {
			if (bS != null) {
				for (int i = 0; i <= 3; i++) {					//if not find any other
					if (bS.inputStorage [i] != null) {
						if (bS.inputStorage [i].myItem == null) {
							bS.inputStorage [i].myItem = (GameObject)Instantiate(movingItem, transform.position, transform.rotation);
							bS.UpdateItemLocations ();
						}
					}
				}
			}
		}
	}*/
}
