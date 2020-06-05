using UnityEngine;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Utilities
{
#if UNITY_EDITOR
	[MenuItem("Utilities/Reset Save")]
	public static void DeleteEsave()
	{
		PlayerPrefs.DeleteAll();
		Debug.Log("Save Reseted!");
	}
#endif
	public decimal ConvertToDecimal(object value, string Name = "")
	{
		decimal Output = new decimal();
		try
		{
			Output = System.Convert.ToDecimal(value);
		}
		catch (FormatException)
		{
			if(Name != "")
			{
				Debug.LogError("WARNING: A Conversion to decimal (at " + Name + ") has failed because of bad format. Make Sure that you have all fields entered properly");
			}
			else
			{
				Debug.LogError("WARNING: A Conversion to decimal has failed because of bad format. Make Sure that you have all fields entered properly");
			}
		}
		return Output;
	}
}
