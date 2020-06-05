using UnityEngine;
using System.Collections;

public class Upgrade : MonoBehaviour
{
	[SerializeField]
	Type CalcType;
	[SerializeField]
	Variable variable;
	[SerializeField]
	IdleEngineMain Main;
	[SerializeField]
	RandomGem GemController;
	[SerializeField]
	BigNumbers BigNumber;
	[SerializeField]
	TextMesh DetailsText;
	[SerializeField]
	TextMesh CostText;
	[SerializeField]
	Collider BuyButton;
	[SerializeField]
	int MaxLevel;
	[SerializeField]
	int ID;
	[Multiline]
	[SerializeField]
	string Details;
	[SerializeField]
	bool DisableColorChange;
	[SerializeField]
	float[] Value;
	[SerializeField]
	string[] Cost;
	decimal[] Costs;
	int Level = 0;
	static int LastTouchedID;
	eSave Saver;
	int GeneratedLevels = 0;
	void Awake()
	{
		if(ID == 0)
		{
			Debug.LogError("WARNING: THE ID of Upgrade Cannot be 0");
		}
		if(LastTouchedID == 0)
		{
			BuyButton.gameObject.SetActive(false);
			CostText.text = "";
			DetailsText.text = "";
		}
		Saver = new eSave();
		if(Saver.isSaved(ID.ToString()))
		{
			Level = Saver.LoadInt(ID.ToString());
		}
		Costs = new decimal[Cost.Length];
		for(int i = 0;i<Costs.Length;i++)
		{
			Costs[i] = System.Convert.ToDecimal(Cost[i]);
		}
		IdleEngineMain.OnSave += Save;
		GeneratedLevels = Saver.LoadInt(ID.ToString()+"TotalNew");
		if(Level == MaxLevel)
		{
			Destroy(gameObject);
		}
		LastTouchedID = 0;
	}

	void Save()
	{
		Saver.SaveInt(ID.ToString(),Level);
		Saver.SaveInt(ID.ToString()+"TotalNew",GeneratedLevels);
	}

	void Update()
	{
		if(LastTouchedID == 0)
		{
			BuyButton.gameObject.SetActive(false);
			CostText.text = "";
			DetailsText.text = "";
		}
		if(TouchScript.TouchedObject == this.gameObject)
		{
			LastTouchedID = ID;	
			DetailsText.text = Details;
			TouchScript.TouchedObject = null;
		}
		if(Main.Money < Costs[Level] && ID == LastTouchedID)
		{
			if(!DisableColorChange)
				CostText.color = Color.red;
			BuyButton.gameObject.SetActive(false);
		}
		else
		{
			if(ID == LastTouchedID && ID == LastTouchedID)
			{
				if(!DisableColorChange)
					CostText.color = Color.white;
				BuyButton.gameObject.SetActive(true);
			}
		}
		if(LastTouchedID != 0)
		{
			CostText.text = "$ " + BigNumber.ShortNumber(Costs[Level]);
		}
		else
		{
			CostText.text = "";
		}
		if(TouchScript.TouchedObject == BuyButton.gameObject)
		{
			if(ID == LastTouchedID)
			{
				Calculate();
			}
			TouchScript.TouchedObject = null;
		}
	}

	void Calculate()
	{
		if(Main.Money >= Costs[Level])
		{
			switch (CalcType)
			{
			case Type.Add:
					Add(variable);
				break;
			case Type.Divide:
					Divide(variable);
				break;
			case Type.Multiply:
					Multiply(variable);
				break;
			case Type.Subtract:
					Subtract(variable);
				break;
			}
				Main.Money -= Costs[Level];
				Level++;
				BuyButton.gameObject.SetActive(false);
				LastTouchedID = 0;
				CostText.text = "";
				DetailsText.text = "";
				if(Level == MaxLevel)
				{
					Destroy(gameObject);
				}
			}
		}
	void Multiply(Variable varType)
	{
		switch (varType)
		{
		case Variable.DamageDone:
			Main.Damage *= (decimal) Value[Level];
		break;
		case Variable.DamageMultiplier:
			Main.DamageMultiplier  *= (decimal) Value[Level];
		break;
		case Variable.MoneyMultiplier:
			Main.MoneyMultiplier *= (decimal) Value[Level];
		break;
		case Variable.GemChances :
			GemController.GemChances *= (int) Value[Level];
		break;
		case Variable.AutoTap:
			Main.TapPerSec *=  Value[Level];
			break;
		case Variable.MoneyPerSec:
			Main.Money *= (decimal) Value[Level];
			break;
		}
	}

	void Subtract(Variable varType)
	{
		switch (varType)
		{
		case Variable.DamageDone:
			Main.Damage -= (decimal) Value[Level];
			break;
		case Variable.DamageMultiplier:
			Main.DamageMultiplier -= (decimal) Value[Level];
			break;
		case Variable.MoneyMultiplier:
			Main.MoneyMultiplier -= (decimal) Value[Level];
			break;
		case Variable.GemChances :
			GemController.GemChances -= (int) Value[Level];
			break;
		case Variable.AutoTap:
			Main.TapPerSec -=  Value[Level];;
			break;
		case Variable.MoneyPerSec:
			Main.Money -= (decimal) Value[Level];;
			break;
		}
	}

	void Divide(Variable varType)
	{
		switch (varType)
		{
		case Variable.DamageDone:
			Main.Damage /= (decimal) Value[Level];
			break;
		case Variable.DamageMultiplier:
			Main.DamageMultiplier  /= (decimal) Value[Level];
			break;
		case Variable.MoneyMultiplier:
			Main.MoneyMultiplier /= (decimal) Value[Level];
			break;
		case Variable.GemChances :
			GemController.GemChances /= (int) Value[Level];
			break;
		case Variable.AutoTap:
			Main.TapPerSec /=  Value[Level];;
			break;
		case Variable.MoneyPerSec:
			Main.Money /= (decimal) Value[Level];
			break;
		}
	}

	void Add(Variable varType)
	{
		switch (varType)
		{
		case Variable.DamageDone:
			Main.Damage += (decimal) Value[Level];
			break;
		case Variable.DamageMultiplier:
			Main.DamageMultiplier  += (decimal) Value[Level];
			break;
		case Variable.MoneyMultiplier:
			Main.MoneyMultiplier += (decimal) Value[Level];
			break;
		case Variable.GemChances :
			GemController.GemChances += (int) Value[Level];
			break;
		case Variable.AutoTap:
			Main.TapPerSec += Value[Level];;
			break;
		case Variable.MoneyPerSec:
			Main.Money += (decimal) Value[Level];
			break;
		}
	}

	enum Variable
	{
		MoneyMultiplier,DamageMultiplier,DamageDone,GemChances,AutoTap,MoneyPerSec
	}
	enum Type
	{
		Add,Subtract,Multiply,Divide
	}
}