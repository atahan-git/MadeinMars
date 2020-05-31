using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("Human/Idle Engine/Idle Item")]
public class IdleItem : MonoBehaviour
{
	public string Name;
	public string RewardScore,RewardPrice,Health,Armor;
	public decimal Price,PriceScore,HP,Toughness,InitialHP;
	public IdleEngineMain Main;
	public string ProjectileTag;
	public AudioClip TapSound;
	public AudioClip KilledSound;
	AudioSource _AS;
	[HideInInspector]
	public decimal DamageApplied;
	public delegate void _OnDeadDamage();
	public delegate void _OnInitilize();
	public delegate void _OnDeath();
	public delegate void _OnTouch();
	public delegate void _OnProjectileHit();
	public static event _OnInitilize OnInitilize;
	public static event _OnDeadDamage OnDealDamage;
	public static event _OnDeath OnDeath;
	public static event _OnTouch OnTouch;
	public static event _OnProjectileHit OnProjectileHit;
	decimal TempLastHP;

	public void Deal_Damage(decimal DamageDone, decimal Armors)
	{
		decimal Damage = DamageDone;
		// If you want another touch algorith enter it here.
		if(Armors > 1)
		{
			Damage = (Damage/(Armors));
		}

		if(Damage <= 0)
		{
			Damage = 0;
		}
		TempLastHP = HP;
		HP -= Damage;
		DamageApplied = TempLastHP-HP;
		if(OnDealDamage != null)
		{
			OnDealDamage();
		}
	}
	Utilities util;

	void Awake()
	{
		Initilize();
	}

	void Update()
	{
		CheckTouch ();
		CheckDeath();
	}
	void CheckDeath()
	{
		if(HP <= 0)
		{
			HP = InitialHP;
			Main.Money += Price*Main.MoneyMultiplier;
			Main.Score += PriceScore;
			if(KilledSound)
			{
				_AS.clip = KilledSound;
				_AS.Play();
			}
			if(OnDeath != null)
			{
				OnDeath();
			}
		}
	}

	void CheckTouch()
	{
		if(TouchScript.TouchedObject == this.gameObject)
		{
			Deal_Damage(Main.Damage*Main.DamageMultiplier,Toughness);
			TouchScript.TouchedObject = null;
			if(TapSound)
			{
				_AS.clip = TapSound;
				_AS.Play();
			}
			if(OnTouch != null)
			{
				OnTouch();
			}
		}
	}

	void Initilize()
	{
		_AS = GetComponent<AudioSource>();
		util = new Utilities();
		Price = util.ConvertToDecimal(RewardPrice,"IdleItem Script of " + gameObject.name);
		PriceScore = util.ConvertToDecimal(RewardScore,"IdleItem Script of " + gameObject.name);
		HP = util.ConvertToDecimal(Health,"IdleItem Script of " + gameObject.name);
		InitialHP = HP;
		Toughness = util.ConvertToDecimal(Armor,"IdleItem Script of " + gameObject.name);
		if(OnInitilize != null)
		{
			OnInitilize();
		}
	}

	void OnTriggerEnter(Collider hit)
	{
		if(hit.gameObject.tag == ProjectileTag)
		{
			Deal_Damage(Main.Damage*Main.DamageMultiplier,Toughness);
			Destroy(hit.gameObject);
			if(OnProjectileHit != null)
			{
				OnProjectileHit();
			}
		}
	}

}