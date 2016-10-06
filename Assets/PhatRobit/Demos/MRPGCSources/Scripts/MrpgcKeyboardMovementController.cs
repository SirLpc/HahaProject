using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class MrpgcKeyboardMovementController : MonoBehaviour
{
	public KeyCode walkKey = KeyCode.RightShift;	// Key to toggle walking on/off
	public KeyCode jumpKey = KeyCode.LeftShift;		// Key to jump

	public float jumpHeight = 8;					// Height to jump
	public float gravity = 2;

	private CharacterController _controller;		// Reference to the CharacterController
	private Animator _animator;						// Reference to the Animator
	private HashIDs _hash;							// Reference to the HashIDs

	private Transform _t;							// Reference to gameObject's transform

	private float _direction = 0;					// Direction player is turning relative to the camera
	private float _angle = 0;						// Angle for pivoting
	private Vector3 _input = new Vector3();			// Player input
	private Vector3 _hitNormal;						// The normal of the CharacterController's collision points

	private bool _walking = false;					// Is the player walking?
	private bool _grounded = false;					// Is the player on the ground?

	private int _idlePivotLeft = 0;
	private int _idlePivotRight = 0;
	private int _idlePivotTransLeft = 0;
	private int _idlePivotTransRight = 0;
	private int _locoPivotLeft = 0;
	private int _locoPivotRight = 0;
	private int _locoPivotTransLeft = 0;
	private int _locoPivotTransRight = 0;

	private AnimatorStateInfo _stateInfo;
	private AnimatorTransitionInfo _transInfo;

	void Awake()
	{
        Application.targetFrameRate = 45;

		GameObject hash = GameObject.FindWithTag(Tags.gameController);

		if(hash)
		{
			_hash = hash.GetComponent<HashIDs>();
		}
	}

	void Start()
	{
		_t = transform;
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController>();

		_idlePivotLeft = Animator.StringToHash("Base Layer.Idle Pivot Left");
		_idlePivotRight = Animator.StringToHash("Base Layer.Idle Pivot Right");
		_idlePivotTransLeft = Animator.StringToHash("Base Layer.Idle -> Base Layer.Idle Pivot Left");
		_idlePivotTransRight = Animator.StringToHash("Base Layer.Idle -> Base Layer.Idle Pivot Right");
		_locoPivotLeft = Animator.StringToHash("Base Layer.Locomotion Pivot Left");
		_locoPivotRight = Animator.StringToHash("Base Layer.Locomotion Pivot Right");
		_locoPivotTransLeft = Animator.StringToHash("Base Layer.Locomotion -> Base Layer.Locomotion Pivot Left");
		_locoPivotTransRight = Animator.StringToHash("Base Layer.Locomotion -> Base Layer.Locomotion Pivot Right");
	}

	void Update()
	{
		if(_hash)
		{
			_stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
			_transInfo = _animator.GetAnimatorTransitionInfo(0);

			if(Input.GetKeyDown(walkKey))
			{
				_walking = !_walking;
			}

#if UNITY_EDITOR
            _input.x = Input.GetAxis("Horizontal");
			_input.y = Input.GetAxis("Vertical");
#else
            _input.x = CrossPlatformInputManager.GetAxis("Horizontal");
			_input.y = CrossPlatformInputManager.GetAxis("Vertical");
#endif
            //StickToWorldspace();
            transform.LookAt(transform.position + new Vector3(_input.x, 0, _input.y));

            float speed = _input.sqrMagnitude >= 0.00001 ? 1 : 0;
            

			if(_walking)
			{
				speed /= 4f;
			}

			if(!_grounded)
			{
				speed = 0;
			}

			//_animator.SetFloat(_hash.Speed, speed, 0.05f, Time.deltaTime);
            _animator.SetFloat(_hash.Speed, speed);

			//if(speed < 0.1f)
			//{
			//	_direction = 0;
			//}

			//_animator.SetFloat(_hash.Direction, _direction, 0.05f, Time.deltaTime);
            //_animator.SetFloat(_hash.Direction, _direction);

			//if(!IsPivoting())
			//{
			//	if(speed > 0.1f)
			//	{
			//		_animator.SetFloat(_hash.Angle, _angle);
			//	}
			//	else
			//	{
			//		_animator.SetFloat(_hash.Direction, 0);
			//		_animator.SetFloat(_hash.Angle, 0);
			//	}
			//}

			if(_controller.isGrounded)
			{
#if UNITY_EDITOR
                if(Input.GetKey(jumpKey))
#else
                if (CrossPlatformInputManager.GetButtonDown("Jump"))
#endif
                {
					_hitNormal.y = jumpHeight;
				}
			}

			if(!_controller.isGrounded)
			{
				_hitNormal.y -= gravity * Time.deltaTime;
			}
			else
			{
				_hitNormal.y -= 0.75f;
			}

			if(_hitNormal != Vector3.zero)
			{
				_controller.Move(_hitNormal * Time.deltaTime);
			}

			// Apply gravity to avoid bouncing down sloped terrain
			//_controller.Move(Physics.gravity * Time.deltaTime);
		}
	}

	private void StickToWorldspace()
	{
		Vector3 rootDirection = _t.forward;

		// Get camera direction
		Vector3 cameraDirection = Camera.main.transform.forward;
		cameraDirection.y = 0;
		Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, cameraDirection);

		// Convert joystick input in worldspace coords
		Vector3 moveDirection = referentialShift * new Vector3(_input.x, 0, _input.y);
		Vector3 axisSign = Vector3.Cross(moveDirection, rootDirection);

		float angleRootToMove = Vector3.Angle(rootDirection, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);

		if(!IsPivoting())
		{
			_angle = angleRootToMove;
		}

		angleRootToMove /= 180f;
		_direction = angleRootToMove * 3;
	}

	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		_hitNormal = Vector3.zero;

		// This keeps the player from sticking to walls
		float angle = hit.normal.y * 90;

		if(angle < _controller.slopeLimit)
		{
			_grounded = false;
			_hitNormal = new Vector3(hit.normal.x, 0, hit.normal.z);
		}
		else
		{
			_grounded = true;
		}
	}

	private bool IsPivoting()
	{
		return _stateInfo.nameHash == _idlePivotLeft ||
			   _stateInfo.nameHash == _idlePivotRight ||
			   _transInfo.nameHash == _idlePivotTransLeft ||
			   _transInfo.nameHash == _idlePivotTransRight ||
			   _stateInfo.nameHash == _locoPivotLeft ||
			   _stateInfo.nameHash == _locoPivotRight ||
			   _transInfo.nameHash == _locoPivotTransLeft ||
			   _transInfo.nameHash == _locoPivotTransRight;
	}
}