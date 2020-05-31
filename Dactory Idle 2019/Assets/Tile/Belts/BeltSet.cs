using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "BeltSet", menuName = "BeltSet", order = 1)]
public class BeltSet : ScriptableObject {

	public GameObject b_middle;
	public GameObject b_in;
	public GameObject b_out;

	public float offset;
}
