using UnityEngine;
using System.Collections;

public class HashIDs : MonoBehaviour
{
	// Here we store the hash tags for various strings used in our animators.
	private int _dyingState;
	private int _deadBool;
	private int _speedFloat;
	private int _playerInSightBool;
	private int _angularSpeedFloat;
	private int _directionFloat;
	private int _angleFloat;

	public int Speed
	{
		get { return _speedFloat; }
	}

	public int AngleSpeed
	{
		get { return _angularSpeedFloat; }
	}

	public int Direction
	{
		get { return _directionFloat; }
	}

	public int Angle
	{
		get { return _angleFloat; }
	}

	public int PlayerInSight
	{
		get { return _playerInSightBool; }
	}

	public int Dying
	{
		get { return _dyingState; }
	}

	public int Dead
	{
		get { return _deadBool; }
	}

	void Start()
	{
		_dyingState = Animator.StringToHash("Base Layer.Dying");
		_deadBool = Animator.StringToHash("Dead");
		_speedFloat = Animator.StringToHash("Speed");
		_playerInSightBool = Animator.StringToHash("PlayerInSight");
		_angularSpeedFloat = Animator.StringToHash("AngularSpeed");
		_directionFloat = Animator.StringToHash("Direction");
		_angleFloat = Animator.StringToHash("Angle");
	}
}