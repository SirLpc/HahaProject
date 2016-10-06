using UnityEngine;
using System.Collections;

public class MrpgcCameraController : MonoBehaviour
{
	public Transform target;
	public Vector3 targetOffset = new Vector3();
	public Vector2 rotationSensitivity = new Vector2(7, 7);
	public float minAngle = -85;
	public float maxAngle = 85;
	public float rotationSmoothing = 10;
	public float distance = 5;
	public float minDistance = 1;
	public float maxDistance = 15;
	public float zoomSpeed = 1;
	public float zoomSmoothing = 10;

	private Vector3 _angle = new Vector3();
	private Quaternion _oldRotation = new Quaternion();
	private float _oldDistance = 0;

	private Transform _t;

	void Start()
	{
		_t = transform;
		_oldRotation = _t.rotation;
		_oldDistance = distance;
	}

	void Update()
	{
		if(target)
		{
			if(Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
			{
				_angle.x += Input.GetAxis("Mouse X") * rotationSensitivity.x;
				_angle.y = Mathf.Clamp(_angle.y - Input.GetAxis("Mouse Y") * rotationSensitivity.y, minAngle, maxAngle);
			}

			float scrollDirection = Input.GetAxis("Mouse ScrollWheel");

			distance = Mathf.Clamp(distance + (scrollDirection != 0 ? (scrollDirection < 0 ? zoomSpeed : -zoomSpeed) : 0), minDistance, maxDistance);
		}
	}

	void LateUpdate()
	{
		if(target)
		{
			Quaternion angleRotation = Quaternion.Euler(_angle.y, _angle.x, 0);
			Quaternion currentRotation = Quaternion.Lerp(_oldRotation, angleRotation, Time.deltaTime * rotationSmoothing);

			_oldRotation = currentRotation;

			float currentDistance = Mathf.Lerp(_oldDistance, distance, Time.deltaTime * zoomSmoothing);

			_oldDistance = currentDistance;

			Vector3 focalPoint = target.position + target.rotation * targetOffset;

			_t.position = focalPoint - currentRotation * Vector3.forward * currentDistance;
			_t.LookAt(focalPoint, Vector3.up);
		}
	}
}