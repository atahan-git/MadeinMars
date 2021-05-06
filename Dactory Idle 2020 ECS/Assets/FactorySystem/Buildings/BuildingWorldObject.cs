using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// The main building object. Should work to store data for the other components, and deal with placing/removing the building.
/// </summary>
public class BuildingWorldObject : MonoBehaviour, IBuildable
{

	public Building myBuilding;
	public BuildingData myData;
	public Position myPos;
	public List<Position> myPositions;
	public List<TileData> myTiles;
	public BuildingCraftingController myCrafter;
	public BuildingInventoryController myInventory;

	SpriteGraphicsController myRend;

	public bool isBuilt = false;

	public GenericCallback buildingBuiltCallback;
	
	public void PlaceInWorld (BuildingData _myData, Position _location, List<Position> _myPositions, bool isSpaceLanding,
		bool isInventory, List<InventoryItemSlot> inventory, bool _isBuild) {
		_PlaceInWorld(_myData, _location, _myPositions, isSpaceLanding, isInventory, inventory, _isBuild);
	}

	public float width;
	public float height;

	void _PlaceInWorld(BuildingData _myData, Position _location, List<Position> _myPositions, bool isSpaceLanding, 
		bool isInventory, List<InventoryItemSlot> inventory, bool _isBuild) {

		myData = _myData;
		myPos = _location;
		myPositions = _myPositions;
		myTiles = new List<TileData>();
		isBuilt = _isBuild;
		
		foreach (Position myPosition in myPositions) {
			if (myPosition != null) {
				var myTile = Grid.s.GetTile(myPosition);
				myTile.worldObject = gameObject;
				myTiles.Add(myTile);
			}
		}

		width = myData.shape.width;
		height = myData.shape.height;
		Vector3 centerOffset = new Vector3(0.5f, myData.shape.maxHeightFromCenter - 1, 0);

		myRend = GetComponentInChildren<SpriteGraphicsController>();
		myRend.transform.localPosition = myData.spriteOffset.vector3() - centerOffset;

		switch (myData.gfxType) {
			case BuildingData.BuildingGfxType.SpriteBased:
				myRend.SetGraphics(myData.gfxSprite, myData.gfxShadowSprite != null ? myData.gfxShadowSprite : myData.gfxSprite);
				break;
			case BuildingData.BuildingGfxType.AnimationBased:
				myRend.SetGraphics(myData.gfxSpriteAnimation, myData.isAnimatedShadow);

				break;
			case BuildingData.BuildingGfxType.PrefabBased:
				myRend.SetGraphics(myData.gfxPrefab);
				break;
		}
		myRend.SetBuildState(false);
		StopAnimationsForced(true);

		DataSaver.saveEvent += SaveYourself;
		transform.position = _location.Vector3(Position.Type.building) + centerOffset;


		if (isSpaceLanding)
			GetComponentInChildren<SpriteGraphicsController>().DoSpaceLanding(null);
		
		if (isBuilt)
			CompleteBuilding();
	}

	
	public void CompleteBuilding() {
		myBuilding = FactorySystem.s.CreateBuilding(myData,myPositions);


		myCrafter = myBuilding.craftController;
		myInventory = myBuilding.invController;

		if (myCrafter != null) {
			myCrafter.continueAnimationsEvent += ContinueAnimations;
			myCrafter.stopAnimationsEvent += StopAnimations;
		}
		

		isBuilt = true;
		
		foreach (Position myPosition in myPositions) {
			if (myPosition != null) {
				var myTile = Grid.s.GetTile(myPosition);
				myTile.worldObject = gameObject;
				myTile.objectUpdatedCallback += TileUpdated;
			}
		}
		
		// This is called by TileUpdated Instead
		//myRend.SetGraphics(FactoryVisuals.s.beltSprites[myBelt.direction]);
		myRend.SetBuildState(true);
		buildingBuiltCallback?.Invoke();
	}

	void SaveYourself () {
		DataSaver.ItemsToBeSaved.Add(new DataSaver.BuildingSaveData(myData.uniqueName, myPos, isBuilt));
	}

	void TileUpdated() {
		foreach (Position myPosition in myPositions) {
			if (myPosition != null) {
				var myTile = Grid.s.GetTile(myPosition);
				if (myTile.areThereBuilding) {
					myBuilding = myTile.myBuilding;
				} else {
					DestroyYourself();
				}
			}
		}
	}
	
	void OnDestroy () {
		DataSaver.saveEvent -= SaveYourself;
	}

	public void DestroyYourself () {
		foreach (Position myPosition in myPositions) {
			if (myPosition != null) {
				var myTile = Grid.s.GetTile(myPosition);
				myTile.worldObject = null;
			}
		}
		
		foreach (Position myPosition in myPositions) {
			if (myPosition != null) {
				var myTile = Grid.s.GetTile(myPosition);
				myTile.objectUpdatedCallback -= TileUpdated;
			}
		}
		
		FactorySystem.s.RemoveBuilding(myBuilding);
		DroneSystem.s.RemoveDroneTask(myPositions[0]);
		
		Destroy(gameObject);
	}

	public bool animationState = true;
    public AnimatedSpriteController[] anims = new AnimatedSpriteController[0];
    public bool isAnimated = true;
    public ParticleSystem[] particles = new ParticleSystem[0];
    public bool isParticled = true;
    
    bool GetAnims() {
        if (anims.Length <= 0) {
            anims = GetComponentsInChildren<AnimatedSpriteController>();
        }

        if (anims.Length <= 0) {
            isAnimated = false;
            return false;
        } else
            return true;
    }
    
    bool GetParticles() {
        if (particles.Length <= 0) {
            particles = GetComponentsInChildren<ParticleSystem>();
        }

        if (particles.Length <= 0) {
            isParticled = false;
            return false;
        } else
            return true;
    }

    void ContinueAnimations() {
	    if (isAnimated) {
		    if (!animationState) {
			    if (GetAnims()) {
				    for (int i = 0; i < anims.Length; i++) {
					    anims[i].Play();
				    }

				    animationState = true;
			    }

			    if (isParticled) {
				    if (GetParticles()) {
					    for (int i = 0; i < particles.Length; i++) {
						    particles[i].Play();
					    }
				    }
			    }
		    }
	    }
    }


    void StopAnimations() {
	    StopAnimationsForced(false);
    }
    
    void StopAnimationsForced(bool isForced) {
        if (isAnimated) {
            if (animationState) {
                if (GetAnims()) {
	                for (int i = 0; i < anims.Length; i++) {
		                if (isForced)
			                anims[i].Stop();
		                else
			                anims[i].SmoothStop();
	                }

	                animationState = false;
                }

                if (isParticled) {
                    if (GetParticles()) {
                        for (int i = 0; i < particles.Length; i++) {
                            particles[i].Stop();
                        }
                    }
                }
            }
        }
    }
	
}
