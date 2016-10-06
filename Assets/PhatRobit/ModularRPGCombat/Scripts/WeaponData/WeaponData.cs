public enum WeaponType
{
	Melee,
	Ranged
}

public enum RangedType
{
	None    = 0,
	Gun     = 1,
	Bow     = 2,
	Spell   = 3,
	AOESpell= 4
}

// Holds information about our currently equipped weapon
public class WeaponData
{
	private WeaponType _weaponType;	// The type of weapon
	private RangedType _rangedType;	// The type of ranged weapon
	private int _weaponDamage;		// The weapon damage
	private float _weaponSpeed;		// The weapon speed

	// Default values
	public WeaponData()
	{
		_weaponType = WeaponType.Melee;
		_weaponDamage = 0;
		_weaponSpeed = 0;
	}

	// Custom values
	public WeaponData(WeaponType weaponType, int weaponDamage, float weaponSpeed, RangedType rangedType = RangedType.None)
	{
		_weaponType = weaponType;
		_rangedType = rangedType;
		_weaponDamage = weaponDamage;
		_weaponSpeed = weaponSpeed;
	}

	// Getters / Setters
	public WeaponType WeaponType
	{
		get { return _weaponType; }
		set { _weaponType = value; }
	}

	public RangedType RangedType
	{
		get { return _rangedType; }
		set { _rangedType = value; }
	}

	public int WeaponDamage
	{
		get { return _weaponDamage; }
		set { _weaponDamage = value; }
	}

	public float WeaponSpeed
	{
		get { return _weaponSpeed; }
		set { _weaponSpeed = value; }
	}

	// Overrides
	public override string ToString()
	{
		return "[" + base.ToString() + "] [Type: " + WeaponType + "] [Damage: " + WeaponDamage + "] [Speed: " + WeaponSpeed + "]";
	}
}