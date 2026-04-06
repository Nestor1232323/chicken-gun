using UnityEngine;

public class Turret : MonoBehaviour
{
	public Bullet bulletPrefab;

	public Transform gun;

	private void Update()
	{
		Plane plane = new Plane(Vector3.up, base.transform.position);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float enter;
		if (plane.Raycast(ray, out enter))
		{
			Vector3 forward = Vector3.Normalize(ray.GetPoint(enter) - base.transform.position);
			Quaternion to = Quaternion.LookRotation(forward);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, 360f * Time.deltaTime);
			if (Input.GetMouseButtonDown(0))
			{
				bulletPrefab.Spawn(gun.position, gun.rotation);
			}
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			bulletPrefab.DestroyPooled();
		}
		if (Input.GetKeyDown(KeyCode.Z))
		{
			bulletPrefab.DestroyAll();
		}
	}
}
