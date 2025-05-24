using UnityEngine;

namespace ProBuilder2.Examples
{
	public class CameraControls : MonoBehaviour
	{
		private const string INPUT_MOUSE_SCROLLWHEEL = "Mouse ScrollWheel";

		private const string INPUT_MOUSE_X = "Mouse X";

		private const string INPUT_MOUSE_Y = "Mouse Y";

		private const float MIN_CAM_DISTANCE = 10f;

		private const float MAX_CAM_DISTANCE = 40f;

		[Range(2f, 15f)]
		public float orbitSpeed = 6f;

		[Range(0.3f, 2f)]
		public float zoomSpeed = 0.8f;

		private float distance;

		public float idleRotation = 1f;

		private Vector2 dir = new Vector2(0.8f, 0.2f);

		private void Start()
		{
			distance = Vector3.Distance(base.transform.position, Vector3.zero);
		}

		private void LateUpdate()
		{
			Vector3 eulerAngles = base.transform.localRotation.eulerAngles;
			eulerAngles.z = 0f;
			if (Input.GetMouseButton(0))
			{
				float axis = Input.GetAxis("Mouse X");
				float num = 0f - Input.GetAxis("Mouse Y");
				eulerAngles.x += num * orbitSpeed;
				eulerAngles.y += axis * orbitSpeed;
				dir.x = axis;
				dir.y = num;
				dir.Normalize();
			}
			else
			{
				eulerAngles.y += Time.deltaTime * idleRotation * dir.x;
				eulerAngles.x += Time.deltaTime * Mathf.PerlinNoise(Time.time, 0f) * idleRotation * dir.y;
			}
			base.transform.localRotation = Quaternion.Euler(eulerAngles);
			base.transform.position = base.transform.localRotation * (Vector3.forward * (0f - distance));
			if (Input.GetAxis("Mouse ScrollWheel") != 0f)
			{
				float axis2 = Input.GetAxis("Mouse ScrollWheel");
				distance -= axis2 * (distance / 40f) * (zoomSpeed * 1000f) * Time.deltaTime;
				distance = Mathf.Clamp(distance, 10f, 40f);
				base.transform.position = base.transform.localRotation * (Vector3.forward * (0f - distance));
			}
		}
	}
}
