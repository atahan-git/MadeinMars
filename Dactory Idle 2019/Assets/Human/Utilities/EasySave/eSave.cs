using UnityEngine;
using System.Collections;
using System;

public class eSave 
{
	public void Delete(string Name)
	{
		PlayerPrefs.DeleteKey(Name);
	}
	public void DeleteAll()
	{
		PlayerPrefs.DeleteAll();
	}
	public void SaveVector3(string Name, Vector3 Value)
	{
		PlayerPrefs.SetFloat(Name+"X",Value.x);
		PlayerPrefs.SetFloat(Name+"Z",Value.z);
		PlayerPrefs.SetFloat(Name+"Y",Value.y);
	}
	public bool isSaved(string Name)
	{
		if(PlayerPrefs.HasKey(Name))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	public Vector3 LoadVector3(string Name)
	{
		Vector3 vector3 = new Vector3();
		vector3.x = PlayerPrefs.GetFloat(Name+"X");
		vector3.y = PlayerPrefs.GetFloat(Name+"Y");
		vector3.z = PlayerPrefs.GetFloat(Name+"Z");
		return vector3;
	}
	public void SaveDecimal(string Name,decimal Value)
	{
		PlayerPrefs.SetString(Name,Value.ToString());
	}
	public decimal LoadDecimal(string Name)
	{
		decimal Output;
		try
		{
			Output = Convert.ToDecimal(PlayerPrefs.GetString(Name));
		}
		catch (FormatException)
		{
			Output = 0;
		}
		return Output;
	}
	public void SaveDateTime(string Name,DateTime Value)
	{
		PlayerPrefs.SetString(Name,Value.ToString());
	}
	public DateTime LoadDateTime(string Name)
	{
		DateTime Output;
		try
		{
			Output = Convert.ToDateTime(PlayerPrefs.GetString(Name));
		}
		catch (FormatException)
		{
			Output = DateTime.Now;
		}
		return Output;
	}
	public void SaveFloat(string Name, float Value)
	{
		PlayerPrefs.SetFloat(Name,Value);
	}
	public float LoadFloat(string Name)
	{
		return PlayerPrefs.GetFloat(Name);
	}
	public void SaveString(string Name,string Value)
	{
		PlayerPrefs.SetString("Name",Value);
	}
	public string LoadString(string Name)
	{
		return PlayerPrefs.GetString(Name);
	}
	public void SaveInt(string Name,int Value)
	{
		PlayerPrefs.SetInt(Name,Value);
	}
	public int LoadInt(string Name)
	{
		return PlayerPrefs.GetInt(Name);
	}
}