using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// The main building object. Should work to store data for the other components, and deal with placing/removing the building.
/// </summary>
public class BuildingWorldObject : MonoBehaviour
{
	
	[SerializeField] bool isConstruction;
	[SerializeField] Building myBuilding;
	[SerializeField] Construction myConstruction;
	[SerializeField] BuildingData myData;
	[SerializeField] List<Position> myLocations;
	[SerializeField] List<TileData> myTiles;
	public BuildingCraftingController myCrafter;
	public BuildingInventoryController myInventory;
	SpriteGraphicsController myRend;

	[SerializeField] float width;
	[SerializeField] float height;

	public bool isInventorySetup = false;
	public GenericCallback buildingInventoryUpdatedCallback;

	public void UpdateSelf(Building _building) {
		// Only update if the building has changed
		if(myBuilding !=null && !myBuilding.center.isValid() && myBuilding.myPositions != null && myBuilding.myPositions.Count > 0 && myBuilding.myPositions[0] == _building.myPositions[0])
			return;

		myBuilding = _building;
		myData = _building.buildingData;
		isConstruction = false;


		/*if (isSpaceLanding)
			GetComponentInChildren<SpriteGraphicsController>().DoSpaceLanding(null);*/
		
		myRend = GetComponentInChildren<SpriteGraphicsController>();
		GenericUpdateSelf(myBuilding.myPositions, _building.center);
		myRend.SetBuildState(SpriteGraphicsController.BuildState.built);


		myCrafter = myBuilding.craftController;
		myCrafter.continueAnimationsEvent -= ContinueAnimations;
		myCrafter.continueAnimationsEvent += ContinueAnimations;
		myCrafter.stopAnimationsEvent -= StopAnimations;
		myCrafter.stopAnimationsEvent += StopAnimations;

		myInventory = myBuilding.invController;
		isInventorySetup = true;
		buildingInventoryUpdatedCallback?.Invoke();
		StopAnimations();
	}
	
	public void UpdateSelf(Construction _construction) {
		myConstruction = _construction;
		myData = myConstruction.myData;
		isConstruction = true;
		
		/*if (isSpaceLanding)
			GetComponentInChildren<SpriteGraphicsController>().DoSpaceLanding(null);*/
		
		myRend = GetComponentInChildren<SpriteGraphicsController>();
		GenericUpdateSelf(myConstruction.locations, _construction.center);
		if (myConstruction.isConstruction) {
			myRend.SetBuildState(SpriteGraphicsController.BuildState.construction);
		} else {
			myRend.SetBuildState(SpriteGraphicsController.BuildState.destruction);
		}


		myInventory = myConstruction.constructionInventory;
		isInventorySetup = true;
		buildingInventoryUpdatedCallback?.Invoke();
		StopAnimations();
	}
	
	void GenericUpdateSelf(List<Position> _locations, Position _location) {
		myLocations = _locations;
		myTiles = new List<TileData>();
		foreach (var loc in myLocations) {
			var tile = Grid.s.GetTile(loc);
			myTiles.Add(tile);
			tile.worldObject = this.gameObject;
			tile.objectUpdatedCallback -= TileUpdated;
			tile.objectUpdatedCallback += TileUpdated;
		}

		width = myData.shape.width;
		height = myData.shape.height;
		Vector3 centerOffset = new Vector3(0.5f, myData.shape.maxHeightFromCenter - 1, 0);

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
		StopAnimationsForced(true);

		transform.position = _location.Vector3(Position.Type.building) + centerOffset;
	}
	
	void TileUpdated() {
		if (isConstruction) {
			foreach (Position myPosition in myLocations) {
				if (myPosition != null) {
					var myTile = Grid.s.GetTile(myPosition);
					if (myTile.areThereConstruction) {
						myConstruction = myTile.myConstruction;
					} else {
						DestroyYourself();
					}
				}
			}
		} else {
			foreach (Position myPosition in myLocations) {
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
	}

	public void DestroyYourself() {
		foreach (Position myPosition in myLocations) {
			if (myPosition != null) {
				var myTile = Grid.s.GetTile(myPosition);
				myTile.worldObject = null;
				myTile.objectUpdatedCallback -= TileUpdated;
			}
		}
		GetComponent<PooledGameObject>().DestroyPooledObject();
	}
	

	public bool animationState = true;
    public AnimatedSpriteController[] anims = new AnimatedSpriteController[0];
    public bool isAnimated = true;
    public ParticleSystem[] particles = new ParticleSystem[0];
    public bool isParticled = true;
    
    bool GetAnims() { 
	    anims = GetComponentsInChildren<AnimatedSpriteController>();
        

        if (anims.Length <= 0) {
            isAnimated = false;
            return false;
        } else
            return true;
    }
    
    bool GetParticles() {
			particles = GetComponentsInChildren<ParticleSystem>();
        

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
