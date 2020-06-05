using UnityEngine;
using System.Collections;

public class AddHigherLevel : MonoBehaviour
{
	[SerializeField]
	public IdleEngineMain Main;
	public string ArmorMultiplier,RewardPriceMultiplier,RewardScoreMultiplier,HealthMultiplier;
	public decimal Armor,Price,Score,Health;
	int GeneratedItems = 0;
	eSave saver;
	int InitialLength;
	void OnEnable()
	{
		saver = new eSave();
		InitialLength = Main.Items.Length;
		if(PlayerPrefs.HasKey("GI"))
		{
			GeneratedItems = PlayerPrefs.GetInt("GI");
			Load ();
		}
		IdleEngineMain.OnSave += Save;
	}
	bool Returnable = true;
	void Load()
	{
		if(GeneratedItems > 0)
		{
			IdleItem[] TempItems = new IdleItem[Main.Items.Length];
			System.Array.Copy(Main.Items,TempItems,Main.Items.Length);
			Main.Items = new IdleItem[InitialLength+GeneratedItems];
			System.Array.Copy(TempItems,Main.Items,TempItems.Length);
			for(int i = InitialLength;i<InitialLength+GeneratedItems;i++)
			{
				Main.Items[i] = (IdleItem) Instantiate(Main.Items[i-1],Main.Items[i-1].transform.position,Main.Items[i-1].transform.rotation);
				Main.Items[i].InitialHP = saver.LoadDecimal(i.ToString()+"HP");
				Main.Items[i].HP = Main.Items[i].InitialHP;
				Main.Items[i].Toughness = saver.LoadDecimal(i.ToString()+"Armor");
				Main.Items[i].PriceScore = saver.LoadDecimal(i.ToString()+"PS");
				Main.Items[i].Price =saver.LoadDecimal(i.ToString()+"Price");
			}
	}
	}

	void Save()
	{
		PlayerPrefs.SetInt("GI",GeneratedItems);
		if(GeneratedItems > 0)
		{
			for(int i = InitialLength;i<InitialLength+GeneratedItems;i++)
			{
				saver.SaveDecimal(i.ToString()+"HP",Main.Items[i].InitialHP);
				saver.SaveDecimal(i.ToString()+"Armor",Main.Items[i].Toughness);
				saver.SaveDecimal(i.ToString()+"PS",Main.Items[i].PriceScore);
				saver.SaveDecimal(i.ToString()+"Price",Main.Items[i].Price);
			}
		}
	}

	void Start()
	{
		Returnable = true;
		Utilities util = new Utilities();
		Armor = util.ConvertToDecimal(ArmorMultiplier);
		Price = util.ConvertToDecimal(RewardPriceMultiplier);
		Score = util.ConvertToDecimal(RewardScoreMultiplier);
		Health = util.ConvertToDecimal(HealthMultiplier);
		IdleEngineMain.OnNewLevelUnlock += MakeNewItem;
	}
	void MakeNewItem()
	{
		if(Returnable && Main.MaxLevelUnlocked < Main.Items.Length)
			return;
		IdleItem[] TempItems = new IdleItem[Main.Items.Length];
		System.Array.Copy(Main.Items,TempItems,Main.Items.Length);
		Main.Items = new IdleItem[TempItems.Length+1];
		System.Array.Copy(TempItems,Main.Items,TempItems.Length);
		Main.Items[Main.Items.Length-1] = (IdleItem) Instantiate(Main.Items[Main.Items.Length-2],Main.Items[Main.Items.Length-2].transform.position,Main.Items[Main.Items.Length-2].transform.rotation);
		if(Returnable)
		{
			GeneratedItems++;
		}
		OverrideInitilize(Main.Items[Main.Items.Length-1],Main.Items[Main.Items.Length-2],GeneratedItems);
	}

	void OverrideInitilize(IdleItem Item,IdleItem Base,int TimesGenerated)
	{
		Item.Toughness = Base.Toughness*TimesGenerated*Armor;
		Item.InitialHP = Base.InitialHP*(Health*TimesGenerated);
		Item.HP = Item.InitialHP;
		Item.PriceScore = Base.PriceScore*(Score*TimesGenerated);
		Item.Price = Base.Price*(TimesGenerated*Price);
	}
}