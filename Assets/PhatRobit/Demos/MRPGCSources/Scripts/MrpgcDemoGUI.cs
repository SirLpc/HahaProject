using UnityEngine;
using System.Collections;

public class MrpgcDemoGUI : MonoBehaviour
{
	private Rect _readmeRect;
	private Rect _playerRect;

	private PlayerStatController _playerStatController;
	private PlayerCombatController _playerCombatController;

	private string _version = "1.0";
    [SerializeField]
	private bool _readme = true;
    [SerializeField]
	private bool _autoAim = false;
	private bool _targetSwitch = false;
	private bool _noAim = false;

	void Awake()
	{
		GameObject player = GameObject.FindWithTag("Player");

		if(player)
		{
			_playerStatController = player.GetComponent<PlayerStatController>();
			_playerCombatController = player.GetComponent<PlayerCombatController>();
		}
	}

	void Start()
	{
		_readmeRect = new Rect(10, 30, Screen.width - 20, 32);
		_playerRect = new Rect(10, 20, 150, 32);
	}

	void Update()
	{
	    if (_playerCombatController == null)
	        return;

		if(_autoAim)
		{
			_playerCombatController.autoAimMelee = true;
			_playerCombatController.autoAimRanged = true;
			_noAim = false;
			_targetSwitch = false;
		}

		if(_targetSwitch)
		{
			_playerCombatController.targetSwitch = true;
			_autoAim = false;
			_noAim = false;
		}

		if(_noAim)
		{
			_autoAim = false;
			_targetSwitch = false;
		}


		if(!_autoAim)
		{
			_playerCombatController.autoAimMelee = false;
			_playerCombatController.autoAimRanged = false;
		}

		if(!_targetSwitch)
		{
			_playerCombatController.targetSwitch = false;
		}


		if(_readme)
		{
			_readmeRect = new Rect(10, Screen.height / 2f - _readmeRect.height / 2f, _readmeRect.width, _readmeRect.height);
		}
	}

	void OnGUI()
	{
		GUILayout.Label("v" + _version);

		if(_readme)
		{
			_readmeRect = GUILayout.Window(0, _readmeRect, Readme, "Readme");
		}
		else
		{
			if(_playerStatController)
			{
				_playerRect = GUILayout.Window(1, _playerRect, PlayerInformation, "Player Info");
			}
		}
	}

	private void Readme(int id)
	{
		GUILayout.Label("Hello!");
		GUILayout.Label("Thanks for trying out the demo for Modular RPG Combat! Your player information will be displayed at the top left corner of the screen.");

		GUILayout.Space(12);

		GUILayout.Label("Here are most of the default controls:");

		GUILayout.Label("Camera Rotation: Mouse Left / Middle / Right buttons + drag");
		GUILayout.Label("Camera Zoom: Mouse ScrollWheel");
		GUILayout.Label("Movement: WSAD");
		GUILayout.Label("Jump: J");
		GUILayout.Label("Toggle walk / run: Right Shift");
		GUILayout.Label("Attack: Spacebar");
		GUILayout.Label("Special Attack (2x damage, only usable with Melee weapon): Left Shift");
		GUILayout.Label("Equip Sword: 1");
		GUILayout.Label("Equip Gun: 2");
		GUILayout.Label("Equip Bow: 3");
		GUILayout.Label("Equip Fireball: 4");
		GUILayout.Label("Equip Firefield (AOE): 5");
		GUILayout.Label("Target the closest enemy: TAB");

		if(GUILayout.Button("Got it!"))
		{
			_readme = false;
		}
	}

	private void PlayerInformation(int id)
	{
		GUILayout.BeginHorizontal();

		GUILayout.BeginVertical();
		GUILayout.Label("Health:");
		GUILayout.Label("Mana:");
		GUILayout.Label("Level:");
		GUILayout.Label("Experience:");

		GUILayout.EndVertical();

		GUILayout.BeginVertical();
		GUILayout.Label(_playerStatController.health + "/" + _playerStatController.maxHealth);
		GUILayout.Label(_playerStatController.mana + "/" + _playerStatController.maxMana);
		GUILayout.Label(_playerStatController.level.ToString());
		GUILayout.Label(_playerStatController.experience + "/" + _playerStatController.maxExperience);
		GUILayout.EndVertical();

		GUILayout.EndHorizontal();

		_autoAim = GUILayout.Toggle(_autoAim, "Auto Aim");
		_targetSwitch = GUILayout.Toggle(_targetSwitch, "Target Switching");
	}
}