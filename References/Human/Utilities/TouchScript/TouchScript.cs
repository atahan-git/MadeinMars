// The namespaces we are using
using UnityEngine;
using System.Collections;

// Ads a option to add this script thtough component menu
[AddComponentMenu("Human/Utilities/Touch Controller")]
public class TouchScript : MonoBehaviour
{
	public static GameObject TouchingObject;// The GameObject player are touching.
	public static GameObject TouchedObject;// The GameObject player just touched
#if UNITY_EDITOR
	[SerializeField]
	PinchType PinchEditor;// The Pinch in Editor (can be modified using inspector)
#endif
	public float SwipeSensivity;// The lesser this is the lesser swipe is sensitive
	public static SwipeType Swipe = SwipeType.None;// The direction player last Swipted in.
	public static PinchType Pinch;// The last pinch player gace to the phone :(
	float InitialDistance = 0f; // The initial Distance on Two fingures (will alyaws be 0).
	public bool MultiplatformTouch;
	// The update function
	void Update()
	{
		Control();// The function that controlls the whole script.
	}
	// Declares the Update function
	void Control()
	{
		if(MultiplatformTouch)
		{
			UniversalTouch();
		}
#if UNITY_EDITOR
		Pinch = PinchEditor; // Sets pich to the editor pinch you gave.
#endif
#if !UNITY_ANDROID && !UNITY_IOS || UNITY_EDITOR
		if(Input.GetMouseButtonDown(0))// If the mouse button is down
		{
			TouchedObject = null;// Makes LastTouchObject empty so it no longer has an effect
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);// Sends a new ray from mouse position
			RaycastHit hit;//  The hit info of ray
			if(Physics.Raycast(ray,out hit))// If the ray was collided with something
			{
				TouchingObject = hit.transform.gameObject;// Then player are touching (clicking) that Object!
			}
		}
		else if (Input.GetMouseButtonUp(0))// if the mouse button is released
		{
			TouchingObject = null;// Player is not touching (clicking) anything now
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);// Sends a new ray from mouse Position
			RaycastHit hit;// The hit info of ray
			if(Physics.Raycast(ray,out hit))// If the ray collided with something.
			{
				TouchedObject = hit.transform.gameObject;// Then player has just touched(clicked) that object
			}
		}
		if(Input.GetMouseButtonDown(0))// Checks if the mouse was just pressed.
		{
			LastPosition = Input.mousePosition;// Sets last position to mouse position
		}
		else if(Input.GetMouseButtonUp(0))// Now if mouse button is released
		{
			UpPosition = Input.mousePosition;// Sets up position to mouse position for easy acess
			if(UpPosition.x >= UpPosition.x+SwipeSensivity)
			{
				Swipe = SwipeType.Right;// Sets swipe to Right
			}
			else if (UpPosition.x <= LastPosition.x-SwipeSensivity)
			{
				Swipe = SwipeType.Left;// Sets swipe to left
			}
			else if(UpPosition.y >= LastPosition.y+SwipeSensivity)
			{
				Swipe = SwipeType.Up;// Sets swipe to Up
			}
			else if(UpPosition.y <= LastPosition.y-SwipeSensivity)
			{
				Swipe = SwipeType.Down;// Sets swip to Down
			}
			else
			{
				Swipe = SwipeType.SamePosition;// No swipe occoured!
			}
		}
#endif
#if UNITY_ANDROID || UNITY_IOS
		if(!MultiplatformTouch)
		{
			UniversalTouch();
		}
#endif
	}

	void UniversalTouch()
	{
		if(Input.touchCount > 0) // Checks if the player is even touching the screen
		{
			for(int i = 0;i<Input.touchCount;i++)// Runs a loop for each touch
			{
				if(Input.GetTouch(0).phase == TouchPhase.Began) // if any of touch just began
				{
					TouchedObject = null;// We havent released touch on object now
					Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);// Sends a new ray from touch position
					RaycastHit hit;// The hit info of the ray
					if(Physics.Raycast(ray,out hit))// If the ray collided with something
					{
						TouchingObject = hit.transform.gameObject;// Then the player is touching that object now!
					}
				}
				else if(Input.GetTouch(0).phase == TouchPhase.Ended)// If any of the touch just ended
				{
					TouchingObject = null;// We arent touching anything now
					Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);// Sends a new ray from touch position
					RaycastHit hit;;// The hit info of the ray
					if(Physics.Raycast(ray,out hit))// If the ray collided with something
					{
						TouchedObject = hit.transform.gameObject;// Now we just touched that object
					}
				}
			}
		}
		DetectSwipe();// Detects if user made any swipe.
	}
	Vector2 LastPosition;// The last position where used touched
	Vector2 UpPosition;// The position where user released touch
	void DetectSwipe()//Declares the new DetectSwipe function
	{
		if(Input.touchCount > 0)
		{
			if(Input.GetTouch(0).phase == TouchPhase.Began)
			{
				LastPosition = Input.GetTouch(0).position;
			}
			else if(Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				UpPosition = Input.GetTouch(0).position;
				if(UpPosition.x >= UpPosition.x+SwipeSensivity)
				{
					Swipe = SwipeType.Right;// Sets swipe to Right
				}
				else if (UpPosition.x <= LastPosition.x-SwipeSensivity)
				{
					Swipe = SwipeType.Left;// Sets swipe to left
				}
				else if(UpPosition.y >= LastPosition.y+SwipeSensivity)
				{
					Swipe = SwipeType.Up;// Sets swipe to Up
				}
				else if(UpPosition.y <= LastPosition.y-SwipeSensivity)
				{
					Swipe = SwipeType.Down;// Sets swip to Down
				}
				else
				{
					Swipe = SwipeType.SamePosition;// No swipe occoured!
				}
			}
			if(Input.touchCount > 1)// If two fingers are touching the screen
			{
				if(Input.GetTouch(1).phase == TouchPhase.Began)// if the second finger has just began touching
				{
					InitialDistance = Vector3.Distance(Input.GetTouch(0).position,Input.GetTouch(1).position);// Initial Distance is the distance between two fingers now
				}
				if(Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)// If any of the finger were moved
				{
					float Distance = Vector3.Distance(Input.GetTouch(0).position,Input.GetTouch(1).position);// Distance between the two fingers now
					if(Distance < InitialDistance)// if the distance was lesser than Initial Distance
					{
						Pinch = PinchType.Decrease;// Sets Pinch to decrese.
					}
					else if(Distance > InitialDistance)// if the distance was more than Initial Distance
					{
						Pinch = PinchType.Increase;// Sets pinch to Increse
					}
				}
				else
				{
					Pinch = PinchType.None;// Player did not pinched
				}
			}
			else
			{
				Pinch = PinchType.None;// Player did not pinched
			}
		}
	}

}
// The enum that has direction in which Swipe can occour
public enum SwipeType
{
	None,
	SamePosition,
	Left,
	Right,
	Up,
	Down
}
// The enum which stores PinchTypes
public enum PinchType
{
	Increase,Decrease,None
}
