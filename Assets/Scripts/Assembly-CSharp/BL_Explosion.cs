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
		for (int i = 0; i < array.Length; i++)
		{
			Rigidbody component = array[i].GetComponent<Rigidbody>();
			if (component != null)
			{
				component.AddExplosionForce(power, position, radius, upModifier, ForceMode.Impulse);
			}
		}
	}
}
