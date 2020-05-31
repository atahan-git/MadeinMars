using UnityEngine;
using System.Collections;

public class TextFieldsController : MonoBehaviour
{
	public IdleEngineMain Main;
	public BigNumbers BigNumber;
	public RandomGem GemController;
	public TextMesh MoneyText,ScoreText,NameText;
	public TextMesh RewardPriceText,RewardScoreText;
	public TextMesh InitialHealthText,CurrentHealthText;
	public TextMesh ArmorText,CurrentLevelText,DamageDealthText;
	public TextMesh TapPerSecText,MoneyPerSecText,DamageMultiplierText,MoneyMultiplierText;
	public TextMesh	GemsText;

	void Update()
	{
		ControllTextFields();
	}
	void ControllTextFields()
	{
		try
		{
			if(MoneyText) MoneyText.text = BigNumber.ShortNumber(Main.Money);
			if(ScoreText) ScoreText.text = BigNumber.ShortNumber(Main.Score);
			if(RewardPriceText) RewardPriceText.text = "$ " + BigNumber.ShortNumber ((Main.Item.Price*Main.MoneyMultiplier));
			if(RewardScoreText) RewardScoreText.text = BigNumber.ShortNumber(Main.Item.PriceScore);
			if(InitialHealthText) InitialHealthText.text = BigNumber.ShortNumber(Main.Item.InitialHP);
			if(CurrentHealthText) CurrentHealthText.text = BigNumber.ShortNumber(Main.Item.HP);
			if(ArmorText) ArmorText.text = BigNumber.ShortNumber(Main.Item.Toughness);
			if(CurrentLevelText) CurrentLevelText.text = Main.CurrentItem+1.ToString();
			if(TapPerSecText) TapPerSecText.text = Main.TapPerSec.ToString();
			if(MoneyPerSecText) MoneyPerSecText.text = BigNumber.ShortNumber(Main.IdleMoneyPerSec);
			if(DamageMultiplierText) DamageMultiplierText.text = BigNumber.ShortNumber(Main.DamageMultiplier).ToString();
			if(MoneyMultiplierText) MoneyMultiplierText.text = BigNumber.ShortNumber(Main.DamageMultiplier).ToString();
			if(NameText) NameText.text = Main.Item.Name;
			if(DamageDealthText) DamageDealthText.text = BigNumber.ShortNumber(Main.Item.DamageApplied);
			if(GemsText) GemsText.text = GemController.Gems.ToString();
		}
		catch(System.NullReferenceException)
		{

		}
	}

}