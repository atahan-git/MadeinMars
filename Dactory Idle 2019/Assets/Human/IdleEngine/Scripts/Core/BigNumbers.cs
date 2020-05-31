// The namespaces we are using
using UnityEngine;
using System.Collections;
using System;

// Ads a option to add this script thtough component menu
[AddComponentMenu("Human/Utilities/Big Numbers")]
public class BigNumbers : MonoBehaviour
{	
	[Tooltip("If this limit is crossed the result will be blank")]
	[SerializeField]// Makes an private field visible in inspector.
	string MaxLimit = decimal.MaxValue.ToString();// The max limit of the script to handle. After this the result will be orignal number with no symbol
	[SerializeField]// Makes an private field visible in inspector.
	StringSettings[] Settings;// Settings of shortning numbers
	decimal Limit;// The maxLimit converted to decimal for calculations.
	void Awake()
	{
		Utilities util = new Utilities();
			for(int i = 0;i<Settings.Length;i++)
			{	// This is done so we wont always Convert string to decimal as it could effect performance.
				Settings[i].ChangeValue = util.ConvertToDecimal(Settings[i].ValueOnSymbolChange);
				Settings[i].Divider = util.ConvertToDecimal(Settings[i].DivideValue);
			}
			// The maxLimit converted to decimal for calculations.
			Limit = util.ConvertToDecimal (MaxLimit);
		// Executes if an error occoured

	}
	// The function you need to call
	public string ShortNumber(decimal Money)
	{
		// The output that will be returned.
		string Output = "";
		// The MinimumRange and MaxRange of number that in needs to be in for the symbol to apply
		decimal MinRange;// It is the value entered in ValueOfSymbolChange in Settings
		decimal MaxRange;// It is the value entered in the next ValueOfSymbolChange in Settings
		for(int i = 0;i<Settings.Length;i++)
		{
			MinRange = Settings[i].ChangeValue;//Sets the MinRange
			if(i != Settings.Length-1) // Checks if 'i' is not the last value of loop as then it would give a Array index out of range error
			{	
				MaxRange = Settings[i+1].ChangeValue;//Sets The MaxRange
			}
			else
			{
				MaxRange = Limit;//Sets the max Range to limit if the 'i' is last so it would not give blank result
			}
			if(Money >= MinRange && Money < MaxRange)// Finally checks if entered money is in any range of the settings
			{
				Output = (Money/Settings[i].Divider).ToString("F" + Settings[i].DecimalPlaces.ToString()) + Settings[i].symbol;// Sets the output variable
			}
		}

		return Output;// Finally returns output giving you the result.
	}
	// The struct that will hold the settings data
	[Serializable]
	public struct StringSettings
	{
		public string Name;//Name of current currency
		[SerializeField]// Makes an private field visible in inspector.
		public int DecimalPlaces;// The number of decimal places in the short numbers
		public string ValueOnSymbolChange;// The value on which this symbol will apply
		public string DivideValue;// The value the number will be divided to make it short
		public string symbol;// The symbol
		[HideInInspector]
		[SerializeField]
		public decimal ChangeValue;// This is set at start to ValueOfSymbolChange so we will not need to calculate it each time in loop
		[HideInInspector]
		[SerializeField]
		public decimal Divider;// The value by which the number will be divided when it reached this range.
	}
}
