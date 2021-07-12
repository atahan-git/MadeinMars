using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class PlanetTileSettings : ScriptableObject {
	public TileBase[] groundTile = new TileBase[4];
	public TileBase[] groundTileEdge = new TileBase[4];
	public TileBase[] groundTileAlternative = new TileBase[4];
	public TileBase[] groundTileAlternativeEdge = new TileBase[4];


	public TileBase GetTile(int level, int type, bool isEdge) {
		isEdge = true;
		if (type == 0) {
			if (isEdge) {
				return groundTileEdge[level];
			} else {
				return groundTile[level];
			}
		} else {
			if (isEdge) {
				return groundTileAlternativeEdge[level];
			} else {
				return groundTileAlternative[level];
			}
		}
	}
	
}
