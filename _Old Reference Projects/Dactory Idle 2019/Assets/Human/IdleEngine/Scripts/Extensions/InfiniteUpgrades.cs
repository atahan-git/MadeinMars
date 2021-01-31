using UnityEngine;
using System.Collections;
public class InfiniteUpgrades : MonoBehaviour
{
	public IdleEngineMain Main;
	public RandomGem GemController;
	public Collider BuyButton;
	public IdleVarController VarController;
	public BigNumbers BigNumber;
	public TextMesh DetailsText;
	public TextMesh CostText;
	public TextMesh CurrentValueText;
	public TextMesh NextValueText;
	public Type RateIncreseType;
	public Type ValueType;
	public Type UpgradeCalculationType;
	public Variable EffectOn;
	public string IdleVarName;
	public string InitialValue;
	public string InitialCost;
	public string RateEffector;
	public string ValueEffector;
	public string Currency;
	public char ID;
	public static char staticID;
	// Initial Value,Rate Effector,Value Effector
	decimal Ivalue,Icost,rateE,valueE;
	[Tooltip("Is this is set it will yet be destroyed if this level is reached. Leave to 0 to Infinity")]
	public int YetMaxLevel;
	public bool DisableColorChange;
	public bool Gems;
	[Multiline]
	public string Details;
	int Level;
	[HideInInspector]
	public string NextLvlVal;
	void Awake()
	{
		Utilities util = new Utilities();
		Ivalue =  util.ConvertToDecimal(InitialValue);
		Icost = util.ConvertToDecimal(InitialCost);
		rateE = util.ConvertToDecimal(RateEffector);
		valueE = util.ConvertToDecimal(ValueEffector);
		staticID = ' ';
		if(PlayerPrefs.HasKey("Money"))
		{
			Load();
		}
		IdleEngineMain.OnSave += Save;
		if(ID == ' ')
		{
			Debug.LogError("WARNING: THE ID CANNOT BE BLANK or SPACE");
		}
		staticID = ' ';
	}

	void CheckTouch()
	{
		if(TouchScript.TouchedObject == this.gameObject)
		{
			staticID = ID;
			TouchScript.TouchedObject = null;
			ValueTextUpdate();
		}
	}

	void ValueTextUpdate()
	{
		switch(EffectOn)
		{
		case Variable.AutoTap:
			CurrentValueText.text = Main.TapPerSec.ToString();
			break;
		case Variable.DamageDone:
			CurrentValueText.text = BigNumber.ShortNumber(Main.Damage).ToString();
			break;
		case Variable.DamageMultiplier:
			CurrentValueText.text = Main.DamageMultiplier.ToString();
			break;
		case Variable.GemChances:
			CurrentValueText.text = GemController.GemChances.ToString();
			break;
		case Variable.IdleVar:
			for(int i = 0;i<VarController.Variables.Length;i++)
			{
				if(VarController.Variables[i].Name == IdleVarName)
				{
					if(VarController.Variables[i].NumberController)
						CurrentValueText.text = VarController.Variables[i].NumberController.ShortNumber(VarController.Variables[i].Value);
					else
						CurrentValueText.text = VarController.Variables[i].Value.ToString();
				}
			}
			break;
		case Variable.MoneyMultiplier:
			CurrentValueText.text = Main.MoneyMultiplier.ToString();
			break;
		case Variable.MoneyPerSec:
			CurrentValueText.text = BigNumber.ShortNumber(Main.IdleMoneyPerSec);
			break;

		}
		NextValueText.text = GetNextLevel();
	}

	void Update()
	{
		CheckBuy();
		CheckTouch();
		if(YetMaxLevel != 0 && Level >= YetMaxLevel)
		{
			Destroy(gameObject);
			staticID = ' ';
		}

		if(staticID == ID)
		{
			if(!Gems)
				CostText.text = Currency + " " + BigNumber.ShortNumber(Icost);
			else
			{
				CostText.text = Currency + " " + Icost;
			}
			DetailsText.text = Details;
			if(Gems)
			{
				if(GemController.Gems >= (int) Icost)
				{
					BuyButton.gameObject.SetActive(true);
				}
				else
				{
					BuyButton.gameObject.SetActive(false);
				}
			}
			else
			{
				if(Main.Money >= Icost)
				{
					BuyButton.gameObject.SetActive(true);
				}
				else
				{
					BuyButton.gameObject.SetActive(false);
				}
			}
		}
		if(staticID == ' ')
		{
			CostText.text = "";
			DetailsText.text = "";
			NextValueText.text = "";
			CurrentValueText.text = "";
			BuyButton.gameObject.SetActive(false);
		}
		if(!DisableColorChange && staticID == ID)
		{
			if(!Gems)
			{
				if(Main.Money < Icost)
					CostText.color = Color.red;

				else
					CostText.color = Color.green;
			}
			else
			{
				if(GemController.Gems < (int) Icost)
					CostText.color = Color.red;
				else
					CostText.color = Color.green;
			}
		}
	}

	void CheckBuy()
	{
		if(TouchScript.TouchedObject == BuyButton.gameObject&& staticID == ID)
		{
			if(Gems)
			{
				if(GemController.Gems >= (int) Icost)
					Process();
			}
			else
			{
				if(Main.Money >= Icost)
				{
					Process();
				}
			}
			TouchScript.TouchedObject = null;
		}
	}

	void Process()
	{
	 switch (UpgradeCalculationType)
		{
		case Type.Add:
			Add (EffectOn,Ivalue);
		break;
		case Type.Divide:
			Divide(EffectOn,Ivalue);
		break;
		case Type.Multiply:
			Multiply(EffectOn,Ivalue);
		break;
		case Type.Subtract:
			Subtract(EffectOn,Ivalue);
		break;
		}
		if(!Gems)
			Main.Money -= Icost;
		else
			GemController.Gems -= (int) Icost;
		staticID = ' ';
		switch (ValueType)
		{
			case Type.Add:
				Ivalue += valueE;
			break;
			case Type.Divide:
				Ivalue /= valueE;
			break;
			case Type.Multiply:
				Ivalue *= valueE;
			break;
			case Type.Subtract:
				Ivalue -= valueE;
			break;
		}
		switch(RateIncreseType)
		{
		case Type.Add:
			Icost += rateE;
			break;
		case Type.Divide:
			Icost /= rateE;
			break;
		case Type.Multiply:
			Icost *= rateE;
			break;
		case Type.Subtract:
			Icost -= rateE;
			break;
		}
		Level++;
	}

	public string GetNextLevel()
	{
		if(YetMaxLevel == Level && YetMaxLevel != 0)
			return "MAXED!";

		switch(UpgradeCalculationType)
		{
		case Type.Add:
			return NxtPrvAdd();
		case Type.Divide:
			return NxtPrvDivide();
		case Type.Multiply:
			return NxtPrvMultiply();
		case Type.Subtract:
			return NxtPrvSubtract();
		}
		return "";
	}

	public string NxtPrvAdd()
	{
		bool DoneVvar = false;
		switch (EffectOn)
		{
		case Variable.AutoTap:
			return (Main.TapPerSec + (float) Ivalue).ToString();
		case Variable.DamageDone:
			return BigNumber.ShortNumber(Main.Damage + Ivalue);
		case Variable.DamageMultiplier:
			return (Main.DamageMultiplier + Ivalue).ToString();
		case Variable.GemChances:
			return (GemController.GemChances+(int)Ivalue).ToString();
		case Variable.IdleVar:
			for(int i = 0;i<VarController.Variables.Length;i++)
			{
				if(VarController.Variables[i].Name == IdleVarName)
				{
					DoneVvar = true;
					if(VarController.Variables[i].NumberController)
						return VarController.Variables[i].NumberController.ShortNumber(VarController.Variables[i].Value+Ivalue);
					else
						return (VarController.Variables[i].Value+Ivalue).ToString();
				}
			}
			break;
		case Variable.MoneyMultiplier:
			return (Main.MoneyMultiplier + Ivalue).ToString();
		case Variable.MoneyPerSec:
			return(BigNumber.ShortNumber(Main.IdleMoneyPerSec+Ivalue));
		}
		if(DoneVvar == false)
		{
			return "NO SUCH VARIABLE WAS FOUND";
		}
		return "";
	}

	public string NxtPrvSubtract()
	{
		bool DoneVvar = false;
		switch (EffectOn)
		{
		case Variable.AutoTap:
			return (Main.TapPerSec - (float) Ivalue).ToString();
		case Variable.DamageDone:
			return BigNumber.ShortNumber(Main.Damage - Ivalue);
		case Variable.DamageMultiplier:
			return (Main.DamageMultiplier - Ivalue).ToString();
		case Variable.GemChances:
			return (GemController.GemChances-(int)Ivalue).ToString();
		case Variable.IdleVar:
			for(int i = 0;i<VarController.Variables.Length;i++)
			{
				if(VarController.Variables[i].Name == IdleVarName)
				{
					DoneVvar = true;
					if(VarController.Variables[i].NumberController)
						return VarController.Variables[i].NumberController.ShortNumber(VarController.Variables[i].Value-Ivalue);
					else
						return (VarController.Variables[i].Value-Ivalue).ToString();
				}
			}
			break;
		case Variable.MoneyMultiplier:
			return (Main.MoneyMultiplier - Ivalue).ToString();
		case Variable.MoneyPerSec:
			return(BigNumber.ShortNumber(Main.IdleMoneyPerSec-Ivalue));
		}
		if(DoneVvar == false)
		{
			return "NO SUCH VARIABLE WAS FOUND";
		}
		return "";
	}

	public string NxtPrvMultiply()
	{
		bool DoneVvar = false;
		switch (EffectOn)
		{
		case Variable.AutoTap:
			return (Main.TapPerSec * (float) Ivalue).ToString();
		case Variable.DamageDone:
			return BigNumber.ShortNumber(Main.Damage * Ivalue);
		case Variable.DamageMultiplier:
			return (Main.DamageMultiplier * Ivalue).ToString();
		case Variable.GemChances:
			return (GemController.GemChances*(int)Ivalue).ToString();
		case Variable.IdleVar:
			for(int i = 0;i<VarController.Variables.Length;i++)
			{
				if(VarController.Variables[i].Name == IdleVarName)
				{
					DoneVvar = true;
					if(VarController.Variables[i].NumberController)
						return VarController.Variables[i].NumberController.ShortNumber(VarController.Variables[i].Value*Ivalue);
					else
						return (VarController.Variables[i].Value*Ivalue).ToString();
				}
			}
			break;
		case Variable.MoneyMultiplier:
			return (Main.MoneyMultiplier * Ivalue).ToString();
		case Variable.MoneyPerSec:
			return(BigNumber.ShortNumber(Main.IdleMoneyPerSec*Ivalue));
		}
		if(DoneVvar == false)
		{
			return "NO SUCH VARIABLE WAS FOUND";
		}
		return "";
	}

	public string NxtPrvDivide()
	{
		bool DoneVvar = false;
		switch (EffectOn)
		{
		case Variable.AutoTap:
			return (Main.TapPerSec / (float) Ivalue).ToString();
		case Variable.DamageDone:
			return BigNumber.ShortNumber(Main.Damage / Ivalue);
		case Variable.DamageMultiplier:
			return (Main.DamageMultiplier / Ivalue).ToString();
		case Variable.GemChances:
			return (GemController.GemChances/(int)Ivalue).ToString();
		case Variable.IdleVar:
			for(int i = 0;i<VarController.Variables.Length;i++)
			{
				if(VarController.Variables[i].Name == IdleVarName)
				{
					DoneVvar = true;
					if(VarController.Variables[i].NumberController)
						return VarController.Variables[i].NumberController.ShortNumber(VarController.Variables[i].Value/Ivalue);
					else
						return (VarController.Variables[i].Value/Ivalue).ToString();
				}
			}
			break;
		case Variable.MoneyMultiplier:
			return (Main.MoneyMultiplier / Ivalue).ToString();
		case Variable.MoneyPerSec:
			return(BigNumber.ShortNumber(Main.IdleMoneyPerSec/Ivalue));
		}
		if(DoneVvar == false)
		{
			return "NO SUCH VARIABLE WAS FOUND";
		}
		return "";
	}

	void Save()
	{
		PlayerPrefs.SetInt("LevelOfUpgrades"+ID.ToString(),Level);
	}

	void Load()
	{
		Level = PlayerPrefs.GetInt("LevelOfUpgrades"+ID.ToString());
		for(int i = 0;i<Level;i++)
		{
			switch (ValueType)
			{
			case Type.Add:
				Ivalue += (valueE);
				break;
			case Type.Divide:
				Ivalue /= (valueE);
				break;
			case Type.Multiply:
				Ivalue *= (valueE);
				break;
			case Type.Subtract:
				Ivalue -= (valueE);
				break;
			}
			switch(RateIncreseType)
			{
			case Type.Add:
				Icost += (rateE);
				break;
			case Type.Divide:
				Icost /= (rateE);
				break;
			case Type.Multiply:
				Icost *= (rateE);
				break;
			case Type.Subtract:
				Icost -= (rateE);
				break;
			}
		}
	}

	void Add(Variable varType,decimal Value)
	{
		switch (varType)
		{
		case Variable.DamageDone:
			Main.Damage += Value;
			break;
		case Variable.DamageMultiplier:
			Main.DamageMultiplier  += Value;
			break;
		case Variable.MoneyMultiplier:
			Main.MoneyMultiplier += Value;
			break;
		case Variable.GemChances :
			GemController.GemChances += (int) Value;
			break;
		case Variable.AutoTap:
			Main.TapPerSec += (float) Value;
			break;
		case Variable.MoneyPerSec:
			Main.Money += Value;
			break;
		case Variable.IdleVar:
			for(int i = 0;i<VarController.Variables.Length;i++)
			{
				if(VarController.Variables[i].Name == IdleVarName)
				{
					VarController.Variables[i].Value += Value;
					Debug.Log(VarController.Variables[i].Value);
				}
			}
			break;
		}
	}

	void Divide(Variable varType,decimal Value)
	{
		switch (varType)
		{
		case Variable.DamageDone:
			Main.Damage /= Value;
			break;
		case Variable.DamageMultiplier:
			Main.DamageMultiplier  /= Value;
			break;
		case Variable.MoneyMultiplier:
			Main.MoneyMultiplier /= Value;
			break;
		case Variable.GemChances :
			GemController.GemChances /= (int) Value;
			break;
		case Variable.AutoTap:
			Main.TapPerSec /= (float) Value;
			break;
		case Variable.MoneyPerSec:
			Main.Money /= Value;
			break;
		case Variable.IdleVar:
			for(int i = 0;i<VarController.Variables.Length;i++)
			{
				if(VarController.Variables[i].Name == IdleVarName)
					VarController.Variables[i].Value /= Value;
			}
			break;
		}
	}
		void Multiply(Variable varType,decimal Value)
		{
			switch (varType)
			{
			case Variable.DamageDone:
				Main.Damage *= Value;
				break;
			case Variable.DamageMultiplier:
				Main.DamageMultiplier  *= Value;
				break;
			case Variable.MoneyMultiplier:
				Main.MoneyMultiplier *= Value;
				break;
			case Variable.GemChances :
				GemController.GemChances *= (int) Value;
				break;
			case Variable.AutoTap:
				Main.TapPerSec *= (float) Value;
				break;
			case Variable.MoneyPerSec:
				Main.Money *= Value;
				break;
		case Variable.IdleVar:
			for(int i = 0;i<VarController.Variables.Length;i++)
			{
				if(VarController.Variables[i].Name == IdleVarName)
					VarController.Variables[i].Value *= Value;
			}
			break;
			}
	}

		void Subtract(Variable varType,decimal Value)
		{
			switch (varType)
			{
			case Variable.DamageDone:
				Main.Damage -= Value;
				break;
			case Variable.DamageMultiplier:
				Main.DamageMultiplier  -= Value;
				break;
			case Variable.MoneyMultiplier:
				Main.MoneyMultiplier -= Value;
				break;
			case Variable.GemChances :
				GemController.GemChances -= (int) Value;
				break;
			case Variable.AutoTap:
				Main.TapPerSec -= (float) Value;
				break;
			case Variable.MoneyPerSec:
				Main.IdleMoneyPerSec -= Value;
				break;
		case Variable.IdleVar:
			for(int i = 0;i<VarController.Variables.Length;i++)
			{
				if(VarController.Variables[i].Name == IdleVarName)
					VarController.Variables[i].Value -= Value;
			}
			break;
			}
		}

	public enum Variable
	{
		MoneyMultiplier,DamageMultiplier,DamageDone,GemChances,AutoTap,MoneyPerSec,IdleVar
	}
	public enum Type
	{
		Add,Subtract,Multiply,Divide
	}
}