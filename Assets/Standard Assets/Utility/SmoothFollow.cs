using UnityEngine;

namespace UnityStandardAssets.Utility
{
	public class SmoothFollow : MonoBehaviour
	{

		// The target we are following
		private Transform _target, _henchman;
		// The distance in the x-z plane to the target
		[SerializeField]
		private float distance = 10.0f;
		// the height we want the camera to be above the target
		[SerializeField]
		private float height = 5.0f;

		[SerializeField]
		private float rotationDamping;
		[SerializeField]
		private float heightDamping;

        private float _targetOriginY;

		// Use this for initialization
		public void InitFollow(Transform target, Transform henchman)
		{
		    _henchman = henchman;
		    _target = target;
            _targetOriginY = target.position.y;
        }

		// Update is called once per frame
		void LateUpdate()
		{
			// Early out if we don't have a target
			if (!_target)
				return;

			// Calculate the current rotation angles
			var wantedRotationAngle = _target.eulerAngles.y;
            //var wantedHeight = target.position.y + height;
            var wantedHeight = _targetOriginY + height;

            var currentRotationAngle = _henchman.eulerAngles.y;
			var currentHeight = _henchman.position.y;

			// Damp the rotation around the y-axis
			currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

            // Damp the height
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            // Convert the angle into a rotation
            var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target
            _henchman.position = _target.position;
            _henchman.position -= currentRotation * Vector3.forward * distance;

            // Set the height of the camera
            _henchman.position = new Vector3(_henchman.position.x ,currentHeight , _henchman.position.z);

			// Always look at the target
			//transform.LookAt(new Vector3(target.position.x, target.position.y, target.position.z));
			//transform.LookAt(new Vector3(target.position.x, targetOriginY, target.position.z));
		}
	}
}