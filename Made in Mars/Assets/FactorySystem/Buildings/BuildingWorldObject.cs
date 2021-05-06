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

	public GenericCallback buildingInventorySetUpCallback;
	

	public float width;
	public float height;

	public void PlaceInWorld(BuildingData _myData, Position _location, List<Position> _myPositions, bool isSpaceLanding, 
		List<InventoryItemSlot> inventory, bool _isBuild) {

		myData = _myData;
		myPos = _location;
		myPositions = _myPositions;
		myTiles = new List<TileData>();
		isBuilt = _isBuild;

		CreateConstructionInventory(inventory);

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
		myRend.SetBuildState(SpriteGraphicsController.BuildState.construction);
		StopAnimationsForced(true);

		DataSaver.saveEvent += SaveYourself;
		transform.position = _location.Vector3(Position.Type.building) + centerOffset;


		if (isSpaceLanding)
			GetComponentInChildren<SpriteGraphicsController>().DoSpaceLanding(null);
		
		if (isBuilt)
			CompleteBuilding();
		
		buildingInventorySetUpCallback?.Invoke();
	}
	
	public BuildingInventoryController GetConstructionInventory() {
		return myInventory;
	}
	public BuildingInventoryController CreateConstructionInventory(List<InventoryItemSlot> inventory) {
		myInventory = new BuildingInventoryController();
		myInventory.SetUpConstruction(myPos);
		myInventory.SetInventory(inventory);

		return myInventory;
	}
	
	public void CompleteBuilding() {
		myBuilding = FactorySystem.s.CreateBuilding(myData,myPositions, myInventory);

		myCrafter = myBuilding.craftController;

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
		
		
		myRend.SetBuildState(SpriteGraphicsController.BuildState.built);
		buildingInventorySetUpCallback?.Invoke();
	}

	void SaveYourself () {
		DataSaver.ItemsToBeSaved.Add(new DataSaver.BuildingSaveData(myData.uniqueName, myPos, isBuilt, myInventory.inventory));
	}

	void TileUpdated() {
		foreach (Position myPosition in myPositions) {
			if (myPosition != null) {
				var myTile = Grid.s.GetTile(myPosition);
				if (myTile.areThereBuilding) {
					myBuilding = myTile.myBuilding;
				} else {
					MarkForDeconstruction();
				}
			}
		}
	}
	
	void OnDestroy () {
		DataSaver.saveEvent -= SaveYourself;
	}
	
	public bool isMarkedForDestruction = false;
	public void MarkForDeconstruction() {
		if (!isMarkedForDestruction) {
			if (isBuilt) {
				isMarkedForDestruction = true;
				isBuilt = false;
				DroneSystem.s.AddDroneDestroyTask(myPos, myData);
				myRend.SetBuildState(SpriteGraphicsController.BuildState.destruction);
				
				foreach (Position myPosition in myPositions) {
					if (myPosition != null) {
						var myTile = Grid.s.GetTile(myPosition);
						myTile.objectUpdatedCallback -= TileUpdated;
					}
				}
				
				FactorySystem.s.RemoveBuilding(myBuilding);
			} else {
				DestroyYourself();
			}
		}
	}

	public void UnmarkDestruction() {
		if (isMarkedForDestruction) {
			isMarkedForDestruction = false;
			DroneSystem.s.RemoveDroneTask(myPos);
			
			DroneSystem.s.AddDroneBuildTask(myPos, myData);
		}
	}

	public void DestroyYourself () {
		foreach (Position myPosition in myPositions) {
			if (myPosition != null) {
				var myTile = Grid.s.GetTile(myPosition);
				myTile.worldObject = null;
			}
		}

		if (isBuilt) {
			foreach (Position myPosition in myPositions) {
				if (myPosition != null) {
					var myTile = Grid.s.GetTile(myPosition);
					myTile.objectUpdatedCallback -= TileUpdated;
				}
			}

			FactorySystem.s.RemoveBuilding(myBuilding);
		}

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
