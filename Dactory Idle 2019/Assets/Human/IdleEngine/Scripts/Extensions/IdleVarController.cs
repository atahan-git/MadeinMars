using UnityEngine;
using System.Collections;

public class IdleVarController : MonoBehaviour
{
	public IdleEngineMain Main;
	public RandomGem GemController;
	public IdleVar[] Variables;
	void Awake()
	{
		IdleEngineMain.OnEct += AdvUpdate;
		IdleEngineMain.OnAutoTap += AdvAutoTap;
		IdleEngineMain.OnSave += AdvSave;
		IdleEngineMain.OnLevelChange += AdvLvlChange;
		IdleEngineMain.OnNewLevelUnlock += AdvNewLvl;
		IdleItem.OnDealDamage += AdvDamage;
		IdleItem.OnDeath += AdvKill;
		IdleItem.OnTouch += AdvTouch;
		IdleItem.OnProjectileHit += AdvHit;

		for(int i = 0;i<Variables.Length;i++)
		{
			Variables[i].Value = System.Convert.ToDecimal(Variables[i].InitialValue);
			AdvRecieveMessages(Variables[i],DoOn.Start);
			if(Variables[i].ToInitialVar.On == SetOn.Start)
			{
				SetToVar(Variables[i]);
			}
		}
		Load();
		for(int i = 0;i<Variables.Length;i++)
		{
			AdvRecieveMessages(Variables[i],DoOn.Inactive);
		}
		AdvRawMessage(SendOn.Start);
	}
	[HideInInspector]
	public System.Collections.Generic.List<string> Messages;
	public void Save()
	{
		eSave saver = new eSave();
		for(int i = 0;i<Variables.Length;i++)
		{
			if(Variables[i].Save)
				saver.SaveDecimal(Variables[i].Name+GetInstanceID(),Variables[i].Value);
		}
		AdvRawMessage(SendOn.Save);
	}

	public void SetToVar(IdleVar Var)
	{
		switch(Var.ToInitialVar.To)
		{
		case SetToPreVar.MoneyMultiplier:
			Var.Value = Main.MoneyMultiplier;
			break;
		case SetToPreVar.DamageDone:
			Var.Value = Main.Damage;
			break;
		case SetToPreVar.DamageMultiplier:
			Var.Value = Main.MoneyMultiplier;
			break;
		case SetToPreVar.GemChances:
			Var.Value = GemController.GemChances;
			break;
		case SetToPreVar.MoneyPerSec:
			Var.Value = Main.IdleMoneyPerSec;
			break;
		case SetToPreVar.AutoTap:
			Var.Value = (decimal) Main.TapPerSec;
			break;
		}
	}

	public void Load()
	{
		eSave Saver = new eSave();
		if(!Saver.isSaved("Money"))
			return;
		for(int i = 0;i<Variables.Length;i++)
		{
			if(Variables[i].Save)
			{
				Variables[i].Value = Saver.LoadDecimal(Variables[i].Name+GetInstanceID());
				Variables[i].ValueOnLoad = Variables[i].Value;
			}
		}
	}

	void AdvHit()
	{
		for(int i = 0;i<Variables.Length;i++)
		{
			AdvRecieveMessages(Variables[i],DoOn.OnProjHit);
		}
		AdvRawMessage(SendOn.OnProjHit);
	}

	void AdvTouch()
	{
		for(int i = 0;i<Variables.Length;i++)
		{
			AdvRecieveMessages(Variables[i],DoOn.OnTouch);
		}
		AdvRawMessage(SendOn.OnTouch);
	}

	void AdvAutoTap()
	{
		for(int i = 0;i<Variables.Length;i++)
		{
			AdvRecieveMessages(Variables[i],DoOn.AutoTap);
		}
		AdvRawMessage(SendOn.AutoTap);
	}

	void AdvRawMessage(SendOn Type)
	{
		for(int i = 0;i<Variables.Length;i++)
		{
			for(int j = 0;j<Variables[i].SendMessages.Length;j++)
			{
				if(Variables[i].SendMessages[j].On == Type)
				{
					if(Type == SendOn.ValueEqual)
					{
						if(!Variables[i].SendMessages[j].VarInstead)
							if(Variables[i].Value == System.Convert.ToDecimal(Variables[i].SendMessages[j].Value))
								AdvSendMessage(Variables[i].SendMessages[j].MessageName);
						else
						{
							for(int k = 0;k<Variables[i].SendMessages[j].Of.Variables.Length;k++)
							{
								if(Variables[i].SendMessages[j].Of.Variables[k].Name == Variables[i].SendMessages[j].RecieveName)
									if(System.Convert.ToDecimal(Variables[i].SendMessages[j].Value) == Variables[i].SendMessages[j].Of.Variables[k].Value)
										AdvSendMessage(Variables[i].SendMessages[j].MessageName);
							}
						}
					}
					else if(Type == SendOn.ValueGreator)
					{
						if(!Variables[i].SendMessages[j].VarInstead)
							if(Variables[i].Value > System.Convert.ToDecimal(Variables[i].SendMessages[j].Value))
								AdvSendMessage(Variables[i].SendMessages[j].MessageName);
						else
						{
							for(int k = 0;k<Variables[i].SendMessages[j].Of.Variables.Length;k++)
							{
								if(Variables[i].SendMessages[j].Of.Variables[k].Name == Variables[i].SendMessages[j].RecieveName)
									if(System.Convert.ToDecimal(Variables[i].SendMessages[j].Value) >= Variables[i].SendMessages[j].Of.Variables[k].Value)
										AdvSendMessage(Variables[i].SendMessages[j].MessageName);
							}
						}
					}
					else if(Type == SendOn.ValueLesser)
					{
						if(!Variables[i].SendMessages[j].VarInstead)
							if(Variables[i].Value < System.Convert.ToDecimal(Variables[i].SendMessages[j].Value))
								AdvSendMessage(Variables[i].SendMessages[j].MessageName);
						else
						{
							for(int k = 0;k<Variables[i].SendMessages[j].Of.Variables.Length;k++)
							{
								if(Variables[i].SendMessages[j].Of.Variables[k].Name == Variables[i].SendMessages[j].RecieveName)
									if(System.Convert.ToDecimal(Variables[i].SendMessages[j].Value) <= Variables[i].SendMessages[j].Of.Variables[k].Value)
										AdvSendMessage(Variables[i].SendMessages[j].MessageName);
							}
						}
					}
					else if(Type == SendOn.Message)
					{
						for(int k = 0;k<Messages.Count;k++)
						{
							if(Messages[k] == Variables[i].SendMessages[j].RecieveName)
							{
								AdvSendMessage(Variables[i].SendMessages[j].MessageName);
								Messages.RemoveAt(k);
							}
						}
					}
					else
					{
						AdvSendMessage(Variables[i].SendMessages[j].MessageName);
					}
				}
			}
		}
	}

	void AdvDamage()
	{
		for(int i = 0;i<Variables.Length;i++)
		{
			AdvRecieveMessages(Variables[i],DoOn.OnDamage);
		}
		AdvRawMessage(SendOn.OnDamage);
	}

	void AdvKill()
	{
		for(int i = 0;i<Variables.Length;i++)
		{
			AdvRecieveMessages(Variables[i],DoOn.Kill);
		}
		AdvRawMessage(SendOn.Kill);
	}

	void AdvSave()
	{
		for(int i = 0;i<Variables.Length;i++)
		{
			AdvRecieveMessages(Variables[i],DoOn.Save);
		}
		Save();
	}

	void AdvNewLvl()
	{
		for(int i = 0;i<Variables.Length;i++)
		{
			AdvRecieveMessages(Variables[i],DoOn.NewLevelUnlock);
		}
		AdvRawMessage(SendOn.NewLevelUnlock);
	}

	void AdvLvlChange()
	{
		for(int i = 0;i<Variables.Length;i++)
		{
			AdvRecieveMessages(Variables[i],DoOn.LevelChange);
		}
		AdvRawMessage(SendOn.LevelChange);
	}

	void AdvUpdate()
	{
		for(int i = 0;i<Variables.Length;i++)
		{
			AdvRecieveMessages(Variables[i],DoOn.Update);
			AdvRecieveMessages(Variables[i],DoOn.DeltaTime);
			if(Variables[i].ToInitialVar.On == SetOn.Update)
				SetToVar(Variables[i]);
		}
		AdvRawMessage(SendOn.Update);
		AdvRawMessage(SendOn.ValueEqual);
		AdvRawMessage(SendOn.ValueLesser);
		AdvRawMessage(SendOn.ValueGreator);
		AdvRawMessage(SendOn.Message);
		AdvanceControll();
	}

	void AdvanceControll()
	{
		for(int i = 0;i<Variables.Length;i++)
		{
			if(Variables[i].TextField)
			{
				if(Variables[i].NumberController)
					Variables[i].TextField.text = Variables[i].NumberController.ShortNumber(Variables[i].Value);
				else
					Variables[i].TextField.text = Variables[i].Value.ToString();
			}
		}
	}

	public void AdvRecieveMessages(IdleVar Var,DoOn On)
	{
		for(int i = 0;i<Var.Effectors.Length;i++)
		{
			if(Var.Effectors[i].On == On && Var.Effectors[i].On != DoOn.Message && Var.Effectors[i].On != DoOn.DeltaTime)
				AdvEffect(Var,Var.Effectors[i]);
			else if(Var.Effectors[i].On == DoOn.DeltaTime)
				AdvEffect(Var,Var.Effectors[i],true);
			else if(Var.Effectors[i].On == DoOn.Message)
			{
				for(int k = 0;k<Messages.Count;k++)
				{
					if(Messages[k] == Var.Effectors[i].MessageName)
					{
						AdvEffect(Var,Var.Effectors[i]);
						Messages.RemoveAt(k);
					}
				}
			}
		}
	}

	public void AdvEffect(IdleVar Var,Effect eff,bool DeltaTime = false)
	{
		decimal Value = 0;
		bool Done = false;
		if(eff.VarInstead)
		{
			for(int i = 0;i<eff.SetToOf.Variables.Length;i++)
			{
				if(eff.SetToOf.Variables[i].Name == eff.VariableName)
				{
					Value = Variables[i].Value;
					Done = true;
				}
			}
		}
		else
			Value = System.Convert.ToDecimal(eff.By);
		if(eff.On == DoOn.Inactive)
		{
			if(!PlayerPrefs.HasKey("LastPlayTime"))
				return;
			System.TimeSpan LastTimePlayed = System.DateTime.Now-(System.Convert.ToDateTime(PlayerPrefs.GetString("LastPlayTime")));
			Value *= (decimal) LastTimePlayed.TotalSeconds;
		}
		if(!Done && eff.VarInstead)
		{
			Debug.LogError("WARNING: No variable of name '" + eff.VariableName + "' was found in the instance." +
			               " Please make sure everything is properly set. Value will be used as the By Value now" +
			               " This occoured in instance name '" + gameObject.name +"'");
			Value = System.Convert.ToDecimal(eff.By);
		}

		if(DeltaTime)
			Value *= (decimal) Time.deltaTime;

		switch(eff.Type)
		{
		case CalcType.Add:
			Var.Value += Value;
			break;
		case CalcType.Divide:
			Var.Value /= Value;
			break;
		case CalcType.Multiply:
			Var.Value *= Value;
			break;
		case CalcType.Subtract:
			Var.Value -= Value;
			break;
		case CalcType.SetTo:
				Var.Value = Value;
			break;
		}
		#if UNITY_EDITOR
		Var.InitialValue = Var.Value.ToString();
		#endif
	}

	public void AdvSendMessage(string Name)
	{
		Messages.Add(Name);
	}

}