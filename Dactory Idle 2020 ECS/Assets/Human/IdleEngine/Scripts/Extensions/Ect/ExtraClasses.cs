using UnityEngine;
using System.Collections;

[System.Serializable]
public class IdleVar
{
	public string Name;
	public string InitialValue;
	public decimal Value;
	public TextMesh TextField;
	public BigNumbers NumberController;
	public bool Save;
	[Tooltip("This is the value that this var will be effected by")]
	public Effect[] Effectors;
	public MessageSender[] SendMessages;
	public SetToInitialVar ToInitialVar;
	public decimal ValueOnLoad;
}
[System.Serializable]
public class Effect
{
	public string Name;
	[Tooltip("By how much will the it be effected")]
	public string By;
	[Tooltip("The name of the message when this action will be performed.")]
	public string MessageName;
	[Tooltip("This is the name of the variable to use instead")]
	public string VariableName;
	[Tooltip("If this is checked a variable will be used Instead of const value")]
	public bool VarInstead;
	[Tooltip("Do this action on what type of event")]
	public DoOn On;
	[Tooltip("The type of calculation to perform")]
	public CalcType Type;
	[Tooltip("The variable of this isntance will be set to.")]
	public IdleVarController SetToOf;
}
[System.Serializable]
public class MessageSender
{
	[Tooltip("The name of the message to be sent")]
	public string MessageName;
	[Tooltip("The name of the message to be recieved to send this message")]
	public string RecieveName;
	[Tooltip("The value that will be comapred to the parent value to send the message")]
	public string Value;
	[Tooltip("The name of the variable to use instead of static valu")]
	public string VarName;
	[Tooltip("If this is checked then only variable be used instead of static var")]
	public bool VarInstead;
	[Tooltip("When to send this message.")]
	public SendOn On;
	[Tooltip("The Instance of the variable using instead of static value")]
	public IdleVarController Of;
}
public enum CalcType
{
	Add,Subtract,Multiply,Divide,SetTo
}
public enum DoOn
{
	Message,DeltaTime,AutoTap,Update,Start,Save,LevelChange,NewLevelUnlock,
	OnTouch,OnDamage,OnProjHit,Kill,Inactive
}
public enum SendOn
{
	Message,AutoTap,Update,Start,Save,LevelChange,NewLevelUnlock,
	OnTouch,OnDamage,OnProjHit,ValueLesser,ValueGreator,ValueEqual,Kill
}
public enum SetOn
{
	Never,Start,Update
}
public enum SetToPreVar
{
	MoneyMultiplier,DamageMultiplier,DamageDone,GemChances,AutoTap,MoneyPerSec
}
[System.Serializable]
public struct SetToInitialVar
{
	public SetToPreVar To;
	public  SetOn On;
}