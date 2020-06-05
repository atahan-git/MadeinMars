using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(TouchScript))]
[AddComponentMenu("Human/Idle Engine/Main")]
public class IdleEngineMain : MonoBehaviour
{
	public decimal Money,Score;
	public float TapPerSec;
	public float SaveDelay;
	[SerializeField]
	string MoneyPerSecond;
	[SerializeField]
	string DamageDone;
	public decimal DamageMultiplier = 1;
	public decimal IdleMoneyPerSec;
	public decimal MoneyMultiplier = 1;
	[HideInInspector]
	public int CurrentItem;
	[SerializeField]
	public GameObject NextItemButton,PrevItemButton;
	[Tooltip("This text field will show the amount of money you got while you were not plaing")]
	public TextMesh MoneyGotText;
	public BigNumbers BigNumber;
	public IdleItem[] Items;
	float RemainingTime = 1;
	[HideInInspector]
	public IdleItem Item;
	public decimal Damage;
	[HideInInspector]
	public int MaxLevelUnlocked = 0;
	public delegate void _AutoTap();
	public delegate void _Initilize();
	public delegate void _Ect();
	public delegate void _Save();
	public delegate void _Load();
	public delegate void _LevelChange();
	public delegate void _NewLevelUnlock();
	public static event _Ect OnEct; 
	public static event _AutoTap OnAutoTap;
	public static event _Initilize OnInitilize;
	public static event _Save OnSave;
	public static event _Load OnLoad;
	public static event _NewLevelUnlock OnNewLevelUnlock;
	public static event _LevelChange OnLevelChange;
	[HideInInspector]
	public decimal TotalSec;
	public decimal MoneyGot;
	void Awake()
	{
		for(int i = 0;i<Items.Length;i++)
		{
			if(Items[i] == null)
			{
				Debug.LogError("WARNING: A null space was found in Items in Script IdleEngineMain. Please make sure that no null space is left.Instance will be deleted");
			}
			Items[i].Main = this;
		}
	}

	void Main()
	{
		Initilize();
		Load();
		Invoke("Save",SaveDelay);
	}

	void Update()
	{
		ect();
		AutoTap();
	}
	Utilities util;
	void Initilize()
	{
		util = new Utilities();
		Damage = util.ConvertToDecimal(DamageDone);
		IdleMoneyPerSec = util.ConvertToDecimal(MoneyPerSecond);
		if(OnInitilize != null)
		{
			OnInitilize();
		}
	}
	
	void ect()
	{
		try
		{
			Item = Items[CurrentItem];
			Item.gameObject.SetActive(true);
			IdleItem.OnDeath += CheckDeath;
		}
		catch (IndexOutOfRangeException)
		{
			CurrentItem = 0;
			return;
		}

		for(int i = 0;i<Items.Length;i++)
		{
			if(i != CurrentItem)
			{
				Items[i].gameObject.SetActive(false);
			}
		}
		if(CurrentItem != 0)
		{
			PrevItemButton.SetActive(true);
		}
		else
		{
			PrevItemButton.SetActive(false);
		}
		if(CurrentItem != Items.Length-1 && MaxLevelUnlocked > CurrentItem)
		{
			NextItemButton.SetActive(true);
		}
		else
		{
			NextItemButton.SetActive(false);
		}
		if(TouchScript.TouchedObject == NextItemButton)
		{
			CurrentItem++;
			TouchScript.TouchedObject = null;
			if(OnLevelChange != null)
			{
				OnLevelChange();
			}
		}
		else if(TouchScript.TouchedObject == PrevItemButton)
		{
			CurrentItem--;
			TouchScript.TouchedObject = null;
			if(OnLevelChange != null)
			{
				OnLevelChange();
			}
		}
		if(OnEct != null)
		{
			OnEct();
		}
	}

	void CheckDeath()
	{
		if(CurrentItem == MaxLevelUnlocked)
		{
			MaxLevelUnlocked++;
			if(OnNewLevelUnlock != null)
			{
				OnNewLevelUnlock();
			}
		}
	}

	void Save()
	{
		eSave saver = new eSave();

		saver.SaveDateTime("LastPlayTime",DateTime.Now);
		saver.SaveDecimal("Money",Money);
		saver.SaveDecimal("MoneyM",MoneyMultiplier);
		saver.SaveDecimal("Score",Score);
		saver.SaveFloat("TPS",TapPerSec);
		saver.SaveDecimal("IdleMoney",IdleMoneyPerSec);
		saver.SaveDecimal("DamageM",DamageMultiplier);
		saver.SaveInt("CurItem",CurrentItem);
		saver.SaveDecimal("Damage",Damage);
		saver.SaveInt("MaxLevel",MaxLevelUnlocked);
		Invoke("Save",SaveDelay);
		if(OnSave != null)
		{
			OnSave();
		}
	}

	void Load()
	{
		eSave loader = new eSave();
		if(loader.isSaved("MaxLevel"))
		{
			if(OnLoad != null)
			{
				OnLoad();
			}
			DateTime LastPlayDay = loader.LoadDateTime("LastPlayTime");
			Money = loader.LoadDecimal("Money");
			MoneyMultiplier = loader.LoadDecimal("MoneyM");
			Score = loader.LoadDecimal("Score");
			TapPerSec = loader.LoadFloat("TPS");
			IdleMoneyPerSec = loader.LoadDecimal("IdleMoney");
			DamageMultiplier = loader.LoadDecimal("DamageM");
			CurrentItem = loader.LoadInt("CurItem");
			Damage = loader.LoadDecimal("Damage");
			MaxLevelUnlocked = (loader.LoadInt("MaxLevel"));
			if(PlayerPrefs.GetInt("GI") > 0)
				MaxLevelUnlocked = Items.Length-1;
			TimeSpan TotalTime = DateTime.Now-LastPlayDay;
			TotalSec = (decimal) TotalTime.TotalSeconds;
			MoneyGot = (TotalSec*IdleMoneyPerSec*util.ConvertToDecimal(Items[CurrentItem].RewardPrice)/2);
			Money += MoneyGot;;
			if(MoneyGot > 0.000001m)
				MoneyGotText.text = BigNumber.ShortNumber(MoneyGot);
		}
	}

	void AutoTap()
	{
		RemainingTime -= (Time.deltaTime*TapPerSec);
		if(RemainingTime <= 0)
		{
			RemainingTime = 1;
			Item.Deal_Damage((Damage*DamageMultiplier),Item.Toughness);
			if(OnAutoTap != null)
			{
				OnAutoTap();
			}
		}
	}

	void OnApplicationQuit()
	{
		Save();
	}

}