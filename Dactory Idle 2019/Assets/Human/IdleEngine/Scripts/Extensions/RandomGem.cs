using UnityEngine;
using System.Collections;

public class RandomGem : MonoBehaviour
{
	[Tooltip("The more this is the more chances of finding gems")]
	[Range(0,100)]
	public int GemChances;
	public int Gems = 0;
	[SerializeField]
	IdleEngineMain Main;
	public TextMesh Messeges;
	[Multiline]
	public string GemGetMessage;
	[Multiline]
	public string NotGetMessage;
	eSave saver;
	void Start()
	{
		saver = new eSave();
		if(saver.isSaved("Gems"))
		{
			Gems = saver.LoadInt("Gems");
			GemChances = saver.LoadInt("GeC");
		}
		IdleItem.OnDeath += GemLuck;
		IdleEngineMain.OnSave += Save;
	}

	void Save()
	{
		saver.SaveInt("Gems",Gems);
		saver.SaveInt("GeC",GemChances);
	}

	void GemLuck()
	{
		int Chance = Random.Range(0,100);
		if(Chance <= GemChances)
		{
			Gems++;
			Messeges.text =  GemGetMessage;
		}
		else
		{
			Messeges.text = NotGetMessage;
		}
	}

}