using UnityEngine;

public class BL_Explosion : MonoBehaviour
{
	public float radius;

	public float power;

	public float upModifier;

	private void OnEnable()
	{
		Vector3 position = base.transform.position;
		Collider[] array = Physics.OverlapSphere(position, radius);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			Rigidbody component = collider.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.AddExplosionForce(power, position, radius, upModifier, ForceMode.Impulse);
			}
		}
	}
}
