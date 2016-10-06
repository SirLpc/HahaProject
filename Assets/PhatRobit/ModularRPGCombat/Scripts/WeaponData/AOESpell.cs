using UnityEngine;
using System.Collections;

public class AOESpell : MonoBehaviour
{
	public float forwardForce = 50;							// How much force to be applied to the forward direction of the spell
	public float upForce = 10;								// How much force to be applied to the up direction of the spell
	public float lifeSpan = 5;								// How long the spell lasts (in seconds) before being destroyed
	public int manaAmount = 20;								// How much Mana does the spell use?
	public GameObject aoe;									// The AOE Game Object
	
	private float _life = 0;								// Timer for spell before being destroyed
	
	private PlayerCombatController _combatController;		// Reference to the player's combat information
	private PlayerStatController _statController;			// Reference to the player's stat information
	private GameObject _player;								// Reference to the Player.
	
	private Transform _t;									// Reference to gameobject's own transform
	
	public PlayerCombatController CombatController
	{
		get { return _combatController; }
		set { _combatController = value; }
	}

	void Start()
	{
		_t = transform;
		
		// Set up our life timer
		_life = lifeSpan;


		_player = GameObject.FindGameObjectWithTag(Tags.player);
		_statController = _player.GetComponent<PlayerStatController>();
		
		if(_statController.mana < manaAmount)	// Check if player got enough Mana to cast spell
		{
			Debug.Log ("Not enough Mana!");
			Destroy(gameObject);
		}
		
		else
		{
			_statController.TakeMana(manaAmount); // Retract Mana
		}
		
		// Apply initial force
		GetComponent<Rigidbody>().AddForce(forwardForce * _t.forward);
		GetComponent<Rigidbody>().AddForce(upForce * _t.up);
	}
	
	void Update()
	{
		// Destory object when life runs out
		if(_life <= 0)
		{
			Destroy(gameObject);
		}
		else
		{
			_life -= Time.deltaTime;
		}
	}
	
	void FixedUpdate()
	{
		// Rotate the spell so it is always facing the direction it is moving
		if(GetComponent<Rigidbody>().velocity != Vector3.zero)
		{
			GetComponent<Rigidbody>().rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity);
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		// Deal damage to enemies if colliding with them and destroy gameobject
		if(_combatController)
		{
			if(other.CompareTag("Terrain"))
			{
				Debug.Log ("Instantiate AOE!");
				GameObject go = (GameObject)Instantiate(aoe, gameObject.transform.position, Quaternion.identity);

				AOE goAOE = go.GetComponent<AOE>();
				
				if(goAOE)
				{
					goAOE.CombatController = _combatController;
				}

			}
		}
	}
}
